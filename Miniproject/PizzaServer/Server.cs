using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using Utils;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PizzaServer
{
    public class Server
    {
        private TcpClient incomingClient;
        private System.Timers.Timer updateTimer;

        public Server()
        {
            //updaten via timers > http://stackoverflow.com/questions/1435876/do-c-sharp-timers-elapse-on-a-separate-thread
            //updateTimer = new System.Timers.Timer(1000);
            //updateTimer.Elapsed += updateURLs;
            //updateTimer.Start();
      
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
            TcpClient client = obj as TcpClient;

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

                string extension = received.Substring(received.Length - 3, 3);
                byte[] bytes = null;
                switch (extension)
                {

                    case "Connect":
                    case "Disconnect":
                        bytes = Encoding.Unicode.GetBytes(UpdateFeed(received)); //bytes = Encoding.Unicode.GetBytes(downloader.GetFeedItems(received));
                        break;
                    case "ForceDisconnect":
                        Console.WriteLine("client has disconnected");
                        client.Close();
                        return;
                    case "forwardGPS":
                        bytes = Encoding.Unicode.GetBytes(HandleDataRequest(received));
                        break;
                    case "Register":
                        string[] splitted = received.Split('@');
                        int index = int.Parse(splitted[1]);
                        bytes = HandleLoadMoreRequest(splitted[0], index);
                        break;
                    case
                    default:
                        bytes = Encoding.Unicode.GetBytes(UpdateFeed(received));
                        break;
                }

                // data terugsturen
                client.GetStream().Write(bytes, 0, bytes.Length);
            }
        }

        private string UpdateFeed(string feedURL)
        {
            if (!urlIO.exist(feedURL))
            {
                urlIO.add(feedURL);
                urlIO.SaveURLs();
                Console.WriteLine("New URL added!");
                newFeedAdded = true;
            }

            using (XMLDownloader downloader = new XMLDownloader())
            {
                // nieuwe items die je binnen haalt
                List<FeedItem> newItems = downloader.ConvertToFeedItems(feedURL);
                if (newItems == null)
                {
                    return "OK";
                }

                // verwijder alle tekens behalve de alphanumerieke
                string filename = Regex.Replace(downloader._title, @"[^A-Za-z0-9]+", string.Empty);

                // vul de arraylist met de nieuwe items die binnen komen
                foreach (FeedItem item in newItems)
                {
                    // inladen van de totale feed van de sitenaam
                    objectIO.LoadFeeds(filename);

                    bool feedItemAlreadyExists = false;
                    foreach (FeedItem existingItem in objectIO.feedItems)
                    {
                        // check of de nieuwe item al bestaat in de arraylist met alle feeditems
                        // contains vergelijkt het object (?ofso?): het werkt niet, dus je vergelijkt alle titls en ids van de FeedItems die al bestaan met
                        // de titel en de id van de items die je ophaalt
                        if ((existingItem.Title.Equals(item.Title)) && (existingItem.ID.Equals(item.ID)))
                        {
                            feedItemAlreadyExists = true;
                            break;
                        }
                    }
                    if (!feedItemAlreadyExists)
                    {
                        objectIO.add(item);
                        objectIO.SaveFeeds(filename);
                        Console.WriteLine("Item added in " + filename + "!");
                    }
                    // clearen omdat het problemen geeft
                    objectIO.feedItems.Clear();
                }
                return "OK";
            }
        }

        private byte[] HandleLoadMoreRequest(string tabname, int index)
        {

            string filename = Regex.Replace(tabname, @"[^A-Za-z0-9]+", string.Empty);
            objectIO.LoadTempFeeds(filename, index);
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, objectIO.tempFeedItems);
            byte[] rval = fs.ToArray();
            fs.Close();
            objectIO.tempFeedItems.Clear();
            return rval;
        }

        private string HandleDataRequest(string request)
        {
            return null;
        }

        private string HandleRSSrequest(string request)
        {
            string returnMessage = "RSS request received ty very much";
            return null;
        }

        // update methode via timers
        private void UpdateURLs(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var url in urlIO._listOfURLs)
            {
                UpdateFeed(url);
                Console.WriteLine("Updated " + url);
            }
            Console.WriteLine("\n");
        }

        // update methode via threads
        private void UpdateAllFeeds(URLIO urlio)
        {
            while (true)
            {
                if (newFeedAdded)
                {
                    urlIO.LoadURLs();
                    newFeedAdded = false;
                }
                try
                {
                    foreach (var url in urlIO._listOfURLs)
                    {
                        UpdateFeed(url);
                        Console.WriteLine("Updated " + url);
                        Thread.Sleep(0);
                    }
                    Console.WriteLine("\n");
                }
                catch (Exception e)
                {
                    continue;
                }
                Thread.Sleep(0);  // Prevent 100% CPU usage

            }
        }
    }
}