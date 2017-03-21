using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppCIELO
{
    public class BrazaR
    {
        
        private string _prefijo;
        private string _folio;
        private string _reserva;
        private string _habi;
        private string _nombre;

        

        public string Prefijo
        {
            get { return _prefijo; }
            set { _prefijo = value; }
        }

        public string Folio
        {
            get { return _folio; }
            set { _folio = value; }
        }

        public string Reserva
        {
            get { return _reserva; }
            set { _reserva = value; }
        }

        public string Habi
        {
            get { return _habi; }
            set { _habi = value; }
        }

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }
    }
}