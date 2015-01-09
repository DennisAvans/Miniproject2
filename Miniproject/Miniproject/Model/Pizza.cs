﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miniproject.Model
{
    public class Pizza
    {
        public string _kaas { get; set; }
        public string _vlees { get; set; }
        public string _paddestoel { get; set; }
        public string _korst { get; set; }
        public DateTime _bezorgtijd { get; set; }

        public Pizza()
        {

        }
        public Pizza(string kaas, string vlees, string paddestoel, string korst, DateTime bezorgtijd)
        {
            this._kaas = kaas;
            this._vlees = vlees;
            this._paddestoel = paddestoel;
            this._korst = korst;
            this._bezorgtijd = bezorgtijd;
        }

        public override string ToString()
        {
            return this._kaas + " " + this._vlees + " " + this._paddestoel + " " + this._korst + " " + this._bezorgtijd;
        }
    }
}
