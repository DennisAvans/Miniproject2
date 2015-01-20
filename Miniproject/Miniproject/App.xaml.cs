using Miniproject.Model;
using Miniproject.View;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace Miniproject
{
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        public static Bestellingen _bestellingen;
        public static StreamSocket _clientSocket;
        private HostName _serverHost;

        private string _serverHostnameString = "127.0.0.1";/*  "145.48.224.168";*/
        private string _serverPort = "1330";
        public static bool _connected = false;
        public static bool _closing = false;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            _bestellingen = new Bestellingen();
            _clientSocket = new StreamSocket();
            tryConnect();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(LoginPagina), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        #region connect
        private async void tryConnect()
        {
            if (_connected)
            {
                //  StatusLabel.Text = "Status: al verbonden";
                return;
            }
            try
            {
                //  StatusLabel.Text = "Status: proberen te verbinden...";
                _serverHost = new HostName(_serverHostnameString);
                await _clientSocket.ConnectAsync(_serverHost, _serverPort);
                _connected = true;
                // StatusLabel.Text = "Status: verbinding gemaakt";

            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                //  StatusLabel.Text = "Status: connectie error: " + exception.Message;

                _closing = true;
                _clientSocket.Dispose();
                _clientSocket = null;
            }
        }
        #endregion

        #region commands sendData & readData
        public static async void sendData(string dataToSend)
        {
            if (!_connected)
            {
                // StatusLabel.Text = "Status: Must be connected to send!";
                return;
            }
            try
            {
                byte[] data = GetBytes(dataToSend);
                IBuffer buffer = data.AsBuffer();

                // StatusLabel.Text = "Status: Trying to send data ...";
                await _clientSocket.OutputStream.WriteAsync(buffer);

                //StatusLabel.Text = "Status: Data was sent" + Environment.NewLine;
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                //StatusLabel.Text = "Status: Send data or receive failed with error: " + exception.Message;
                _closing = true;
                _clientSocket.Dispose();
                _clientSocket = null;
                _connected = false;
            }
        }

        public static async Task<string> readData()
        {
            // string result = await App.readData();
            //  StatusLabel.Text = "Trying to receive data ...";
            try
            {
                // Read the string.
                IBuffer buffer = new byte[1024].AsBuffer();
                await _clientSocket.InputStream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.Partial);
                byte[] result = buffer.ToArray();
                return GetString(result);
                //     StatusLabel.Text = GetString(result);
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                //  StatusLabel.Text = "Receive failed with error: " + exception.Message;
                _closing = true;
                _clientSocket.Dispose();
                _clientSocket = null;
                _connected = false;
                return "ERROR";
            }
        }
        #endregion

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

    }
}