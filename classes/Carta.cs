using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Carta
    {
        public enum ePalo { Hearts, Clubs, Diamonds, Spades}
        private int _valor;
        public int Valor
        {
            set
            {
                if (value < 0 || value > 14)
                    throw new Exception("El valor debe estar entre 1 i 12");
                else
                    _valor = value;
            }

            get { return _valor; }
        }

        private ePalo _palo;
        public ePalo Palo
        {
            set { _palo = value; }
            get { return _palo; }
        }

        public Carta(int valor, ePalo palo)
        {
            Valor = valor;
            Palo = palo;
        }
        public override string ToString()
        {
            return _valor + " de " + _palo.ToString();
        }
    }
}
