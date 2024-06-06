using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class ManoPoker
    {
        private List<Carta> _mano;
        public List<Carta> Mano { get { return _mano; } }

        public ManoPoker() 
        { 
            _mano = new List<Carta>();
        }

        public ManoPoker(List<Carta> mano)
        {
            _mano = mano;
        }


        public override string ToString()
        {
            string s = "\n";
            
            foreach (Carta c in _mano)
            {
                s +=  " " + c.ToString();
            }
            return s;
        }
    }
}
