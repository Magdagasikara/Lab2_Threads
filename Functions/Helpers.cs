using Lab2_Threads.Models;

namespace Lab2_Threads.Functions
{

    public static class Helpers
    {
        public static int seconds = 1; // the universial time of the race
        static Object lockObject1 = new Object();

        public static void Print(int row, string[] msgs)
        {
            lock (lockObject1)
            {
                Console.SetCursorPosition(0, row);
                foreach (string msg in msgs) Console.WriteLine(msg);
            }
        }

        public static List<Car> CreateCars()
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
                foreach (Car car in carList) // simple check if the name already exists
                {
                    if (car.Name == input) name = $"{input}_2";
                }
                carList.Add(new Car(name));
            }
            return carList;
        }

        public static void GetRaceUpdate(List<Car> carList, CancellationToken token) // info on each car position and speed
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
                        if (car.Distance == Program.totalDistance) x[carCount] = $"* {car.Name} är i mål                                                                                       ";
                        else x[carCount] = $"* {car.Name} kämpar på med hastighet {car.SpeedPerH} km/h, har nu kommit {car.Distance:#0.#0}km på vägen";
                        carCount++;
                    }
                    Print(3, x);

                }
            }
        }

        public static void CountTime(CancellationToken token) // the general timing of the race
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

    }
}
