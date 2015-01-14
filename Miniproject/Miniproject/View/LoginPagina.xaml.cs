using Miniproject.Common;
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
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Miniproject.View
{
    public sealed partial class LoginPagina : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private StreamSocket clientSocket;
        private HostName serverHost;
        private string serverHostnameString = "127.0.0.1";
        private string serverPort = "1330";
        private bool connected = false;
        private bool closing = false;

        public LoginPagina()
        {
            this.InitializeComponent();
            clientSocket = new StreamSocket();

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
            tryConnect();
            sendData("test");
            #region franks code, heb ik even in methodes gezet
            /*
            if (!connected)
            {
                StatusLabel.Text = "Must be connected to send!";
                return;
            }

            UInt32 len = 0; // Gets the UTF-8 string length.

            try
            {
                StatusLabel.Text = "Trying to send data ...";

                // add a newline to the text to send
                string sendData = "CON@" + UsernameTextBox.Text + "@" + PasswordTextBox.Password.ToString();
                DataWriter writer = new DataWriter(clientSocket.OutputStream);
                len = writer.MeasureString(sendData); // Gets the UTF-8 string length.

                // Call StoreAsync method to store the data to a backing stream
                await writer.StoreAsync();

                StatusLabel.Text = "Data was sent" + Environment.NewLine;

                // detach the stream and close it
                writer.DetachStream();
                writer.Dispose();

            }
            catch (Exception exception)
            {
                // If this is an unknown status, 
                // it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                StatusLabel.Text = "Send data or receive failed with error: " + exception.Message;
                // Could retry the connection, but for this simple example
                // just close the socket.

                closing = true;
                clientSocket.Dispose();
                clientSocket = null;
                connected = false;

            }

            // Now try to receive data from server
            try
            {
                StatusLabel.Text = "";
                StatusLabel.Text = "Trying to receive data ...";

                DataReader reader = new DataReader(clientSocket.InputStream);

                uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                if (sizeFieldCount != sizeof(uint))
                {
                    // The underlying socket was closed before we were able to read the whole data.
                    return;
                }

                // Read the string.
                uint stringLength = reader.ReadUInt32();
                uint actualStringLength = await reader.LoadAsync(stringLength);
                if (stringLength != actualStringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data.
                    return;
                }

                string answer = reader.ReadString(actualStringLength);
                if (answer.Equals("OK"))
                {
                    StatusLabel.Text = "OK";
                }
                else
                {
                    StatusLabel.Text = "ERROR";
                }

            }
            catch (Exception exception)
            {
                // If this is an unknown status, 
                // it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                StatusLabel.Text = "Receive failed with error: " + exception.Message;
                // Could retry, but for this simple example
                // just close the socket.

                closing = true;
                clientSocket.Dispose();
                clientSocket = null;
                connected = false;

            }
             */
            #endregion
        }

        private async void tryConnect()
        {
            if (connected)
            {
                StatusLabel.Text = "Status: al verbonden";
                return;
            }
            try
            {
                StatusLabel.Text = "Status: proberen te verbinden...";
                serverHost = new HostName(serverHostnameString);
                // Try to connect to the 
                await clientSocket.ConnectAsync(serverHost, serverPort);
                connected = true;
                StatusLabel.Text = "Status: verbinding gemaakt";

            }
            catch (Exception exception)
            {
                // If this is an unknown status, 
                // it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                StatusLabel.Text = "Status: connectie error: " + exception.Message;
                // Could retry the connection, but for this simple example
                // just close the socket.

                closing = true;
                // the Close method is mapped to the C# Dispose
                clientSocket.Dispose();
                clientSocket = null;
            }
        }

        private async void sendData(string data)
        {
            if (!connected)
            {
                StatusLabel.Text = "Status: Must be connected to send!";
                return;
            }
            UInt32 len = 0; // Gets the UTF-8 string length.
            try
            {
                StatusLabel.Text = "Status: Trying to send data ...";

                // add a newline to the text to send
                string sendData = data;
                DataWriter writer = new DataWriter(clientSocket.OutputStream);
                len = writer.MeasureString(sendData); // Gets the UTF-8 string length.

                // Call StoreAsync method to store the data to a backing stream
                await writer.StoreAsync();

                StatusLabel.Text = "Status: Data was sent" + Environment.NewLine;

                // detach the stream and close it
                writer.DetachStream();
                writer.Dispose();

            }
            catch (Exception exception)
            {
                // If this is an unknown status, 
                // it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                StatusLabel.Text = "Status: Send data or receive failed with error: " + exception.Message;
                // Could retry the connection, but for this simple example
                // just close the socket.

                closing = true;
                clientSocket.Dispose();
                clientSocket = null;
                connected = false;
            }
            /*
            // Now try to receive data from server
            try
            {
                StatusLabel.Text = "Trying to receive data ...";

                DataReader reader = new DataReader(clientSocket.InputStream);
                // Set inputstream options so that we don't have to know the data size
                reader.InputStreamOptions = InputStreamOptions.Partial;
                await reader.LoadAsync(reader.UnconsumedBufferLength);

            }
            catch (Exception exception)
            {
                // If this is an unknown status, 
                // it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                StatusLabel.Text = "Receive failed with error: " + exception.Message;
                // Could retry, but for this simple example
                // just close the socket.

                closing = true;
                clientSocket.Dispose();
                clientSocket = null;
                connected = false;

            }
            */
        }

        private void GoToRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistreerPagina));
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HelpPagina));
        }

    }
}
