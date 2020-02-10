using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3 = SharpDX.Vector3;

namespace JellySimulation
{
    public class Graphics3DContainer : ObservableObject
    {
        private HelixToolkit.Wpf.SharpDX.Camera camera;
        private IEffectsManager effectsManager;
        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);
        public HelixToolkit.Wpf.SharpDX.Camera Camera { get => camera; protected set => SetValue(ref camera, value, "Camera"); }
        public IEffectsManager EffectsManager { get => effectsManager; protected set => SetValue(ref effectsManager, value); }

        // Environment light direction and colors
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        #region Grid, Bounding Box and Frame
        public LineGeometry3D Grid { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public LineGeometry3D BoundingBox { get; private set; }
        public Transform3D BoundingBoxTransform { get; private set; }


        private Transform3D frameBoxTransform;
        public LineGeometry3D FrameBox { get; private set; }
        public Transform3D FrameBoxTransform { get => frameBoxTransform; private set => SetValue(ref frameBoxTransform, value); }

        #endregion

        public PointGeometry3D BezierPoints { get; private set; }

        private LineGeometry3D springs;
        public LineGeometry3D Springs { get => springs; private set => SetValue(ref springs, value); }
        public Transform3D SpringsTransform { get; private set; }
        public Transform3D BezierCubeTransform { get; private set; }

        public MeshGeometry3D BezierCubeFace1 { get => bezierCubeFace1; private set => SetValue(ref bezierCubeFace1, value); }
        public MeshGeometry3D BezierCubeFace2 { get => bezierCubeFace2; private set => SetValue(ref bezierCubeFace2, value); }
        public MeshGeometry3D BezierCubeFace3 { get => bezierCubeFace3; private set => SetValue(ref bezierCubeFace3, value); }
        public MeshGeometry3D BezierCubeFace4 { get => bezierCubeFace4; private set => SetValue(ref bezierCubeFace4, value); }
        public MeshGeometry3D BezierCubeFace5 { get => bezierCubeFace5; private set => SetValue(ref bezierCubeFace5, value); }
        public MeshGeometry3D BezierCubeFace6 { get => bezierCubeFace6; private set => SetValue(ref bezierCubeFace6, value); }

        private MeshGeometry3D bezierCubeFace1;
        private MeshGeometry3D bezierCubeFace2;
        private MeshGeometry3D bezierCubeFace3;
        private MeshGeometry3D bezierCubeFace4;
        private MeshGeometry3D bezierCubeFace5;
        private MeshGeometry3D bezierCubeFace6;
        private Geometry3D bezierShapeGeometry;
        private Geometry3D[] objGeometry;

        public PhongMaterial RedMaterial { get; private set; } = PhongMaterials.Red;
        public Geometry3D BezierShapeGeometry { get => bezierShapeGeometry; private set => SetValue(ref bezierShapeGeometry, value); }

        public PhongMaterial GoldMaterial { get; private set; } = PhongMaterials.Gold;
        public ObservableElement3DCollection CustomModel3DCollection { get; set; } = new ObservableElement3DCollection();


        public Config Config { get; set; } = new Config();

        public Graphics3DContainer()
        {
            Initialize3DObjects();
        }

        private void Initialize3DObjects()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new HelixToolkit.Wpf.SharpDX.PerspectiveCamera
            {
                Position = new Point3D(3, 3, 5),
                LookDirection = new Vector3D(-3, -3, -5),
                UpDirection = this.UpDirection,
                FarPlaneDistance = 500000
            };

            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            float SIZE = Config.SIZE;
            Grid = LineBuilder.GenerateGrid(new Vector3(0, 1, 0), -5, 5);
            GridTransform = new Media3D.TranslateTransform3D(0, -1.3 * SIZE, 0);

            MeshBuilder mb = new MeshBuilder();
            mb.AddBoundingBox(new Media3D.Rect3D(-SIZE, -SIZE, -SIZE, 2 * SIZE, 2 * SIZE, 2 * SIZE), 0.2);
            BoundingBox = LineBuilder.GenerateBoundingBox(mb.ToMeshGeometry3D());
            BoundingBoxTransform = Transform3D.Identity;

            SIZE = (Config.POINTS - 1) / 2.0f;
            mb = new MeshBuilder();
            mb.AddBoundingBox(new Media3D.Rect3D(-SIZE, -SIZE, -SIZE, 2 * SIZE, 2 * SIZE, 2 * SIZE), 0.2);
            FrameBox = LineBuilder.GenerateBoundingBox(mb.ToMeshGeometry3D());
            FrameBoxTransform = Transform3D.Identity;

            BezierPoints = new PointGeometry3D();
            SpringsTransform = Transform3D.Identity;
            BezierCubeTransform = Transform3D.Identity;

            //SimulationPoint[][][] points = new SimulationPoint[4][][];
            //for (int i = 0; i < 4; i++)
            //{
            //    points[i] = new SimulationPoint[4][];
            //    for (int j = 0; j < 4; j++)
            //    {
            //        points[i][j] = new SimulationPoint[4];
            //        for (int k = 0; k < 4; k++)
            //        {
            //            points[i][j][k] = new SimulationPoint(i - 3/2.0f, j - 3 / 2.0f, k - 3 / 2.0f);
            //            //points[i][j][k] = new SimulationPoint(Config.SIZE * i /3.0f, Config.SIZE * j / 3.0f, Config.SIZE * k / 3.0f);
            //        }
            //    }
            //}
        }

