using System.Windows.Controls;
using DentalControl.ViewModels.Patients;

namespace DentalControl.Views;

public partial class PatientsView : UserControl
{
    private PatientsViewModel? _viewModel;

    public PatientsView()
    {
        InitializeComponent();
        Loaded += (_, _) => AttachViewModel();
        Unloaded += (_, _) => DetachViewModel();
        DataContextChanged += (_, _) => AttachViewModel();
    }

    private void AttachViewModel()
    {
        DetachViewModel();
        if (!IsLoaded || DataContext is not PatientsViewModel viewModel) return;
        _viewModel = viewModel;
        if (_viewModel.IsEntryVisible) _viewModel.List.Activate();
    }

    private void DetachViewModel()
    {
        _viewModel?.List.Deactivate();
        _viewModel?.Detail.Deactivate();
        _viewModel = null;
    }
}