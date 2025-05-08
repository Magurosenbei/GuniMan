using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;
using XNAnimation.Pipeline;

using Engine;

namespace Game
{
    public class AnimatedObject : ObjectType
    {
        SkinnedModel    AnimatedModel;
        public Vector4 Color = Vector4.One;
        public bool Cullable = true;
        public float ViewRange = 1.0f;

        //public float    OutlineScale = 1.05f;
        //public Vector3 OutlineScale = Vector3.One;
        public bool DisplayShadow = true;
        public Effect Shader = null;

        public Camera Camera;

        public Matrix [] Transformation;
        public  AnimationController Animator;

        public AnimatedObject(SkinnedModel Mdl, Vector3 Pos)
        {
            Setup(Mdl, Pos);
        }

        public SkinnedModel GetAnimatedModel() { return AnimatedModel; }

        void Setup(SkinnedModel Mdl, Vector3 Pos)
        {
            this.AnimatedModel = Mdl;
            this.Position = Pos;
            Scale = Vector3.One;
            EulerRotation = Vector3.Zero;
            Animator = new AnimationController(AnimatedModel.SkeletonBones);
            Animator.Speed = 1.0f;
            Animator.TranslationInterpolation   = InterpolationMode.Linear;
            Animator.OrientationInterpolation = InterpolationMode.Linear;
            Animator.ScaleInterpolation = InterpolationMode.Linear;

            //Transformation = new Matrix[AnimatedModel.Model.Bones.Count];
            //AnimatedModel.Model.CopyAbsoluteBoneTransformsTo(Transformation);
        }
        public void CopyBodeTransform()
        {
            Transformation = new Matrix[AnimatedModel.Model.Bones.Count];
            AnimatedModel.Model.CopyAbsoluteBoneTransformsTo(Transformation);
        }
        public void LoadShader(string Filename, IEContentManager ContentManager)
        {
            Camera = GameEngine.Services.GetService<Camera>();
            Shader = ContentManager.Load<Effect>(Filename);
            foreach (ModelMesh mesh in AnimatedModel.Model.Meshes)
                for (int i = 0; i < mesh.MeshParts.Count; i++)
                {
                    Texture2D tmp;
                    try
                    {
                        tmp = mesh.MeshParts[i].Effect.Parameters["diffuseMap0"].GetValueTexture2D();
                    }
                    catch (Exception e)
                    {
                        Debug.Write("\nAnimated Model Exception Handled Ignore this =P : " + e.Message);
                        tmp = mesh.MeshParts[i].Effect.Parameters["BasicTexture"].GetValueTexture2D();
                    }
                    Shader.Parameters["BasicTexture"].SetValue(tmp);
                    mesh.MeshParts[i].Effect = Shader.Clone(GameEngine.GraphicDevice);
                }
        }
        public override void Draw(string Technique)
        {
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW && !DisplayShadow) return;
            /*if(Color.W < 1.0f)
                GameEngine.GraphicDevice.RenderState.AlphaBlendEnable   = true;
            else
                GameEngine.GraphicDevice.RenderState.AlphaBlendEnable = false;
            GameEngine.GraphicDevice.RenderState.SourceBlend        = Blend.SourceAlpha;
            GameEngine.GraphicDevice.RenderState.DestinationBlend   = Blend.InverseSourceAlpha;
            GameEngine.GraphicDevice.RenderState.DepthBufferEnable  = true;*/

            if (Camera == null) throw new Exception("No camera in gameEngine, Unable to draw");
            for(int i = 0; i < AnimatedModel.Model.Meshes.Count; i++)
            {
                for (int j = 0; j < AnimatedModel.Model.Meshes[i].Effects.Count; j++)
                {
                    if (SceneControl.RenderMode == SceneControl.Rendering.WIREFRAME)
                    {
                        AnimatedModel.Model.Meshes[i].Effects[j].CurrentTechnique = AnimatedModel.Model.Meshes[i].Effects[j].Techniques["Wireframe"];
                        AnimatedModel.Model.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(World);
                    }
                    else if (SceneControl.RenderMode == SceneControl.Rendering.NORMAL)
                    {
                        AnimatedModel.Model.Meshes[i].Effects[j].CurrentTechnique = AnimatedModel.Model.Meshes[i].Effects[j].Techniques["CellShade"];
                        AnimatedModel.Model.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(World);
                    }
                    else if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW)
                    {
                        AnimatedModel.Model.Meshes[i].Effects[j].CurrentTechnique = AnimatedModel.Model.Meshes[i].Effects[j].Techniques["Shadow"];
                        AnimatedModel.Model.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(World * Shadow.ShadowMatrix);
                    }
                    //AnimatedModel.Model.Meshes[i].Effects[j].Parameters["WorldMatrix"].SetValue(World);
                    AnimatedModel.Model.Meshes[i].Effects[j].Parameters["ViewProj"].SetValue(Camera.MatrixViewProj);
                    AnimatedModel.Model.Meshes[i].Effects[j].Parameters["BonesMatrix"].SetValue(Animator.SkinnedBoneTransforms);
                    AnimatedModel.Model.Meshes[i].Effects[j].Parameters["OverallColor"].SetValue(Color);
                }
                AnimatedModel.Model.Meshes[i].Draw();
            }
        }
        public override void Update()
        {
            CalculatedOffset = (Matrix.CreateTranslation(CenterOffset) * Rotation).Translation;
            World = MathsUtility.CreateWorldMatrix(Position + CalculatedOffset, Rotation, Scale);
            Animator.Update(GameEngine.GameTime.ElapsedGameTime, Matrix.Identity);      
        }
        public void FindBonePosition(string name, out int AbsTransID, out int SkinnnedID)
        {
            try
            {
                AbsTransID = AnimatedModel.Model.Bones[name].Index;
            }
            catch (Exception e)
            {
                AbsTransID = -1;
                Debug.Write("\nWrong name or no such bone " + e.Message);
            }
            for (int i = 0; i < AnimatedModel.SkeletonBones.Count; i++)
                if (AnimatedModel.SkeletonBones[i].Name == name)
                {
                    SkinnnedID = AnimatedModel.SkeletonBones[i].Index;
                    return;
                }
            SkinnnedID = -1;
        }
    }
}
