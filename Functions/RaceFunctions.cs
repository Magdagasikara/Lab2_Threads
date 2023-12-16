using Lab2_Threads.Models;


namespace Lab2_Threads.Functions
{
    public static class RaceFunctions
    {
        public static ThreadLocal<int> localSeconds = new ThreadLocal<int>(true); // time lapse for each car, in seconds
        public static ThreadLocal<int> stopDuration = new ThreadLocal<int>(true); // seconds
        public static ThreadLocal<int> stopDurationHelper = new ThreadLocal<int>(true); // locally in Obstacle
        public static ThreadLocal<int> randomNumber = new ThreadLocal<int>(true);
        public static ThreadLocal<int> eventsCount = new ThreadLocal<int>(true); // events = car starts, obstacles, getting to the goal
        public static ThreadLocal<string> obstacle = new ThreadLocal<string>(true);
        public static ThreadLocal<bool> finito = new ThreadLocal<bool>(true);

        static Object lockObject1 = new Object();
        static Object lockObject2 = new Object();
        static Object lockObject3 = new Object();

        public static void Race(Car car, int totalDistance) // let's race!
        {
            Console.Clear();

            localSeconds.Value = 1;
            stopDuration.Value = 0;

            // each car announces when it starts the race
            lock (lockObject1)
            {
                eventsCount.Value++;
                Helpers.Print(4 + Program.carNumber + eventsCount.Values.Sum(), new string[] { $"Brrrum brrum brum, nu kör jag!! /{car.Name}" });
            }

            // during the race cars can encounter obstacles that make them miss part of the race
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
                            eventsCount.Value++;
                            if (finito.Values.All(x => x == false))
                            {
                                Helpers.Print(4 + Program.carNumber + eventsCount.Values.Sum(), new string[] { $"{car.Name} är först i mål!!!!" });
                            }
                            else
                            {
                                Helpers.Print(4 + Program.carNumber + eventsCount.Values.Sum(), new string[] { $"{car.Name} har kommit i mål!" });
                            }
                            finito.Value = true;
                        }
                        

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

        static int Obstacle(Car car) // returns time [in seconds] when the car cannot race
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

                    Helpers.Print(4 + Program.carNumber + eventsCount.Values.Sum(), new string[] { obstacle.Value });

                }
            }
            return stopDurationHelper.Value;
        }

    }
}
