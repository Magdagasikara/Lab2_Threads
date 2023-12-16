using System.Collections.Concurrent;
using Lab2_Threads.Models;

namespace Lab2_Threads
{
    internal class Program
    {
        //vid stopDuration hastighet är 0!

        // 10+ är 10 men även texten ska ändras att den är i mål (inte hastigheten)
        // avslutstext när alla klara. avbryter jag tidsräknaren? 
        // cancellation tokens då? ska man även använda dem för att bilar ska avsluta efter 10km?


        static int seconds = 1; // the universial time of the race
        static int carNumber = 0;
        public static int totalDistance = 0;
        static ThreadLocal<int> localSeconds = new ThreadLocal<int>(true); // local seconds for each car
        static ThreadLocal<int> stopDuration = new ThreadLocal<int>(true); // seconds
        static ThreadLocal<int> stopDurationHelper = new ThreadLocal<int>(true); // locally in Obstacle
        static ThreadLocal<int> randomNumber = new ThreadLocal<int>(true);
        static ThreadLocal<int> eventsCount = new ThreadLocal<int>(true); // events = obstacles, getting to the goal
        static ThreadLocal<string> obstacle = new ThreadLocal<string>(true);
        static ThreadLocal<bool> finito = new ThreadLocal<bool>(true);
        static Object lockObject1 = new Object();
        static Object lockObject2 = new Object();
        static Object lockObject3 = new Object();

        static void Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            totalDistance = 10;

            // create cars
            List<Car> carList = CreateCars();
            carNumber = carList.Count;

            // create and start car-threads
            List<Thread> carThreads = new List<Thread>();
            foreach (Car car in carList) carThreads.Add(new Thread(() => Race(car, totalDistance)));
            foreach (Thread thread in carThreads) thread.Start();

            // universal time counter
            Thread threadT = new Thread(() => CountTime(token));
            threadT.Start();

            // getting updates by pressing Enter
            Thread threadU = new Thread(() => GetRaceUpdate(carList, token));
            threadU.Start();

            // when all cars are done it's time to finish the race 
            foreach (Thread thread in carThreads) thread.Join();
            tokenSource.Cancel();

        }
        static void Print(int row, string[] msgs)
        {
            lock (lockObject1)
            {
                Console.SetCursorPosition(0, row);
                foreach (string msg in msgs) Console.WriteLine(msg);
            }
        }

        static void CountTime(CancellationToken token) // räknar tävlingens tid i sekunder 
        {
            Console.CursorVisible = false;

            string[] x = new string[1];
            x[0] = "Tryck Enter om du vill veta hur det går i tävlingen! ";
            Print(0, x);
            while (!token.IsCancellationRequested)
            {

                x[0] = $"Tävlingen har nu pågått {seconds:###0} sekunder";
                Print(1, x);
                Thread.Sleep(1000);
                seconds++;

            }
            Print(0, new string[] { $"Tävlingen är klar efter {seconds:###0} sekunder                               " });
            Print(1, new string[] { "                                                                              " });
        }

        static void Race(Car car, int totalDistance) // utför tävlingen 
        {
            Console.Clear();

            localSeconds.Value = 1;
            stopDuration.Value = 0;
            while (true)
            {
                if (localSeconds.Value % 30 == 0)
                {
                    stopDuration.Value = Obstacle(car);
                }

                if (stopDuration.Value == 0)
                {
                    car.Distance += car.SpeedPerH / 3600m;
                    if (car.Distance >= totalDistance)
                    {
                        lock (lockObject3)
                        {
                            string[] x = new string[1];
                            if (finito.Values.All(x => x == false))
                            {
                                Print(4 + carNumber + eventsCount.Values.Sum(), new string[] { $"{car.Name} är först i mål!!!!                                                          " });
                            }
                            else
                            {
                                Print(4 + carNumber + eventsCount.Values.Sum(), new string[] { $"{car.Name} har kommit i mål!                                                           " });
                            }
                            eventsCount.Value++;
                        }
                        finito.Value = true;

                        break;
                    }
                }
                else
                {
                    stopDuration.Value--;
                }

                Thread.Sleep(1000);
                localSeconds.Value++;
            }
        }
        static int Obstacle(Car car) // returnerar tid som bilen inte kan röra sig 
        {
            stopDurationHelper.Value = 0;
            obstacle.Value = "";

            Random r = new Random();
            randomNumber.Value = r.Next(0, 50);

            lock (lockObject2)
            {
                if (randomNumber.Value < 1)
                {
                    obstacle.Value = $"[{localSeconds.Value}sek av tävlingen] {car.Name} har slut på bensin. Tankar 30sek.";
                    eventsCount.Value++;
                    stopDurationHelper.Value = 30;
                }
                else if (randomNumber.Value < 3)
                {
                    obstacle.Value = $"[{localSeconds.Value}sek av tävlingen] Fck it! {car.Name} fick punka. Däckbyte 20sek.";
                    eventsCount.Value++;
                    stopDurationHelper.Value = 20;
                }
                else if (randomNumber.Value < 8)
                {
                    obstacle.Value = $"[{localSeconds.Value}sek av tävlingen] {car.Name} HAR FÅGEL!!! Tvättar vindrutan i 10sek.";
                    eventsCount.Value++;
                    stopDurationHelper.Value = 10;
                }
                else if (randomNumber.Value < 18)
                {
                    car.SpeedPerH -= 1;
                    obstacle.Value = $"[{localSeconds.Value}sek av tävlingen] {car.Name} fick motorfel :( Får sakta ner lite..";
                    eventsCount.Value++;
                }

                if (obstacle.Value != "")
                {

                    Print(4 + carNumber + eventsCount.Values.Sum(), new string[] { obstacle.Value });

                }
            }
            return stopDurationHelper.Value;
        }

        static void GetRaceUpdate(List<Car> carList, CancellationToken token)
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.Enter)
                {

                    string[] x = new string[carList.Count + 1];
                    x[0] = $"Det här är sååå schpännande!! Sekund {seconds} av tävlingen: ";
                    int carCount = 1;
                    foreach (Car car in carList)
                    {
                        if (car.Distance == 10) x[carCount] = $"* {car.Name} är i mål                           ";
                        else x[carCount] = $"* {car.Name} kämpar på med hastighet {car.SpeedPerH} km/h, har nu kommit {car.Distance:#0.#0}km på vägen";
                        carCount++;
                    }
                    Print(3, x);
                    
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

}
