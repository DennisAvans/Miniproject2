using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miniproject.Model
{
    public class PizzaJongen
    {
        public double _longitude { get; set; }
        public double _latitude { get; set; }

        public PizzaJongen()
        {

        }

        public PizzaJongen(double lon, double lat)
        {
            this._longitude = lon;
            this._latitude = lat;
        }
    }
}
