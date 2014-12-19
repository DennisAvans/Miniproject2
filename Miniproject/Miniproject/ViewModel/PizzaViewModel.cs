using Miniproject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miniproject.ViewModel
{
    public class PizzaViewModel : INotifyPropertyChanged
    {


        Pizza _pizza;
        public PizzaViewModel()
        {
            _pizza = new Pizza
            {
                _kaas = "",
                _korst = "",
                _paddestoel = "",
                _vlees = ""

            };
        }
        public string Kaas
        {
            get { return _pizza._kaas; }
            set
            {
                if (_pizza._kaas != value)
                {
                    _pizza._kaas = value;
                    RaisePropertyChanged("Kaas");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
