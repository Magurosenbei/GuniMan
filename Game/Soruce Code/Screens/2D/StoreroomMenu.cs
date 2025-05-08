//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Engine;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using System.Diagnostics;

//namespace Game
//{
//    public class StoreRoomMenu : Component, I2DComponent
//    {
//        public Rectangle Rectangle { get; set; }

//        ////////////////////////
//        /// Selection States
//        ////////////////////////
//        protected enum StoreMenu { SAVE, REST, EXIT }
//        StoreMenu MenuChoice = StoreMenu.SAVE;

//        ///////////////////
//        /// Font
//        //////////////////
//        public SpriteFont WriteText;                  // The Font
       
//        /////////////////
//        /// Sound
//        ///////////////
//        Cue SoundThingy = null;

//        ////////////////////
//        /// Textures
//        ////////////////////

//        private Texture2D BackGround;
//        private Texture2D BigBackGround;
        
//        private Texture2D Selector;
//        private Texture2D Save;
//        private Texture2D Rest;
//        private Texture2D Exit;

//        //private Texture2D SFX;
//        //private Texture2D BG;


//        ////////////////////
//        /// Sprites stuff
//        ////////////////////
//        private Rectangle BackGroundRectangle;
//        private Vector2 BackGroundOrigin;
//        private Vector2 BackGroundScale;

//        private Rectangle BigBackGroundRectangle;
//        private Vector2 BigBackGroundOrigin;
//        private Vector2 BigBackGroundScale;


//        private Rectangle ButtonRectangle;
//        private Vector2 ButtonOrigin;
//        private Vector2 ButtonScale;

//        /////////////////
//        /// Positions
//        ////////////////
//        private Vector2 BackGroundPosition;
//        private Vector2 BigBackGroundPosition;
//        private Vector2 RestPosition;
//        private Vector2 SavePosition;
//        private Vector2 ExitPosition;
//        private Vector2 SelectorPosition;
  
//        /////////////////
//        /// KeyBoard
//        ////////////////
//        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
//        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

//        ////////////////////
//        /// Menu stuff
//        /////////////////////
//        private int MenuSelection;
//        private Effect myEffect;

//        public StoreRoomMenu() : base() { }
//        public StoreRoomMenu(GameScreen Parent) : base(Parent) { }
//        public StoreRoomMenu(SpriteFont spritefont, GameScreen Parent)
//            : base(Parent)
//        {
//            BackGround = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/InGameMenu");
//            BigBackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");
//            Selector = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Selector");
//            Rest = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Resume");
//            Save = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Achievements");
//            Exit = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Quit");

//            WriteText = spritefont;

//            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
//            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
//            BackGroundScale = new Vector2((((float)GameEngine.GraphicDevice.Viewport.Width * 0.8f / (float)BackGround.Width) * 0.2f), (((GameEngine.GraphicDevice.Viewport.Height * 0.8f) / BackGround.Height) * 0.6f));

//            BigBackGroundRectangle = new Rectangle(0, 0, BigBackGround.Width, BigBackGround.Height);
//            BigBackGroundOrigin = new Vector2(BigBackGround.Width * 0.5f, BigBackGround.Height * 0.5f);
//            BigBackGroundScale = new Vector2((((float)GameEngine.GraphicDevice.Viewport.Width * 0.8f / (float)BigBackGround.Width) * 1.0f), (((GameEngine.GraphicDevice.Viewport.Height * 0.8f) / BigBackGround.Height) * 1.0f));

//            BigBackGroundPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
//            BackGroundPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f + BackGroundOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.1f + BackGroundOrigin.Y);

//            ButtonRectangle = new Rectangle(0, 0, Save.Width, Save.Height);
//            ButtonOrigin = new Vector2(Save.Width * 0.5f, Save.Height * 0.5f);
//            ButtonScale = new Vector2((((float)GameEngine.GraphicDevice.Viewport.Width * 0.8f / (float)Save.Width) * 0.15f), (((GameEngine.GraphicDevice.Viewport.Height * 0.8f) / Save.Height) * 0.05f));

//            SelectorPosition = SavePosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f + BackGroundOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.15f);
//            RestPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f + BackGroundOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.25f);
//            ExitPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f + BackGroundOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.35f);

//            MenuSelection = -1;

//            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
//            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
            
//        }

//        protected override void InitializeComponent(GameScreen Parent)
//        {
//            Visible = false;
//            base.InitializeComponent(Parent);
//        }
//        public void Reset()
//        {
//            MenuSelection = -1;
//        }
//        private void MenuSelectionUp()
//        {
//            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
//            SoundThingy.Play();
//            if (MenuChoice == StoreMenu.MENU)
//            {
//                if (MenuSelection == 3)
//                {
//                    MenuSelection = 0;
//                }
//                else
//                {
//                    MenuSelection++;
//                }
//            }
//        }
//        private void MenuSelectionDown()
//        {
//            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
//            SoundThingy.Play();
//            if (MenuChoice == StoreMenu.MENU)
//            {
//                if (MenuSelection == 0)
//                {
//                    MenuSelection = 3;
//                }
//                else
//                {
//                    MenuSelection--;
//                }
//            }
//        }
//        private void Select()
//        {
//            if (MenuChoice == StoreMenu.MENU)
//            {
//                if (MenuSelection == 0)
//                {
//                    //Fuse
//                    MenuChoice = StoreMenu.FUSE;
//                    //MenuChoice = 0;
//                }
//                if (MenuSelection == 1)
//                {
//                   //Sell
//                    MenuChoice = StoreMenu.SELL;
//                    //MenuChoice = 0;
//                }
//                if (MenuSelection == 2)
//                {
//                    //Save
                    
