using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellySimulation
{
    internal class SimulationPoint
    {
        public SimulationPoint(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
            Velocity = Vector3.Zero;
            Force = Vector3.Zero;
        }
        private const float MI = 0.4f;

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Force { get; set; }

        internal void Next(float precision, int max, float k)
        {
            Force -= k * Velocity;
            Position += Velocity * precision;
            Velocity += Force * precision;
            if (Position.Length() > max)
            {
                if (Position.X > max)
                {
                    Position.X = max;
                    if (Velocity.X > 0) Velocity.X *= -MI;
                }
                if (Position.X < -max)
                {
                    Position.X = -max;
                    if (Velocity.X < 0) Velocity.X *= -MI;
                }
                if (Position.Y > max)
                {
                    Position.Y = max;
                    if (Velocity.Y > 0) Velocity.Y *= -MI;
                }
                if (Position.Y < -max)
                {
                    Position.Y = -max;
                    if (Velocity.Y < 0) Velocity.Y *= -MI;
                }
                if (Position.Z > max)
                {
                    Position.Z = max;
                    if (Velocity.Z > 0) Velocity.Z *= -MI;
                }
                if (Position.Z < -max)
                {
                    Position.Z = -max;
                    if (Velocity.Z < 0) Velocity.Z *= -MI;
                }
            }
            Force = Vector3.Zero;
        }
    }
}
