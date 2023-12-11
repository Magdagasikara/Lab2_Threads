using System.Diagnostics;
using System.Threading;

namespace Lab2_Threads
{
    internal class Program
    {

        // 10+ är 10 men även texten ska ändras att den är i mål (inte hastigheten)
        // avslutstext när alla klara 
        // avbryter jag threads då?
        // skriv ut sekund varje sekund för kontroll

        static int seconds = 0; // tiden tävlingen har pågått
        static ThreadLocal<int> stopDuration = new ThreadLocal<int>(true); // seconds
        static ThreadLocal<decimal> distance = new ThreadLocal<decimal>(true); // kilometers
        static bool finito = false; // bool if any car reached the goal

        static void Main(string[] args)
        {
            int numberOfCars = 0;
            Console.Write("Hej, hur många bilar ska va med i tävlingen? (rekommenderar max 5) ");
            while (true)
            {

                if (int.TryParse(Console.ReadLine(), out numberOfCars) && numberOfCars > 1) break;
                Console.Write("Fel input. Minimum 2 bilar krävs. Hur många vill du följa? ");

            }

            List<Car> carList = new List<Car>();
            for (int i = 0; i < numberOfCars; i++)
            {
                Console.Write($"Ange namn på bil {i + 1}: ");
                string input = Console.ReadLine();
                carList.Add(new Car(input));
            }

            List<Thread> carThreads = new List<Thread>();
            foreach (Car car in carList)
            {
                carThreads.Add(new Thread(() => Race(car, 10)));

            }

            foreach (Thread thread in carThreads)
            {
                thread.Start();
            }

            Thread threadU = new Thread(() => GetUpdate(carList));
            threadU.Start();

            foreach (Thread thread in carThreads)
            {
                thread.Join();
            }
        }
        static void Race(Car car, int totalDistance)
        {
            Console.WriteLine($"Jag kör igång!! brrruumm brrrum brum /{car.Name}");

            seconds = 1;
            int stopDuration = 0;
            while (true)
            {
                Thread.Sleep(1000);
                if (seconds % 30 == 0)
                {
                    stopDuration = Obstacle(car);
                }

                if (stopDuration == 0)
                {
                    car.Distance += car.SpeedPerH / 3600m;
                }

                if (car.Distance >= totalDistance)
                {
                    if (finito == false)
                    {
                        Console.WriteLine($"{car.Name} är först i mål!!!!");
                        finito = true;
                    }
                    else Console.WriteLine($"{car.Name} har kommit i mål!");
                    break;
                }
                stopDuration = stopDuration > 0 ? stopDuration-- : stopDuration;
                seconds++;

            }
        }
        static int Obstacle(Car car)
        {
            // returnerar tid som bilen inte kan röra sig
            Random r = new Random();
            int radomNumber = r.Next(0, 50);
            if (radomNumber < 1)
            {
                //Slut på bensin. Behöver tanka, stannar 30 sekunder
                Console.WriteLine($"\n[{seconds}sek av tävlingen] {car.Name} har slut på bensin. Tankar 30sek.");
                return 30;
            }
            else if (radomNumber < 3)
            {
                //Punktering. Behöver byta däck, stannar 20 sekunder
                Console.WriteLine($"\n[{seconds}sek av tävlingen] Fuck it! {car.Name} fick punka. Däckbyte 20sek.");
                return 20;
            }
            else if (radomNumber < 8)
            {
                //Fågel på vindrutan. Behöver tvätta vindrutan, stannar 10 sekunder
                Console.WriteLine($"\n[{seconds}sek av tävlingen] {car.Name} HAR FÅGEL!!! Torkar vindrutan i 10sek.");
                return 10;
            }
            else if (radomNumber < 18)
            {
                //Motorfel. Hastigheten på bilen sänks med 1km / h
                car.SpeedPerH -= 1;
                Console.WriteLine($"\n[{seconds}sek av tävlingen] {car.Name} fick motorfel :( Får sakta ner lite..");
                return 0;
            }
            else return 0;
        }

        static void GetUpdate(List<Car> carList)
        {
            while (true)
            {
                Console.WriteLine("Tryck Enter om du vill veta hur det går i tävlingen! ");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine($"\nDet här är sååå schpännande!! sekund {seconds} av tävlingen: ");
                    foreach (Car car in carList)
                    {
                        Console.WriteLine($"* {car.Name} kämpar på med hastighet {car.SpeedPerH}, har nu kommit {car.Distance:0.##}km på vägen");// hur skriver jag ut per bil?
                    }
                }
            }
        }
    }
    internal class Car
    {
        public string Name { get; set; }
        public int SpeedPerH { get; set; }
        
        private decimal distance;
        public decimal Distance
        {
            get
            {
                return distance;
            }
            set
            {
                if (value > 10) distance = 10;
                else distance = value;
            }
        }// hmm osäker om det ska verkligen vara en property...

        public Car(string name)
        {
            Name = name;
            SpeedPerH = 120;
            Distance = 0;
        }
    }
}