//                    // call alertbox 
//                    GameEngine.Services.GetService<AlertBox>().Visible = true;
//                    GameEngine.Services.GetService<AlertBox>().PopUpBox(5, "");
//                }
//                if (MenuSelection == 3)
//                {
//                    //Exit
//                    Visible = false;
//                    Reset();
//                    PlayerState.Currently = PlayerState.State.NORMAL;
//                    //GameEngine.Services.GetService<TransitionScreen>().StartFading(4, 2);
//                }
//            }
//        }
//        private void SelectorUpdate()
//        {
//            if (MenuChoice == StoreMenu.MENU)
//            {
//                if (MenuSelection == 0)
//                {
//                    SelectorPosition = FusePosition;
//                }
//                if (MenuSelection == 1)
//                {
//                    SelectorPosition = SellPosition;
//                }
//                if (MenuSelection == 2)
//                {
//                    SelectorPosition = SavePosition;
//                }
//                if (MenuSelection == 3)
//                {
//                    SelectorPosition = ExitPosition;
//                }
//            }
//        }
//        private void UnSelect()
//        {
//            if (MenuChoice == StoreMenu.MENU)
//            {
//                Visible = false;
//                Reset();
//                PlayerState.Currently = PlayerState.State.NORMAL;
//            }
//            else if (MenuChoice == StoreMenu.FUSE)
//            {
//                MenuChoice = StoreMenu.MENU;
//            }
//        }
//        private void KeyBoardInput()
//        {
//            if (KB.Key_Pressed(Keys.Down) || GP.Button_Pressed(Buttons.DPadDown))
//            {
//                MenuSelectionUp();
//            }
//            if (KB.Key_Pressed(Keys.Up) || GP.Button_Pressed(Buttons.DPadUp))
//            {
//                MenuSelectionDown();
//            }
//            if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A)) // ok
//            {
//                Select();
//            }
//            if (KB.Key_Pressed(Keys.X) || GP.Button_Pressed(Buttons.B)) // cancel
//            {
//                UnSelect();
//            }
//        }
//        public override void Update()
//        {
//            if (!Visible)
//            {
//                return;
//            }
//            if (MenuSelection == -1)
//            {
//                MenuSelection = 0;
//                return;
//            }
//            try
//            {
//                GameEngine.Services.GetService<Interactive>().Visible = false;
//            }
//            catch (Exception e)
//            {
//                Debug.Write("Exception caught in In game Menu for interactivity" + e.Message);
//            }
//            SelectorUpdate();
//            KeyBoardInput();
//            base.Update();
//        }
//        public override void Draw()
//        {
//            if (MenuChoice == StoreMenu.MENU)
//            {
//                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
//                {
//                    BackGroundScale = new Vector2((((float)GameEngine.GraphicDevice.Viewport.Width * 0.8f / (float)BackGround.Width) * 0.4f), (((GameEngine.GraphicDevice.Viewport.Height * 0.8f) / BackGround.Height) * 0.6f));
//                    //BackGround
//                    GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, Color.White,
//                    0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);
//                }
//                GameEngine.SpriteBatch.End();


//                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
//                {

//                    //Fuse
//                    GameEngine.SpriteBatch.Draw(Fuse, FusePosition, ButtonRectangle, Color.White,
//                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);

//                    //Sell
//                    GameEngine.SpriteBatch.Draw(Sell, SellPosition, ButtonRectangle, Color.White,
//                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);

//                    //Save
//                    GameEngine.SpriteBatch.Draw(Save, SavePosition, ButtonRectangle, Color.White,
//                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);

//                    //Exit
//                    GameEngine.SpriteBatch.Draw(Exit, ExitPosition, ButtonRectangle, Color.White,
//                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);

//                    //Selector
//                    GameEngine.SpriteBatch.Draw(Selector, SelectorPosition, ButtonRectangle, Color.White,
//                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);

//                }
//                GameEngine.SpriteBatch.End();

//            }
//            else if (MenuChoice == StoreMenu.FUSE)
//            {
//                GameEngine.SpriteBatch.Begin();
//                {
//                    //Big BackGround
//                    GameEngine.SpriteBatch.Draw(BigBackGround, BigBackGroundPosition, BigBackGroundRectangle, Color.White,
//                        0.0f, BigBackGroundOrigin, BigBackGroundScale, SpriteEffects.None, 0f);
//                }
//                GameEngine.SpriteBatch.End();

//            }
//        }
//        protected void DrawButtonOnChoice(string ButtonName)
//        {
//            //GameEngine.SpriteBatch.Draw(AssetImages[ButtonName], AssetPosition[ButtonName], Color.White);
//        }
//    }
//}
