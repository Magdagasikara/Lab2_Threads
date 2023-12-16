using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2_Threads;

namespace Lab2_Threads.Models
{
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
