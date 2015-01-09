using Miniproject.Common;
using Miniproject.Model;
using Miniproject.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        private string filename = "pizzahistory2.txt";
        private string result;

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
                this.PizzaTimePicker.Time = TimeSpan.FromTicks(DateTime.Now.Add(new TimeSpan(0, 0, 30, 0)).Ticks);
            }
            else
            {
                this._fromHistory = true;
                Debug.WriteLine("Geschiedenis");
                _pizza = (Pizza)e.Parameter;
                this.PizzaTimePicker.Time = TimeSpan.FromTicks(_pizza._bezorgtijd.Ticks);
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
                this.PizzaTimePicker.Time = TimeSpan.FromTicks(DateTime.Now.Add(new TimeSpan(0, 0, 30, 0)).Ticks);
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
             
                //pizzastring = result + "~" + _viewModel.Kaas + "@" + _viewModel.Vlees + "@" + _viewModel.Paddestoel + "@" + _viewModel.Korst;
                Debug.WriteLine(pizza.ToString() + " ;;;" + App._bestellingen.getPizzasCount());
            }
            //Debug.WriteLine(_viewModel.Kaas + "\n" + _viewModel.Vlees + "\n" + _viewModel.Paddestoel + "\n" + _viewModel.Korst);
        }

        private void comboBox_Kaas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_fromHistory)
            _viewModel.Kaas = comboBox_Kaas.SelectedItem.ToString();
        }

        private void comboBox_Fleesch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_fromHistory)
            _viewModel.Vlees = comboBox_Fleesch.SelectedItem.ToString();
        }

        private void comboBox_Paddos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_fromHistory)
            _viewModel.Paddestoel = comboBox_Paddos.SelectedItem.ToString();
        }

        private void comboBox_Korst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_fromHistory)
            _viewModel.Korst = comboBox_Korst.SelectedItem.ToString();
        }
    }
}
