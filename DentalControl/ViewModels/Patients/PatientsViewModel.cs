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
    private bool _isDetailOpen;
    private int? _editingPatientId;
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

    public PatientsViewModel(IPatientService patientService, ICatalogService catalogService, PatientListViewModel list,
        PatientDetailViewModel detail)
    {
        ArgumentNullException.ThrowIfNull(patientService);
        ArgumentNullException.ThrowIfNull(catalogService);
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(detail);
        _patientService = patientService;
        _catalogService = catalogService;
        List = list;
        Detail = detail;
        Genders = new ReadOnlyObservableCollection<CatalogItem>(_genders);
        NewPatientCommand = new AsyncRelayCommand(OpenRegistrationAsync, () => IsEntryVisible && !IsBusy);
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new RelayCommand(CancelRegistration, () => IsFormOpen);
        CloseDuplicateCuiPopupCommand = new RelayCommand(() => IsDuplicateCuiPopupOpen = false);
        OpenDetailCommand = new AsyncRelayCommand<Patient>(OpenDetailAsync,
            patient => patient is not null && IsEntryVisible && !IsBusy, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        BackToListCommand = new RelayCommand(BackToList, () => IsDetailOpen && !IsFormOpen && !IsBusy);
        EditPatientCommand = new AsyncRelayCommand(OpenEditAsync,
            () => IsDetailVisible && !IsFormOpen && !IsBusy && Detail.HasPatient);
        Detail.PropertyChanged += (_, _) => EditPatientCommand.NotifyCanExecuteChanged();
    }

    public PatientListViewModel List { get; }
    public PatientDetailViewModel Detail { get; }
    public IAsyncRelayCommand<Patient> OpenDetailCommand { get; }
    public IRelayCommand BackToListCommand { get; }
    public IAsyncRelayCommand EditPatientCommand { get; }
    public ReadOnlyObservableCollection<CatalogItem> Genders { get; }
    public IAsyncRelayCommand NewPatientCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand CloseDuplicateCuiPopupCommand { get; }
    public bool IsEntryVisible => !IsFormOpen && !IsDetailOpen;
    public bool IsDetailVisible => IsDetailOpen;
    public bool IsDetailReadOnly => IsDetailOpen && !IsFormOpen;
    public bool IsEditing => _editingPatientId.HasValue;
    public string FormTitle => IsEditing ? $"Editar datos · {Detail.PatientCode}" : "Nuevo paciente";
    public bool IsDetailOpen
    {
        get => _isDetailOpen;
        private set
        {
            if (!SetProperty(ref _isDetailOpen, value)) return;
            OnPropertyChanged(nameof(IsEntryVisible));
            OnPropertyChanged(nameof(IsDetailVisible));
            OnPropertyChanged(nameof(IsDetailReadOnly));
            NotifyStateChanged();
        }
    }
    public bool CanEdit => IsFormOpen && !IsBusy && Genders.Count > 0 && !IsDuplicateCuiPopupOpen;
    public bool IsSaving => IsBusy && !IsLoadingGenders;
    public string SaveButtonText => IsSaving ? "Guardando…" : IsEditing ? "Guardar cambios" : "Guardar";
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
            OnPropertyChanged(nameof(IsDetailVisible));
            OnPropertyChanged(nameof(IsDetailReadOnly));
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
        if (!IsEntryVisible || IsBusy) return;
        await OpenFormAsync(null, cancellationToken);
    }

    private async Task OpenEditAsync(CancellationToken cancellationToken)
    {
        if (!IsDetailVisible || IsFormOpen || IsBusy || Detail.Patient is null) return;
        await OpenFormAsync(Detail.Patient, cancellationToken);
    }

    private async Task OpenFormAsync(Patient? patient, CancellationToken cancellationToken)
    {
        List.Deactivate();
        ResetFields();
        SetEditingPatient(patient?.Id);
        if (patient is not null)
        {
            FirstName = patient.FirstName;
            MiddleName = patient.MiddleName ?? string.Empty;
            LastName = patient.LastName;
            SecondLastName = patient.SecondLastName ?? string.Empty;
            BirthDate = patient.BirthDate.ToDateTime(TimeOnly.MinValue);
            Cui = patient.Cui ?? string.Empty;
            Phone = patient.Phone ?? string.Empty;
            Address = patient.Address ?? string.Empty;
            Occupation = patient.Occupation ?? string.Empty;
            ReferredBy = patient.ReferredBy ?? string.Empty;
        }
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
            if (patient is not null)
            {
                SelectedGender = Genders.FirstOrDefault(item => item.Id == patient.GenderId);
                if (SelectedGender is null && Genders.Count > 0)
                    ErrorMessage = "El género actual no está activo. Selecciona uno para guardar los cambios.";
            }
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
            var editingId = _editingPatientId;
            var saved = editingId.HasValue
                ? await _patientService.UpdateAsync(editingId.Value, patient, operation.Token)
                : await _patientService.CreateAsync(patient, operation.Token);
            // Un resultado confirmado es éxito aunque la cancelación se solicite tarde.
            ErrorMessage = string.Empty;
            ResetFields();
            _genders.Clear();
            IsFormOpen = false;
            SetEditingPatient(null);
            if (editingId.HasValue)
            {
                Detail.ShowPatient(saved);
                SuccessMessage = "Datos del paciente actualizados correctamente.";
            }
            else
            {
                SuccessMessage = $"Paciente registrado correctamente. Código: {saved.PatientCode}.";
                List.ShowCreatedPatient();
            }
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
            ErrorMessage = "La operación se canceló. Puedes revisar los datos e intentarlo de nuevo.";
        }
        catch (KeyNotFoundException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            Trace.TraceError("Error al registrar el paciente: {0}", ex);
            ErrorMessage = "No se pudieron guardar los datos del paciente. Tus datos se conservan; intenta nuevamente.";
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
        SetEditingPatient(null);
        if (!IsDetailOpen) List.Activate();
    }

    private async Task OpenDetailAsync(Patient? patient, CancellationToken cancellationToken)
    {
        if (patient is null || !IsEntryVisible || IsBusy) return;
        List.Deactivate();
        SuccessMessage = string.Empty;
        IsDetailOpen = true;
        await Detail.LoadAsync(patient.Id, cancellationToken);
    }

    private void BackToList()
    {
        if (!IsDetailOpen || IsFormOpen || IsBusy) return;
        Detail.Deactivate();
        IsDetailOpen = false;
        List.Activate();
    }

    private void SetEditingPatient(int? id)
    {
        _editingPatientId = id;
        OnPropertyChanged(nameof(IsEditing));
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
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
        OpenDetailCommand.NotifyCanExecuteChanged();
        BackToListCommand.NotifyCanExecuteChanged();
        EditPatientCommand.NotifyCanExecuteChanged();
    }
}
