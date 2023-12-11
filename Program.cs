using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;

namespace Lab2_Threads
{
    internal class Program
    {

        // 10+ är 10 men även texten ska ändras att den är i mål (inte hastigheten)
        // avslutstext när alla klara. avbryter jag tidsräknaren? 
        // cancellation tokens då? ska man även använda dem för att bilar ska avsluta efter 10km?
        // hit a human -> withdraw from race => if one car left end the race(? or continue but say sth)


        // här måste jag städa när jag testat klart
        static int seconds = 1; // tiden tävlingen har pågått
        static int obstacleCount = 0;
        static ThreadLocal<int> localSeconds = new ThreadLocal<int>(true); // local seconds för kontroll
        static ThreadLocal<int> stopDuration = new ThreadLocal<int>(true); // sekunder
        static ThreadLocal<decimal> distance = new ThreadLocal<decimal>(true); // kilometer
        static bool finito = false; // bool if any car reached the goal.
                                    // skulle kunna kolla int och räkna antal bilar som är i mål..
                                    // men då måste jag dela antalet bilar eller listan med Race :/
        static List<string> obstaclesEncountered = new List<string>(); //annars skrevs det om varje gång
        // men det här delas av alla bilar så det blir kaos
        Object lockObject=new Object();
        // jag tror att Obstacle behöver få eget tråd? nää

        static void Main(string[] args)
        {


            List<Car> carList = CreateCars();

            // create and start threads
            List<Thread> carThreads = new List<Thread>();
            foreach (Car car in carList)
            {
                carThreads.Add(new Thread(() => Race(car, 10)));

            }
            foreach (Thread thread in carThreads)
            {
                thread.Start();
            }

            Thread threadT = new Thread(CountTime);
            threadT.Start();

            Thread threadU = new Thread(() => GetRaceUpdate(carList));
            threadU.Start();

            foreach (Thread thread in carThreads)
            {
                thread.Join();
            }

            //threadT.Abort();//byt till cancelation token sen
        }

        static void CountTime() // räknar tävlingens tid i sekunder 
        {
            Console.Clear();
            Console.CursorVisible = false;

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Tryck Enter om du vill veta hur det går i tävlingen! ");

            while (true)
            {
                Console.SetCursorPosition(0, 1);
                Console.WriteLine($"Tävlingen har nu pågått {seconds:###0} sekunder");
                Thread.Sleep(1000);
                seconds++;
            }
        }

        static void Race(Car car, int totalDistance) // utför tävlingen 
        {
            Console.SetCursorPosition(0, 3);
            Console.WriteLine($"Jag kör igång!! brrruumm brrrum brum /{car.Name}");

            localSeconds.Value = 1;
            int stopDuration = 0;
            while (true)
            {
                if (localSeconds.Value != seconds) continue;
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
                localSeconds.Value++;

            }
        }
        static int Obstacle(Car car) // returnerar tid som bilen inte kan röra sig 
        {
            int stopDuration = 0;

            Random r = new Random();
            int radomNumber = r.Next(0, 50);

            if (radomNumber < 1)
            {
                obstaclesEncountered.Add($"[{seconds}sek av tävlingen] {car.Name} har slut på bensin. Tankar 30sek.");
                stopDuration = 30;
            }
            else if (radomNumber < 3)
            {
                obstaclesEncountered.Add($"[{seconds}sek av tävlingen] Fck it! {car.Name} fick punka. Däckbyte 20sek.");
                stopDuration = 20;
            }
            else if (radomNumber < 8)
            {
                obstaclesEncountered.Add($"[{seconds}sek av tävlingen] {car.Name} HAR FÅGEL!!! Tvättar vindrutan i 10sek.");
                stopDuration = 10;
            }
            else if (radomNumber < 18)
            {
                car.SpeedPerH -= 1;
                obstaclesEncountered.Add($"[{seconds}sek av tävlingen] {car.Name} fick motorfel :( Får sakta ner lite..");
            }

            Console.SetCursorPosition(0, 8);
            foreach (string obstacle in obstaclesEncountered)
            {
                Console.WriteLine(obstacle);
            }

            return stopDuration;
        }

        static void GetRaceUpdate(List<Car> carList)
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, 3);
                    Console.WriteLine($"Det här är sååå schpännande!! Sekund {seconds} av tävlingen: ");
                    foreach (Car car in carList)
                    {
                        if (car.Distance == 10) Console.WriteLine($"* {car.Name} är i mål");
                        else Console.WriteLine($"* {car.Name} kämpar på med hastighet {car.SpeedPerH} km/h, har nu kommit {car.Distance:#0.#0}km på vägen");// hur skriver jag ut per bil?
                    }
                }
            }
        }

        static List<Car> CreateCars()
        {
            int numberOfCars = 0;
            Console.Write("Hej, hur många bilar ska vara med i tävlingen? (rekommenderar max 5) ");
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
                if (input == "") input = "En random bil som du inte har kontroll över";
                string name = input;
                foreach (Car car in carList) // koll om namnet redan använts
                {
                    if (car.Name == input) name = $"{input}_2";
                }
                carList.Add(new Car(name));
            }
            return carList;
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
        }// hmm osäker om det verkligen ska vara en property...

        public Car(string name)
        {
            Name = name;
            SpeedPerH = 120;
            Distance = 0;
        }
    }
}
