using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellySimulation
{
    internal class Spring
    {
        public int P1I1, P1I2, P1I3, P2I1, P2I2, P2I3;

        private readonly float c;
        private readonly float l;
        public float Force(float length) => -c * (length - l);

        public Spring(int p1I1, int p1I2, int p1I3, float c, float l) : this(c, l)
        {
            P1I1 = p1I1;
            P1I2 = p1I2;
            P1I3 = p1I3;
        }

        public Spring(int p1I1, int p1I2, int p1I3, int p2I1, int p2I2, int p2I3, float c, float l) : this(c, l)
        {
            P1I1 = p1I1;
            P1I2 = p1I2;
            P1I3 = p1I3;
            P2I1 = p2I1;
            P2I2 = p2I2;
            P2I3 = p2I3;
        }

        private Spring(float c, float l)
        {
            this.c = c;
            this.l = l;
        }
    }
}
