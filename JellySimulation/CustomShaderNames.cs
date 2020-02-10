using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Shaders;
using System;
using System.IO;

namespace JellySimulation
{
    public static class CustomShaderNames
    {
        public static readonly string BezierMesh = "BezierMesh";
    }
    public static class ShaderHelper
    {
        public static byte[] LoadShaderCode(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                throw new ArgumentException($"Shader File not found: {path}");
            }
        }
    }

    /// <summary>
    /// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
    /// </summary>
    public static class CustomVSShaderDescription
    {
        public static byte[] VSBezierTransformByteCode
        {
            get
            {
                return ShaderHelper.LoadShaderCode(@"Shaders\BezierTransformVS.cso");
            }
        }
        public static ShaderDescription VSBezierTransform = new ShaderDescription(nameof(VSBezierTransform), ShaderStage.Vertex,
            new ShaderReflector(), VSBezierTransformByteCode);
    }

    public class CustomEffectsManager : DefaultEffectsManager
    {
        public CustomEffectsManager()
        {
            LoadCustomTechniqueDescriptions();
        }


        private void LoadCustomTechniqueDescriptions()
        {
            var bezierTransform = new TechniqueDescription(CustomShaderNames.BezierMesh)
            {
                InputLayoutDescription = new InputLayoutDescription(CustomVSShaderDescription.VSBezierTransformByteCode, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription("MyPass")
                    {
                        ShaderList = new[]
                        {
                            CustomVSShaderDescription.VSBezierTransform,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };
            AddTechnique(bezierTransform);
        }
    }
}