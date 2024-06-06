using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Baraja
    {
        private List<Carta> _cartas;
        private int peekCounter;

        public Baraja(List<Carta> cartas)
        {
            _cartas = cartas;
            peekCounter = -1;
        }

        public void AddCartaToBaraja(Carta carta)
        {
            _cartas.Add(carta);
        }

        public Carta RobarCarta() { 
            
            if (_cartas.Count == 0)
                return null;

            Carta c = _cartas[0];
            _cartas.Remove(c); // la eliminamos de la list

            return c;
        }

        public Carta CogerCartaN(int n) // n must be 0-indexed
        {
            if (_cartas.Count == 0 || _cartas.Count - 1 < n)
                return null;

            Carta aux = _cartas[n];
            _cartas.Remove(aux); // la eliminamos del stack, supone que no hay cartas duplicadas

            return aux;
        }

        public Carta CogerCartaAlAzar() 
        {
            if (_cartas.Count == 0)
                return null;

            Random random = new Random();
            int randomNumber = random.Next(0, _cartas.Count - 1);

            return this.CogerCartaN(randomNumber);
        }

        public void Barajar()
        {

            Random random = new Random();
            int n = _cartas.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1); // Selecciona un índice aleatorio
                
                Carta temp = _cartas[i];
                _cartas[i] = _cartas[j];
                _cartas[j] = temp;
            }
        }

        public void ImprimirBaraja()
        {
            foreach (Carta carta in _cartas)
                Console.WriteLine(carta.ToString());
            
        }

        public int NumeroCartas()
        {
            return _cartas.Count;
        }


        // reparte todas las cartas de la baraja en las barajas de la lista
        // no modifica el objeto de baraja inicial
        public void RepartirEn(int cartasPorBaraja, List<Baraja> barajas)
        {
            if (cartasPorBaraja < 1)
                return;

            int n = barajas.Count;
            if (cartasPorBaraja * n > _cartas.Count)
                return;

            for (int i = 0; i < cartasPorBaraja * n; ++i) 
            {
                barajas[i % n].AddCartaToBaraja(_cartas[i]);
            }
        }

        // miramos la carta en la posicion peekCounter
        public Carta PeekNextCard()
        {
            if (peekCounter + 1 > _cartas.Count - 1)
                return null;

            return _cartas[++peekCounter];
        }
    }
}
