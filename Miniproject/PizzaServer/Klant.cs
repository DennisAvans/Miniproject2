namespace PizzaServer
{
    public class Klant
    {
        private string adres;
        private string username { get; set; }
        private string password { get; set; }


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

    }
}
