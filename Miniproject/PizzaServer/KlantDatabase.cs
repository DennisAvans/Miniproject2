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

        public void add(string username, string password)
        {
            if (!userExist(username))
            {
                _logins.Add(username, password);
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

        public void saveLogins()
        {
            _serializer.SerializeObject(_filename, _logins);
        }

        public void loadLogins()
        {
            if (File.Exists(_filename))
                this._logins = (Dictionary<string, string>)_serializer.DeSerializeObject(_filename);
        }
    }
}
