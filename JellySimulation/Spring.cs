using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellySimulation
{
    internal class Spring
    {
        public int Point1_X, Point1_Y, Point1_Z;
        public int Point2_X, Point2_Y, Point2_Z;

        private readonly float c;
        private readonly float l;
        public float Force(float distance) => -c * (distance - l);

        public Spring(int p1I1, int p1I2, int p1I3, float c, float l) : this(c, l)
        {
            Point1_X = p1I1;
            Point1_Y = p1I2;
            Point1_Z = p1I3;
        }

        public Spring(int p1I1, int p1I2, int p1I3, int p2I1, int p2I2, int p2I3, float c, float l) : this(c, l)
        {
            Point1_X = p1I1;
            Point1_Y = p1I2;
            Point1_Z = p1I3;
            Point2_X = p2I1;
            Point2_Y = p2I2;
            Point2_Z = p2I3;
        }

        private Spring(float c, float l)
        {
            this.c = c;
            this.l = l;
        }
    }
}
