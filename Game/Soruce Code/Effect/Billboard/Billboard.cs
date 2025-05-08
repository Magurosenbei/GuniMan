using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Engine;

namespace Game
{
    // Must be last drawn
    public class Billboard : I3DComponent
    {
        VertexDeclaration mDeclare;
        VertexPositionTexture[] mVertice;
        short[] Indices;
        #region I3DComponent stuff
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
        #endregion

        Effect Shader;

        public Billboard(string ShaderPath, IEContentManager ContentManager)
        {
            Shader = ContentManager.Load<Effect>(ShaderPath).Clone(GameEngine.GraphicDevice);
            mVertice = new VertexPositionTexture[4];
            mVertice[0] = new VertexPositionTexture(Vector3.Zero, new Vector2(1,1));
            mVertice[1] = new VertexPositionTexture(Vector3.Zero, new Vector2(0,1));
            mVertice[2] = new VertexPositionTexture(Vector3.Zero, new Vector2(0,0));
            mVertice[3] = new VertexPositionTexture(Vector3.Zero, new Vector2(1,0));

            Indices = new short[] { 0, 1, 2, 2, 3, 0 };
            mDeclare = new VertexDeclaration(GameEngine.GraphicDevice, VertexPositionTexture.VertexElements);
        }
        public void Disable()
        {
            Shader.Dispose();
        }
        public void SetSize(float size)
        {
            size *= 0.5f;
            mVertice[0].Position.X = -size;
            mVertice[0].Position.Y = size;
            mVertice[0].Position.Z = 0;

            mVertice[1].Position.X = -size;
            mVertice[1].Position.Y = -size;
            mVertice[1].Position.Z = 0;

            mVertice[2].Position.X = size;
            mVertice[2].Position.Y = size;
            mVertice[2].Position.Z = 0;

            mVertice[3].Position.X = size;
            mVertice[3].Position.Y = -size;
            mVertice[3].Position.Z = 0;
        }
        public void Render(Camera Cam, Texture2D Tex)
        {
            GameEngine.GraphicDevice.RenderState.AlphaBlendEnable = true;
            GameEngine.GraphicDevice.RenderState.DepthBufferEnable = false;
            GameEngine.GraphicDevice.RenderState.DepthBufferWriteEnable = false;

            Vector3 Dir = Cam.View - Cam.Position;
            Dir.Normalize();
            GameEngine.GraphicDevice.VertexDeclaration = mDeclare;
            Shader.Begin();
            Shader.Parameters["WorldViewProj"].SetValue(Matrix.CreateBillboard(Position, Cam.Position, Cam.Up, Dir) * Cam.MatrixViewProj);
            Shader.Parameters["BasicTexture"].SetValue(Tex);
            Shader.CommitChanges();
            for (int i = 0; i < Shader.CurrentTechnique.Passes.Count; i++)
            {
                Shader.CurrentTechnique.Passes[i].Begin();
                GameEngine.GraphicDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, mVertice, 0, 2);
                //GameEngine.GraphicDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, mVertice, 0, mVertice.Length, Indices, 0, 2);
                Shader.CurrentTechnique.Passes[i].End();
            }
            Shader.End();
        }
    }
}
