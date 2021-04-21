using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithmPCB.Model
{
    class Segment
    {
        public string orientation { get; set; }
        public int distance { get; set; }
        public Segment(string orientation, int distance)
        {
            this.orientation = orientation;
            this.distance = distance;
        }

        public void Clear()
        {
            orientation = null;
            distance = 0;
        }

        public override string ToString()
        {
            return  " Orientation: " + orientation + " Distance:  " + distance ;
        }
    }
}
