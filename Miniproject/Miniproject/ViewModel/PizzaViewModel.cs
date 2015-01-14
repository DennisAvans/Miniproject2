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
                _vlees = "",
                _bezorgtijd = DateTime.Now.Add(new TimeSpan(0, 0, 30, 0))

            };
        }
        public TimeSpan addTime(int mins)
        {
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(0, 0, mins, 0);
            DateTime combined = date.Add(time);

            // now to show the timespan you can use
            return TimeSpan.FromTicks(combined.Ticks);
            
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

        public string Korst
        {
            get { return _pizza._korst; }
            set
            {
                if (_pizza._korst != value)
                {
                    _pizza._korst = value;
                    RaisePropertyChanged("Korst");
                }
            }
        }
        public string Paddestoel
        {
            get { return _pizza._paddestoel; }
            set
            {
                if (_pizza._paddestoel != value)
                {
                    _pizza._paddestoel = value;
                    RaisePropertyChanged("Paddestoel");
                }
            }
        }
        public string Vlees
        {
            get { return _pizza._vlees; }
            set
            {
                if (_pizza._vlees != value)
                {
                    _pizza._vlees = value;
                    RaisePropertyChanged("Vlees");
                }
            }
        }

        public DateTime Bezorgtijd
        {
            get { return _pizza._bezorgtijd; }
            set
            {
                if (_pizza._bezorgtijd != value)
                {
                    _pizza._bezorgtijd = value;
                    RaisePropertyChanged("Bezorgtijd");
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
