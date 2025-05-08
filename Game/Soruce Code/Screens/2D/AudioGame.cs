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

using XNAnimation.Controllers;

namespace Game
{
    public class AudioGame : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        ////////////////
        /// Buttons
        ////////////////
        private List<AudioButtonDef> ButtonsDef;
        private List<AudioButton> DaButtons;

        ////////////////
        /// Textures
        ////////////////
        private Texture2D Number3;
        private Texture2D Number2;
        private Texture2D Number1;
        private Texture2D NumberG;
        private Texture2D NumberO;
        private Texture2D NumberEX;

        private Texture2D AButton;
        private Texture2D BButton;
        private Texture2D XButton;
        private Texture2D YButton;
        private Texture2D LRButton;
        private Texture2D Bar;
        private Texture2D Title;
        private Texture2D TrashBinBtm;
        private Texture2D TrashBinTop;
        private Texture2D Border;
        private Texture2D BackGround;

        ////////////////
        /// Font
        ////////////////
        SpriteFont WriteText;

        ////////////////
        // Time
        ///////////////
        private float CurrentTime;
        private float PreviousTime;

        ////////////////
        /// Positions
        ////////////////
        private Vector2 Center;
        private Vector2 LeftBorder;
        private Vector2 RightBorder;
        private Vector2 BarPosition;

        ////////////////
        /// Sound
        ////////////////
        Cue SoundThingy = null;
        Cue SoundThingy2 = null;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        //////////////////////////
        /// Variables for sprites
        /////////////////////////

        private Rectangle BarRectangle;
        private Vector2 BarOrigin;
        private Rectangle ButtonRectangle;
        private Vector2 Buttonorigin;
        private Rectangle NumberRectangle;
        private Vector2 NumberOrigin;
        private Vector2 NumberPosition;


        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundPosition;
        private Vector2 BackGroundScale;

        private Rectangle TrashBinRectangle;
        private Vector2 TrashBinOrigin;
        private Vector2 TrashBinPosition;
        private Vector2 TrashBinTargetPosition;

        private Rectangle TitleRectangle;
        private Vector2 TitleOrigin;
        private Vector2 TitlePosition;
        private Vector2 TitleTargetPosition;

        private Rectangle BorderRectangle;
        private Vector2 BorderOrigin;
        private Vector2 Border2Origin;
        private Vector2 BorderLRPosition;
        private Vector2 BorderRLPosition;

 
        //////////////////////////////
        /// Audio Game Variables
        /////////////////////////////
        private bool Start;
        private bool Result;
        private bool OtherStart;
        private float Speed;
        private float Allowance;

        private int EXcounter;
        private int WOWcounter;
        private int COOLcounter;
        private int Goodcounter;
        private int Misscounter;

        private int FunStuff;
        private int ResultThingy;

        private float BarLengther;
        private Color BinColor;

        private float TrashBinMovement;

        private float Fade3;
        private float Fade2;
        private float Fade1;
        private float FadeGo;

        private bool Appear1;
        private bool Appear2;
        private bool Appear3;
        private bool AppearGo;

        /////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;

