﻿using Miniproject.Common;
using Miniproject.View;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Miniproject
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        public MainPage()
        {
            this.InitializeComponent();

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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await App._bestellingen.getPizzas();
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
           // Application.Current.Exit();
        }

        #endregion

        private void BestelPizzaButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BestelPagina));
        }

        private void GeschiedenisButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GeschiedenisPagina));
        }
    }
}
