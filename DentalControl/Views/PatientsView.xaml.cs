using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DentalControl.ViewModels.Patients;

namespace DentalControl.Views
{
    /// <summary>
    /// Interaction logic for PatientsView.xaml
    /// </summary>
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
            _viewModel.ValidationFailed += FocusFirstInvalidField;
        }

        private void DetachViewModel()
        {
            if (_viewModel is not null) _viewModel.ValidationFailed -= FocusFirstInvalidField;
            _viewModel = null;
        }

        private void FocusFirstInvalidField(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
            {
                if (!IsLoaded || !ReferenceEquals(sender, DataContext) || DuplicateCuiPopup.IsOpen) return;
                Control[] inputs = [FirstNameInput, MiddleNameInput, LastNameInput, SecondLastNameInput,
                    BirthDateInput, GenderInput, CuiInput, PhoneInput, OccupationInput, ReferredByInput, AddressInput];
                var first = inputs.FirstOrDefault(input => input.IsEnabled && Validation.GetHasError(input));
                first?.BringIntoView();
                first?.Focus();
            }));
        }

        private void DuplicateCuiPopup_Opened(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
            {
                if (DuplicateCuiPopup.IsOpen) AcceptDuplicateCuiButton.Focus();
            }));
        }

        private void DuplicateCuiPopup_Closed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
            {
                if (!DuplicateCuiPopup.IsOpen && CuiInput.IsVisible && CuiInput.IsEnabled)
                {
                    CuiInput.Focus();
                    CuiInput.SelectAll();
                }
            }));
        }
    }
}