        internal void SetState(SimulationPoint[][][] points, Vector3D framePos)
        {
            int size = Config.POINTS;
            int it = size - 1;
            BezierPoints.Positions = new Vector3Collection(points.SelectMany(v => v).SelectMany(v => v).Select(p => p.Position));
            var lb = new LineBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int k = 0; k < it; k++)
                    {
                        lb.AddLine(points[i][j][k].Position, points[i][j][k + 1].Position);
                        lb.AddLine(points[j][k][i].Position, points[j][k + 1][i].Position);
                        lb.AddLine(points[k][i][j].Position, points[k + 1][i][j].Position);
                    }
                }
            }
            Springs = lb.ToLineGeometry3D();
            FrameBoxTransform = new Media3D.TranslateTransform3D(framePos);

            UpdateBezierCube(points);
            if (objGeometry != null)
            {
                Vector3[][][] bezierPts = new Vector3[4][][];
                for (int i = 0; i < 4; i++)
                {
                    bezierPts[i] = new Vector3[4][];
                    for (int j = 0; j < 4; j++)
                    {
                        bezierPts[i][j] = new Vector3[4];
                        for (int k = 0; k < 4; k++)
                        {
                            bezierPts[i][j][k] = points[i][j][k].Position;

                        }
                    }
                }
                CustomModel3DCollection.Clear();
                foreach (var g in objGeometry)
                {
                    MeshGeometryModel3D m = new MeshGeometryModel3D();
                    MeshGeometry3D mesh = new MeshGeometry3D();
                    Vector3[] tmp = new Vector3[g.Positions.Count];
                    g.Positions.CopyTo(tmp);
                    mesh.Positions = new Vector3Collection(tmp);
                    int[] tmp2 = new int[g.Indices.Count];
                    g.Indices.CopyTo(tmp2);
                    mesh.Indices = new IntCollection(tmp2);
                    Vector3 offset = new Vector3(1.5f, 1.5f, 1.5f);
                    for (int i = 0; i < mesh.Positions.Count; i++)
                    {
                        //mesh.Positions[i] = 0.75f*(Transform(mesh.Positions[i]/1.5f, bezierPts) + offset);
                        mesh.Positions[i] = Transform(mesh.Positions[i], bezierPts);
                    }
                    mesh.Normals = CalculateNormals(mesh.Positions, mesh.Indices);
                    m.Geometry = mesh;
                    m.Material = GoldMaterial;
                    CustomModel3DCollection.Add(m);
                }
                OnPropertyChanged(nameof(CustomModel3DCollection));
            }
        }

        private void UpdateBezierCube(SimulationPoint[][][] points)
        {
            var pts = new Vector3[4][];
            for (int i = 0; i < 4; i++)
            {
                pts[i] = new Vector3[4];
                for (int j = 0; j < 4; j++)
                {
                    pts[i][j] = points[j][i][0].Position;
                }
            }
            BezierCubeFace1 = CalculateBezierFace((u, v) =>
            {
                var res = Vector3.Zero;
                for (int i = 0; i < basis.Length; i++)
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        res += basis[i](u) * basis[j](v) * pts[i][j];
                    }
                }
                return res;
            });
            pts = new Vector3[4][];
            for (int i = 0; i < 4; i++)
            {
                pts[i] = new Vector3[4];
                for (int j = 0; j < 4; j++)
                {
                    pts[i][j] = points[i][j][3].Position;
                }
            }
            BezierCubeFace2 = CalculateBezierFace((u, v) =>
            {
                var res = Vector3.Zero;
                for (int i = 0; i < basis.Length; i++)
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        res += basis[i](u) * basis[j](v) * pts[i][j];
                    }
                }
                return res;
            });
            pts = new Vector3[4][];
            for (int i = 0; i < 4; i++)
            {
                pts[i] = new Vector3[4];
                for (int j = 0; j < 4; j++)
                {
                    pts[i][j] = points[0][j][i].Position;
                }
            }
            BezierCubeFace3 = CalculateBezierFace((u, v) =>
            {
                var res = Vector3.Zero;
                for (int i = 0; i < basis.Length; i++)
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        res += basis[i](u) * basis[j](v) * pts[i][j];
                    }
                }
                return res;
            });
            pts = new Vector3[4][];
            for (int i = 0; i < 4; i++)
            {
                pts[i] = new Vector3[4];
                for (int j = 0; j < 4; j++)
                {
                    pts[i][j] = points[3][i][j].Position;
                }
            }
            BezierCubeFace4 = CalculateBezierFace((u, v) =>
            {
                var res = Vector3.Zero;
                for (int i = 0; i < basis.Length; i++)
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        res += basis[i](u) * basis[j](v) * pts[i][j];
                    }
                }
                return res;
            });
            pts = new Vector3[4][];
            for (int i = 0; i < 4; i++)
            {
                pts[i] = new Vector3[4];
                for (int j = 0; j < 4; j++)
                {
                    pts[i][j] = points[i][0][j].Position;
                }
            }
            BezierCubeFace5 = CalculateBezierFace((u, v) =>
            {
                var res = Vector3.Zero;
                for (int i = 0; i < basis.Length; i++)
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        res += basis[i](u) * basis[j](v) * pts[i][j];
                    }
                }
                return res;
            });
            pts = new Vector3[4][];
            for (int i = 0; i < 4; i++)
            {
                pts[i] = new Vector3[4];
                for (int j = 0; j < 4; j++)
                {
                    pts[i][j] = points[j][3][i].Position;
                }
            }
            BezierCubeFace6 = CalculateBezierFace((u, v) =>
            {
                var res = Vector3.Zero;
                for (int i = 0; i < basis.Length; i++)
                {
                    for (int j = 0; j < basis.Length; j++)
                    {
                        res += basis[i](u) * basis[j](v) * pts[i][j];
                    }
                }
                return res;
            });
        }

        protected virtual MeshGeometry3D CalculateBezierFace(Func<float, float, Vector3> Evaluate)
        {
            var mesh = new MeshGeometry3D()
            {
                Positions = new Vector3Collection(),
                Normals = new Vector3Collection(),
                Indices = new IntCollection()
            };
            int n = 10;
            int m = 10;
            var p = new Vector3[m * n];

            for (int i = 0; i < n; i++)
            {
                float u = 1.0f * i / (n - 1);

                for (int j = 0; j < m; j++)
                {
                    float v = 1.0f * j / (m - 1);
                    int ij = (i * m) + j;
                    p[ij] = Evaluate(u, v);
                }
            }
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++, idx++)
                {
                    mesh.Positions.Add(p[idx]);
                }
            }
            for (int i = 0; i + 1 < n; i++)
            {
                for (int j = 0; j + 1 < m; j++)
                {
                    int x0 = i * m;
                    int x1 = (i + 1) * m;
                    int y0 = j;
                    int y1 = j + 1;
                    AddTriangle(mesh, x0 + y0, x1 + y0, x0 + y1);
                    AddTriangle(mesh, x1 + y0, x1 + y1, x0 + y1);
                }
            }
            mesh.Normals = CalculateNormals(mesh.Positions, mesh.Indices);

            return mesh;
        }

        private static void AddTriangle(MeshGeometry3D mesh, int i1, int i2, int i3)
        {
            var p1 = mesh.Positions[i1];
            if (!IsDefined(p1))
            {
                return;
            }

            var p2 = mesh.Positions[i2];
            if (!IsDefined(p2))
            {
                return;
            }

            var p3 = mesh.Positions[i3];
            if (!IsDefined(p3))
            {
                return;
            }

            mesh.TriangleIndices.Add(i1);
            mesh.TriangleIndices.Add(i2);
            mesh.TriangleIndices.Add(i3);
        }

        private static bool IsDefined(Vector3 point)
        {
            return !double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z);
        }

        Func<float, float>[] basis = new Func<float, float>[]
        {
                    t => (1 - t) * (1 - t) * (1 - t),
                    t => 3 * (1 - t) * (1 - t) * t,
                    t => 3 * (1 - t) * t * t,
                    t => t * t * t
        };

        private Vector3 Transform(Vector3 v, Vector3[][][] bezierPts)
        {
            var res = Vector3.Zero;
            if (v.AllSmallerOrEqual(1.0f) && v.AllGreaterOrEqual(-1.0f))
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            res += basis[i](v.X) * basis[j](v.Y) * basis[k](v.Z) * bezierPts[i][j][k];
                        }
                    }
                }
                return res;
            }
            else throw new ArgumentException("niedobrze");
        }

        internal void ReadObj(string fileName)
        {
            CustomModel3DCollection.Clear();
            var reader = new ObjReader();
            var objList = reader.Read(fileName);
            objList.RemoveAll(o => o.Geometry.Indices.Count == 0);
            //var max = objList.Max(o => o.Geometry.Positions.Max(v => Math.Max(Math.Max(Math.Abs(v.X), Math.Abs(v.Y)), Math.Abs(v.Z))));
            //var max = objList.Max(o => o.Geometry.Bound.Maximum);
            var min = objList.Max(o => o.Geometry.Bound.Minimum);
            //var diff = max - min;
            //var diffmax = diff.X > diff.Y ? (diff.X > diff.Z ? diff.X : diff.Z) : (diff.Y > diff.Z ? diff.Y : diff.Z);
            //Vector3 p = new Vector3(-0.9f, 1.5f, -2.0f);
            Vector3 p = new Vector3(-0.5f, -0.5f, -0.5f);
            var size = objList.Max(o => o.Geometry.Bound.Size);
            var maxSize = Math.Max(size.X, Math.Max(size.Y, size.Z));
            objList.ForEach(o =>
            {
                //new Vector3((1-size.X)/2, (1 - size.Y) / 2, (1 - size.Z) / 2)/
                Vector3 center = o.Geometry.Bound.Center;
                center -= min;
                center /= (maxSize);
                for (int i = 0; i < o.Geometry.Positions.Count; i++)
                {
                    o.Geometry.Positions[i] -= min;
                    //o.Geometry.Positions[i] /= (maxSize / 2.0f);
                    o.Geometry.Positions[i] /= (maxSize);
                    o.Geometry.Positions[i] -= p + center;
                }
                o.Geometry.UpdateBounds();
            });


            objGeometry = objList.Select(o => o.Geometry).ToArray();
            //foreach (var ob in objList)
            //{
            //    var meshGeometry = new MeshGeometryModel3D
            //    {
            //        Geometry = ob.Geometry,
            //        Material = GoldMaterial
            //    };
            //    CustomModel3DCollection.Add(meshGeometry);
            //}
            foreach (var ob in objList)
            {
                CustomModel3DCollection.Add(new CustomBezierMeshModel3D()
                {
                    Geometry = ob.Geometry,
                    Material = GoldMaterial
                });
            }
            OnPropertyChanged(nameof(CustomModel3DCollection));
        }

        public static Vector3Collection CalculateNormals(Vector3Collection positions, IntCollection triangleIndices)
        {
            var normals = new Vector3Collection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                normals.Add(new Vector3());
            }

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int index0 = triangleIndices[i];
                int index1 = triangleIndices[i + 1];
                int index2 = triangleIndices[i + 2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                Vector3 u = p1 - p0;
                Vector3 v = p2 - p0;
                Vector3 w = Vector3.Cross(u, v);
                w.Normalize();
                normals[index0] += w;
                normals[index1] += w;
                normals[index2] += w;
            }

            normals.ForEach(n => n.Normalize());

            return normals;
        }
    }
}
