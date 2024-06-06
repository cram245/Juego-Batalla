using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class poker
    {
        public enum eHands { Single, Pair, DublePair, Trio, Straight, Flush, Full, Poker, StraigthFlush};


        public poker()
        {

        }

        public void repartirCartas(int numPlayers, Baraja baraja, List<ManoPoker> manosPlayers, ManoPoker mesa)
        {
            baraja.Barajar();

            // cada jugador tiene dos cartas suyas
            foreach (ManoPoker mano in manosPlayers)
            {
                mano.Mano.Add(baraja.PeekNextCard());
                mano.Mano.Add(baraja.PeekNextCard());
            }

            // la mesa tiene 5 cartas
            for (int i = 0; i < 5; ++i)
                mesa.Mano.Add(baraja.PeekNextCard());
        }



        /*
            Para evaluar cada mano usamos una tupla que en el primer parametro tendra un Ehand
            En el segundo deberá llevar una lista de ints con los valores de desempate, ordenados
            de más peso a menos, por ej. {value del poker, 5 carta}, {valor pareja, 3ra carta, 4rta , 5a}


            Segunda idea: Usar dos listas una con el numero de apariciones de cada valor y otra con el numero de
            apariciones de cada palo. esto nos permite detectar parejas, trios i pokers. 
            El 1 lo vamos a contar dos veces como 1 i como 14

            Tercera idea: Estan ordenados por valor de la carta
            Para ordenar por cantidad de repeticiones en caso de empate también vemos el valor.
            Para los desempates usamos una tupla <eHands, List<Carta>>
            Para ver el tema del color si que necesitaremos el count palo para no hacer todos los subgrupos de (7  5)
            Solo nos hacen falta los desempates en los casos que hayan empates

            Hay que tener cuidado con el hecho que con nuestra manera de ordenar en caso que tuvieramos una pareja
            y necesitaramos alguna de las cartas para hacer una escalera entonces no tendriamos ordenadas y nuestro
            código no funcionaria!!

            No se pueden probar todos los subsets puesto que serian muy lentos, lo que deberiamos hacer seria quedarnos
            con la mejor mano posible y luego compararlas entre ellas
            
            Idea, simplemente hacer aleatorio el stack de cartas y luego mirar las posiciones
         
         */

        public eHands testing(List<Carta> cartas)
        {
            List<int> countNum = new List<int>() {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}; 
            List<int> countPalo = new List<int>() {0, 0, 0, 0};

            cartas = OrderCards(cartas);

            foreach (Carta carta in cartas)
            {
                ++countNum[carta.Valor];
                ++countPalo[(int)carta.Palo];
            }

            return BesteHand(cartas, countNum, countPalo);
        }

        // en max cards guardamos cual es el número de cartas que nos faltan, de manera que si solo nos faltan 3 lo maximo que
        // podremos conseguir sera un trio, en hand tenemos las cartas que ya hemos usado de cartas ordenadas
        private eHands BesteHand(List<Carta> cartasOrdenadas, List<int>countNum, List<int>countPalo)
        {
            ManoPoker escaleraColor = GetBestFlushStraight(cartasOrdenadas, countPalo);

            if (escaleraColor != null)
            {
                Console.WriteLine(escaleraColor);
                return eHands.StraigthFlush;
            }

            List<ManoPoker> pokers = GetAllNRepetitions(cartasOrdenadas, countNum, 4);
            if (pokers.Count > 0)
            {
                Console.WriteLine(pokers[0]);
                return eHands.Poker;
            }

            
            List<ManoPoker> trios = GetAllNRepetitions(cartasOrdenadas, countNum, 3);
            List<ManoPoker> parejas = GetAllNRepetitions(cartasOrdenadas, countNum, 2);

            ManoPoker fulls = GetBestFull(trios, parejas);
            if (fulls != null)
            {
                Console.WriteLine(fulls);
                return eHands.Full;
            }

            List<ManoPoker> escaleras = FindAllStraight(new ManoPoker(cartasOrdenadas));
            if (escaleras.Count() > 0)
            {
                Console.WriteLine(escaleras[0]);
                return eHands.Straight;
            }            
            
            if (trios.Count > 0)
            {
                Console.WriteLine(trios[0]);
                return eHands.Trio;
            }

            if (parejas.Count > 1)
            {
                
                Console.WriteLine(parejas[0]);
                Console.WriteLine(parejas[1]);
                
                return eHands.DublePair;
            }
            else if (parejas.Count == 1)
            {
                Console.WriteLine(parejas[0]);
                return eHands.Pair;
            }
            
            return eHands.Single;
        }

        // no hay problema con una mano que fuera J Q K A 2 puesto que ordenada tendriamos
        // (A = 14) K Q J 2 (A = 1)
        private bool AreCardsConsecutive(Carta carta1, Carta carta2)
        {
            return carta1.Valor == carta2.Valor + 1;
        }

        // si toda la mano es de un color
        private bool IsHandColored(List<Carta> hand)
        {
            Carta.ePalo color = hand[0].Palo;
            foreach (Carta carta in hand)
            {
                if (carta.Palo != color)
                    return false;
            }

            return true;
        }


        private List<Carta> OrderCards(List<Carta> cartas) 
        {
            // Verifica si hay alguna carta con valor 1
            bool hasValueOne = cartas.Any(c => c.Valor == 1);

            // Si hay alguna carta con valor 1, añade una carta con valor 14
            if (hasValueOne)
            {
                cartas.AddRange(cartas.Where(c => c.Valor == 1).Select(c => new Carta(14,c.Palo)).ToList());
            }


            return cartas
                .OrderByDescending(c => c.Valor)
                .ToList();
        }


        // completamos una mano con las mejores cartas para el desempate, que son siempre las que tengan valor más alto
        // y que no hayamos usado ya, completamos la mano en cartasUsadas
        private void AddBestCarts(List<Carta> cartasOrdenadas, List<Carta> cartasUsadas) 
        { 

            foreach (Carta c in cartasOrdenadas)
            {
                if (cartasUsadas.Count < 5) // si podemos seguir añadiendo cartas lo hacemos
                    break;
                    
                if (!cartasUsadas.Contains(c)) // si aun no esta contenido como estan ordenadas ascendientes nos toca meterla
                    cartasUsadas.Add(c);

            }
            
        }


        private List<ManoPoker> GetAllNRepetitions(List<Carta> orderedCards, List<int> countNum, int numRepetitions)
        {
            List<ManoPoker> setFound = new List<ManoPoker>(); 
            for (int i = 14; i > 0; --i)
            {
                if (countNum[i] >= numRepetitions)
                    setFound.Add(GetAllCardsWithValueN(orderedCards, i));   
            }

            return setFound;
        }
        

        private ManoPoker GetAllCardsWithValueN(List<Carta> cartas, int value)
        {
            return new ManoPoker(cartas.FindAll(c => c.Valor == value));
        }

        private ManoPoker GetAllCardsWithPaloP(List<Carta> cartas, Carta.ePalo palo)
        {
            return new ManoPoker(cartas.FindAll(c => c.Palo == palo));
        }


        private ManoPoker GetBestFull(List<ManoPoker> trios, List<ManoPoker> parejas) 
        {
            // seguro que nos quedamos con el primer trio
            // para la pareja podemos mirar los valores de los trios restantes y de las parejas para ver quien tiene un numerico mejor

            if (trios != null)
                return null;
            if (trios.Count == 1 && parejas.Count == 0)
                return null;

            ManoPoker manoFinal = trios[0];

            if (parejas != null) // nos quedamos con los dos primero trios
            {
                manoFinal.Mano.Add(trios[1].Mano[0]);
                manoFinal.Mano.Add(trios[1].Mano[1]);
            }
            else // hay que mirar que es mayor
            {
                // estamos en el caso anterior
                if (trios[1].Mano[0].Valor >= parejas[0].Mano[0].Valor)
                {
                    
                    manoFinal.Mano.Add(trios[1].Mano[0]);
                    manoFinal.Mano.Add(trios[1].Mano[1]);
                }
                else
                {
                    manoFinal.Mano.Add(parejas[0].Mano[0]);
                    manoFinal.Mano.Add(parejas[0].Mano[1]);
                }
            }

            return manoFinal;
        }


        private ManoPoker GetBestFlushStraight(List<Carta> orderedCards, List<int> countPalo)
        {
            List<ManoPoker> flushStraights = new List<ManoPoker>();
            for (int i = 0; i < countPalo.Count; ++i)
            {
                if (countPalo[i] > 4) // 5 cards are needed to do a straight flush 
                {
                    ManoPoker flushed = GetAllCardsWithPaloP(orderedCards, (Carta.ePalo)i);
                    List<ManoPoker> sameColorStraights = FindAllStraight(flushed);

                    if (sameColorStraights.Count > 0)
                        flushStraights.Add(sameColorStraights[0]); // nos quedamos solo con la mejor de cada color
                }
            }

            if (flushStraights.Count == 0)
                return null;

            int bestPos = 0;
            foreach (ManoPoker flushStraight in flushStraights)
            {
                if (flushStraight.Mano[0].Valor > flushStraights[bestPos].Mano[0].Valor) // miramos en la carta en la que acaban
                    bestPos = flushStraights.IndexOf(flushStraight);
            }

            return flushStraights[bestPos];
        }


        // cartas esta ordenado de manera decreciente segun el valor de cada carta
        // devuelve todas las escaleras ordenadas de forma decreciente según su valor
        private List<ManoPoker> FindAllStraight(ManoPoker cartas)
        {
            List<ManoPoker> straights = new List<ManoPoker>();
            for (int i = 0; i < cartas.Mano.Count - 4; ++i) 
            {
                if (IsAStraight(cartas, i))
                {
                    ManoPoker straight = new ManoPoker(new List<Carta>());
                    for (int j = 0; j < 5; ++j)
                        straight.Mano.Add(cartas.Mano[i + j]);

                    straights.Add(straight);
                }
                
            } 

            return straights;
        }


        // miramos si en [startPos, startPos + 4] forman una escalera
        private bool IsAStraight(ManoPoker mano, int startPos)
        {

            for (int i = startPos; i < startPos + 5; ++i)
                if (i < startPos + 4 && !AreCardsConsecutive(mano.Mano[i], mano.Mano[i + 1]))
                    return false;

            return true;
        }

        // all cards of mano must have the same color, mano poker is order descendently, 1 must be discarted
        private ManoPoker GetBestFlush(ManoPoker mano)
        {

            if (mano.Mano.Count < 5)
                return null;
            else if (mano.Mano.Count == 5)
                return mano;
            
            else
            {
                return new ManoPoker(mano.Mano.Take(5).ToList()); // no hay que preocuparse por repeticiones ya que no hay dos cartas con un mismo valor del mismo palo
            }
            
        }

    }
}
