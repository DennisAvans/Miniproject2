using System;
using System.Collections.Generic;
using System.IO;

namespace PizzaServer
{
    [Serializable()]
    public class KlantDatabase
    {
        public Dictionary<string, string> _logins { get; set; }
        static string _filename = "klanten.wietje";
        Serializer _serializer;

        public KlantDatabase()
        {
            _logins = new Dictionary<string, string>();
            _serializer = new Serializer();
        }

        public void add(string credentials)
        {
            string[] splitted = credentials.Split(new string[] { "\n", "\r\n", ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (!_logins.ContainsKey(splitted[0]))
            {
                _logins.Add(splitted[0], splitted[1]);
            }
        }

        public bool loginValid(string name, string password)
        {
            string ActualPassword;
            return _logins.TryGetValue(name, out ActualPassword) &&
                       ActualPassword.Equals(password);
        }

        public bool userExist(string key)
        {
            return _logins.ContainsKey(key);
        }
        public void del(string key)
        {
            _logins.Remove(key);
        }

        public void empty()
        {
            _logins.Clear();
        }

        public int getSize()
        {
            return _logins.Count;
        }

        // save the arraylist of measurements to a file
        public void saveLogins()
        {
            _serializer.SerializeObject(_filename, _logins);
        }

        // load the arraylist of measurements from a file, returns the arraylist
        public void loadLogins()
        {
            if (File.Exists(_filename))
                this._logins = (Dictionary<string, string>)_serializer.DeSerializeObject(_filename);
        }
    }
}
