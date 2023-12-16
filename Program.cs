using System.Collections.Concurrent;
using Lab2_Threads.Models;
using Lab2_Threads.Functions;

namespace Lab2_Threads
{
    public class Program
    {
        //vid stopDuration hastighet är 0!


        public static int carNumber = 0;
        public static int totalDistance = 0;


        static void Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            totalDistance = 3;

            // create cars
            List<Car> carList = Helpers.CreateCars();
            carNumber = carList.Count;

            // create and start car-threads
            List<Thread> carThreads = new List<Thread>();
            foreach (Car car in carList) carThreads.Add(new Thread(() => RaceFunctions.Race(car, totalDistance)));
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

        }

    }

}
