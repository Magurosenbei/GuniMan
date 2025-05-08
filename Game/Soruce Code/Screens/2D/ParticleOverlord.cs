using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Engine;

namespace Game
{
    public class ParticleOverlord : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }

        private List<ParticleEngine> DaParticleEngine;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        /////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;

        public ParticleOverlord(GameScreen Parent)
            : base(Parent)
        {
            DaParticleEngine = new List<ParticleEngine>();
            random = new Random(); // random is random /// lol
        }
        public ParticleOverlord()
            : base()
        {
            DaParticleEngine = new List<ParticleEngine>();
            random = new Random(); // random is random /// lol
        }
        public void CreateParticle(Vector2 Position, int type, int texturetype, int timer, int particletime, int direction)
        {
            //timer -100 for infininte
            DaParticleEngine.Add(new ParticleEngine(texturetype, Position, type, timer, particletime, direction));
        }
        private void KeyboardInput()
        {
            //if (KB.Key_Pressed(Keys.J))
            //{
            //    DaParticleEngine.Add(new ParticleEngine(1, 
            //        new Vector2(random.Next(1, GameEngine.GraphicDevice.Viewport.Width), random.Next(1, GameEngine.GraphicDevice.Viewport.Height)),
            //        5));
            //}
            //if (KB.Key_Pressed(Keys.H))
            //{
            //
            //}
        }
        public void End()
        {
            for (int i = 0; i < DaParticleEngine.Count; i++)
            {
                DaParticleEngine[i].SetTimer(0);
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            for (int i = 0; i < DaParticleEngine.Count; i++)
            {
                DaParticleEngine[i].Update();
                if (DaParticleEngine[i].GetTimer() == 0)
                {
                    DaParticleEngine.RemoveAt(i);
                    i--;
                }
            }
            KeyboardInput();
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public override void Draw()
        {
            for (int i = 0; i < DaParticleEngine.Count; i++)
            {
                DaParticleEngine[i].Draw();
            }
            //Debug.Write(DaParticleEngine.Count.ToString() + "\n");
        }
    }
}
