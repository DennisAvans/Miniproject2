using Miniproject.Common;
using Miniproject.Model;
using Miniproject.ViewModel;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace Miniproject.View
{
    public sealed partial class BestelPagina : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public PizzaViewModel _viewModel;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private Pizza _pizza;
        private bool _fromHistory = false;
        private string pizzastring = String.Empty;

        public BestelPagina()
        {
            this.InitializeComponent();

            _viewModel = (PizzaViewModel)base.DataContext;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                Debug.WriteLine("Nieuw");
                this._fromHistory = false;
                this.PizzaTimePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + 30, 0);
            }
            else
            {
                this._fromHistory = true;
                Debug.WriteLine("Geschiedenis");
                _pizza = (Pizza)e.Parameter;

                _viewModel.Bezorgtijd = _pizza._bezorgtijd;
                _viewModel.Kaas = _pizza._kaas;
                _viewModel.Vlees = _pizza._vlees;
                _viewModel.Paddestoel = _pizza._paddestoel;
                _viewModel.Korst = _pizza._korst;

                this.PizzaTimePicker.Time = _pizza._bezorgtijd;
                this.comboBox_Kaas.SelectedValue = _pizza._kaas;
                this.comboBox_Fleesch.SelectedValue = _pizza._vlees;
                this.comboBox_Paddos.SelectedValue = _pizza._paddestoel;
                this.comboBox_Korst.SelectedValue = _pizza._korst;
            }
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this._fromHistory == true)
            {
                this.PizzaTimePicker.Time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + 30, 0);
                this.comboBox_Kaas.SelectedValue = string.Empty;
                this.comboBox_Fleesch.SelectedValue = string.Empty;
                this.comboBox_Paddos.SelectedValue = string.Empty;
                this.comboBox_Korst.SelectedValue = string.Empty;
                this.ToS_checkbox.IsChecked = false;
            }
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Kaas.Equals(String.Empty) || _viewModel.Vlees.Equals(String.Empty) || _viewModel.Paddestoel.Equals(String.Empty) || _viewModel.Korst.Equals(String.Empty))
            {
                MessageDialog msgbox = new MessageDialog("Je heb een of meerdere dingen nog niet geselecteerd!");
                await msgbox.ShowAsync();
            }
            else if (ToS_checkbox.IsChecked == false)
            {
                MessageDialog msgbox = new MessageDialog("Je moet akkoord gaan met de algemene voorwaarden!");
                await msgbox.ShowAsync();
            }
            else
            {
                Frame.Navigate(typeof(Kaart));
                Model.Pizza pizza = new Model.Pizza(_viewModel.Kaas, _viewModel.Vlees, _viewModel.Paddestoel, _viewModel.Korst, _viewModel.Bezorgtijd);
                App._bestellingen.addPizza(pizza);

                Debug.WriteLine(pizza.ToString() + " ;;; " + App._bestellingen.getPizzasCount());
            }
        }

        private void comboBox_Kaas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.Kaas = comboBox_Kaas.SelectedItem.ToString();
        }

        private void comboBox_Fleesch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.Vlees = comboBox_Fleesch.SelectedItem.ToString();
        }

        private void comboBox_Paddos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.Paddestoel = comboBox_Paddos.SelectedItem.ToString();
        }

        private void comboBox_Korst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.Korst = comboBox_Korst.SelectedItem.ToString();
        }

        private void PizzaTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            _viewModel.Bezorgtijd = PizzaTimePicker.Time;
        }
    }
}
