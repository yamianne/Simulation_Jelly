using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace JellySimulation
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Vector3D v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }
        public static Vector3D ToVector3D(this Vector3 v)
        {
            return new Vector3D(v.X, v.Y, v.Z);
        }
    }
}
