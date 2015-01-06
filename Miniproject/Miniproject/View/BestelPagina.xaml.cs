using Miniproject.Common;
using Miniproject.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Miniproject.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BestelPagina : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public PizzaViewModel _viewModel;

        public BestelPagina()
        {
            this.InitializeComponent();

            _viewModel = (PizzaViewModel)base.DataContext;

            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            //if (_viewModel.Kaas.Equals(String.Empty) || _viewModel.Vlees.Equals(String.Empty) || _viewModel.Paddestoel.Equals(String.Empty) || _viewModel.Korst.Equals(String.Empty))
            //{
            //    MessageDialog msgbox = new MessageDialog("Je heb een of meerdere dingen nog niet geselecteerd!");
            //    await msgbox.ShowAsync();
            //}
            //else if (ToS_checkbox.IsChecked == false)
            //{
            //    MessageDialog msgbox = new MessageDialog("Je moet akkoord gaan met de algemene voorwaarden!");
            //    await msgbox.ShowAsync();
            //}
            //else
            //{
                Frame.Navigate(typeof(Kaart));
            //}
            Debug.WriteLine(_viewModel.Kaas + "\n" + _viewModel.Vlees + "\n" + _viewModel.Paddestoel + "\n" + _viewModel.Korst);
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
    }
}
