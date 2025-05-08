using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Engine;

namespace Game
{
    public class StaticObject : ObjectType
    {
        Model MDL;
        public Vector4 Color = Vector4.One;
        public bool     Cullable = true;
        public bool     CustomFade = false;
        public float    ViewRange = 1.0f;
        public Effect   Shader = null;

        public Camera camera;
        public bool     DisplayShadow = true;

        Matrix[]    Transformation;

        public StaticObject(Model Mdl, Vector3 Position) 
        {
            Setup(Mdl, Position);
        }
        public Model GetModel() { return MDL; }

        void Setup(Model Mdl, Vector3 Pos)
        {
            this.MDL = Mdl;
            this.Position = Pos;
            Scale = Vector3.One;
            EulerRotation = Vector3.Zero;
            Transformation = new Matrix[MDL.Bones.Count];
            MDL.CopyAbsoluteBoneTransformsTo(Transformation);
        }
        public void LoadShader(string Filename, IEContentManager ContentManager)
        {
            Shader = ContentManager.Load<Effect>(Filename);
            
            foreach (ModelMesh mesh in MDL.Meshes)
            {
                for (int i = 0; i < mesh.MeshParts.Count; i++)
                {
                    Texture2D tmp = mesh.MeshParts[i].Effect.Parameters["BasicTexture"].GetValueTexture2D();
                    Shader.Parameters["BasicTexture"].SetValue(tmp);
                    mesh.MeshParts[i].Effect = Shader.Clone(GameEngine.GraphicDevice);              
                }
            }
            camera = GameEngine.Services.GetService<Camera>();
        }

        public override void Draw(string Technique)
        {
            /*#region RenderStates
            if(Color.W < 1.0f)
                GameEngine.GraphicDevice.RenderState.AlphaBlendEnable = true;
            else
                GameEngine.GraphicDevice.RenderState.AlphaBlendEnable = false;
            GameEngine.GraphicDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            GameEngine.GraphicDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            GameEngine.GraphicDevice.RenderState.DepthBufferEnable = true;
            //GameEngine.GraphicDevice.RenderState.DepthBufferWriteEnable = true;
            #endregion */
            if (!DisplayShadow && SceneControl.RenderMode == SceneControl.Rendering.SHADOW)
                return;
            if (camera == null)
            {
                try
                {
                    camera = GameEngine.Services.GetService<Camera>();
                }
                catch (Exception e)
                {
                    throw new Exception("No camera in gameEngine, Unable to draw" + e.Message);
                }
            }
            for(int i = 0; i < MDL.Meshes.Count; i++)
            {
                if(UpdateWorldFromDraw)
                    World = MathsUtility.CreateWorldMatrix(Position + CalculatedOffset, Rotation, Scale);
                for (int j = 0; j < MDL.Meshes[i].Effects.Count; j++)
                {
                    Matrix Wrld = Transformation[MDL.Meshes[i].ParentBone.Index] * World;

                    if (SceneControl.RenderMode == SceneControl.Rendering.WIREFRAME)
                    {
                        MDL.Meshes[i].Effects[j].CurrentTechnique = MDL.Meshes[i].Effects[j].Techniques["Wireframe"];
                        MDL.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(Wrld);
                    }
                    else if (SceneControl.RenderMode == SceneControl.Rendering.NORMAL)
                    {
                        MDL.Meshes[i].Effects[j].CurrentTechnique = MDL.Meshes[i].Effects[j].Techniques["CellShade"];
                        MDL.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(Wrld);
                    }
                    else if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW)
                    {
                        MDL.Meshes[i].Effects[j].CurrentTechnique = MDL.Meshes[i].Effects[j].Techniques["Shadow"];
                        MDL.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(Wrld * Shadow.ShadowMatrix);
                    }
                    if(Technique != "Normal")
                        MDL.Meshes[i].Effects[j].CurrentTechnique = MDL.Meshes[i].Effects[j].Techniques[Technique];
                     
                    MDL.Meshes[i].Effects[j].Parameters["ViewProj"].SetValue(camera.MatrixViewProj);
                    MDL.Meshes[i].Effects[j].Parameters["OverallColor"].SetValue(Color);
                }
                MDL.Meshes[i].Draw();
            }
        }

        public override void Update()
        {
            if(!UpdateWorldFromDraw)
                World = MathsUtility.CreateWorldMatrix(Position + CalculatedOffset, Rotation, Scale);
        }
    }
}
