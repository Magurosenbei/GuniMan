using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Engine;

namespace Game
{
    public struct Particle
    {
        public Vector3 Position;
        public Vector4 Color;

        public float Size;
        public float FadeRate;
        public Vector3 Velocity;

        public static readonly VertexElement[] VertexElements = new VertexElement[] 
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, sizeof(float)*3, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Color, 0),  
            new VertexElement(0, sizeof(float)*7, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.PointSize, 0),              
        };
        public static int SizeInBytes
        {
            get{return sizeof(float) * 8;}
        }
    }
    public class ParticleEmitor : ObjectType
    {
        protected Effect      ParticleFx;
        protected Texture2D   ParticleSprite;
        protected Camera Camera;

        protected VertexDeclaration mDeclare;
        // Using TexCoord as Size of x and y
        //List <VertexPositionColorTexture> mParticles;
        //protected List <Particle> mParticles;
        protected Particle[] mParticles;
        protected bool Static = false;
        public bool Done = false;
        public bool StopSpawn = false;
        // Using .a as Life
        public ParticleEmitor(int Amount, bool IsStatic, IEContentManager ContentManager)
        {
            Static = IsStatic;
            Setup(Amount, ContentManager);
        }
        virtual protected void Setup(int Amount, IEContentManager ContentManager)
        {
            mParticles = new Particle[Amount];
            for (int i = 0; i < Amount; i++)
            {
                mParticles[i].Position = Position;
                mParticles[i].Velocity =  new Vector3(
                                (float)GameEngine.RandomValue.NextDouble() - 0.5f,
                                (float)GameEngine.RandomValue.NextDouble() + 1.0f,
                                (float)GameEngine.RandomValue.NextDouble() - 0.5f);
                mParticles[i].Color = Vector4.One;
                mParticles[i].FadeRate = -(float)GameEngine.RandomValue.Next(1, 10) * 0.1f;
                mParticles[i].Size = (float)GameEngine.RandomValue.NextDouble() + (float)GameEngine.RandomValue.NextDouble();
            }
            mDeclare = new VertexDeclaration(GameEngine.GraphicDevice, Particle.VertexElements);

            ParticleSprite = ContentManager.Load<Texture2D>("Content/Shader Fx/Sprite/Particle");
            ParticleFx = ContentManager.Load<Effect>("Content/Shader Fx/ParticleFX").Clone(GameEngine.GraphicDevice);
            ParticleFx.Parameters["BasicTexture"].SetValue(ParticleSprite);
            Camera = GameEngine.Services.GetService<Camera>();
            Rotation = Matrix.Identity;
            Scale = Vector3.One;
        }
        public override void  Disable()
        {
            //mParticles.Clear();
            mParticles = null;
            ParticleFx.Dispose();
            mDeclare.Dispose();
        }
        public override void  Update()
        {
            //for (int i = 0; i < mParticles.Length; i++)
            float dt = (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
            Parallel.For(0, mParticles.Length, delegate(int i)
            {
                if (mParticles[i].Color.W > 0)
                {
                    if(!Static)
                        mParticles[i].Position += mParticles[i].Velocity * dt;
                    mParticles[i].Color.W += mParticles[i].FadeRate * dt;
                }
                else if(!StopSpawn)
                {
                    mParticles[i].Position = Vector3.Zero;
                    mParticles[i].Velocity = new Vector3(
                    (float)GameEngine.RandomValue.NextDouble() - 0.5f + (float)GameEngine.RandomValue.NextDouble() - 0.5f,
                    (float)GameEngine.RandomValue.NextDouble() - 0.15f,
                    (float)GameEngine.RandomValue.NextDouble() - 0.5f + (float)GameEngine.RandomValue.NextDouble() - 0.5f) * GameEngine.RandomValue.Next(1, 3);
                    mParticles[i].Color = new Vector4((float)GameEngine.RandomValue.NextDouble(), (float)GameEngine.RandomValue.NextDouble(), (float)GameEngine.RandomValue.NextDouble(), 1);
                    mParticles[i].FadeRate = -(float)GameEngine.RandomValue.Next(6, 10) * 0.1f;
                    mParticles[i].Size = (float)GameEngine.RandomValue.NextDouble() + (float)GameEngine.RandomValue.NextDouble();
                }
            });
        }
        public override void Draw(string Technique)
        {
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW) return;
            ParticleFx.Parameters["ViewPortHeight"].SetValue(GameEngine.GraphicDevice.Viewport.Height);
            ParticleFx.Parameters["Projection"].SetValue(Camera.MatrixProjection);
            ParticleFx.Parameters["WorldViewProj"].SetValue(MathsUtility.CreateWorldMatrix(Position, Rotation, Scale) * Camera.MatrixViewProj);
            ParticleFx.Begin();
            for (int i = 0; i < ParticleFx.CurrentTechnique.Passes.Count; i++)
            {
                ParticleFx.CurrentTechnique.Passes[i].Begin();
                GameEngine.GraphicDevice.VertexDeclaration = mDeclare;
                GameEngine.GraphicDevice.DrawUserPrimitives(PrimitiveType.PointList, mParticles, 0, mParticles.Length);
                ParticleFx.CurrentTechnique.Passes[i].End();
            }
            GameEngine.GraphicDevice.VertexDeclaration = null;
            ParticleFx.End();
        }
    }
}
