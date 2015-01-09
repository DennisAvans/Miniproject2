using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaServer
{
    class PizzaJongen
    {
        private string currentLocation;

        public PizzaJongen()
        {
            currentLocation = "";
        }

        public PizzaJongen(string location)
        {
            this.currentLocation = location;
        }

        public void setLocation(string location)
        {
            this.currentLocation = location;
        }

    }
}
