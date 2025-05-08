using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine;


namespace Game
{
    class RadialBlur : PostProcessor
    {
        float fBlurWidth = 0.0f;
        float fBlurStart = 1.0f;
        EffectParameter BlurWidth;
        EffectParameter BlurStart;

        bool flip = false;
        public bool EndSequence = false;
        public RadialBlur(int Width, int Height, GameScreen Parent) 
            : base(GameEngine.Content.Load<Effect>("Content/Shader Fx/RadialBlur"), Width, Height, Parent)
        {
            Setup();
        }
        public RadialBlur(int Width, int Height)
            : base(GameEngine.Content.Load<Effect>("Content/Shader Fx/RadialBlur"), Width, Height)
        {
            Setup();
        }

        void Setup()
        {
            Visible = false;
            BlurWidth = Effect.Parameters["BlurWidth"];
            BlurStart = Effect.Parameters["BlurStart"];
        }
        public override void Update()
        {
            if (!Visible) return;
            if (EndSequence)
            {
                if (fBlurWidth < 0.0f)
                    fBlurWidth += 0.1f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                else
                {
                    fBlurWidth = 0.0f;
                    Visible = false;
                }
            }
            else
            {
                if (!flip)
                    if (fBlurWidth > -0.25f)
                        fBlurWidth -= 0.1f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    else
                        flip = !flip;
                else
                    if (fBlurWidth < -0.1f)
                        fBlurWidth += 0.1f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    else
                        flip = !flip;
            }
            base.Update();
        }
        public override void Draw()
        {
            BlurWidth.SetValue(fBlurWidth);
            BlurStart.SetValue(fBlurStart);
            GetInputFromFrameBuffer(); // Set Input texture again
            GameEngine.GraphicDevice.Clear(Color.Black);
            base.Draw();
        }
    }
}
