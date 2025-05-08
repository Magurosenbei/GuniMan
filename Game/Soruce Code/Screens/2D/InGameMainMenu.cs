using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace Game
{
    public class InGameMainMenu : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        ////////////////////////
        /// Selection States
        ////////////////////////
        protected enum InGameMenu { MENU, OPTIONS, ACHIEVEMENTS }
        InGameMenu MenuChoice = InGameMenu.MENU;

        ///////////////////
        /// Font
        //////////////////
        public SpriteFont WriteText;                  // The Font
       
        /////////////////
        /// Sound
        ///////////////
        Cue SoundThingy = null;

        ////////////////////
        /// Textures
        ////////////////////
  
        private Texture2D BackGround;               //used when just press START
        private Texture2D BigBackGround;            //used for OPTIONS, ACHIEVEMENTS
        
        private Texture2D Selector;
        private Texture2D Resume;
        private Texture2D Options;
        private Texture2D Achievements;
        private Texture2D Quit;

        private Texture2D OptionsIndicator;

        private Texture2D VolumeChangerAdd;         //plus
        private Texture2D VolumeChangerMinus;       //minus
        private Texture2D VolumeIndicator;          //indicator  0 - 10%

        private Texture2D SFX;
        private Texture2D BGM;
        private Texture2D TXTSPD;
        private Texture2D[] SlowMediumFast;

        ////////////////////
        /// Sprites stuff
        ////////////////////
        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundScale;

        private Rectangle OptionsIndicatorRectangle;
        private Vector2 OptionsIndicatorOrigin;

        private Rectangle SlowMediumFastRectangle;
        private Vector2 SlowMediumFastOrigin;

        private Rectangle VolumeChangerRectangle;
        private Vector2 VolumeChangerOrigin;

        private Rectangle VolumeIndicatorRectangle;
        private Vector2 VolumeIndicatorOrigin;

        private Rectangle VolumeTextureRectangle;
        private Vector2 VolumeTextureOrigin;

        private Rectangle BigBackGroundRectangle;
        private Vector2 BigBackGroundOrigin;
        private Vector2 BigBackGroundScale;

        private Rectangle ButtonRectangle;
        private Vector2 ButtonOrigin;
        private Vector2 ButtonScale;

        /////////////////
        /// Positions
        ////////////////
        private Vector2 BackGroundPosition;
        private Vector2 BigBackGroundPosition;
        private Vector2 ResumePosition;
        private Vector2 OptionsPosition;
        private Vector2 AchievementsPosition;
        private Vector2 QuitPosition;
        private Vector2 SelectorPosition;

        private Vector2 OptionsIndicatorPosition;

        private Vector2 AddVolumePosition;
        private Vector2 MinusVolumePosition;
        private Vector2 VolumeIndicatorPosition;

        private Vector2 VolumeTitlePosition;

        private Vector2 SlowMediumFastPosition;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        ////////////////////
        /// Menu stuff
        /////////////////////
        private int MenuSelection;
        private int OptionsSelection;

        private int SFXVolume;
        private int BGMVolume;
        private int TxtSpdAmount;
        private bool NotInGame;

        //////////////////
        /// Effect thingy
        //////////////////
        private Effect myEffect;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public InGameMainMenu() : base() { }
        public InGameMainMenu(GameScreen Parent) : base(Parent) { }
        public InGameMainMenu(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {

            BackGround = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/InGameMenu");
            BigBackGround = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/OptionsScreen");
            Selector = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Selector");
            Resume = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Resume");
            Options = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Options");
            Achievements = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Achievements");
            Quit = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Quit");

            VolumeIndicator = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/VolumeIcon");
            VolumeChangerAdd = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Vplus");
            VolumeChangerMinus = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Vminus");
            SlowMediumFast = new Texture2D[3];
            TextSpdArrayInit();

            OptionsIndicator = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/HornSelector");
            SFX = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/SFX");
            BGM = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/BGM");
            TXTSPD = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/TxtSpd");

            WriteText = spritefont;

            VolumeTextureRectangle = new Rectangle(0, 0, SFX.Width, SFX.Height);
            VolumeTextureOrigin = new Vector2(SFX.Width * 0.5f, SFX.Height * 0.5f);
            VolumeTitlePosition = new Vector2(Resizer.GetWidth(0.3f), Resizer.GetHeight(0.5f));

            VolumeIndicatorRectangle = new Rectangle(0, 0, VolumeIndicator.Width, VolumeIndicator.Height);
            VolumeIndicatorOrigin = new Vector2(VolumeIndicator.Width * 0.5f, VolumeIndicator.Height * 0.5f);

            VolumeChangerRectangle = new Rectangle(0, 0, VolumeChangerAdd.Width, VolumeChangerAdd.Height);
            VolumeChangerOrigin = new Vector2(VolumeChangerAdd.Width * 0.5f, VolumeChangerAdd.Height * 0.5f);

            VolumeIndicatorPosition = new Vector2(Resizer.GetWidth(0.6f), Resizer.GetHeight(0.5f) + 20.0f);
            MinusVolumePosition = new Vector2(VolumeIndicatorPosition.X - 50.0f, VolumeIndicatorPosition.Y - VolumeIndicatorOrigin.Y);
            AddVolumePosition = new Vector2(VolumeIndicatorPosition.X + (9.0f * 20.0f) + 50.0f , VolumeIndicatorPosition.Y - VolumeIndicatorOrigin.Y);

            SlowMediumFastRectangle = new Rectangle(0, 0, SlowMediumFast[0].Width, SlowMediumFast[0].Height);
            SlowMediumFastOrigin = new Vector2(SlowMediumFast[0].Width * 0.5f, SlowMediumFast[0].Height * 0.5f);
            SlowMediumFastPosition = new Vector2(MinusVolumePosition.X, VolumeTitlePosition.Y + 2 * 100.0f);

            OptionsIndicatorRectangle = new Rectangle(0, 0, OptionsIndicator.Width, OptionsIndicator.Height);
            OptionsIndicatorOrigin = new Vector2(OptionsIndicator.Width * 0.5f, OptionsIndicator.Height * 0.5f);
            OptionsIndicatorPosition = new Vector2(VolumeTitlePosition.X - VolumeTextureOrigin.X - OptionsIndicatorOrigin.X - 50.0f, VolumeTitlePosition.Y );

            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 0.4f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.6f));

            BigBackGroundRectangle = new Rectangle(0, 0, BigBackGround.Width, BigBackGround.Height);
            BigBackGroundOrigin = new Vector2(BigBackGround.Width * 0.5f, BigBackGround.Height * 0.5f);
            BigBackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BigBackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BigBackGround.Height) * 1.0f));

            BigBackGroundPosition = BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));

            ButtonRectangle = new Rectangle(0, 0, Resume.Width, Resume.Height);
            ButtonOrigin = new Vector2(Resume.Width * 0.5f, Resume.Height * 0.5f);
            ButtonScale = new Vector2(((Resizer.GetRealWidth(1.0f) / 300.0f) * 0.3f), ((Resizer.GetRealHeight(1.0f) / 70.0f) * 0.1f));

            SelectorPosition = ResumePosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.35f);
            OptionsPosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.45f);
            AchievementsPosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.55f);
            QuitPosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.65f);

            MenuSelection = 0;
            OptionsSelection = 0;

            NotInGame = false;

            SFXVolume = 5;
            BGMVolume = 5;
            TxtSpdAmount = GameEngine.Services.GetService<PlayerStats>().GetDelayValue();

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
            
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public void ScreenUpdate()
        {
            //SelectorPosition = ResumePosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.35f);
            //OptionsPosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.45f);
            //AchievementsPosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.55f);
            //QuitPosition = new Vector2(Resizer.GetWidth(0.5f), GameEngine.GraphicDevice.Viewport.Height * 0.65f);
            
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 0.4f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.6f));
            BigBackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BigBackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BigBackGround.Height) * 1.0f));
        }
       
        public void Reset()
        {
            Visible = false;
            NotInGame = false;
            MenuSelection = 0;
            OptionsSelection = 0;

            MenuChoice = InGameMenu.MENU;
        }
        private void TextSpdArrayInit()
        {
            SlowMediumFast[0] = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Txtspdind_1");
            SlowMediumFast[1] = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Txtspdind_2");
            SlowMediumFast[2] = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/Txtspdind_3");
        }
        private void MenuSelectionUp()
        {
            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
            SoundThingy.Play();
            if (MenuChoice == InGameMenu.MENU)
            {
                if (MenuSelection == 3)
                {
                    MenuSelection = 0;
                }
                else
                {
                    MenuSelection++;
                }
            }
            else if (MenuChoice == InGameMenu.OPTIONS)
            {
                if (OptionsSelection == 2)
                {
                    OptionsSelection = 0;
                }
                else
                {
                    OptionsSelection++;
                }
            }
        }
        private void MenuSelectionDown()
        {
            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
            SoundThingy.Play();
            if (MenuChoice == InGameMenu.MENU)
            {
                if (MenuSelection == 0)
                {
                    MenuSelection = 3;
                }
                else
                {
                    MenuSelection--;
                }
            }
            else if (MenuChoice == InGameMenu.OPTIONS)
            {
                if (OptionsSelection == 0)
                {
                    OptionsSelection = 2;
                }
                else
                {
                    OptionsSelection--;
                }
            }
        }
        private void MenuSelectionLeft()
        {
            if (MenuChoice == InGameMenu.OPTIONS)
            {
                if (OptionsSelection == 0)
                {
                    if (SFXVolume == 1)
                    {
                        SFXVolume = 1;
                    }
                    else
                    {
                        SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
                        SoundThingy.Play();
                        SFXVolume--;
                    }
                    GameEngine.Services.GetService<AudioComponent>().SFXVolume((((float)this.SFXVolume) / 10));
                }
                else if (OptionsSelection == 1)
                {
                    if (BGMVolume == 1)
                    {
                        BGMVolume = 1;
                    }
                    else
                    {
                        SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
                        SoundThingy.Play();
                        BGMVolume--;
                    }
                    GameEngine.Services.GetService<AudioComponent>().MusicVolume((((float)this.BGMVolume) / 10));
                }
                else if (OptionsSelection == 2)
                {
                    if (TxtSpdAmount == 1)
                    {
                        TxtSpdAmount = 1;
                    }
                    else
                    {
                        SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
                        SoundThingy.Play();
                        TxtSpdAmount -= 2;
                    }
                    GameEngine.Services.GetService<PlayerStats>().DelayValueChange(TxtSpdAmount);
                }
            }
        }
        private void MenuSelectionRight()
        {
            if (MenuChoice == InGameMenu.OPTIONS)
            {
                if (OptionsSelection == 0)
                {
                    if (SFXVolume == 10)
                    {
                        SFXVolume = 10;
                    }
                    else
                    {
                        SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
                        SoundThingy.Play();
                        SFXVolume++;
                    }
                    GameEngine.Services.GetService<AudioComponent>().SFXVolume((((float)this.SFXVolume) / 10));
                }
                else if (OptionsSelection == 1)
                {
                    if (BGMVolume == 10)
                    {
                        BGMVolume = 10;
                    }
                    else
                    {
                        SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
                        SoundThingy.Play();
                        BGMVolume++;
                    }
                    GameEngine.Services.GetService<AudioComponent>().MusicVolume((((float)this.BGMVolume) / 10));
                }
                else if (OptionsSelection == 2)
                {
                    if (TxtSpdAmount == 5)
                    {
                        TxtSpdAmount = 5;
                    }
                    else
                    {
                        SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("click1");
                        SoundThingy.Play();
                        TxtSpdAmount += 2;
                    }
                    GameEngine.Services.GetService<PlayerStats>().DelayValueChange(TxtSpdAmount);
                }
            }
        }
        public void OutsideOptions()
        {
            MenuChoice = InGameMenu.OPTIONS;
            Visible = true;
            NotInGame = true;
        }
        public void OutsideAchievements()
        {
            MenuChoice = InGameMenu.ACHIEVEMENTS;
            Visible = true;
            NotInGame = true;
        }
        private void Select()
        {
            if (MenuChoice == InGameMenu.MENU)
            {
                if (MenuSelection == 0)
                {
                    Visible = false;
                    PlayerState.Currently = PlayerState.State.NORMAL;
                }
                if (MenuSelection == 1)
                {
                    BigBackGround = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/OptionsScreen");
                    MenuChoice = InGameMenu.OPTIONS;
                }
                if (MenuSelection == 2)
                {
                    BigBackGround = GameEngine.Content.Load<Texture2D>("Content/GameMenu/Sprites/AchievementsScreen");
                    MenuChoice = InGameMenu.ACHIEVEMENTS;
                }
                if (MenuSelection == 3)
                {
                    //Visible = false;
                    GameEngine.Services.GetService<TransitionScreen>().StartFading(4, 2);
                    GameEngine.Services.GetService<SoundManager>().StopSong();
                    //MainGame.APP_STATE = APP_STATE.UNLOAD;
                    //Quit to main menu
                }
            }
        }
        private void SelectorUpdate()
        {
            if (MenuChoice == InGameMenu.MENU)
            {
                if (MenuSelection == 0)
                {
                    SelectorPosition = ResumePosition;
                }
                if (MenuSelection == 1)
                {
                    SelectorPosition = OptionsPosition;
                }
                if (MenuSelection == 2)
                {
                    SelectorPosition = AchievementsPosition;
                }
                if (MenuSelection == 3)
                {
                    SelectorPosition = QuitPosition;
                }
            }
        }
        private void UnSelect()
        {
            if (MenuChoice == InGameMenu.MENU)
            {
                Visible = false;
                PlayerState.Currently = PlayerState.State.NORMAL;
            }
            else if (MenuChoice == InGameMenu.ACHIEVEMENTS || MenuChoice == InGameMenu.OPTIONS)
            {
                if (NotInGame)
                {
                    Reset();
                    GameEngine.Services.GetService<MainMenu>().Visible = true;
                }
                else
                {
                    MenuChoice = InGameMenu.MENU;
                }
            }
        }
        private void KeyBoardInput()
        {
            if (KB.Key_Pressed(Keys.Down) || GP.Button_Pressed(Buttons.DPadDown))
            {
                MenuSelectionUp();
            }
            else if (KB.Key_Pressed(Keys.Up) || GP.Button_Pressed(Buttons.DPadUp))
            {
                MenuSelectionDown();
            }
            else if (KB.Key_Pressed(Keys.Left) || GP.Button_Pressed(Buttons.DPadLeft))
            {
                MenuSelectionLeft();
            }
            else if (KB.Key_Pressed(Keys.Right) || GP.Button_Pressed(Buttons.DPadRight))
            {
                MenuSelectionRight();
            }
            else if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A)) // ok
            {
                Select();
            }
            else if (KB.Key_Pressed(Keys.X) || GP.Button_Pressed(Buttons.B)) // cancel
            {
                UnSelect();
            }
            // Clash with V calling Visible = true
            //if (KB.Key_Pressed(Keys.V) || GP.Button_Pressed(Buttons.Y)) // cancel
            //{
            //    Visible = false;
            //    PlayerState.Currently = PlayerState.State.NORMAL;
            //}
            else if (KB.Key_Pressed(Keys.K))
            {
                //GameEngine.Services.GetService<DialogueEngine>().Visible = true;
                //GameEngine.Services.GetService<DialogueEngine>().StartConversation("Konata_fanart", "Konata_fanart", false, 0);
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            try
            {
                GameEngine.Services.GetService<Interactive>().Visible = false;
            }
            catch (Exception e)
            {
                Debug.Write("Exception caught in In game Menu for interactivity" + e.Message);
            }
            SelectorUpdate();
            KeyBoardInput();
            
           
            base.Update();
        }
        public override void Draw()
        {
            if (MenuChoice == InGameMenu.MENU)
            {
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    BackGroundScale = new Vector2((((float)GameEngine.GraphicDevice.Viewport.Width * 0.8f / (float)BackGround.Width) * 0.4f), (((GameEngine.GraphicDevice.Viewport.Height * 0.8f) / BackGround.Height) * 0.6f));
                    //BackGround
                    GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, new Color(Color.White,0.7f),
                    0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);
                }
                GameEngine.SpriteBatch.End();

               
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    
                    //Resume
                    GameEngine.SpriteBatch.Draw(Resume, ResumePosition, ButtonRectangle, Color.White,
                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);
               
                    //Options
                    GameEngine.SpriteBatch.Draw(Options, OptionsPosition, ButtonRectangle, Color.White,
                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);
                
                    //Achievements
                    GameEngine.SpriteBatch.Draw(Achievements, AchievementsPosition, ButtonRectangle, Color.White,
                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);
               
                    //Quit
                    GameEngine.SpriteBatch.Draw(Quit, QuitPosition, ButtonRectangle, Color.White,
                             0.0f, ButtonOrigin, ButtonScale, SpriteEffects.None, 0f);

                    //Selector
                    GameEngine.SpriteBatch.Draw(Selector, SelectorPosition, ButtonRectangle, new Color(Color.White,0.85f),
                             0.0f, ButtonOrigin, new Vector2(ButtonScale.X + 0.2f, ButtonScale.Y), SpriteEffects.None, 0f);
                
                } 
                GameEngine.SpriteBatch.End();
                
            }
            else if (MenuChoice == InGameMenu.ACHIEVEMENTS)
            {
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    //Big BackGround
                    GameEngine.SpriteBatch.Draw(BigBackGround, BigBackGroundPosition, BigBackGroundRectangle, Color.White,
                        0.0f, BigBackGroundOrigin, BigBackGroundScale, SpriteEffects.None, 0f);
                }
                GameEngine.SpriteBatch.End();

                GameEngine.Services.GetService<AchievementEngine>().FullDraw();
            }
            else if (MenuChoice == InGameMenu.OPTIONS)
            {
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    //Big BackGround
                    GameEngine.SpriteBatch.Draw(BigBackGround, BigBackGroundPosition, BigBackGroundRectangle, Color.White,
                        0.0f, BigBackGroundOrigin, BigBackGroundScale, SpriteEffects.None, 0f);

                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            //SFX Title
                            GameEngine.SpriteBatch.Draw(SFX, new Vector2(VolumeTitlePosition.X, VolumeTitlePosition.Y + i * 100.0f), VolumeTextureRectangle, Color.White,
                                0.0f, VolumeTextureOrigin, 0.7f, SpriteEffects.None, 0f);

                            //SFX Add
                            GameEngine.SpriteBatch.Draw(VolumeChangerAdd, new Vector2(AddVolumePosition.X, AddVolumePosition.Y + i * 100.0f), VolumeChangerRectangle, new Color(Color.White, 0.6f),
                                0.0f, VolumeChangerOrigin, 0.5f, SpriteEffects.None, 0f);

                            //SFX Minus
                            GameEngine.SpriteBatch.Draw(VolumeChangerMinus, new Vector2(MinusVolumePosition.X, MinusVolumePosition.Y + i * 100.0f), VolumeChangerRectangle, new Color(Color.White, 0.6f),
                                0.0f, VolumeChangerOrigin, 0.5f, SpriteEffects.None, 0f);

                        }
                        else if (i == 1)
                        {
                            //BGM Title
                            GameEngine.SpriteBatch.Draw(BGM, new Vector2(VolumeTitlePosition.X, VolumeTitlePosition.Y + i * 100.0f), VolumeTextureRectangle, Color.White,
                                0.0f, VolumeTextureOrigin, 0.7f, SpriteEffects.None, 0f);

                            //SFX Add
                            GameEngine.SpriteBatch.Draw(VolumeChangerAdd, new Vector2(AddVolumePosition.X, AddVolumePosition.Y + i * 100.0f), VolumeChangerRectangle, new Color(Color.White, 0.6f),
                                0.0f, VolumeChangerOrigin, 0.5f, SpriteEffects.None, 0f);

                            //SFX Minus
                            GameEngine.SpriteBatch.Draw(VolumeChangerMinus, new Vector2(MinusVolumePosition.X, MinusVolumePosition.Y + i * 100.0f), VolumeChangerRectangle, new Color(Color.White, 0.6f),
                                0.0f, VolumeChangerOrigin, 0.5f, SpriteEffects.None, 0f);
                        }
                        else if (i == 2)
                        {
                            //TXTSPD Title
                            GameEngine.SpriteBatch.Draw(TXTSPD, new Vector2(VolumeTitlePosition.X, VolumeTitlePosition.Y + i * 100.0f), VolumeTextureRectangle, Color.White,
                                0.0f, VolumeTextureOrigin, 0.7f, SpriteEffects.None, 0f);
                        }
                    }
                    //Selector
                    GameEngine.SpriteBatch.Draw(OptionsIndicator, new Vector2(OptionsIndicatorPosition.X,OptionsIndicatorPosition.Y + OptionsSelection * 100.0f) , OptionsIndicatorRectangle, Color.White,
                        0.0f, OptionsIndicatorOrigin, 1.0f, SpriteEffects.None, 0f);
                    

                    for (int i = 0; i < SFXVolume; i++)
                    {
                        //Volume Indicator
                        GameEngine.SpriteBatch.Draw(VolumeIndicator, new Vector2(VolumeIndicatorPosition.X + i * 20.0f, VolumeIndicatorPosition.Y - VolumeIndicatorOrigin.Y * 0.1f * i ), VolumeIndicatorRectangle, Color.White,
                            0.0f, VolumeIndicatorOrigin, new Vector2(0.5f, ((float)i+1)/10 ), SpriteEffects.None, 0f);
                    }
                    for (int i = 0; i < BGMVolume; i++)
                    {
                        //Volume Indicator
                        GameEngine.SpriteBatch.Draw(VolumeIndicator, new Vector2(VolumeIndicatorPosition.X + i * 20.0f, (VolumeIndicatorPosition.Y - VolumeIndicatorOrigin.Y * 0.1f * i) + 100.0f), VolumeIndicatorRectangle, Color.White,
                            0.0f, VolumeIndicatorOrigin, new Vector2(0.5f, ((float)i + 1) / 10), SpriteEffects.None, 0f);
                    }
                }
                GameEngine.SpriteBatch.End();

                for (int j = 0; j < 3; j++)
                {
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        if (TxtSpdAmount == 1 && j == 0 ||
                            TxtSpdAmount == 3 && j == 1 ||
                            TxtSpdAmount == 5 && j == 2)
                        {
                            myEffect.Parameters["Intensity"].SetValue(1.5f);
                            myEffect.Begin();
                            myEffect.CurrentTechnique.Passes[0].Begin();
                        }
                        else
                        {
                            myEffect.Parameters["Intensity"].SetValue(0.5f);
                            myEffect.Begin();
                            myEffect.CurrentTechnique.Passes[0].Begin();
                        }
                        //Slowmediumfast
                        GameEngine.SpriteBatch.Draw(SlowMediumFast[j], new Vector2(SlowMediumFastPosition.X + j * 140.0f, SlowMediumFastPosition.Y), SlowMediumFastRectangle, Color.White,
                             0.0f, SlowMediumFastOrigin, 1.0f, SpriteEffects.None, 0f);
                        
                        if (TxtSpdAmount == 1 && j == 0 ||
                            TxtSpdAmount == 3 && j == 1 ||
                            TxtSpdAmount == 5 && j == 2)
                        {
                            myEffect.CurrentTechnique.Passes[0].End();
                            myEffect.End();
                        }
                        else
                        {
                            myEffect.CurrentTechnique.Passes[0].End();
                            myEffect.End();
                        }
                    }
                    GameEngine.SpriteBatch.End();
    
                }

            }
            base.Draw();
        }
        protected void DrawButtonOnChoice(string ButtonName)
        {
            //GameEngine.SpriteBatch.Draw(AssetImages[ButtonName], AssetPosition[ButtonName], Color.White);
        }
    }
}
