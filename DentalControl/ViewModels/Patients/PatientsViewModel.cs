using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DentalControl.Common;
using DentalControl.Models.Catalog;
using DentalControl.Models.Entities;
using DentalControl.Services.Interface.Catalog;
using DentalControl.Services.Interface.Patients;
using DentalControl.Services.Implemetations.Patients;

namespace DentalControl.ViewModels.Patients;

public sealed partial class PatientsViewModel : ObservableObject, INotifyDataErrorInfo
{
    private readonly IPatientService _patientService;
    private readonly ICatalogService _catalogService;
    private readonly ObservableCollection<CatalogItem> _genders = new();
    private CancellationTokenSource? _activeOperation;
    private bool _isFormOpen;
    private bool _isBusy;
    private bool _isLoadingGenders;
    private bool _isDuplicateCuiPopupOpen;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;
    private bool _validationRequested;
    private Dictionary<string, List<string>> _fieldErrors = new();
    private readonly Dictionary<string, string> _serviceFieldErrors = new();

    // CommunityToolkit genera las propiedades públicas y sus notificaciones para los bindings.
    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _middleName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _secondLastName = string.Empty;
    [ObservableProperty] private DateTime? _birthDate;
    [ObservableProperty] private CatalogItem? _selectedGender;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private string _cui = string.Empty;
    [ObservableProperty] private string _occupation = string.Empty;
    [ObservableProperty] private string _referredBy = string.Empty;

    public PatientsViewModel(IPatientService patientService, ICatalogService catalogService)
    {
        ArgumentNullException.ThrowIfNull(patientService);
        ArgumentNullException.ThrowIfNull(catalogService);
        _patientService = patientService;
        _catalogService = catalogService;
        Genders = new ReadOnlyObservableCollection<CatalogItem>(_genders);
        NewPatientCommand = new AsyncRelayCommand(OpenRegistrationAsync, () => !IsFormOpen && !IsBusy);
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new RelayCommand(CancelRegistration, () => IsFormOpen);
        CloseDuplicateCuiPopupCommand = new RelayCommand(() => IsDuplicateCuiPopupOpen = false);
    }

    public ReadOnlyObservableCollection<CatalogItem> Genders { get; }
    public IAsyncRelayCommand NewPatientCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand CloseDuplicateCuiPopupCommand { get; }
    public bool IsEntryVisible => !IsFormOpen;
    public bool CanEdit => IsFormOpen && !IsBusy && Genders.Count > 0 && !IsDuplicateCuiPopupOpen;
    public bool IsSaving => IsBusy && !IsLoadingGenders;
    public string SaveButtonText => IsSaving ? "Guardando…" : "Guardar";
    public bool HasErrors => _fieldErrors.Count > 0;
    public string FormErrorMessage => HasErrors ? string.Empty : ErrorMessage;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public event EventHandler? ValidationFailed;

    public IEnumerable GetErrors(string? propertyName) => string.IsNullOrEmpty(propertyName)
        ? _fieldErrors.Values.SelectMany(messages => messages).ToArray()
        : _fieldErrors.TryGetValue(propertyName, out var messagesForField) ? messagesForField.ToArray() : Array.Empty<string>();

    public bool IsDuplicateCuiPopupOpen
    {
        get => _isDuplicateCuiPopupOpen;
        set
        {
            if (SetProperty(ref _isDuplicateCuiPopupOpen, value)) NotifyStateChanged();
        }
    }

