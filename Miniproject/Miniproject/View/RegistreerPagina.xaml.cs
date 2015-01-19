using Miniproject.Common;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Miniproject.View
{
    public sealed partial class RegistreerPagina : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private MessageDialog _dialog;

        public RegistreerPagina()
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

        private void UsernameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                PasswordTextBox.Focus(FocusState.Keyboard);
        }

        private void PasswordTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                RegisterButton.Focus(FocusState.Keyboard);
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text.Equals(string.Empty) || PasswordTextBox.Password.ToString().Equals(string.Empty))
            {
               showMessageDialog("Gebruikersnaam of wachtwoord leeg");
            }
            else
            {
                App.sendData("REGISTER@" + UsernameTextBox.Text + "@" + PasswordTextBox.Password.ToString());
                string result = await App.readData();
                if (result.Equals("~User already exists"))
                {
                    showMessageDialog("Gebruikersnaam bestaat al");
                }
                else if (result.Equals("~User added"))
                {
                    showMessageDialog("Account aangemaakt!");
                    UsernameTextBox.Text = string.Empty;
                    PasswordTextBox.Password = string.Empty;
                }
            }
        }

        private async void showMessageDialog(string message)
        {
            _dialog = new MessageDialog(message);
            await _dialog.ShowAsync();
        }

    }
}
