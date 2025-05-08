using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Engine;


namespace Game
{
    public class Shadow : I3DComponent
    {
        public static Vector3   LightPosition = new Vector3(100, 70, 50);
        public static Matrix    ShadowMatrix;
        VertexDeclaration VertexDeclare;
        List<VertexPositionTexture> Vertices = new List<VertexPositionTexture>();
        List<short> Indices = new List<short>();
        Texture2D Texture;
        BasicEffect NormalFx;
        Camera Cam;

        public Shadow() { InitializeComponent(); }

        protected void InitializeComponent()
        {
            //ShadowTarget = GraphicUtility.CreateRenderTarget();
            LightPosition.Normalize();
            ShadowMatrix = Matrix.CreateShadow(-LightPosition, new Plane(new Vector4(0, -1, 0, 0))) * Matrix.CreateTranslation(new Vector3(0, 0.01f, 0));
            VertexDeclare = new VertexDeclaration(GameEngine.GraphicDevice, VertexPositionTexture.VertexElements);
            Vertices.Add(new VertexPositionTexture(new Vector3(-0.5f, 0, 0.5f), new Vector2(0, 1)));
            Vertices.Add(new VertexPositionTexture(new Vector3(-0.5f, 0, -0.5f), new Vector2(0, 0)));
            Vertices.Add(new VertexPositionTexture(new Vector3(0.5f, 0, 0.5f), new Vector2(1, 1)));
            Vertices.Add(new VertexPositionTexture(new Vector3(0.5f, 0, -0.5f), new Vector2(1, 0)));
            Texture = GameEngine.Content.Load<Texture2D>("Content/Textures/Shadow");
            Cam = GameEngine.Services.GetService<Camera>();
            NormalFx = new BasicEffect(GameEngine.GraphicDevice, null);
            NormalFx.Texture = Texture;
            NormalFx.TextureEnabled = true;
        }
        public void Disable()
        {
            NormalFx.Dispose();
        }

        public virtual Vector3 Position { get; set; }
        public Vector3 EulerRotation
        {
            get { return MathsUtility.MatrixToVector3(Rotation); }
            set { Rotation = MathsUtility.Vector3ToMatrix(value); }
        }
        public virtual Matrix Rotation { get; set; }
        public virtual Vector3 Scale { get; set; }
        public virtual BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(
                    Position - (Scale / 2),
                    Position + (Scale / 2)
                );
            }
        }

        public void Draw(Vector3 Position, Vector3 Offset, Vector3 Scale, Matrix Orientate)
        {
            /* Depth Bias needed to set to solve overlapping */
            GameEngine.GraphicDevice.RenderState.AlphaBlendEnable = true;
            GameEngine.GraphicDevice.RenderState.AlphaDestinationBlend = Blend.DestinationAlpha;
            GameEngine.GraphicDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            GameEngine.GraphicDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            GameEngine.GraphicDevice.RenderState.DepthBufferEnable = true;
            GameEngine.GraphicDevice.RenderState.DepthBufferWriteEnable = false;
            GameEngine.GraphicDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            NormalFx.World = MathsUtility.CreateWorldMatrix(Position + Offset + new Vector3(0, 0.02f, 0), Orientate, Scale);
            NormalFx.View = Cam.MatrixView;
            NormalFx.Projection = Cam.MatrixProjection;
            NormalFx.CommitChanges();
            NormalFx.Begin();
            for (int i = 0; i < NormalFx.CurrentTechnique.Passes.Count; i++)
            {
                NormalFx.CurrentTechnique.Passes[i].Begin();
                GameEngine.GraphicDevice.VertexDeclaration = VertexDeclare;
                GameEngine.GraphicDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Vertices.ToArray(), 0, 2);
                NormalFx.CurrentTechnique.Passes[i].End();
            }
            NormalFx.End();  
        }
    }
}
