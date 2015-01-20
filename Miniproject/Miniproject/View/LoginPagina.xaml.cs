using Miniproject.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Popups;
using Miniproject.Model;

namespace Miniproject.View
{
    public sealed partial class LoginPagina : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private MessageDialog _dialog;

        public LoginPagina()
        {
            this.InitializeComponent();

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
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                PasswordTextBox.Focus(FocusState.Keyboard);
        }

        private void PasswordTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                LoginButton.Focus(FocusState.Keyboard);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            App.sendData("LOGIN@" + UsernameTextBox.Text + "@" + PasswordTextBox.Password.ToString());
            string result = await App.readData();
            if (result.Equals("~Invalid credentials"))
            {
                showMessageDialog("Gebruikersnaam of wachtwoord verkeerd");
            }
            else if (result.Equals("~Valid credentials"))
            {
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void GoToRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistreerPagina));
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HelpPagina));
        }

        private async void showMessageDialog(string message)
        {
            _dialog = new MessageDialog(message);
            await _dialog.ShowAsync();
        }

    }
}
