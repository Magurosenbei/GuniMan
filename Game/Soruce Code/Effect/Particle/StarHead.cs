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
    public static class LastDraw
    {
        public static List<ObjectType> LastList = new List<ObjectType>();
    }
    public class StarHead : ParticleEmitor
    {
        float MoveBy = 0.0f;
        float DeltaAngle = 0.0f;
        float[] Bearing;
        float RadiusDiff = 0.0f;
        TimeSpan Duration = new TimeSpan(0, 0, 0, 10, 0);
        TimeSpan Elapsed = new TimeSpan();
        public StarHead(int Stars, float Radius, IEContentManager ContentManager) : base(Stars, false, ContentManager) { RadiusDiff = Radius; }

        protected override void Setup(int Amount, IEContentManager ContentManager)
        {
            LastDraw.LastList.Add(this);
            bLastDraw = true;
            Bearing = new float[Amount];
            mParticles = new Particle[Amount];

            float Interval = (float)Math.PI * 2.0f / Amount;
            for (int i = 0; i < Amount; i++)
            {
                Bearing[i] = i * Interval;
                mParticles[i].Position.X = (float)Math.Sin(Bearing[i]) * RadiusDiff;
                mParticles[i].Position.Z = (float)Math.Cos(Bearing[i]) * RadiusDiff;
                mParticles[i].Color = new Vector4(1, 1, 1, 1);
                mParticles[i].FadeRate = 0.0f;
                mParticles[i].Size = 2.0f;
            }
            mDeclare = new VertexDeclaration(GameEngine.GraphicDevice, Particle.VertexElements);

            ParticleSprite = ContentManager.Load<Texture2D>("Content/Shader Fx/Sprite/Star");
            ParticleFx = ContentManager.Load<Effect>("Content/Shader Fx/ParticleFX").Clone(GameEngine.GraphicDevice);
            ParticleFx.Parameters["BasicTexture"].SetValue(ParticleSprite);
            ParticleFx.CurrentTechnique = ParticleFx.Techniques["StarToon"];
            Camera = GameEngine.Services.GetService<Camera>();
            Rotation = Matrix.Identity;
            Scale = Vector3.One;

            DeltaAngle = 2.409f;
        }
        public override void Disable()
        {
            Bearing = null;
            base.Disable();
        }
        public override void Update()
        {
            Elapsed += GameEngine.GameTime.ElapsedGameTime;
            Remove = true;
            for (int i = 0; i < mParticles.Length; i++)
            {
                if (mParticles[i].Color.W > 0) Remove = false;
                if (Bearing[i] > Math.PI * 2.0f)
                    Bearing[i] -= (float)Math.PI * 2.0f;
                else if (Bearing[i] < 0)
                    Bearing[i] += (float)Math.PI * 2.0f;
                mParticles[i].Position.X = (float)Math.Sin(Bearing[i]) * (RadiusDiff / Scale.X);
                mParticles[i].Position.Z = (float)Math.Cos(Bearing[i]) * (RadiusDiff / Scale.Z);
                Bearing[i] += DeltaAngle * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                MoveBy += DeltaAngle * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                if (Elapsed > Duration && mParticles[i].FadeRate == 0.0f)
                {
                    Elapsed -= new TimeSpan(0, 0, 0, 0, 50);
                    mParticles[i].FadeRate = -(float)1.0f;
                }
                if (mParticles[i].Color.W > 0)
                    mParticles[i].Color.W += mParticles[i].FadeRate * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
            }
            CalculatedOffset = (Matrix.CreateTranslation(CenterOffset) * Rotation).Translation;
        }
        public override void Draw(string Technique)
        {
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW) return;
            ParticleFx.Parameters["ViewPortHeight"].SetValue(GameEngine.GraphicDevice.Viewport.Height);
            ParticleFx.Parameters["Projection"].SetValue(Camera.MatrixProjection);
            ParticleFx.Parameters["WorldViewProj"].SetValue(World* Camera.MatrixViewProj);
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
