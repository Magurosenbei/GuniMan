using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Engine;
using XmlContentExtension;

namespace Game
{
    public class Interactive : Component, I2DComponent
    {

        public Rectangle Rectangle { get; set; }
        //////////////////
        ///Textures
        /////////////////
        private Texture2D LightBulb;
      

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        //////////////////////////////
        /// Interactive variables 
        /////////////////////////////
                       

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle LightBulbRectangle;
        private Vector2 LightBulbOrigin;
        private Vector2 LightBulbPosition;

        private bool LightUp;


        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();
        
        private Effect myEffect;

        public Interactive(GameScreen Parent)
            : base(Parent)
        {
            LightBulb = GameEngine.Content.Load<Texture2D>("Content/Interactive/LightBulb");
            LightBulbRectangle = new Rectangle(0, 0, LightBulb.Width, LightBulb.Height);
            LightBulbOrigin = new Vector2(LightBulb.Width * 0.5f, LightBulb.Height * 0.5f);
            LightBulbPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f) );

            LightUp = false;

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
        }
        public void ScreenUpdate()
        {

        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            for (int i = 0; i < 5; i++)
            {
                if (GameEngine.GameTime.TotalGameTime.Milliseconds >= 200 * i && GameEngine.GameTime.TotalGameTime.Milliseconds < (100 + (200 * i)))
                {
                    LightUp = true;
                }
                if (GameEngine.GameTime.TotalGameTime.Milliseconds >= (100 + (200 * i)) && GameEngine.GameTime.TotalGameTime.Milliseconds < (200 * (i + 1) ) )
                {
                    LightUp = false;
                }
            }
        }
        
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public override void Draw()
        {
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                if (LightUp)
                {
                    myEffect.Begin();
                    myEffect.CurrentTechnique.Passes[1].Begin();
                }
                //LightBulb
                GameEngine.SpriteBatch.Draw(LightBulb, LightBulbPosition, LightBulbRectangle, Color.White,
                           0.0f, LightBulbOrigin, 1.0f, SpriteEffects.None, 0f);

                if (LightUp)
                {
                    myEffect.CurrentTechnique.Passes[1].End();
                    myEffect.End();
                }

            }
            GameEngine.SpriteBatch.End();

        }
    }
}
