using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PizzaServer
{
    public class Server
    {
        private TcpClient incomingClient;
        private System.Timers.Timer updateTimer;
        private static KlantDatabase KDB;

        public Server()
        {
            //updaten via timers > http://stackoverflow.com/questions/1435876/do-c-sharp-timers-elapse-on-a-separate-thread
            //updateTimer = new System.Timers.Timer(1000);
            //updateTimer.Elapsed += updateURLs;
            //updateTimer.Start();

            KDB = new KlantDatabase();
            KDB.loadLogins();
      
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1330);
            listener.Start();

            Console.WriteLine("Waiting for connection...");
            while (true)
            {
                //AcceptTcpClient waits for a connection from the client
                incomingClient = listener.AcceptTcpClient();
                //start a new thread to handle this connection so we can go back to waiting for another client
                Thread thread = new Thread(HandleClientThread);
                thread.IsBackground = true;
                thread.Start(incomingClient);
            }
        }

        private void HandleClientThread(object obj)
        {
            //DataReader reader = new DataReader(clientSocket.InputStream);
            TcpClient client = obj as TcpClient;
            Klant klant = null;

            Console.WriteLine("Connection found!");
            while (true)
            {
                // data lezen
                byte[] buffer = new byte[50000];
                int totalRead = 0;
                do
                {
                    int read = client.GetStream().Read(buffer, totalRead, buffer.Length - totalRead);
                    totalRead += read;
                } while (client.GetStream().DataAvailable);

                // data afhandelen
                string received = Encoding.Unicode.GetString(buffer, 0, totalRead).ToLower();

                Console.WriteLine("\nResponse from client: {0}", received);

                string[] splitted = received.Split('@');
                byte[] bytes = null;
                switch (splitted[0])
                {
                    case "Con": 
                        
                        //klant = KDB.find(splitted[1]);
                        if (klant != null)
                        {
                            if (!klant.autheticate(splitted[2]))
                            {
                                klant = null;
                            }
                        }
                        bytes = Encoding.Unicode.GetBytes("OK");

                        break;
                    case "FDC":
                        Console.WriteLine("client has disconnected");
                        client.Close();
                        klant = null;
                        return;
                    case "GPS":
                        break;
                    case "Reg": // get string as "Reg@Username@password"
                        Klant k = new Klant(splitted[1], splitted[2]);
                        break; 
                    default:
                        break;
                }

                // data terugsturen
                client.GetStream().Write(bytes, 0, bytes.Length);
            }
        }

        private void handleConnect()
        {
            
        }
    }
}