using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Engine;

namespace Game
{
    public class ParticleEngine 
    {
        protected enum PatternChoice { SPREAD, LINE, CROSS, SPIN, SPINCROSS, RAIN }
        PatternChoice Pattern = PatternChoice.SPREAD;

        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<DaParticle> particles;
        private List<Texture2D> textures;
        private float SpinFactor;
        private int Timer;
        private int particletimespan;
        private int direction;
        
        public ParticleEngine(int texturetype, Vector2 location, int patterntype)
        {
            EmitterLocation = location;
            this.textures = new List<Texture2D>();
 
            this.particles = new List<DaParticle>();

            random = new Random();
            SpinFactor = 0.0f;
            Initialise(texturetype,patterntype);
            Timer = -100;
        }
        public ParticleEngine(int texturetype, Vector2 location, int patterntype, int timer, int particletimespanthingy, int direct)
        {
            EmitterLocation = location;
            this.textures = new List<Texture2D>();

            this.particles = new List<DaParticle>();

            random = new Random();
            SpinFactor = 0.0f;
            Initialise(texturetype, patterntype);
            Timer = timer;
            particletimespan = particletimespanthingy;
            direction = direct;
        }
        
        private void Initialise(int ttype, int ptype)
        {
            if (ttype == 1)
            {
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/circle"));
            }
            else if (ttype == 3)
            {
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/circle"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/diamond"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/star"));

                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/circle"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/diamond"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/star"));

                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/circle"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/diamond"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/star"));
            }
            else
            {
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/circle"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/diamond"));
                textures.Add(GameEngine.Content.Load<Texture2D>("Content/Particles/star"));

            }
            for (int i = 0; i < ptype; i++)
            {
                Pattern++;
            }
        }
        public int GetTimer()
        {
            return Timer;
        }
        public void SetTimer(int temp)
        {
            Timer = temp;
        }
        public void Update()
        {
            if (Timer > 0)
            {
                Timer--;
            }
            if (Timer - 15 > 0 || Timer == -100)
            {
                if (Pattern == PatternChoice.SPREAD)
                {
                    for (int i = 0; i < 1; i++)                     //how much particles * 60 per second
                    {
                        particles.Add(GenerateNewParticle(0, 1.0f, particletimespan));
                    }
                }
                if (Pattern == PatternChoice.LINE)
                {
                    for (int i = 0; i < 1; i++)                     //how much particles * 60 per second
                    {
                        particles.Add(GenerateNewParticle(direction, 1.0f, particletimespan));
                    }
                }
                if (Pattern == PatternChoice.CROSS)
                {
                    for (int i = 0; i < 4; i++)                     //how much particles * 60 per second
                    {
                        particles.Add(GenerateNewParticle(i * 90.0f, 1.0f, particletimespan));
                    }
                }
                if (Pattern == PatternChoice.SPIN)
                {
                    for (int i = 0; i < 1; i++)                     //how much particles * 60 per second
                    {
                        particles.Add(GenerateNewParticle(0, 1.0f, particletimespan));
                    }
                }
                if (Pattern == PatternChoice.SPINCROSS)
                {
                    for (int i = 0; i < 4; i++)                     //how much particles * 60 per second
                    {
                        particles.Add(GenerateNewParticle(i * 90.0f, 1.0f, particletimespan));
                    }
                }
                if (Pattern == PatternChoice.RAIN)
                {
                    for (int i = 0; i < 1; i++)                     //how much particles * 60 per second
                    {
                        particles.Add(GenerateNewParticle(180, 2.0f, particletimespan));
                    }
                }
            }
       
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private DaParticle GenerateNewParticle(float direction, float speed, int particletimespan)
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(0.0f, -10.0f);
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());
            float size = (float)random.NextDouble();
            int ttl = particletimespan + random.Next(40);

           
            if (Pattern == PatternChoice.SPREAD)
            {
                velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));
            }
            if (Pattern == PatternChoice.LINE)
            {
                
            }
            if (Pattern == PatternChoice.CROSS)
            {

            }
            if (Pattern == PatternChoice.SPIN)
            {
                velocity = Vector2.Transform(velocity, Matrix.CreateRotationZ(MathHelper.ToRadians(SpinFactor)));
                if (SpinFactor >= 360)
                {
                    SpinFactor = 0;
                }
                SpinFactor++;
            }
            if (Pattern == PatternChoice.SPINCROSS)
            {
                velocity = Vector2.Transform(velocity, Matrix.CreateRotationZ(MathHelper.ToRadians(SpinFactor)));
                if (SpinFactor >= 360)
                {
                    SpinFactor = 0;
                }
                SpinFactor++;
            }
            if (Pattern == PatternChoice.RAIN)
            {
                position = new Vector2(position.X + (float)random.Next(-50, 50), position.Y); 
            }
            

            //direction
            velocity = Vector2.Transform(velocity, Matrix.CreateRotationZ(MathHelper.ToRadians(direction)));
            velocity = new Vector2(velocity.X * speed, velocity.Y * speed);

            
            return new DaParticle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
            
        }
        public void Draw()
        {
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.Additive);
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw();
            }
            GameEngine.SpriteBatch.End();
        }
    }
}
