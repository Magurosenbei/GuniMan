using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;


namespace Game
{
    public struct Scanline
    {
        public Vector2 Line;
        public Vector2 Velocity;
        public Vector2 Thickness;
        public float Life;
        public float delta;
        public bool flip;
    }
    public class OldTv : PostProcessor
    {
        float XPixel;
        float YPixel;
        int ShakeOffset = 0;
        TimeSpan Elapsed = new TimeSpan();
        TimeSpan FlashElapsed = new TimeSpan();
        float HighLight = 0.8f;

        EffectParameter Random;
        EffectParameter RandomFactor;
        EffectParameter ShakeOff;
        EffectParameter Timer;
        EffectParameter HighLights;

        Scanline[] Scans = new Scanline[3];
        public OldTv(int Width, int Height, GameScreen Parent) :
            base(GameEngine.Content.Load<Effect>("Content/Shader Fx/OldTv"), Width, Height, Parent)
        {
            Setup();
        }
        public OldTv(int Width, int Height) :
            base(GameEngine.Content.Load<Effect>("Content/Shader Fx/OldTv"), Width, Height)
        {
            Setup();
        }

        public void Setup()
        {
            XPixel = 1.0f / Width;
            YPixel = 1.0f / Height;
            DrawOrder = 0;
            Visible = false;
            for (int i = 0; i < 3; i++)
            {
                Scans[i].Line = new Vector2((float)GameEngine.RandomValue.NextDouble(), (float)GameEngine.RandomValue.NextDouble());
                Scans[i].Velocity = new Vector2(XPixel * (float)GameEngine.RandomValue.Next(100, 300));
                if (Scans[i].Line.X > 0.5f)
                    Scans[i].Velocity.X *= -1.0f;
                if (Scans[i].Line.Y > 0.5f)
                    Scans[i].Velocity.Y *= -1.0f;
                Scans[i].Life = 1.0f;
                Scans[i].flip = false;
                Scans[i].delta = -(float)GameEngine.RandomValue.Next(60, 100) * 0.01f;
            }
            Effect.Parameters["XPixel"].SetValue(XPixel);
            Effect.Parameters["YPixel"].SetValue(YPixel);
            Random = Effect.Parameters["Random"];
            RandomFactor = Effect.Parameters["RandomFact"];
            Timer = Effect.Parameters["Timer"];
            ShakeOff = Effect.Parameters["Shake"];
            HighLights = Effect.Parameters["HighLight"];
        }
        public override void Update()
        {
            if (!Visible) return;
            Elapsed += GameEngine.GameTime.ElapsedGameTime;
            FlashElapsed += GameEngine.GameTime.ElapsedGameTime;
            if (Elapsed > TimeSpan.FromMilliseconds(85))
            {
                Elapsed = new TimeSpan(0, 0, 0, 0, 0);
                ShakeOffset = GameEngine.RandomValue.Next(1, 5);
            }
            if (FlashElapsed > TimeSpan.FromMilliseconds(100))
            {
                FlashElapsed = new TimeSpan(0, 0, 0, 0, 0);
                if (HighLight < 1.0f)
                    HighLight = 1.0f;
                else
                    HighLight = 0.8f;
            }
            for (int i = 0; i < Scans.Length; i++)
            {
                if (Scans[i].flip && (Scans[i].Life > 1.0f || Scans[i].Life < 0))
                {
                    Scans[i].Line = new Vector2((float)GameEngine.RandomValue.NextDouble(), (float)GameEngine.RandomValue.NextDouble());
                    Scans[i].Velocity = new Vector2(XPixel * (float)GameEngine.RandomValue.Next(200, 600), 0);
                    if (Scans[i].Line.X > 0.5f)
                        Scans[i].Velocity.X *= -1.0f;
                    if (Scans[i].Line.Y > 0.5f)
                        Scans[i].Velocity.Y *= -1.0f;
                    Scans[i].Life = 1.0f;
                    Scans[i].flip = false;
                    Scans[i].delta = -(float)GameEngine.RandomValue.Next(60, 100) * 0.01f;
                }
                if ((Scans[i].Line.X < 1.0f && Scans[i].Line.X > 0) || (Scans[i].Line.Y < 1.0f && Scans[i].Line.Y > 0))
                {
                    Scans[i].Life += Scans[i].delta * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    Scans[i].Line += Scans[i].Velocity * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    if (!Scans[i].flip)
                        if (Scans[i].Life > 1.0f || Scans[i].Life < 0)
                        {
                            Scans[i].flip = true;
                            if (Scans[i].delta > 0)
                                Scans[i].delta = -(float)GameEngine.RandomValue.Next(60, 100) * 0.01f;
                            else
                                Scans[i].delta = (float)GameEngine.RandomValue.Next(60, 100) * 0.01f;
                        }
                }
                else
                {
                    Scans[i].Line = new Vector2((float)GameEngine.RandomValue.NextDouble(), (float)GameEngine.RandomValue.NextDouble());
                    Scans[i].Velocity = new Vector2(XPixel * (float)GameEngine.RandomValue.Next(200, 600), 0);
                    if (Scans[i].Line.X > 0.5f)
                        Scans[i].Velocity.X *= -1.0f;
                    if (Scans[i].Line.Y > 0.5f)
                        Scans[i].Velocity.Y *= -1.0f;
                    Scans[i].Life = 1.0f;
                    Scans[i].flip = false;
                    Scans[i].delta = -(float)GameEngine.RandomValue.Next(60, 100) * 0.01f;
                }
            }
            base.Update();
        }
        public override void Draw()
        {
            GetInputFromFrameBuffer(); // Set Input texture again
            GameEngine.GraphicDevice.Clear(Color.Black);
            Random.SetValue((float)GameEngine.RandomValue.NextDouble());
            RandomFactor.SetValue(GameEngine.RandomValue.Next(1, 3));
            ShakeOff.SetValue(ShakeOffset);
            Timer.SetValue((float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds);
            HighLights.SetValue(HighLight);

            Effect.Parameters["ScanPosition1"].SetValue(Scans[0].Line);
            Effect.Parameters["ScanLife1"].SetValue(Scans[0].Life);
            Effect.Parameters["ScanPosition2"].SetValue(Scans[1].Line);
            Effect.Parameters["ScanLife2"].SetValue(Scans[1].Life);
            Effect.Parameters["ScanPosition3"].SetValue(Scans[2].Line);
            Effect.Parameters["ScanLife3"].SetValue(Scans[2].Life);
            base.Draw(); // Apply blur
        }
    }
}