using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaServer
{
    public class KlantDatabase
    {
        private const string filename = "klanten.dat";
        PizzaServer.Serializer serializer;
        private List<Klant> klanten;

        public KlantDatabase()
        {
            serializer = new PizzaServer.Serializer();
        }

        public void load()
        {
            try {
                klanten = (List<Klant>)serializer.DeSerializeObject(filename);
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void save()
        {
            try
            {
                serializer.SerializeObject(filename, klanten);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public bool add(string username, string password)
        {
            foreach(var k in klanten)
            {
                if(k.getUserName().Equals(username)) 
                {
                    return false;
                }
            }

            klanten.Add(new Klant(username, password));
            return true;
        }

        public Klant find(string username)
        {
            foreach (var k in klanten)
            {
                if (k.getUserName().Equals(username)) return k;
            }
            return null;
        }
    }
}
