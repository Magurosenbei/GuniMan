using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class GraphicUtility
    {
        public static RenderTarget2D CreateRenderTarget()
        {
            return CreateRenderTarget(GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height);
        }
        public static RenderTarget2D CreateRenderTarget(int width, int height)
        {
            return CreateRenderTarget(width, height, GameEngine.GraphicDevice.DisplayMode.Format);
        }
        public static RenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat format)
        {
            return CreateRenderTarget(width, height, format, 
                GameEngine.GraphicDevice.PresentationParameters.MultiSampleQuality,
                GameEngine.GraphicDevice.PresentationParameters.MultiSampleType);
        }
        public static RenderTarget2D CreateRenderTarget(int width, int height, SurfaceFormat format, int MultiSampleQuality, MultiSampleType SampleType)
        {
            return new RenderTarget2D(GameEngine.GraphicDevice, width,
                height, 1, format, SampleType, MultiSampleQuality,
                RenderTargetUsage.DiscardContents);
        }
        // Creates a ResolveTexture2D
        public static ResolveTexture2D CreateResolveTexture()
        {
            return new ResolveTexture2D(GameEngine.GraphicDevice,
                GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height,
                1, GameEngine.GraphicDevice.DisplayMode.Format);
        }

        public static Vector3 LargestHalfExtent(Model MDL, bool AboveGround)
        {
            Vector3 HighestPoint = Vector3.Zero;
            foreach (ModelMesh modelMesh in MDL.Meshes)
            {
                foreach (ModelMeshPart x in modelMesh.MeshParts)
                {
                    VertexPositionNormalTexture[] tmp = new VertexPositionNormalTexture[x.NumVertices];
                    modelMesh.VertexBuffer.GetData<VertexPositionNormalTexture>(tmp);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (Math.Abs(tmp[i].Position.Y) > HighestPoint.Y)
                            HighestPoint.Y = Math.Abs(tmp[i].Position.Y);
                        if (Math.Abs(tmp[i].Position.X) > HighestPoint.X)
                            HighestPoint.X = Math.Abs(tmp[i].Position.X);
                        if (Math.Abs(tmp[i].Position.Z) > HighestPoint.Z)
                            HighestPoint.Z = Math.Abs(tmp[i].Position.Z);
                    }
                }
            }
            if(AboveGround)
                HighestPoint.Y *= 0.5f;
            return HighestPoint;
        }
    }
}
