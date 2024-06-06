using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{

    internal class Program
    {
        public static List<Baraja> barajas = new List<Baraja>();
        public static List<string> playerNames = new List<string>();

        static void Main(string[] args)
        {
            Menu();   
        }

        public static void Menu()
        {
            Console.WriteLine("Escojer juego: ");
            Console.WriteLine("1 - Batalla");
            Console.WriteLine("2 - Poker");

            int opcion = Int32.Parse(Console.ReadLine());
            switch (opcion)
            {
                case 1:
                    Console.WriteLine("Introducir numero de jugadores: ");
                    int n = Int32.Parse(Console.ReadLine());


                    Repartir(n);

                    Jugar();
                    break;
                case 2:

                    bool stop = false;
                    while (!stop)
                    {
                        //Console.Clear();
                        //Repartir(1);
                        
                        poker test = new poker();
                        Baraja inicial = PopulateBaraja();
                        
                        List<ManoPoker> listaManos = new List<ManoPoker>();
                        ManoPoker mano = new ManoPoker();
                        ManoPoker mano2 = new ManoPoker();
                        listaManos.Add(mano);

                        test.repartirCartas(1, inicial, listaManos, mano2);

                        /*
                        List<Carta> test2 = new List<Carta>();
                        test2 = enganyifa();
                        */

                        /*
                        poker.eHands ma = test.testing(test2);
                        Console.WriteLine(ma);
                        */                        
                        //if (ma == poker.eHands.Straight || ma == poker.eHands.StraigthFlush)
                            stop = true;

                        Console.ReadKey();
                    }
                    break;
            }
        }

        public static List<Carta> enganyifa()
        {
            List<Carta> modifiedHand = new List<Carta>();

            modifiedHand.Add(new Carta(1, Carta.ePalo.Hearts));
            modifiedHand.Add(new Carta(12, Carta.ePalo.Spades));
            modifiedHand.Add(new Carta(2, Carta.ePalo.Spades));
            modifiedHand.Add(new Carta(4, Carta.ePalo.Spades));
            modifiedHand.Add(new Carta(3, Carta.ePalo.Spades));
            modifiedHand.Add(new Carta(1, Carta.ePalo.Diamonds));
            modifiedHand.Add(new Carta(5, Carta.ePalo.Spades));
            

            return modifiedHand;
        }

        public static void Jugar()
        {
            int turno = 1;

            foreach (Baraja b in barajas)
            {
                b.ImprimirBaraja();
                Console.WriteLine("--------");
            }

            while (barajas.Count > 1)
            {

                // guardamos en la pos i la carta que ha sacado el jugador i en este turno
                List<Carta> cartasJugadas = new List<Carta>();
                for (int i = 0; i < barajas.Count; ++i)
                    cartasJugadas.Add(barajas[i].RobarCarta());

                Console.WriteLine("Resultados turno " + turno);
                for (int i = 0; i < cartasJugadas.Count; ++i)
                    Console.WriteLine(playerNames[i] + ": " + cartasJugadas[i].ToString()); // ToString is not needed in this case
                

                int ganador = PickWinner(cartasJugadas);

                Console.WriteLine("Ganador del turno: " + playerNames[ganador]);

                GiveWinnerCards(ganador, cartasJugadas);

                CheckPlayers();

                Console.WriteLine("-----------------------");
                Console.ReadKey();
                ++turno;
            }

            Console.WriteLine("Ganador del juego: " + playerNames[0]); // es el ultimo que haya sobrevivido
            Console.ReadKey();
        }

        // we check if there are any players without cards in case of founding them we eliminate them
        public static void CheckPlayers()
        {
            List<int> positionsToDelete = new List<int>(); 
            for (int i = 0; i < barajas.Count; ++i)
            {
                if (barajas[i].NumeroCartas() == 0)
                    positionsToDelete.Add(i);
                else
                {
                    Console.WriteLine("Al jugador " + playerNames[i] + " le quedan " + barajas[i].NumeroCartas());
                }
            }

            for (int i = 0; i < positionsToDelete.Count; ++i)
            {
                int index = positionsToDelete[i] - i;
                barajas.Remove(barajas[index]); 
                Console.WriteLine("El jugador " + playerNames[index] + " ha sido eliminado");
                playerNames.Remove(playerNames[index]);
                
            }
        }

        // gives the winner of the cards played in that round
        public static void GiveWinnerCards(int ganador, List<Carta> cartas)
        {
            foreach (Carta carta in cartas)
                barajas[ganador].AddCartaToBaraja(carta);
        }


        // returns the position in the list of the higgest card
        public static int PickWinner(List<Carta> cartasJugadas)
        {
            int max = 0; // i with the higgest card value
            for (int i = 0; i < cartasJugadas.Count; ++i)
            {
                if (cartasJugadas[i].Valor > cartasJugadas[max].Valor)
                    max = i;
            }

            return max;
        }

        // REPARTE las cartas de manera aleatoria entre los jugadores
        public static void Repartir(int numJugadores) 
        {
            for (int i = 0; i < numJugadores; ++i)
                playerNames.Add("Player " + (i + 1));

            for (int i = 0; i < numJugadores; ++i)
                barajas.Add(new Baraja(new List<Carta>()));

            Baraja inicial = PopulateBaraja();
            inicial.Barajar();

            int contador = 0;
            while (true)
            {
                Carta cartaARepartir = inicial.RobarCarta();

                // si ya hemos repartido todas las cartas acabamos
                if (cartaARepartir == null)
                    break;

                // añadimos al jugador que le toque la carta que se esta repartiendo
                barajas[contador % numJugadores].AddCartaToBaraja(cartaARepartir);
                ++contador;
            }
        }
        
        // returns a populated baraja
        public static Baraja PopulateBaraja()
        {
            Baraja baraja = new Baraja(new List<Carta>());
            

            Carta cardToAdd = null;
            foreach (Carta.ePalo palo in Enum.GetValues(typeof(Carta.ePalo)))
            {
                for (int j = 1; j < 14; ++j)
                {
                    cardToAdd = new Carta(j, palo);
                    baraja.AddCartaToBaraja(cardToAdd);
                }
            }

            return baraja;
        }
    }
}
