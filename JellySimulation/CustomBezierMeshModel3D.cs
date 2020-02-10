using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;

namespace JellySimulation
{
    class CustomBezierMeshModel3D : MeshGeometryModel3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            var node = base.OnCreateSceneNode();
            node.OnSetRenderTechnique = (host) => { return host.EffectsManager[CustomShaderNames.BezierMesh]; };
            return node;
        }
    }
}