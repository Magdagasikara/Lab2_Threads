using Lab2_Threads.Functions;
using Lab2_Threads.Models;

namespace Lab2_Threads
{
    public class Program
    {
        
        public static int carNumber = 0;
        public static int totalDistance = 0;

        static void Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            totalDistance = 10; // in kilometers, set distance of the race

            // create cars
            List<Car> carList = Helpers.CreateCars();
            carNumber = carList.Count;

            // create and start car-threads
            List<Thread> carThreads = new List<Thread>();
            foreach (Car car in carList) carThreads.Add(new Thread(() => RaceFunctions.Race(car, totalDistance)));
            Console.Clear();
            foreach (Thread thread in carThreads) thread.Start();
            
            // universal time counter
            Thread threadT = new Thread(() => Helpers.CountTime(token));
            threadT.Start();

            // getting updates by pressing Enter
            Thread threadU = new Thread(() => Helpers.GetRaceUpdate(carList, token));
            threadU.Start();

            // when all cars are done it's time to finish the race 
            foreach (Thread thread in carThreads) thread.Join();
            tokenSource.Cancel();

            // close down at Enter
            Console.ReadKey();
        }

    }

}
