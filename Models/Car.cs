using Lab2_Threads.Functions;

namespace Lab2_Threads.Models
{
    public class Car
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
                if (value > Program.totalDistance) distance = Program.totalDistance;
                else distance = value;
            }
        } 

        public Car(string name)
        {
            Name = name;
            SpeedPerH = 120;
            Distance = 0;
        }
    }
}
