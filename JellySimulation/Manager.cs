using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace JellySimulation
{
    public class Manager
    {
        public Graphics3DContainer GraphicsContainer { get; set; } = new Graphics3DContainer();
        public Config SimulationConfig => GraphicsContainer.Config;

        private DispatcherTimer timer;

        JellyPoint[][][] points;
        Spring[] springs;
        Spring[] springs2;
        const float precision = 0.05f;
        public Vector3D FramePos { get; private set; } = new Vector3D(0, 0, 0);

        float Offset(int idx) => idx == 0 ? -(Config.POINTS - 1) / 2.0f : (Config.POINTS - 1) / 2.0f;
        public HelixToolkit.Wpf.SharpDX.Camera Camera => GraphicsContainer.Camera;

        public Manager()
        {
            InitializeSimulation();
            StartSimultaion();
        }

        internal void StartSimultaion()
        {
            timer?.Stop();
            InitializeSimulation();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, (int)(precision * 100));
            timer.Start();
        }

        private void InitializeSimulation()
        {
            points = new JellyPoint[Config.POINTS][][];
            for (int i = 0; i < Config.POINTS; i++)
            {
                points[i] = new JellyPoint[Config.POINTS][];
                for (int j = 0; j < Config.POINTS; j++)
                {
                    points[i][j] = new JellyPoint[Config.POINTS];
                    for (int k = 0; k < Config.POINTS; k++)
                    {
                        points[i][j][k] = new JellyPoint(i - (Config.POINTS - 1) / 2.0f, j - (Config.POINTS - 1) / 2.0f, k - (Config.POINTS - 1) / 2.0f);
                    }
                }
            }

            var spr = new List<Spring>();
            var IT = Config.POINTS - 1;
            var c1 = SimulationConfig.Elasticity1;
            var sqrt2 = (float)Math.Sqrt(2);
            for (int i = 0; i < Config.POINTS; i++)
            {
                for (int j = 0; j < Config.POINTS; j++)
                {
                    for (int k = 0; k < IT; k++)
                    {
                        spr.Add(new Spring(i, j, k, i, j, k + 1, c1, 1));
                        spr.Add(new Spring(j, k, i, j, k + 1, i, c1, 1));
                        spr.Add(new Spring(k, i, j, k + 1, i, j, c1, 1));
                    }
                }
            }
            for (int i = 0; i < Config.POINTS; i++)
            {
                for (int j = 0; j < IT; j++)
                {
                    for (int k = 0; k < IT; k++)
                    {
                        spr.Add(new Spring(i, j, k, i, j + 1, k + 1, c1, sqrt2));
                        spr.Add(new Spring(i, j + 1, k, i, j, k + 1, c1, sqrt2));
                        spr.Add(new Spring(j, k, i, j + 1, k + 1, i, c1, sqrt2));
                        spr.Add(new Spring(j + 1, k, i, j, k + 1, i, c1, sqrt2));
                        spr.Add(new Spring(k, i, j, k + 1, i, j + 1, c1, sqrt2));
                        spr.Add(new Spring(k, i, j + 1, k + 1, i, j, c1, sqrt2));

                    }
                }
            }
            springs = spr.ToArray();
            spr.Clear();
            var c2 = SimulationConfig.Elasticity2;
            spr.Add(new Spring( 0,  0,  0, c2, 0));
            spr.Add(new Spring(IT,  0,  0, c2, 0));
            spr.Add(new Spring( 0, IT,  0, c2, 0));
            spr.Add(new Spring( 0,  0, IT, c2, 0));
            spr.Add(new Spring( 0, IT, IT, c2, 0));
            spr.Add(new Spring(IT,  0, IT, c2, 0));
            spr.Add(new Spring(IT, IT,  0, c2, 0));
            spr.Add(new Spring(IT, IT, IT, c2, 0));

            springs2 = spr.ToArray();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RecalculatePoints();
        }

        private void RecalculatePoints()
        {
            var damping = SimulationConfig.Damping;

            foreach (var s in springs)
            {
                var forceDir = points[s.Point1_X][s.Point1_Y][s.Point1_Z].Position - points[s.Point2_X][s.Point2_Y][s.Point2_Z].Position;
                var f = s.Force(forceDir.Length());
                forceDir.Normalize();

                points[s.Point1_X][s.Point1_Y][s.Point1_Z].Force += forceDir * f;
                points[s.Point2_X][s.Point2_Y][s.Point2_Z].Force -= forceDir * f;
            }
            var framePos = FramePos.ToVector3();
            foreach (var s in springs2)
            {
                var forceDir = points[s.Point1_X][s.Point1_Y][s.Point1_Z].Position - (framePos + new Vector3(Offset(s.Point1_X), Offset(s.Point1_Y), Offset(s.Point1_Z)));
                var f = s.Force(forceDir.Length());
                forceDir.Normalize();

                points[s.Point1_X][s.Point1_Y][s.Point1_Z].Force += forceDir * f;
            }
            for (int i = 0; i < Config.POINTS; i++)
            {
                for (int j = 0; j < Config.POINTS; j++)
                {
                    for (int k = 0; k < Config.POINTS; k++)
                    {
                        points[i][j][k].Next(precision, Config.BOUNDINGBOX_SIZE, damping);
                    }
                }
            }

            GraphicsContainer.SetState(points, FramePos);
        }

        internal void MoveFrame(Vector delta)
        {
            var right = Vector3D.CrossProduct(Camera.LookDirection, GraphicsContainer.UpDirection);
            right.Normalize();
            var up = Vector3D.CrossProduct(right, Camera.LookDirection);
            up.Normalize();

            FramePos += 0.02f * (delta.X * right - delta.Y * up);
            //GraphicsContainer.SetState(points, FramePos);
            RecalculatePoints();
        }
    }
}
