using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellySimulation
{
    public class Config : ObservableObject
    {
        private float pointsMass = 1;
        public float PointsMass { get => pointsMass; set => SetValue(ref pointsMass, value); }

        private float elasticity1 = 1;
        public float Elasticity1 { get => elasticity1; set => SetValue(ref elasticity1, value); }

        private float elasticity2 = 2;
        public float Elasticity2 { get => elasticity2; set => SetValue(ref elasticity2, value); }

        private float damping = 1;
        public float Damping { get => damping; set => SetValue(ref damping, value); }

        private float disorder = 1;
        public float Disorder { get => disorder; set => SetValue(ref disorder, value); }

        public const int BOUNDINGBOX_SIZE = 4;
        public const int POINTS = 4;
    }
}
