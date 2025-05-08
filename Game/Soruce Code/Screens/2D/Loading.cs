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
    public class LoadingScreen : Component, I2DComponent
    {

        public Rectangle Rectangle { get; set; }
        //////////////////
        ///Textures
        /////////////////
        private Texture2D Loading;
        private Texture2D Logo;


        //////////////////
        /// Load Text
        //////////////////
        private List<LoadingDef> LoadingDefList;

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle LoadingRectangle;
        private Vector2 LoadingOrigin;

        private Rectangle LogoRectangle;
        private Vector2 LogoOrigin;

        private Vector2 LoadingPosition;

        /////////////////////
        /// Variables
        //////////////////////
        private float Spin;
        private float Spin2;
        private float SpinValue;
        private float Brightness;

        //////////////
        /// The Font
        ///////////////
        SpriteFont WriteText;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        private Effect myEffect;

        public LoadingScreen(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {
            WriteText = spritefont;
            Spin = -1;
            Spin2 = 10;
            Spin2 = 20;
            SpinValue = 3.0f;

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");

            Brightness = 0.2f;
            Loading = GameEngine.Content.Load<Texture2D>("Content/Loading/Sprites/mainload2");
            Logo = GameEngine.Content.Load<Texture2D>("Content/Loading/Sprites/maintitle3");
            LoadingRectangle = new Rectangle(0, 0, Loading.Width, Loading.Height);
            LoadingOrigin = new Vector2(Loading.Width * 0.5f, Loading.Height * 0.5f);
            LogoRectangle = new Rectangle(0, 0, Logo.Width, Logo.Height);
            LogoOrigin = new Vector2(Logo.Width * 0.5f, Logo.Height * 0.5f);
            LoadingPosition = new Vector2(Resizer.GetWidth(1.0f) - LoadingOrigin.X, Resizer.GetHeight(1.0f) - LoadingOrigin.Y);

            LoadingDefList = new List<LoadingDef>();
   
        }
        public void ScreenUpdate()
        {
            LoadingPosition = new Vector2(Resizer.GetWidth(1.0f) - LoadingOrigin.X, Resizer.GetHeight(1.0f) - LoadingOrigin.Y);
            
            for (int i = 0; i < LoadingDefList.Count; i++)
            {
                LoadingDefList[i].PositionCheck(Resizer.GetWidth(0.0f), Resizer.GetHeight(1.0f));
            }
        }
        public void Reset()
        {
            LoadingDefList.Clear();
            Brightness = 0.2f;
            Spin = -1;
            SpinValue = 3.0f;
        }
        public void AddNewLine(string Text, int status)
        {
            if (status == 0 || status == 2)
            {
                for (int i = 0; i < LoadingDefList.Count; i++)
                {
                    LoadingDefList[i].GoUpOneLine();
                }
            }
            if (status == 0)
            {
                LoadingDefList.Add(new LoadingDef("Loading - " + Text, new Vector2(Resizer.GetWidth(0.0f), Resizer.GetHeight(1.0f) - 20.0f), 0));
            }
            else if (status == 1)
            {
                LoadingDefList[LoadingDefList.Count - 1].SetText("Loaded - " + Text);
                BrightnessChange(0.1f);
                SpinValueChange(0.4f);
            }
            else if (status == 2)
            {
                LoadingDefList.Add(new LoadingDef(Text, new Vector2(Resizer.GetWidth(0.0f), Resizer.GetHeight(1.0f) - 20.0f), 0));
                BrightnessChange(0.1f);
                SpinValueChange(0.4f);
            }
        }
        private void SpinTheSpin()
        {
            if (Spin >= 360.0f)
            {
                Spin -= 360.0f;
            }
            Spin += SpinValue;
            if (Spin2 >= 360.0f)
            {
                Spin -= 360.0f;
            }
            Spin2 += SpinValue;
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (Spin == -1)
            {
                Spin = 0;
            }
            SpinTheSpin();
        }
        private void BrightnessChange(float temp)
        {
            Brightness += temp;
        }
        private void SpinValueChange(float temp)
        {
            SpinValue += temp;
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
                myEffect.Parameters["Intensity"].SetValue(Brightness);
                myEffect.Begin();
                myEffect.CurrentTechnique.Passes[0].Begin();
                
                //Loading
                GameEngine.SpriteBatch.Draw(Loading, LoadingPosition, LoadingRectangle, Color.White,
                            MathHelper.ToRadians(Spin), LoadingOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0f);

                ////Loading
                //GameEngine.SpriteBatch.Draw(Loading, LoadingPosition, LoadingRectangle, Color.White,
                //            MathHelper.ToRadians(Spin2), LoadingOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0f);


                myEffect.CurrentTechnique.Passes[0].End();
                myEffect.End();
            }
            GameEngine.SpriteBatch.End();

            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                
                //Loading
                //GameEngine.SpriteBatch.Draw(Loading, LoadingPosition, LoadingRectangle, Color.White,
                //            MathHelper.ToRadians(Spin2), LoadingOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0f);

                //Title
                GameEngine.SpriteBatch.Draw(Logo, LoadingPosition, LogoRectangle, Color.White,
                            0.0f, LogoOrigin, 1.0f, SpriteEffects.None, 0f);
                
                //Loading List
                for (int i = 0; i < LoadingDefList.Count; i++)
                {
                    GameEngine.SpriteBatch.DrawString(WriteText, LoadingDefList[i].GetText(), LoadingDefList[i].GetPosition(), Color.White,
                        0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
                }
                //GameEngine.SpriteBatch.DrawString(WriteText, DisplayText.Substring(0, counter), DialoguePosition, Color.White,
                   //0.0f, new Vector2(3.5f * DisplayText.Length, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
        



            }
            GameEngine.SpriteBatch.End();

        }
    }
}
