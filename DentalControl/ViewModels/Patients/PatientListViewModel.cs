using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DentalControl.Models.Entities;
using DentalControl.Services.Interface.Patients;

namespace DentalControl.ViewModels.Patients;

/// <summary>Estado de búsqueda, filtros y paginación; no accede directamente a EF Core.</summary>
public sealed class PatientListViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private readonly ObservableCollection<Patient> _patients = new();
    private CancellationTokenSource? _pendingQuery;
    private long _requestVersion;
    private bool _isActive;
    private bool _isLoading;
    private string _searchText = string.Empty;
    private PatientStatusFilter _selectedStatus = PatientStatusFilter.Active;
    private string _loadErrorMessage = string.Empty;
    private int _pageNumber = 1;
    private int _totalCount;

    public PatientListViewModel(IPatientService patientService)
    {
        ArgumentNullException.ThrowIfNull(patientService);
        _patientService = patientService;
        Patients = new ReadOnlyObservableCollection<Patient>(_patients);
        RefreshCommand = new AsyncRelayCommand(() => LoadPageAsync(PageNumber), () => _isActive && !IsLoading);
        PreviousPageCommand = new AsyncRelayCommand(
            () => LoadPageAsync(PageNumber - 1), () => _isActive && !IsLoading && PageNumber > 1 && !HasLoadError);
        NextPageCommand = new AsyncRelayCommand(
            () => LoadPageAsync(PageNumber + 1), () => _isActive && !IsLoading && PageNumber < TotalPages && !HasLoadError);
    }

    public const int PageSize = 10;
    public ReadOnlyObservableCollection<Patient> Patients { get; }
    public IReadOnlyList<KeyValuePair<PatientStatusFilter, string>> StatusOptions { get; } =
    [
        new(PatientStatusFilter.All, "Todos"),
        new(PatientStatusFilter.Active, "Activos"),
        new(PatientStatusFilter.Inactive, "Inactivos")
    ];
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand PreviousPageCommand { get; }
    public IAsyncRelayCommand NextPageCommand { get; }
    public int PageNumber => _pageNumber;
    public int TotalCount => _totalCount;
    public int TotalPages => TotalCount == 0 ? 0 : (TotalCount - 1) / PageSize + 1;
    public bool HasPatients => !IsLoading && !HasLoadError && Patients.Count > 0;
    public bool HasLoadError => !string.IsNullOrEmpty(LoadErrorMessage);
    public bool ShowEmptyState => !IsLoading && !HasLoadError && Patients.Count == 0;
    public string EmptyMessage => !string.IsNullOrWhiteSpace(SearchText)
        ? "No se encontraron pacientes que coincidan con la búsqueda."
        : SelectedStatus switch
        {
            PatientStatusFilter.Active => "No hay pacientes activos.",
            PatientStatusFilter.Inactive => "No hay pacientes inactivos.",
            _ => "Aún no hay pacientes registrados."
        };
    public string PageSummary => IsLoading ? "Cargando…" : HasLoadError ? "Listado no disponible"
        : TotalCount == 0 ? "0 pacientes"
        : $"Página {PageNumber} de {TotalPages} · {(PageNumber - 1) * PageSize + 1}–{Math.Min(PageNumber * (long)PageSize, TotalCount)} de {TotalCount} pacientes";

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value ?? string.Empty)) CriteriaChanged(debounce: true);
        }
    }

    public PatientStatusFilter SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (Enum.IsDefined(value) && SetProperty(ref _selectedStatus, value)) CriteriaChanged(debounce: false);
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value)) NotifyListState();
        }
    }

    public string LoadErrorMessage
    {
        get => _loadErrorMessage;
        private set
        {
            if (SetProperty(ref _loadErrorMessage, value)) NotifyListState();
        }
    }

    /// <summary>Se invoca al mostrar el listado; nunca consulta desde el constructor.</summary>
    public void Activate()
    {
        if (_isActive) return;
        _isActive = true;
        _ = LoadPageAsync(PageNumber);
    }

    /// <summary>Cancela la consulta y evita publicar resultados al abandonar la vista.</summary>
    public void Deactivate()
    {
        _isActive = false;
        _requestVersion++;
        var pending = _pendingQuery;
        _pendingQuery = null;
        pending?.Cancel();
        IsLoading = false;
        NotifyListState();
    }

    /// <summary>El registro nuevo es activo y tiene el mayor código; se muestra la última página.</summary>
    public void ShowCreatedPatient()
    {
        SetProperty(ref _searchText, string.Empty, nameof(SearchText));
        SetProperty(ref _selectedStatus, PatientStatusFilter.Active, nameof(SelectedStatus));
        _isActive = true;
        _ = LoadPageAsync(int.MaxValue);
    }

    private void CriteriaChanged(bool debounce)
    {
        _pageNumber = 1;
        NotifyListState();
        if (_isActive) _ = LoadPageAsync(1, debounce);
    }

    private async Task LoadPageAsync(int requestedPage, bool debounce = false)
    {
        if (!_isActive || requestedPage < 1) return;
        var version = ++_requestVersion;
        _pendingQuery?.Cancel();
        using var operation = new CancellationTokenSource();
        _pendingQuery = operation;
        var search = SearchText;
        var status = SelectedStatus;
        _patients.Clear();
        _totalCount = 0;
        LoadErrorMessage = string.Empty;
        IsLoading = true;
        NotifyListState();

        try
        {
            if (debounce) await Task.Delay(300, operation.Token);
            var result = await _patientService.GetPageAsync(
                requestedPage, PageSize, search, status, operation.Token);
            // La cancelación es cooperativa: además se descartan respuestas de solicitudes anteriores.
            if (!_isActive || version != _requestVersion || operation.IsCancellationRequested) return;
            foreach (var patient in result.Items) _patients.Add(patient);
            _pageNumber = result.PageNumber;
            _totalCount = result.TotalCount;
        }
        catch (OperationCanceledException) when (operation.IsCancellationRequested)
        {
            // Es normal al seguir escribiendo, abrir el registro o abandonar la sección.
        }
        catch (Exception ex)
        {
            if (_isActive && version == _requestVersion)
            {
                Trace.TraceError("Error al consultar el listado de pacientes: {0}", ex);
                LoadErrorMessage = "No se pudo cargar el listado de pacientes. Intenta nuevamente.";
            }
        }
        finally
        {
            if (version == _requestVersion)
            {
                _pendingQuery = null;
                IsLoading = false;
                NotifyListState();
            }
        }
    }

    private void NotifyListState()
    {
        OnPropertyChanged(nameof(PageNumber));
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(HasPatients));
        OnPropertyChanged(nameof(HasLoadError));
        OnPropertyChanged(nameof(ShowEmptyState));
        OnPropertyChanged(nameof(EmptyMessage));
        OnPropertyChanged(nameof(PageSummary));
        RefreshCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
    }
}
