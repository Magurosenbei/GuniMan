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
    public class MainMenu : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        ////////////////////////
        /// Selection States
        ////////////////////////
        protected enum ButtonChoice { START = 0, CONITNUE = 1, ACHIEVEMENTS = 2, OPTION = 3, EXIT = 4 , NONE = 5}
        ButtonChoice CurrentChoice = ButtonChoice.START;
        ButtonChoice Selected = ButtonChoice.NONE;
       
        ///////////////////
        /// Font
        //////////////////
        public SpriteFont WriteText;                  // The Font

        ///////////////////////////
        /// Timing Variables
        //////////////////////////

        private int PreviousSecond;
        private int Seconds;
        private int counter;

        /////////////////
        /// Sound
        ///////////////
        Cue SoundThingy = null;
        Cue MusicThingy = null;

        ////////////////////
        /// Textures
        ////////////////////
        private Texture2D BackGround;
        private Texture2D StartBin;
        private Texture2D ContinueBin;
        private Texture2D AchievementBin;
        private Texture2D OptionsBin;
        private Texture2D ExitBin;
        private Texture2D Arrows;
        private Texture2D Title;


       
        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundorigin;
        private Rectangle BinRectangle;
        private Vector2 Binorigin;
        private Rectangle ArrowsRectangle;
        private Vector2 Arrowsorigin;
        private Rectangle TitleRectangle;
        private Vector2 Titleorigin;

        /////////////////
        /// Positions
        ////////////////

        private Vector2 BackGroundPosition;
        private Vector2 TitlePosition;
        private Vector2 StartBinPosition;
        private Vector2 ContinueBinPosition;
        private Vector2 AchievementsBinPosition;
        private Vector2 OptionsBinPosition;
        private Vector2 ExitBinPosition;
        private Vector2 ArrowsPosition;
        private Vector2 DialoguePosition;
        

        private Effect myEffect;

        private Vector2 BackGroundScale;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        /////////////////
        /// Variables
        ////////////////
        private bool Moving;
        private bool Forward;
        private int MovingCounter;
        private string DisplayText;
        private float temp;
        private float SpinThingy;                   //Used to spin the dustbin arrow

        ////////////////////
        /// Fun Font Stuff
        ///////////////////

        private float FontScaleX;
        private float FontScaleY;
        private float FontScaleXLimit;
        private float FontScaleYLimit;
        private int FontAnimateCounter;
        private int FontAnimateLimit;
       
        private Color FontColor;
        private bool FontOkay;
        private bool FontUpSize;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();
        
        //private static Vector2 rotateVector2(Vector2 vector, double angle)
        //{
        //    float sinAngle = Math.Sin(angle);
        //    double cosAngle = Math.Cos(angle);

        //    return new Vector2(
        //      (vector.X * cosAngle) - (vector.Y * sinAngle),
        //      (vector.Y * cosAngle) + (vector.X * sinAngle)
        //    );
        //}
        public Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            Vector2 u = point - origin; //point relative to origin  

            if (u == Vector2.Zero)
                return point;

            float a = (float)Math.Atan2(u.Y, u.X); //angle relative to origin  
            a += rotation; //rotate  

            //u is now the new point relative to origin  
            u = u.Length() * new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
            return u + origin;
        }

        public MainMenu() : base() { }
        public MainMenu(GameScreen Parent) : base(Parent) { }
        public MainMenu(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {
            
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background2");
            StartBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/newgamebincircle");
            ContinueBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/continuebincircle");
            AchievementBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/achievementsbincircle");
            OptionsBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/optionsbincircle");
            ExitBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/exitbincircle");
            Arrows = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/arrow");
            Title = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/maintitle2");

            WriteText = spritefont;
            Seconds = 0;
            PreviousSecond = 0;
            counter = 0;
            SpinThingy = 0;
            MovingCounter = 0;


          
            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundorigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BinRectangle = new Rectangle(0, 0, StartBin.Width, StartBin.Height);
            Binorigin = new Vector2(StartBin.Width * 0.5f, StartBin.Height * 0.5f);
            ArrowsRectangle = new Rectangle(0, 0, Arrows.Width, Arrows.Height);
            Arrowsorigin = new Vector2(Arrows.Width * 0.5f, Arrows.Height * 0.5f);
            TitleRectangle = new Rectangle(0, 0, Title.Width, Title.Height);
            Titleorigin = new Vector2(Title.Width * 0.5f, Title.Height * 0.5f);


            //BottomCenter = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.9f);
            //TopLeft = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.1f, GameEngine.GraphicDevice.Viewport.Height * 0.1f);
            BackGroundPosition = TitlePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            ArrowsPosition = StartBinPosition = new Vector2(TitlePosition.X, TitlePosition.Y + StartBin.Height + 50.0f);
            ContinueBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-72.0f));
            AchievementsBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-144.0f));
            OptionsBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-216.0f));
            ExitBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-288.0f));
            DialoguePosition = new Vector2(TitlePosition.X, TitlePosition.Y + Titleorigin.Y - 30.0f);

            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
            MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("MenuTrack");

            temp = 1.0f;

            FontScaleX = 10.0f;
            FontScaleY = 0.0f;
            FontScaleXLimit = 1.5f;
            FontScaleYLimit = 1.5f;

            FontAnimateCounter = 0;
            FontAnimateLimit = 10;

            FontColor = new Color(Color.Red, 1.0f);
            FontOkay = false;
            FontUpSize = true;

            Moving = false;
            Forward = true;
            DisplayText = "Start Game";
        }
        public void ScreenUpdate()
        {
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public void Reset()
        {
            TitlePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            ArrowsPosition = StartBinPosition = new Vector2(TitlePosition.X, TitlePosition.Y + StartBin.Height + 50.0f);
            ContinueBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-72.0f));
            AchievementsBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-144.0f));
            OptionsBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-216.0f));
            ExitBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-288.0f));

            Seconds = 0;
            counter = 0;
            Visible = false;
            Moving = false;
            Forward = true;

            FontScaleX = 10.0f;
            FontScaleY = 0.0f;
            FontScaleXLimit = 1.5f;
            FontScaleYLimit = 1.5f;

            FontAnimateCounter = 0;
            FontAnimateLimit = 10;

            FontOkay = false;
            FontUpSize = true;
            
            FontColor = new Color(Color.Red, 1.0f);

            DisplayText = "Start Game";
            CurrentChoice = ButtonChoice.START;
            
            MusicThingy.Stop(AudioStopOptions.Immediate);
            MusicThingy.Dispose();
        }
        private void PlayBGM()
        {
            if (!MusicThingy.IsPlaying)
            {
                MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("MenuTrack");
                MusicThingy.Play();
            }

        }
        public int GetState()
        {
            //Visible = false;    // makes the menu go away
            int tmp = (int)Selected;
            Selected = ButtonChoice.NONE;
            //CurrentChoice = ButtonChoice.START;
            return tmp;
            /*if (CurrentChoice == ButtonChoice.START)
            {
                return 0;
            }
            if (CurrentChoice == ButtonChoice.CONITNUE)
            {
                return 1;
            }
            if (CurrentChoice == ButtonChoice.OPTION)
            {
                return 2;
            }
            if (CurrentChoice == ButtonChoice.ACHIEVEMENTS)
            {
                return 3;
            }
            else
            {
                return 4;
            }*/


        }
        private void FontScaleThingy()
        {
            if (!FontOkay)
            {
                if (FontScaleX > FontScaleXLimit)
                {
                    FontScaleX -= 0.2f;
                }
                if (FontScaleY < FontScaleYLimit)
                {
                    FontScaleY += 0.1f;
                }
                if (FontScaleY >= FontScaleYLimit && FontScaleX <= FontScaleXLimit)
                {
                    FontOkay = true;
                }
            }
            else
            {
                if (FontUpSize)
                {
                    FontScaleX += 0.05f;
                    FontScaleY += 0.05f;
                    FontAnimateCounter++;
                    if (FontAnimateCounter >= FontAnimateLimit)
                    {
                        FontAnimateCounter = 0;
                        FontUpSize = false;
                    }
                }
                else
                {
                    FontScaleX -= 0.05f;
                    FontScaleY -= 0.05f;
                    FontAnimateCounter++;
                    if (FontAnimateCounter >= FontAnimateLimit)
                    {
                        FontAnimateCounter = 0;
                        FontUpSize = true;
                    }
                }
            }
        }
        private void SpinThingyUpdate()
        {
            if (SpinThingy >= 360.0f)
            {
                SpinThingy = 0.0f;
            }
            else
            {
                SpinThingy += 0.005f * (float)GameEngine.GameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (Moving && !Forward)
            {
                StartBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(2.0f));
                ContinueBinPosition = RotateAboutOrigin(ContinueBinPosition, TitlePosition, MathHelper.ToRadians(2.0f));
                AchievementsBinPosition = RotateAboutOrigin(AchievementsBinPosition, TitlePosition, MathHelper.ToRadians(2.0f));
                OptionsBinPosition = RotateAboutOrigin(OptionsBinPosition, TitlePosition, MathHelper.ToRadians(2.0f));
                ExitBinPosition = RotateAboutOrigin(ExitBinPosition, TitlePosition, MathHelper.ToRadians(2.0f));
                
                MovingCounter++;
            }
            if (Moving && Forward)
            {
                StartBinPosition = RotateAboutOrigin(StartBinPosition, TitlePosition, MathHelper.ToRadians(-2.0f));
                ContinueBinPosition = RotateAboutOrigin(ContinueBinPosition, TitlePosition, MathHelper.ToRadians(-2.0f));
                AchievementsBinPosition = RotateAboutOrigin(AchievementsBinPosition, TitlePosition, MathHelper.ToRadians(-2.0f));
                OptionsBinPosition = RotateAboutOrigin(OptionsBinPosition, TitlePosition, MathHelper.ToRadians(-2.0f));
                ExitBinPosition = RotateAboutOrigin(ExitBinPosition, TitlePosition, MathHelper.ToRadians(-2.0f));
                MovingCounter++;
            }
            if (MovingCounter >= 36)
            {
                Moving = false;
                MovingCounter = 0;
            }
            //Debug.Write("\nX: " + StartBinPosition.X.ToString() + "\nX: " + StartBinPosition.Y.ToString()); 
        }
        private void SetText()
        {
            if (CurrentChoice == ButtonChoice.START)
            {
                DisplayText = "Start Game";
                FontColor = new Color(Color.Red, 1.0f);
                counter = 0;
            }
            if (CurrentChoice == ButtonChoice.CONITNUE)
            {
                DisplayText = "Continue";
                FontColor = new Color(Color.Orange, 1.0f);
                counter = 0;
            }
            if (CurrentChoice == ButtonChoice.ACHIEVEMENTS)
            {
                DisplayText = "Achievements";
                FontColor = new Color(Color.Yellow, 1.0f);
                counter = 0;
            }
            if (CurrentChoice == ButtonChoice.OPTION)
            {
                DisplayText = "Options";
                FontColor = new Color(Color.Magenta, 1.0f);
                counter = 0;
            }
            if (CurrentChoice == ButtonChoice.EXIT)
            {
                DisplayText = "Exit Game";
                FontColor = new Color(Color.CornflowerBlue, 1.0f);
                counter = 0;
            }
        }
        public void UpdateArrows()
        {
            if (CurrentChoice == ButtonChoice.START)
            {
                ArrowsPosition = StartBinPosition;
            }
            if (CurrentChoice == ButtonChoice.CONITNUE)
            {
                ArrowsPosition = ContinueBinPosition;
            }
            if (CurrentChoice == ButtonChoice.ACHIEVEMENTS)
            {
                ArrowsPosition = AchievementsBinPosition;
            }
            if (CurrentChoice == ButtonChoice.OPTION)
            {
                ArrowsPosition = OptionsBinPosition;
            }
            if (CurrentChoice == ButtonChoice.EXIT)
            {
                ArrowsPosition = ExitBinPosition;
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }

            PlayBGM();
            SpinThingyUpdate();
            UpdateArrows();
            FontScaleThingy();

            if (counter < DisplayText.Length)
            {
                counter++;
            } 

            if (KB.Key_Pressed(Keys.K))
            {
                //StartBinPosition = new Vector2(StartBinPosition.X + 1.0f, StartBinPosition.Y);//GameEngine.Services.GetService<DialogueEngine>().Visible = true;
                //GameEngine.Services.GetService<DialogueEngine>().StartConversation("Konata_fanart", "Konata_fanart", false, 0);
            }
            
            if (PreviousSecond != GameEngine.GameTime.TotalGameTime.Seconds)
            {
                Seconds++;
                PreviousSecond = GameEngine.GameTime.TotalGameTime.Seconds;
            }
            if (KB.Key_Pressed(Keys.L))
            {
                temp -= 0.1f;
                GameEngine.Services.GetService<AudioComponent>().MusicVolume(temp);
            }
            if (KB.Key_Pressed(Keys.Left) || GP.Button_Pressed(Buttons.DPadLeft) || 
                KB.Key_Held(Keys.Left) || GP.Button_Held(Buttons.DPadLeft) )
            {
                Seconds = 0;
               
                if (!Moving)
                {
                    FontScaleX = 7.0f;
                    FontScaleY = 0.0f;
                    FontAnimateCounter = 0;
                    FontAnimateLimit = 10;
                    FontOkay = false;
                    FontUpSize = true;

                    SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("bin_move");
                    SoundThingy.Play();
                    if (CurrentChoice > ButtonChoice.START)
                    {
                        CurrentChoice--;
                    }
                    else
                    {
                        CurrentChoice = ButtonChoice.EXIT;
                    }
                    SetText();
                    Moving = true;
                    Forward = true;
                }
            }
            else if (KB.Key_Pressed(Keys.Right) || GP.Button_Pressed(Buttons.DPadRight) ||
                    KB.Key_Held(Keys.Right) || GP.Button_Held(Buttons.DPadRight) )
            {
                Seconds = 0;
                if (!Moving)
                {
                    FontScaleX = 7.0f;
                    FontScaleY = 0.0f;
                    FontAnimateCounter = 0;
                    FontAnimateLimit = 10;
                    FontOkay = false;
                    FontUpSize = true;

                    SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("bin_move");
                    SoundThingy.Play();
                    if (CurrentChoice < ButtonChoice.EXIT)
                    {
                        CurrentChoice++;
                    }
                    else
                    {
                        CurrentChoice = ButtonChoice.START;
                    }
                    SetText();
                    Moving = true;
                    Forward = false;
                }
            }
            else if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A) )
                Selected = CurrentChoice;

            if (Seconds == 30 + 1)          //goes to prologue
            {
                Reset();
                GameEngine.Services.GetService<Prologue>().Visible = true;
            }
            base.Update();
        }
       
        public override void Draw()
        {
            //StartBinPosition = new Vector2((int)StartBinPosition.X, (int)StartBinPosition.Y);
            //ContinueBinPosition = new Vector2((int)ContinueBinPosition.X, (int)ContinueBinPosition.Y);
            //AchievementsBinPosition = new Vector2((int)AchievementsBinPosition.X, (int)AchievementsBinPosition.Y);
            //OptionsBinPosition = new Vector2((int)OptionsBinPosition.X, (int)OptionsBinPosition.Y);
            //ExitBinPosition = new Vector2((int)ExitBinPosition.X, (int)ExitBinPosition.Y);
            
            //***IMPORTANT***
            // need to static cast to int 
            // else will produce lines on image
            // reason - rotation function
            //***IMPORTANT***

            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //Background
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, Color.White,
                             0.0f, BackGroundorigin, BackGroundScale, SpriteEffects.None, 0f);

                //not ptactical as more memory is used... lol
                //for (int i = 0; i < 2; i++)
                //{
                    
                //    if (i == 0)
                //    {
                //        StartBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/newgamebincircle");
                //        GameEngine.SpriteBatch.Draw(StartBin, new Vector2((int)StartBinPosition.X, (int)StartBinPosition.Y), BinRectangle, Color.White,
                //                     0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);

                //    }
                //    if (i == 1)
                //    {
                //        StartBin = GameEngine.Content.Load<Texture2D>("Content/Menu/Sprites/continuebincircle");
                //        GameEngine.SpriteBatch.Draw(StartBin, new Vector2((int)ContinueBinPosition.X, (int)ContinueBinPosition.Y), BinRectangle, Color.White,
                //                         0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);

                //    }
                //}

                //Bins
                GameEngine.SpriteBatch.Draw(StartBin, new Vector2((int)StartBinPosition.X, (int)StartBinPosition.Y), BinRectangle, Color.White,
                             0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);
                GameEngine.SpriteBatch.Draw(ContinueBin, new Vector2((int)ContinueBinPosition.X, (int)ContinueBinPosition.Y), BinRectangle, Color.White,
                             0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);
                GameEngine.SpriteBatch.Draw(AchievementBin, new Vector2((int)AchievementsBinPosition.X, (int)AchievementsBinPosition.Y), BinRectangle, Color.White,
                             0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);
                GameEngine.SpriteBatch.Draw(OptionsBin, new Vector2((int)OptionsBinPosition.X, (int)OptionsBinPosition.Y), BinRectangle, Color.White,
                             0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);
                GameEngine.SpriteBatch.Draw(ExitBin, new Vector2((int)ExitBinPosition.X, (int)ExitBinPosition.Y), BinRectangle, Color.White,
                             0.0f, Binorigin, 1.0f, SpriteEffects.None, 0f);

                //arrows
                GameEngine.SpriteBatch.Draw(Arrows, ArrowsPosition, ArrowsRectangle, Color.White,
                    SpinThingy, Arrowsorigin, 1.0f, SpriteEffects.None, 0f);

                //arrows
                GameEngine.SpriteBatch.Draw(Arrows, ArrowsPosition, ArrowsRectangle, Color.White,
                    SpinThingy + MathHelper.ToRadians(120.0f), Arrowsorigin, 1.0f, SpriteEffects.None, 0f);

                //arrows
                GameEngine.SpriteBatch.Draw(Arrows, ArrowsPosition, ArrowsRectangle, Color.White,
                    SpinThingy + MathHelper.ToRadians(240.0f), Arrowsorigin, 1.0f, SpriteEffects.None, 0f);


                //title         
                GameEngine.SpriteBatch.Draw(Title, TitlePosition, TitleRectangle, Color.White,
                             0.0f, Titleorigin, 1.2f, SpriteEffects.None, 0f);

             

            }
            GameEngine.SpriteBatch.End();
          
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //myEffect.Parameters["Intensity"].SetValue(0.8f);
                //myEffect.Begin();
                //myEffect.CurrentTechnique.Passes[3].Begin();

                //desciption
                GameEngine.SpriteBatch.DrawString(WriteText, DisplayText.Substring(0, counter), DialoguePosition, FontColor,
                    0.0f, new Vector2(3.5f * (float)DisplayText.Length, 6.0f), new Vector2(FontScaleX,FontScaleY), SpriteEffects.None, 0.0f);

                //myEffect.CurrentTechnique.Passes[3].End();
                //myEffect.End();
            }
            GameEngine.SpriteBatch.End();
           
            base.Draw();
        }
        protected void DrawButtonOnChoice(string ButtonName)
        {
            //GameEngine.SpriteBatch.Draw(AssetImages[ButtonName], AssetPosition[ButtonName], Color.White);
        }
    }
}
