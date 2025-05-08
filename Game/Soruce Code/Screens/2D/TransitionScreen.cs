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
    public class TransitionScreen : Component, I2DComponent
    {

        public Rectangle Rectangle { get; set; }
        //////////////////
        ///Textures
        /////////////////
        private Texture2D BackGround;
      

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        //////////////////////////////
        /// Interactive variables 
        /////////////////////////////
        private bool Start;
        private float FadeValue;
        private bool FadeIn;
        private bool FadeOut;

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundPosition;

        //scaling
        private Vector2 BackGroundScale;

        /////////////////////
        /// Stuff
        //////////////////////
        private int Com1;
        private int Com2;

        //Cutscene
        private Vector3 Position;
        private Matrix Rotation;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public TransitionScreen(GameScreen Parent)
            : base(Parent)
        {
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background2");
            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BackGroundPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.5f );
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));

            Start = false;
            FadeValue = 0.0f;
            FadeIn = false;
            FadeOut = false;

            Com1 = Com2 = 0;
            //Cutscene
            Vector3 Position = new Vector3(0.0f);
            Matrix Rotation = new Matrix();


        }
        public void ScreenUpdate()
        {
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));
        }
        public void StartFading(int com1, int com2)
        {
            //if (!Start)
            //{
                Com1 = com1;
                Com2 = com2;

                Visible = true;
                Start = true;
                FadeIn = true;
                FadeOut = false;
            //}
        }
        private void Reset()
        {
            FadeIn = FadeOut = false;
            FadeValue = 0.0f;
            Com1 = Com2 = 0;

            Visible = false;
            Start = false;
            
        }
        public void SetScene(Vector3 pos, Matrix rot)
        {
            Position = pos;
            Rotation = rot;
        }
                          
        private void PowerControl()
        {
            //-1 just fade
            //0 - game
            //1 - Prologue
            //2 - MainMenu
            //3 - Loading
            //4 - InGameMenu
            //5 - End Scene
            //6 - SellScene

            //Com1 End // Com2 Start
            

            if (Com1 == 1)
            {
                GameEngine.Services.GetService<Prologue>().Reset();
            }
            if (Com1 == 2)
            {
                GameEngine.Services.GetService<MainMenu>().Reset();
            }
            if (Com1 == 4)
            {
                MainGame.APP_STATE = APP_STATE.UNLOAD;
                GameEngine.Services.GetService<Interactive>().Visible = false;
                GameEngine.Services.GetService<InGameMainMenu>().Reset();
            }
            if (Com1 == 5)
            {
                MainGame.APP_STATE = APP_STATE.UNLOAD;
                GameEngine.Services.GetService<ObtainHouseCut>().EndScene();                
                GameEngine.Services.GetService<Interactive>().Visible = false;
            }
            if (Com1 == 6)
            {
                GameEngine.Services.GetService<SellCutScene>().EndScene();
                GameEngine.Services.GetService<SoundManager>().ChangeSong("normal");
            }


            if (Com2 == 2)
            {   
                GameEngine.Services.GetService<MainMenu>().Visible = true;
            }
            if (Com2 == 3)
            {
                GameEngine.Services.GetService<LoadingScreen>().Visible = true;
            }
            if (Com2 == 5)
            {
                GameEngine.Services.GetService<ObtainHouseCut>().Initialize_Scene(Position, new Vector3(0, 0, 25), Rotation);            
            }
            if (Com2 == 6)
            {
                GameEngine.Services.GetService<InventoryDisplay>().Reset();
                GameEngine.Services.GetService<SellCutScene>().Initialize_Scene(GameEngine.Services.GetService<PlayerStats>().GetPopularity(),
                    GameEngine.Services.GetService<InventoryDisplay>().GetSellPercent());
            }
          
        }
        public void Fading()
        {
            // Spd up for debug
            if (FadeIn)
            {
                FadeValue += 0.1f;
            }
            if (FadeOut)
            {
                FadeValue -= 0.01f;
            }
            if (FadeValue >= 1.0f && FadeIn)
            {
                FadeIn = false;
                FadeOut = true;
                PowerControl();
            }
            if (FadeValue <= 0.0f && FadeOut)
            {
                Reset();
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (Start)
            {
                Fading();
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
                
                //Background
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, new Color(Color.White,FadeValue),
                           0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);


            }
            GameEngine.SpriteBatch.End();

        }
    }
}
