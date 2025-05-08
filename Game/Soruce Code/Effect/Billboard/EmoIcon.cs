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
    public class EmoIconSub
    {
        public Vector3 Position = Vector3.Zero;
        public TimeSpan Timeout = new TimeSpan(0, 0, 0, 3, 0);
        public TimeSpan Duration = new TimeSpan(0, 0, 0, 3, 0);
        public bool Visisble = false;
        public float Size = 4.0f;
        public int Emotion = 0;
        public EmoIconSub() { }
    }
    public class EmoIconSystem : ParticleEmitor
    {
        static public List<EmoIconSub> Subscribe = new List<EmoIconSub>();
        List<Texture2D> Emotion;
        public EmoIconSystem(IEContentManager Contentmanager) : base(1, true, Contentmanager) { }

        protected override void Setup(int Amount, IEContentManager ContentManager)
        {
            //mParticles = new List<Particle>();
            mParticles = new Particle[1];
            //Particle TBA = new Particle();
            mParticles[0].Color = Vector4.One;
            mParticles[0].Position = Vector3.Zero;
            mParticles[0].Velocity = Vector3.Zero;
            mParticles[0].Size = 5.0f;
            //mParticles.Add(TBA);
            Position = Vector3.Zero;
            mDeclare = new VertexDeclaration(GameEngine.GraphicDevice, Particle.VertexElements);
            ParticleFx = ContentManager.Load<Effect>("Content/Shader Fx/ParticleFX").Clone(GameEngine.GraphicDevice);
            ParticleFx.CurrentTechnique = ParticleFx.Techniques["BillBoardSprite"];
            Camera = GameEngine.Services.GetService<Camera>();
            Rotation = Matrix.Identity;
            Scale = Vector3.One;
            Emotion = new List<Texture2D>();
            for (int i = 0; i < VariableAsset.EmoIcon.Count; i++)
                Emotion.Add(ContentManager.Load<Texture2D>(VariableAsset.EmoIcon[i]));
        }
        public override void Disable()
        {
            for (int i = 0; i < Emotion.Count; i++)
                Emotion[i].Dispose();
            Emotion.Clear();
            Subscribe.Clear();
            base.Disable();
        }
        public override void Update()
        {
            Parallel.For(0, Subscribe.Count, delegate(int i)
            {
                if (Subscribe[i].Timeout > Subscribe[i].Duration)
                    Subscribe[i].Visisble = false;
                else
                    Subscribe[i].Timeout += GameEngine.GameTime.ElapsedGameTime;
            });
        }
        public override void Draw(string Technique)
        {
            for (int i = 0; i < Subscribe.Count; i++)
            {
                if (!Subscribe[i].Visisble) continue;
                Position = Subscribe[i].Position;
                mParticles[0].Size = Subscribe[i].Size;
                ParticleFx.Parameters["BasicTexture"].SetValue(Emotion[Subscribe[i].Emotion]);
                base.Draw(Technique);
            }
        }
    }
}
