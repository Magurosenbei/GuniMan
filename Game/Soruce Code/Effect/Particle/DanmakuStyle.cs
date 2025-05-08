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
    public class DanmakuStyle_Spiral : ParticleEmitor
    {
        TimeSpan Elapsed = new TimeSpan();
        TimeSpan BulletInterval = new TimeSpan(0, 0, 0, 0, 26);
        public Vector3 VelocityFactor = new Vector3(3, 0, 3);
        public float YVelocity = 0.0f;
        float OffsetAngle = 0.0f;
        float Height = 0.0f;
        public DanmakuStyle_Spiral(IEContentManager ContentManager) : base(512, false, ContentManager) { }

        protected override void Setup(int Amount, IEContentManager ContentManager)
        {
            mParticles = new Particle[Amount];
            for (int i = 0; i < Amount; i++)
            {
                mParticles[i].Position = Position;
                mParticles[i].Color = new Vector4(1,1,1,0);
                mParticles[i].Size = 1.0f;
            }
            mDeclare = new VertexDeclaration(GameEngine.GraphicDevice, Particle.VertexElements);

            ParticleSprite = ContentManager.Load<Texture2D>("Content/Shader Fx/Sprite/Particle");
            ParticleFx = ContentManager.Load<Effect>("Content/Shader Fx/ParticleFX").Clone(GameEngine.GraphicDevice);
            ParticleFx.Parameters["BasicTexture"].SetValue(ParticleSprite);
            Camera = GameEngine.Services.GetService<Camera>();
            Rotation = Matrix.Identity;
            Scale = Vector3.One;
        }
        public override void Update()
        {
            if (Remove)
            {
                Height = 0.0f;
                Remove = false;
            }
            float dt = (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
            Done = true;
            Parallel.For(0, mParticles.Length, delegate(int i)
            {
                //Particle Access = mParticles[i];
                if (mParticles[i].Color.W > 0)
                {
                    mParticles[i].Position += mParticles[i].Velocity * dt;
                    if(mParticles[i].Velocity.Y < 9.8f)
                        mParticles[i].Velocity.Y += -0.98f * dt;
                    mParticles[i].Color.W += mParticles[i].FadeRate * dt;
                    Done = false;
                }
            });
            Elapsed += GameEngine.GameTime.ElapsedGameTime;

            if (Elapsed < BulletInterval || StopSpawn) return;
            if (OffsetAngle < Math.PI * 2.0f) OffsetAngle += 0.05f;
            else OffsetAngle = 0.0f;
            if (Height < 10.0f)
                Height += 0.1f;
            else
            {
                StopSpawn = true;
                Height = 0;
            }
            Parallel.For(0, 4, delegate(int index)
            {
                float Angle = index * (float)Math.PI * 0.5f;
                for (int i = 0; i < mParticles.Length; i++)
                {
                    if (mParticles[i].Color.W > 0) continue;
                    mParticles[i].Position = new Vector3(0, Height * 0.5f, 0);
                    mParticles[i].Velocity.X = (float)Math.Sin(Angle + OffsetAngle) * VelocityFactor.X;
                    mParticles[i].Velocity.Z = (float)Math.Cos(Angle + OffsetAngle) * VelocityFactor.Z;
                    mParticles[i].Velocity.Y = YVelocity * VelocityFactor.Y;
                    mParticles[i].Color.X = (float)GameEngine.RandomValue.NextDouble();
                    mParticles[i].Color.Y = (float)GameEngine.RandomValue.NextDouble();
                    mParticles[i].Color.Z = (float)GameEngine.RandomValue.NextDouble();
                    mParticles[i].Color.W = 1.0f;
                    mParticles[i].FadeRate = -(float)GameEngine.RandomValue.Next(2, 6) * 0.1f;
                    break;
                }
            });
            Elapsed = new TimeSpan();
        }
    }
}
