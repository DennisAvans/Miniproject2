using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PizzaServer
{
    public class Server
    {
        private TcpClient _incomingClient;
        private KlantDatabase _klantDatabase;
        private NetworkStream _netStream = null;
        PizzaJongen jongen = new PizzaJongen();

        public Server()
        {
            _klantDatabase = new KlantDatabase();
            _klantDatabase.loadLogins();

            TcpListener listener = new TcpListener(1330);
            listener.Start();

            Console.WriteLine("Waiting for connection...");
            while (true)
            {
                //AcceptTcpClient waits for a connection from the client
                _incomingClient = listener.AcceptTcpClient();
                //start a new thread to handle this connection so we can go back to waiting for another client
                Thread thread = new Thread(HandleClientThread);
                thread.IsBackground = true;
                thread.Start(_incomingClient);
            }
        }

        private void HandleClientThread(object obj)
        {
            TcpClient client = obj as TcpClient;
            _netStream = client.GetStream();
            Console.WriteLine("Connection found!");

            while (true)
            {
                // data lezen
                int connectionCounter = 0;
                byte[] buffer = new byte[1024];
                int totalRead = 0;
                do
                {
                    int read = client.GetStream().Read(buffer, totalRead, buffer.Length - totalRead);
                    totalRead += read;
                    connectionCounter = 0;
                } while (client.GetStream().DataAvailable);

                string received = Encoding.ASCII.GetString(buffer, 0, totalRead).ToLower().Replace("\0", "");
                connectionCounter++;

                if(connectionCounter >= 10) 
                {

                }
                Console.WriteLine("\nResponse from client: {0}", received); // DEBUG

                // data afhandelen

                string[] splitted = received.Split('@');
                byte[] bytes = null;
                switch (splitted[0])
                {
                    case "register": // get string as "register@Username@password"
                        bytes = handleRegister(received);
                        break;
                    case "login": // get string as "login@Username@password"
                        bytes = handleLogin(received);
                        break;
                    case "disconnect":
                        Console.WriteLine("A client has disconnected");
                        client.Close();
                        return;
                    case "updatepizza":
                        bytes = GetBytes(jongen.getNextLocation());
                        break;
                    default:
                        bytes = Encoding.ASCII.GetBytes(received);
                        break;
                }

                // data terugsturen
                client.GetStream().WriteAsync(bytes, 0, bytes.Length);
            }
        }

        private byte[] handleRegister(string credentials)
        {
            string[] splitted = credentials.Split('@');
            if (_klantDatabase.userExist(splitted[1]))
                return GetBytes("~User already exists");
            else
            {
                _klantDatabase.add(splitted[1], splitted[2]);
                _klantDatabase.saveLogins();
                return GetBytes("~User added");
            }
        }

        private byte[] handleLogin(string credentials)
        {
            string[] splitted = credentials.Split('@');
            if (!_klantDatabase.loginValid(splitted[1], splitted[2]))
                return GetBytes("~Invalid credentials");
            else
            {
                _klantDatabase.add(splitted[1], splitted[2]);
                _klantDatabase.saveLogins();
                return GetBytes("~Valid credentials");
            }
        }

        private void handleConnect()
        {

        }

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