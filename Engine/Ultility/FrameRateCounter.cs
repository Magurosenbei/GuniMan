using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class FrameRateCounter : Component, I2DComponent
    {
        int frameRate = 0;
        int frameCounter = 0;
        int updateCounter = 0;
        int updateRate = 0;
        float LargestMemoryHeap = 0.0f;
        TimeSpan elapsedTime = TimeSpan.Zero;

        SpriteFont spriteFont;

        string FPSText = "FPS : 0";

        public Rectangle Rectangle { get; set; }
        public FrameRateCounter()
            : base()
        {
            spriteFont = GameEngine.Content.Load<SpriteFont>("Content/Fonts/Comic");

        }
        public FrameRateCounter(GameScreen Parent)
            : base(Parent)
        {
            Visible = false;
            spriteFont = GameEngine.Content.Load<SpriteFont>("Content/Fonts/Comic");
        }

        public override void Update()
        {
            if (!Visible) return;
            elapsedTime += GameEngine.GameTime.ElapsedGameTime;
            if ((float)Environment.WorkingSet * 0.000001f > LargestMemoryHeap)
                LargestMemoryHeap = (float)Environment.WorkingSet * 0.000001f;
            updateCounter++;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                try
                {
                    FPSText = "FPS : " + frameCounter.ToString() + " UPS : " + updateCounter.ToString();
                    FPSText += "\nMemory : " + ((float)Environment.WorkingSet * 0.000001f).ToString().Substring(0, 5) + "Mb \nBiggest Heap : " + LargestMemoryHeap.ToString().Substring(0, 5) + " MB";
                    FPSText += "\nPhysics Count : " + GameEngine.Services.GetService<Physics>().PhysicsSystem.Bodies.Count.ToString();
                    FPSText += "\nGC Memory : " + ((float)GC.GetTotalMemory(false) * 0.000001f).ToString().ToString().Substring(0, 5);
                }
                catch (Exception e)
                {
                    Debug.Write("\nText Problem in FPS counter Ignore =P " + e.Message);
                }
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                updateRate = updateCounter;
                frameCounter = 0;
                updateCounter = 0;
            }
            base.Update();
        }
        public override void Draw()
        {
            frameCounter++;
            GameEngine.SpriteBatch.Begin();
            GameEngine.SpriteBatch.DrawString(spriteFont, FPSText, new Vector2(20, 10), Color.White);
            GameEngine.SpriteBatch.End();
            base.Draw();
        }
    }
}