    public bool IsFormOpen
    {
        get => _isFormOpen;
        private set
        {
            if (!SetProperty(ref _isFormOpen, value)) return;
            OnPropertyChanged(nameof(IsEntryVisible));
            NotifyStateChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value)) NotifyStateChanged();
        }
    }

    public bool IsLoadingGenders
    {
        get => _isLoadingGenders;
        private set
        {
            if (SetProperty(ref _isLoadingGenders, value)) NotifyStateChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value)) OnPropertyChanged(nameof(FormErrorMessage));
        }
    }

    public string SuccessMessage
    {
        get => _successMessage;
        private set => SetProperty(ref _successMessage, value);
    }

    private bool CanSave() => IsFormOpen && !IsBusy && Genders.Count > 0 && !IsDuplicateCuiPopupOpen;

    private async Task OpenRegistrationAsync(CancellationToken cancellationToken)
    {
        if (IsFormOpen || IsBusy) return;
        ResetFields();
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        _genders.Clear();
        using var operation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _activeOperation = operation;
        IsLoadingGenders = true;
        IsBusy = true;
        IsFormOpen = true;
        try
        {
            var genders = await _catalogService.GetActiveItemsAsync(CategoryCodes.GENDER, operation.Token);
            operation.Token.ThrowIfCancellationRequested();
            foreach (var item in genders) _genders.Add(item);
            if (Genders.Count == 0)
                ErrorMessage = "No hay géneros activos disponibles. No es posible registrar un paciente.";
        }
        catch (OperationCanceledException) when (operation.IsCancellationRequested)
        {
            ErrorMessage = "La carga de géneros se canceló. Cancela el formulario y vuelve a abrirlo para reintentar.";
        }
        catch (Exception ex)
        {
            Trace.TraceError("Error al cargar los géneros: {0}", ex);
            ErrorMessage = "No se pudieron cargar los géneros. Cancela el formulario y vuelve a abrirlo para reintentar.";
        }
        finally
        {
            _activeOperation = null;
            IsBusy = false;
            IsLoadingGenders = false;
        }
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        // La guarda también protege llamadas directas que no consulten CanExecute.
        if (!CanSave()) return;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        _serviceFieldErrors.Clear();
        _validationRequested = true;
        ValidateFields();
        if (HasErrors)
        {
            ValidationFailed?.Invoke(this, EventArgs.Empty);
            return;
        }
        var patient = CreatePatient();

        using var operation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _activeOperation = operation;
        IsBusy = true;
        try
        {
            var saved = await _patientService.CreateAsync(patient, operation.Token);
            // Un resultado confirmado es éxito aunque la cancelación se solicite tarde.
            ErrorMessage = string.Empty;
            ResetFields();
            _genders.Clear();
            IsFormOpen = false;
            SuccessMessage = $"Paciente registrado correctamente. Código: {saved.PatientCode}.";
        }
        catch (ValidationException ex)
        {
            var fields = ex.ValidationResult?.MemberNames.Select(ToFormField).Where(IsFormField).ToArray()
                ?? Array.Empty<string>();
            foreach (var field in fields) _serviceFieldErrors[field] = ex.Message;
            ValidateFields();
            IsDuplicateCuiPopupOpen = fields.Contains(nameof(Cui));
            ErrorMessage = fields.Length == 0 || IsDuplicateCuiPopupOpen ? ex.Message : string.Empty;
            ValidationFailed?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException) when (operation.IsCancellationRequested)
        {
            ErrorMessage = "El registro se canceló. Puedes revisar los datos e intentarlo de nuevo.";
        }
        catch (Exception ex)
        {
            Trace.TraceError("Error al registrar el paciente: {0}", ex);
            ErrorMessage = "No se pudo registrar el paciente. Tus datos se conservan; intenta nuevamente.";
        }
        finally
        {
            _activeOperation = null;
            IsBusy = false;
        }
    }

    private void CancelRegistration()
    {
        if (!IsFormOpen) return;
        if (IsBusy)
        {
            ErrorMessage = "Cancelando la operación…";
            _activeOperation?.Cancel();
            // Espera al resultado para no ocultar un registro que alcanzó a confirmarse.
            return;
        }

        ResetFields();
        _genders.Clear();
        IsDuplicateCuiPopupOpen = false;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        IsFormOpen = false;
    }

    private void ResetFields()
    {
        _validationRequested = false;
        _serviceFieldErrors.Clear();
        ReplaceFieldErrors(new Dictionary<string, List<string>>());
        FirstName = MiddleName = LastName = SecondLastName = string.Empty;
        BirthDate = null;
        SelectedGender = null;
        Phone = Address = Cui = Occupation = ReferredBy = string.Empty;
    }

    private Patient CreatePatient() => new()
    {
        FirstName = FirstName,
        MiddleName = MiddleName,
        LastName = LastName,
        SecondLastName = SecondLastName,
        BirthDate = BirthDate.HasValue ? DateOnly.FromDateTime(BirthDate.Value) : default,
        GenderId = SelectedGender?.Id ?? 0,
        Phone = Phone,
        Address = Address,
        Cui = Cui,
        Occupation = Occupation,
        ReferredBy = ReferredBy
    };

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (_validationRequested && e.PropertyName is { } field && IsFormField(field))
        {
            if (_serviceFieldErrors.Remove(field)) ErrorMessage = string.Empty;
            ValidateFields();
        }
    }

    private static string ToFormField(string field) => field == nameof(Patient.GenderId) ? nameof(SelectedGender) : field;

    private static bool IsFormField(string field) => field is
        nameof(FirstName) or nameof(MiddleName) or nameof(LastName) or nameof(SecondLastName) or
        nameof(BirthDate) or nameof(SelectedGender) or nameof(Phone) or nameof(Address) or
        nameof(Cui) or nameof(Occupation) or nameof(ReferredBy);

    private void ValidateFields()
    {
        var next = new Dictionary<string, List<string>>();
        foreach (var result in PatientBusinessRules.GetValidationResults(CreatePatient()))
        {
            foreach (var member in result.MemberNames)
            {
                var field = ToFormField(member);
                if (!next.TryGetValue(field, out var messages)) next[field] = messages = new List<string>();
                messages.Add(result.ErrorMessage ?? "Revisa este campo.");
            }
        }
        foreach (var (field, message) in _serviceFieldErrors)
        {
            if (!next.TryGetValue(field, out var messages)) next[field] = messages = new List<string>();
            messages.Add(message);
        }
        ReplaceFieldErrors(next);
    }

    private void ReplaceFieldErrors(Dictionary<string, List<string>> next)
    {
        var fields = _fieldErrors.Keys.Concat(next.Keys).Distinct().ToArray();
        _fieldErrors = next;
        foreach (var field in fields) ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(field));
        OnPropertyChanged(nameof(HasErrors));
        OnPropertyChanged(nameof(FormErrorMessage));
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanEdit));
        OnPropertyChanged(nameof(IsSaving));
        OnPropertyChanged(nameof(SaveButtonText));
        NewPatientCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }
}
