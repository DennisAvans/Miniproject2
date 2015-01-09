using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaServer
{
    public class Klant
    {
        private string adres;
        private string username;
        private string password;

        public Klant(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public bool autheticate(string password)
        {
            if (password.Equals(this.password))
            {
                return true;
            }
            return false;
        }

        public string getUserName()
        {
            return username;
        }

    }
}
