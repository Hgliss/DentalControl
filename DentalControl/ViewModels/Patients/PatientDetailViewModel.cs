using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DentalControl.Data.Database.Common;
using DentalControl.Models.Entities;
using DentalControl.Services.Interface.Patients;

namespace DentalControl.ViewModels.Patients;

public sealed class PatientDetailViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private CancellationTokenSource? _pendingQuery;
    private long _version;
    private int _patientId;
    private Patient? _patient;
    private bool _isLoading;
    private bool _isNotFound;
    private string _errorMessage = string.Empty;

    public PatientDetailViewModel(IPatientService patientService)
    {
        _patientService = patientService;
        RetryCommand = new AsyncRelayCommand(token => LoadAsync(_patientId, token), () => _patientId > 0 && !IsLoading);
    }

    public Patient? Patient => _patient;
    public IAsyncRelayCommand RetryCommand { get; }
    public bool HasPatient => Patient is not null && !IsLoading;
    public bool IsLoading { get => _isLoading; private set { if (SetProperty(ref _isLoading, value)) NotifyState(); } }
    public bool IsNotFound { get => _isNotFound; private set => SetProperty(ref _isNotFound, value); }
    public string ErrorMessage { get => _errorMessage; private set { if (SetProperty(ref _errorMessage, value)) OnPropertyChanged(nameof(HasError)); } }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public string PatientCode => Display(Patient?.PatientCode);
    public string FullName => Patient is null ? string.Empty : string.Join(" ",
        new[] { Patient.FirstName, Patient.MiddleName, Patient.LastName, Patient.SecondLastName }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
    public string Initials => Patient is null ? "" :
        string.Concat(Patient.FirstName.Take(1)).ToUpperInvariant() + string.Concat(Patient.LastName.Take(1)).ToUpperInvariant();
    public string FirstName => Display(Patient?.FirstName);
    public string MiddleName => Display(Patient?.MiddleName);
    public string LastName => Display(Patient?.LastName);
    public string SecondLastName => Display(Patient?.SecondLastName);
    public string BirthDateText => Patient?.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) ?? "—";
    public string GenderName => Display(Patient?.Gender?.Name);
    public string Cui => Display(Patient?.Cui);
    public string HeaderCui
    {
        get
        {
            var cui = Patient?.Cui;
            return string.IsNullOrWhiteSpace(cui) ? "Sin CUI" : cui;
        }
    }
    public string Phone => Display(Patient?.Phone);
    public string Address => Display(Patient?.Address);
    public string Occupation => Display(Patient?.Occupation);
    public string ReferredBy => Display(Patient?.ReferredBy);
    public string StatusText => Patient is null ? "—" : Patient.Status == EntityStatus.Inactive ? "Inactivo" : "Activo";
    public string AgeText
    {
        get
        {
            if (Patient is null) return "—";
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (Patient.BirthDate == default || Patient.BirthDate > today) return "—";
            var age = today.Year - Patient.BirthDate.Year;
            if (Patient.BirthDate > today.AddYears(-age)) age--;
            return $"{age} años";
        }
    }

    public async Task LoadAsync(int patientId, CancellationToken cancellationToken = default)
    {
        Deactivate();
        var version = _version;
        _patientId = patientId;
        _patient = null;
        IsNotFound = false;
        ErrorMessage = string.Empty;
        using var operation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _pendingQuery = operation;
        IsLoading = true;
        NotifyPatient();
        try
        {
            var patient = patientId > 0 ? await _patientService.GetByIdAsync(patientId, operation.Token) : null;
            if (version != _version || operation.IsCancellationRequested) return;
            _patient = patient;
            IsNotFound = patient is null;
            NotifyPatient();
        }
        catch (OperationCanceledException) when (operation.IsCancellationRequested) { }
        catch (Exception ex)
        {
            if (version == _version)
            {
                Trace.TraceError("Error al consultar los datos del paciente: {0}", ex);
                ErrorMessage = "No se pudieron cargar los datos del paciente. Intenta nuevamente.";
            }
        }
        finally
        {
            if (version == _version) { _pendingQuery = null; IsLoading = false; NotifyState(); }
        }
    }

    public void ShowPatient(Patient patient)
    {
        Deactivate();
        _patientId = patient.Id;
        _patient = patient;
        IsNotFound = false;
        ErrorMessage = string.Empty;
        NotifyPatient();
    }

    public void Deactivate()
    {
        _version++;
        var pending = _pendingQuery;
        _pendingQuery = null;
        pending?.Cancel();
        IsLoading = false;
    }

    private static string Display(string? value) => string.IsNullOrWhiteSpace(value) ? "—" : value;
    private void NotifyState()
    {
        OnPropertyChanged(nameof(HasPatient));
        RetryCommand.NotifyCanExecuteChanged();
    }
    private void NotifyPatient()
    {
        foreach (var name in new[] { nameof(Patient), nameof(FullName), nameof(Initials), nameof(PatientCode), nameof(FirstName),
            nameof(MiddleName), nameof(LastName), nameof(SecondLastName), nameof(BirthDateText), nameof(GenderName),
            nameof(Cui), nameof(HeaderCui), nameof(Phone), nameof(Address), nameof(Occupation), nameof(ReferredBy), nameof(StatusText), nameof(AgeText) })
            OnPropertyChanged(name);
        NotifyState();
    }
}