        /////////////////////
        // Particle
        /////////////////////
        ParticleOverlord Shiny;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        private void RandomizeButtons()
        {
            int RandomStorage = 0;
            for (int i = 0; i < DaButtons.Count; i++)
            {
                if (DaButtons[i].GetButton() == "X Button" ||
                    DaButtons[i].GetButton() == "Y Button" ||
                    DaButtons[i].GetButton() == "A Button" ||
                    DaButtons[i].GetButton() == "B Button")
                {

                    RandomStorage = random.Next(1, 5);
                    if (RandomStorage == 1)
                    {
                        DaButtons[i].SetButton("X Button");
                    }
                    if (RandomStorage == 2)
                    {
                        DaButtons[i].SetButton("Y Button");
                    }
                    if (RandomStorage == 3)
                    {
                        DaButtons[i].SetButton("A Button");
                    }
                    if (RandomStorage == 4)
                    {
                        DaButtons[i].SetButton("B Button");
                    }
                }
            }
        }
        private void ButtonsInit(float Speed)
        {
            for (int i = 0; i < ButtonsDef.Count; i++)
            {
                DaButtons.Add(new AudioButton(ButtonsDef[i].Button, ButtonsDef[i].Sound,
                    ButtonsDef[i].Timing, Speed, BarPosition));
            }
            //Buttons.Add(new AudioButton("X Button", "Ga", 2.275f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("X Button", "Rang", 2.55f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("X Button", "Gu", 3.05f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("X Button", "Ni", 3.35f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn1", 4.15f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn2", 4.55f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn3", 4.79f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn4", 5.15f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn5", 5.40f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn6", 5.53f, Speed, BarPosition));
            //Buttons.Add(new AudioButton("LR Button", "horn7", 5.75f, Speed, BarPosition));

        }
        public AudioGame(SpriteFont spritetext, GameScreen Parent)
            : base(Parent)
        {
            DrawOrder = 1;
            random = new Random(); // random is random /// lol

            Center = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
            RightBorder = new Vector2(GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
            LeftBorder = new Vector2(0, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
            

            CurrentTime = PreviousTime = 0;
            Start = false;
            Result = false;
            OtherStart = false;
            WriteText = spritetext;

            Speed = 2.0f;
            Allowance = 17.0f;
            FunStuff = 0;
            BarLengther = 1.0f;
            TrashBinMovement = 0.03f;

            Fade1 = 1.0f;
            Fade2 = 1.0f;
            Fade3 = 1.0f;
            FadeGo = 1.0f;
            
            Appear1 = false;
            Appear2 = false;
            Appear3 = false;
            AppearGo = false;
            
            ResultThingy = 0;

            Shiny = new ParticleOverlord();

            Number3 = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/3");
            Number2 = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/2");
            Number1 = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/1");
            NumberG = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/G");
            NumberO = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/O");
            NumberEX = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/EX");

            Bar = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/blackbar");
            AButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Big/Button/AButton");
            BButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Big/Button/BButton");
            XButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Big/Button/XButton");
            YButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Big/Button/YButton");
            LRButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Big/Button/LRButton");
            Title = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/AudioGameTitle");
            TrashBinBtm = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/Trashbinbtm");
            TrashBinTop = GameEngine.Content.Load<Texture2D>("Content/Audio/Sprites/Trashbintop");
            Border = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");

            EXcounter = 0;
            WOWcounter = 0;
            COOLcounter = 0;
            Goodcounter = 0;
            Misscounter = 0;

            BinColor = new Color(Color.White, 1.0f);

            BarRectangle = new Rectangle(0, 0, Bar.Width, Bar.Height);
            BarOrigin = new Vector2(Bar.Width * 0.5f, Bar.Height * 0.5f);
            ButtonRectangle = new Rectangle(0, 0, AButton.Width, AButton.Height);
            Buttonorigin = new Vector2(AButton.Width * 0.5f, AButton.Height * 0.5f);

            NumberRectangle = new Rectangle(0, 0, Number3.Width, Number3.Height);
            NumberOrigin = new Vector2(Number3.Width * 0.5f, Number3.Height * 0.5f);

            TitleRectangle = new Rectangle(0, 0, Title.Width, Title.Height);
            TitleOrigin = new Vector2(Title.Width * 0.5f, Title.Height * 0.5f);
            TrashBinRectangle = new Rectangle(0, 0, TrashBinBtm.Width, TrashBinBtm.Height);
            TrashBinOrigin = new Vector2(TrashBinBtm.Width * 0.5f, TrashBinBtm.Height * 0.5f);

            BorderRectangle = new Rectangle(0, 0, Border.Width, Border.Height);
            BorderOrigin = new Vector2(0,0);
            Border2Origin = new Vector2(Border.Width, Border.Height);

            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));
            BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));

            NumberPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));

            BarPosition = new Vector2(RightBorder.X * 0.9f - 100.0f, RightBorder.Y);
            BorderLRPosition = new Vector2(Resizer.GetWidth(0.0f), BarPosition.Y - BarOrigin.Y);
            BorderRLPosition = new Vector2(Resizer.GetWidth(1.0f), BarPosition.Y + BarOrigin.Y);
           
            TrashBinPosition = new Vector2(BarPosition.X + TrashBinOrigin.X, Resizer.GetHeight(1.0f));
            TrashBinTargetPosition = new Vector2(BarPosition.X + TrashBinOrigin.X, BarPosition.Y + TrashBinOrigin.Y);;
            TitlePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.0f));
            TitleTargetPosition = new Vector2(Resizer.GetWidth(0.5f), BorderLRPosition.Y - TitleOrigin.Y);

            ButtonsDef = GameEngine.Content.Load<List<AudioButtonDef>>(@"Content/Audio/XML/XMLAudioButtonDef");
            DaButtons = new List<AudioButton>();
            ButtonsInit(Speed);
            RandomizeButtons();
            Shiny.CreateParticle(BorderLRPosition, 1, 3, -100, 100, 90);
            Shiny.CreateParticle(BorderRLPosition, 1, 3, -100, 100, 270);

        }
        public void Animation()
        {
            if (BarLengther < Resizer.GetWidth(1.0f))
            {
                BarLengther += 10.0f;
            }
            if (BarLengther >= Resizer.GetWidth(1.0f) && !Start && !Result && !OtherStart)
            {
                OtherStart = true;
                Appear3 = true;
            }
            
            if ((TrashBinPosition - TrashBinTargetPosition).Length() > 1)
            {
                TrashBinPosition = new Vector2(
                    TrashBinPosition.X + (TrashBinTargetPosition - TrashBinPosition).X * TrashBinMovement,
                    TrashBinPosition.Y + (TrashBinTargetPosition - TrashBinPosition).Y * TrashBinMovement);
            }
            if ((TitlePosition - TitleTargetPosition).Length() > 1)
            {
                TitlePosition = new Vector2(
                    TitlePosition.X + (TitleTargetPosition - TitlePosition).X * TrashBinMovement,
                    TitlePosition.Y + (TitleTargetPosition - TitlePosition).Y * TrashBinMovement);
            }
            if (OtherStart)
            {
                if (Appear3)
                {
                    if (Fade3 > 0.0f)
                    {
                        Fade3 -= 0.02f;
                    }
                    if (Fade3 < 0.1f && !Appear2)
                    {
                        Appear2 = true;
                    }
                    if (Fade3 <= 0.0f)
                    {
                        Appear3 = false;
                    }
                }
                if (Appear2)
                {
                    if (Fade2 > 0.0f)
                    {
                        Fade2 -= 0.02f;
                    }
                    if (Fade2 < 0.1f && !Appear1)
                    {
                        Appear1 = true;
                    }
                    if (Fade2 <= 0.0f)
                    {
                        Appear2 = false;
                    }
                }
                if (Appear1)
                {
                    if (Fade1 > 0.0f)
                    {
                        Fade1 -= 0.02f;
                    }
                    if (Fade1 < 0.1f && !AppearGo)
                    {
                        AppearGo = true;
                    }
                    if (Fade1 <= 0.0f)
                    {
                        Appear1 = false;                 
                    }
                }
                if (AppearGo)
                {
                    if (FadeGo > 0.0f)
                    {
                        FadeGo -= 0.02f;
                    }
                    if (Fade1 < 0.1f && !Start)
                    {
                        Start = true;
                    }
                    if (FadeGo <= 0.0f)
                    {
                        AppearGo = false;
                    }
                }
            }
            
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            Animation();
            Shiny.Visible = true;
            if (Start)
            {
                for (int i = 0; i < DaButtons.Count; i++)
                {
                    if (!DaButtons[i].GetDone())
                    {
                        DaButtons[i].Update(Speed, 0.0f);
                    }
                }
                if (SoundThingy2 == null)
                {
                    //plays the background music
                    SoundThingy2 = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("KarangGuniFull");
                    SoundThingy2.Play();
                }
            }
            if (Start)
            {
                //stop the timer
                PreviousTime = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds;
            }
            if (DaButtons[DaButtons.Count - 1].GetDone() && !Result)
            {
                CalculateTotalResult();
                if (SoundThingy2 != null)
                {
                    SoundThingy2.Stop(AudioStopOptions.Immediate);
                    SoundThingy2.Dispose();
                    SoundThingy2 = null;
                }
                Result = true;
                Start = false;
                PreviousTime = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds;
                Shiny.End();

            }
            if (FunStuff < 300)
            {
                PreviousTime = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds;
            }
            if (Result)
            {
                if (FunStuff < 300)
                {
                    FunStuff++;
                }
            }
            if (Result && FunStuff >= 150)
            {
                CurrentTime = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds - PreviousTime;
                if(CurrentTime > 2.0f)
                {
                    Reset();
                    Visible = false;
                    GameEngine.Services.GetService<SoundManager>().ChangeSong("normal");
                    PlayerState.Currently = PlayerState.State.NORMAL;
                }
            }  
            KeyboardInput();
        }
        public void Reset()
        {
            Start = false;
            Result = false;

            Fade1 = 1.0f;
            Fade2 = 1.0f;
            Fade3 = 1.0f;
            FadeGo = 1.0f;

            Appear1 = false;
            Appear2 = false;
            Appear3 = false;
            AppearGo = false;

            EXcounter = 0;
            WOWcounter = 0;
            COOLcounter = 0;
            Goodcounter = 0;
            Misscounter = 0;
            FunStuff = 0;
            ResultThingy = 0;
            BarLengther = 1.0f;

            TrashBinPosition = new Vector2(BarPosition.X + TrashBinOrigin.X, Resizer.GetHeight(1.0f));
            TitlePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.0f));
            BinColor = new Color(Color.White, 1.0f);

            Shiny.CreateParticle(BorderLRPosition, 1, 3, -100, 100, 90);
            Shiny.CreateParticle(BorderRLPosition, 1, 3, -100, 100, 270);

            if (SoundThingy2 != null)
            {
                SoundThingy2.Stop(AudioStopOptions.Immediate);
                SoundThingy2.Dispose();
                SoundThingy2 = null;
            }
            for (int i = 0; i < DaButtons.Count; i++)
            {
                DaButtons[i].Reset(Speed, BarPosition);
            }
            RandomizeButtons();

            Shiny.Visible = false;
        }
        private void BinColorChange(string Button)
        {
            if (Button == "A Button")
            {
                BinColor = new Color(Color.LightGreen, 1.0f);
            }
            else if (Button == "B Button")
            {
                BinColor = new Color(Color.Red, 1.0f);
            }
            else if (Button == "X Button")
            {
                BinColor = new Color(Color.LightSkyBlue, 1.0f);
            }
            else if (Button == "Y Button")
            {
                BinColor = new Color(Color.Yellow, 1.0f);
            }
            else if (Button == "LR Button")
            {
                BinColor = new Color(Color.LightSlateGray, 1.0f);
            }
        }
        private void CheckButton(int i)
        {
            SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue(DaButtons[i].GetSound());
            SoundThingy.Play();
            DaButtons[i].Finishing();
            DaButtons[i].SetGrade(CalculateGrade(DaButtons[i].GetPosition()));
        }
        private void KeyboardInput()
        {
            //if (GP.Button_Pressed(Buttons.A))
            //{
            //    return;
            //}
            if (KB.Key_Pressed(Keys.I))
            {
                Reset();
            }
            if (KB.Key_Pressed(Keys.O))
            {
                Start = !Start;
            }
            for (int i = 0; i < DaButtons.Count; i++)
            {
                if (!DaButtons[i].GetDone())
                {
                    if (DaButtons[i].GetPosition().X <= BarPosition.X + Allowance
                        && DaButtons[i].GetPosition().X >= BarPosition.X - Allowance)
                    {
                        if (GP.Button_Pressed(Buttons.A) && DaButtons[i].GetButton() == "A Button")
                        {
                            CheckButton(i);
                        }
                        else if (GP.Button_Pressed(Buttons.B) && DaButtons[i].GetButton() == "B Button")
                        {
                            CheckButton(i);
                        }
                        else if (GP.Button_Pressed(Buttons.X) && DaButtons[i].GetButton() == "X Button")
                        {
                            CheckButton(i);
                        }
                        else if (GP.Button_Pressed(Buttons.Y) && DaButtons[i].GetButton() == "Y Button")
                        {
                            CheckButton(i);
                        }
                        else if (GP.Button_Pressed(Buttons.LeftShoulder) || GP.Button_Pressed(Buttons.RightShoulder) && DaButtons[i].GetButton() == "LR Button")
                        {
                            CheckButton(i);
                        }
                        if (KB.Key_Pressed(Keys.M))
                        {
                            CheckButton(i);
                        }
                    }
                    if (DaButtons[i].GetPosition().X > BarPosition.X + Allowance && !DaButtons[i].GetAlmostDone())
                    {
                        //DaButtons[i].Finish();
                        DaButtons[i].SetDrop(true);
                        DaButtons[i].SetGrade("Miss");
                    }
                    if (DaButtons[i].GetPosition().X > BarPosition.X + Allowance + 75)
                    {
                        BinColorChange(DaButtons[i].GetButton());
                        DaButtons[i].Finish();
                        //DaButtons[i].SetGrade("Miss");
                    }
                }
            }
        }
        public int GetResultThingy()
        {
            return ResultThingy;
        }
        private void CalculateTotalResult()
        {
            for (int i = 0; i < DaButtons.Count; i++)
            {
                if (DaButtons[i].GetGrade() == "EX")
                {
                    EXcounter++;
                    ResultThingy += 4;
                }
                else if (DaButtons[i].GetGrade() == "WOW")
                {
                    WOWcounter++;
                    ResultThingy += 3;
                }
                else if (DaButtons[i].GetGrade() == "COOL")
                {
                    COOLcounter++;
                    ResultThingy += 2;
                }
                else if (DaButtons[i].GetGrade() == "Good")
                {
                    Goodcounter++;
                    ResultThingy += 1;
                }
                else if (DaButtons[i].GetGrade() == "Miss")
                {
                    Misscounter++;
                    ResultThingy += 0;
                }
            }
            ResultThingy = (int)(((float)ResultThingy / (float)DaButtons.Count * 2) * 100);
            ResultThingy *= 1;
            GameEngine.Services.GetService<PlayerStats>().PopularityChange(ResultThingy / 100);
        }
        private string CalculateGrade(Vector2 position)
        {
            if ((BarPosition - position).Length() < Allowance * 0.3f)
            {
                return "EX";
            }
            else if ((BarPosition - position).Length() < Allowance * 0.5f)
            {
                return "WOW";
            }
            else if ((BarPosition - position).Length() < Allowance * 0.7f)
            {
                return "COOL";
            }
            else
            {
                return "Good";
            }
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public void Exit()
        {
            Visible = false;
        }
        private void ShowResults()
        {
            GameEngine.SpriteBatch.DrawString(WriteText, "EX", new Vector2(Center.X * 0.75f, Center.Y * 0.8f), Color.White,
                  0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            GameEngine.SpriteBatch.DrawString(WriteText, "WOW", new Vector2(Center.X * 0.75f, Center.Y * 0.9f), Color.White,
                  0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            GameEngine.SpriteBatch.DrawString(WriteText, "COOL", new Vector2(Center.X * 0.75f, Center.Y), Color.White,
                  0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            GameEngine.SpriteBatch.DrawString(WriteText, "Good", new Vector2(Center.X * 0.75f, Center.Y * 1.1f), Color.White,
                  0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            GameEngine.SpriteBatch.DrawString(WriteText, "Miss", new Vector2(Center.X * 0.75f, Center.Y * 1.2f), Color.White,
                  0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);

           
            if (FunStuff < 60)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, random.Next(10, 100).ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 0.8f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff >= 60)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, EXcounter.ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 0.8f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff < 120 && FunStuff > 60)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, random.Next(10, 100).ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 0.9f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff >= 120)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, WOWcounter.ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 0.9f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff < 180 && FunStuff > 120)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, random.Next(10, 100).ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff >= 180)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, COOLcounter.ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff < 240 && FunStuff > 180)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, random.Next(10, 100).ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 1.1f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff >= 240)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, Goodcounter.ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 1.1f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff < 300 && FunStuff > 240)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, random.Next(10, 100).ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 1.2f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
            if (FunStuff >= 300)
            {
                GameEngine.SpriteBatch.DrawString(WriteText, Misscounter.ToString(), new Vector2(Center.X * 0.75f + 100.0f, Center.Y * 1.2f), Color.White,
                   0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
            }
        }
        public bool GetResult() { return Result; }
        public override void Draw()
        {
            
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //temp timer
                //GameEngine.SpriteBatch.DrawString(WriteText, CurrentTime.ToString(), new Vector2(200.0f, 200.0f), Color.White,
                //    0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);
                
                //background
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, new Color(Color.White, 0.7f),
                0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);

                if (Appear3)
                {
                    //number 3
                    GameEngine.SpriteBatch.Draw(Number3, NumberPosition, NumberRectangle, new Color(Color.White, Fade3),
                    0.0f, NumberOrigin, 1.0f, SpriteEffects.None, 0f);
                }
                if (Appear2)
                {
                    //number 2
                    GameEngine.SpriteBatch.Draw(Number2, NumberPosition, NumberRectangle, new Color(Color.White, Fade2),
                    0.0f, NumberOrigin, 1.0f, SpriteEffects.None, 0f);
                }
                if (Appear1)
                {
                    //number 1
                    GameEngine.SpriteBatch.Draw(Number1, NumberPosition, NumberRectangle, new Color(Color.White, Fade1),
                    0.0f, NumberOrigin, 1.0f, SpriteEffects.None, 0f);
                }
                if (AppearGo)
                {
                    //number G
                    GameEngine.SpriteBatch.Draw(NumberG, new Vector2(NumberPosition.X - NumberOrigin.X, NumberPosition.Y), NumberRectangle, new Color(Color.White, FadeGo),
                    0.0f, NumberOrigin, 1.0f + (1.0f - FadeGo), SpriteEffects.None, 0f);
                    //number O
                    GameEngine.SpriteBatch.Draw(NumberO, new Vector2(NumberPosition.X + NumberOrigin.X, NumberPosition.Y), NumberRectangle, new Color(Color.White, FadeGo),
                    0.0f, NumberOrigin, 1.0f + (1.0f - FadeGo), SpriteEffects.None, 0f);
                    //number EX
                    GameEngine.SpriteBatch.Draw(NumberEX, new Vector2(NumberPosition.X + NumberOrigin.X * 2, NumberPosition.Y), NumberRectangle, new Color(Color.White, FadeGo),
                    0.0f, NumberOrigin, 1.0f + (1.0f - FadeGo), SpriteEffects.None, 0f);
                }

                if (!Result)
                {
                    //border
                    GameEngine.SpriteBatch.Draw(Border, BorderLRPosition, BorderRectangle, new Color(Color.White, 0.5f),
                    0.0f, BorderOrigin, new Vector2(BarLengther, 5.0f), SpriteEffects.None, 0f);

                    //border
                    GameEngine.SpriteBatch.Draw(Border, BorderRLPosition, BorderRectangle, new Color(Color.White, 0.5f),
                    0.0f, Border2Origin, new Vector2(BarLengther, 5.0f), SpriteEffects.None, 0f);

                    //bar
                    GameEngine.SpriteBatch.Draw(Bar, BarPosition, BarRectangle, Color.White,
                    0.0f, BarOrigin, 1.0f, SpriteEffects.None, 0f);

                    //bin top 
                    GameEngine.SpriteBatch.Draw(TrashBinTop, TrashBinPosition, TrashBinRectangle, BinColor,
                    0.0f, TrashBinOrigin, 0.5f, SpriteEffects.None, 0f);

                    //title 
                    GameEngine.SpriteBatch.Draw(Title, TitlePosition, TitleRectangle, Color.White,
                    0.0f, TitleOrigin, 1.0f, SpriteEffects.None, 0f);

                    //buttons
                    for (int i = 0; i < DaButtons.Count; i++)
                    {
                        if (!DaButtons[i].GetDone())
                        {
                            if (DaButtons[i].GetButton() == "A Button")
                            {
                                GameEngine.SpriteBatch.Draw(AButton, DaButtons[i].GetPosition(), ButtonRectangle, new Color(Color.White, DaButtons[i].GetAlpha()),
                                0.0f, Buttonorigin, DaButtons[i].GetSize(), SpriteEffects.None, 0f);
                            }
                            if (DaButtons[i].GetButton() == "B Button")
                            {
                                GameEngine.SpriteBatch.Draw(BButton, DaButtons[i].GetPosition(), ButtonRectangle, new Color(Color.White, DaButtons[i].GetAlpha()),
                                0.0f, Buttonorigin, DaButtons[i].GetSize(), SpriteEffects.None, 0f);
                            }
                            if (DaButtons[i].GetButton() == "X Button")
                            {
                                GameEngine.SpriteBatch.Draw(XButton, DaButtons[i].GetPosition(), ButtonRectangle, new Color(Color.White, DaButtons[i].GetAlpha()),
                                0.0f, Buttonorigin, DaButtons[i].GetSize(), SpriteEffects.None, 0f);
                            }
                            if (DaButtons[i].GetButton() == "Y Button")
                            {
                                GameEngine.SpriteBatch.Draw(YButton, DaButtons[i].GetPosition(), ButtonRectangle, new Color(Color.White, DaButtons[i].GetAlpha()),
                                0.0f, Buttonorigin, DaButtons[i].GetSize(), SpriteEffects.None, 0f);
                            }
                            if (DaButtons[i].GetButton() == "LR Button")
                            {
                                GameEngine.SpriteBatch.Draw(LRButton, DaButtons[i].GetPosition(), ButtonRectangle, new Color(Color.White, DaButtons[i].GetAlpha()),
                                0.0f, Buttonorigin, DaButtons[i].GetSize(), SpriteEffects.None, 0f);
                            }
                        }
                    }

                    //bin btm 
                    GameEngine.SpriteBatch.Draw(TrashBinBtm, TrashBinPosition, TrashBinRectangle, BinColor,
                    0.0f, TrashBinOrigin, 0.5f, SpriteEffects.None, 0f);
                }
                //results
                if (Result)
                {
                    ShowResults();
                }

            }
            GameEngine.SpriteBatch.End();
            Shiny.Draw();
        }
    }

}
