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
    public class Winlose : Component, I2DComponent
    {

        public Rectangle Rectangle { get; set; }
        //////////////////
        ///Textures
        /////////////////
        private Texture2D Picture;
      

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        //////////////////////////////
        /// Interactive variables 
        /////////////////////////////

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle PictureRectangle;
        private Vector2 PictureOrigin;
        private Vector2 PicturePosition;
        private Vector2 PictureScale;

        public Winlose(GameScreen Parent)
            : base(Parent)
        {
            Picture = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/AchievementsScreen");
            PictureRectangle = new Rectangle(0, 0, Picture.Width, Picture.Height);
            PictureOrigin = new Vector2(Picture.Width * 0.5f, Picture.Height * 0.5f);
            PicturePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            PictureScale = new Vector2( ((Resizer.GetRealWidth(1.0f) / (float)Picture.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / Picture.Height) * 1.0f));

        }
        public void ScreenUpdate()
        {
            PictureScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)Picture.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / Picture.Height) * 1.0f));
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
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
                
                //Win
                GameEngine.SpriteBatch.Draw(Picture, PicturePosition, PictureRectangle, Color.White,
                          0.0f, PictureOrigin,PictureScale, SpriteEffects.None, 0f);


            }
            GameEngine.SpriteBatch.End();

        }
    }
}
