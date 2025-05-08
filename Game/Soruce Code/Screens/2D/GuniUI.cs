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
    public class GuniUI : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        ///////////////////
        /// Font
        //////////////////
        public SpriteFont WriteText;                  // The Font

        //////////////////
        ///Textures
        /////////////////
        private Texture2D GuniFace;
        private Texture2D HumanFace;
        private Texture2D BackGround;

        private Texture2D AButton;
        private Texture2D BButton;
        private Texture2D XButton;
        private Texture2D YButton;
      

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        //////////////////////////////
        /// GuniUI variables 
        /////////////////////////////
        private string AButtonUsage;
        private string BButtonUsage;
        private string XButtonUsage;
        private string YButtonUsage;

        //public int Money;

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle FaceRectangle;
        private Vector2 FaceOrigin;
        private Vector2 GuniFacePosition;
        private Vector2 HumanFacePosition;

        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;

        private Rectangle ButtonRectangle;
        private Vector2 ButtonOrigin;


        private Vector2 LeftBackGroundPosition;
        private Vector2 RightBackGroundPosition;

        private Vector2 AButtonPosition;
        private Vector2 BButtonPosition;
        private Vector2 XButtonPosition;
        private Vector2 YButtonPosition;

        private bool Portrait;
       
        //Effect myEffect;

        public GuniUI(SpriteFont spritefont,GameScreen Parent)
            : base(Parent)
        {
            WriteText = spritefont;

            GuniFace = GameEngine.Content.Load<Texture2D>("Content/GuniUI/Sprites/mizore_peacesign");
            HumanFace = GameEngine.Content.Load<Texture2D>("Content/GuniUI/Sprites/mizore_peacesign");
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");

            AButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/AButton");
            BButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/BButton");
            XButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/XButton");
            YButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/YButton");

            AButtonUsage = "Test";
            BButtonUsage = "Danightmare";
            XButtonUsage = "-";
            YButtonUsage = "-";

            Portrait = false;

            ButtonRectangle = new Rectangle(0, 0, AButton.Width, AButton.Height);
            ButtonOrigin = new Vector2(AButton.Width * 0.5f, AButton.Height * 0.5f);
            
            FaceRectangle = new Rectangle(0, 0, GuniFace.Width, GuniFace.Height);
            FaceOrigin = new Vector2(GuniFace.Width * 0.5f, GuniFace.Height * 0.5f);
            GuniFacePosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f + FaceOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.9f - FaceOrigin.Y);
            HumanFacePosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.9f - FaceOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.9f - FaceOrigin.Y);

            BackGroundRectangle = new Rectangle(0, 0, 150, 150);
            BackGroundOrigin = new Vector2(75.0f, 75.0f);
            LeftBackGroundPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f + BackGroundOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.9f - BackGroundOrigin.Y);
            RightBackGroundPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.9f - BackGroundOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.9f - BackGroundOrigin.Y);

            AButtonPosition = new Vector2(RightBackGroundPosition.X - BackGroundOrigin.X + ButtonOrigin.X * 2, RightBackGroundPosition.Y - BackGroundOrigin.Y + ButtonOrigin.Y * 2 + 10.0f);
            BButtonPosition = new Vector2(AButtonPosition.X , AButtonPosition.Y + 25.0f);
            XButtonPosition = new Vector2(BButtonPosition.X , BButtonPosition.Y + 25.0f);
            YButtonPosition = new Vector2(XButtonPosition.X , XButtonPosition.Y + 25.0f);


            //myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
        }
        public void SetCommand(string A, string B, string X, string Y)
        {
            AButtonUsage = A;
            BButtonUsage = B;
            XButtonUsage = X;
            YButtonUsage = Y;
        }
        public void SetPortrait(string name)
        {
            HumanFace = GameEngine.Content.Load<Texture2D>("Content/GuniUI/Sprites/" + name);
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (KB.Key_Pressed(Keys.Z))
            {
                Portrait = !Portrait;
            }
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = true;
            base.InitializeComponent(Parent);
        }
        public override void Draw()
        {
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //Left Background
                GameEngine.SpriteBatch.Draw(BackGround, LeftBackGroundPosition, BackGroundRectangle, Color.White,
                          0.0f, BackGroundOrigin, 1.0f, SpriteEffects.None, 0f); 
                //GuniMan
                GameEngine.SpriteBatch.Draw(GuniFace, GuniFacePosition, FaceRectangle, Color.White,
                           0.0f, FaceOrigin, 1.0f, SpriteEffects.None, 0f);

                //Right Background
                GameEngine.SpriteBatch.Draw(BackGround, RightBackGroundPosition, BackGroundRectangle, Color.White,
                          0.0f, BackGroundOrigin, 1.0f, SpriteEffects.None, 0f);

                if (!Portrait)
                {
                    //A Button
                    GameEngine.SpriteBatch.Draw(AButton, AButtonPosition, ButtonRectangle, Color.White,
                              0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                    //A Button Text
                    GameEngine.SpriteBatch.DrawString(WriteText, AButtonUsage, new Vector2(AButtonPosition.X + 20.0f, AButtonPosition.Y - ButtonOrigin.Y - 1.0f), Color.White,
                       0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);

                    //B Button
                    GameEngine.SpriteBatch.Draw(BButton, BButtonPosition, ButtonRectangle, Color.White,
                              0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                    //B Button Text
                    GameEngine.SpriteBatch.DrawString(WriteText, BButtonUsage, new Vector2(BButtonPosition.X + 20.0f, BButtonPosition.Y - ButtonOrigin.Y - 1.0f), Color.White,
                       0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);

                    //X Button
                    GameEngine.SpriteBatch.Draw(XButton, XButtonPosition, ButtonRectangle, Color.White,
                              0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                    //X Button Text
                    GameEngine.SpriteBatch.DrawString(WriteText, XButtonUsage, new Vector2(XButtonPosition.X + 20.0f, XButtonPosition.Y - ButtonOrigin.Y - 1.0f), Color.White,
                       0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);

                    //Y Button
                    GameEngine.SpriteBatch.Draw(YButton, YButtonPosition, ButtonRectangle, Color.White,
                              0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                    //Y Button Text
                    GameEngine.SpriteBatch.DrawString(WriteText, YButtonUsage, new Vector2(YButtonPosition.X + 20.0f, YButtonPosition.Y - ButtonOrigin.Y - 1.0f), Color.White,
                       0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    //Human
                    GameEngine.SpriteBatch.Draw(HumanFace, HumanFacePosition, FaceRectangle, Color.White,
                               0.0f, FaceOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0f);

                }
               

            }
            GameEngine.SpriteBatch.End();

        }
    }
}
