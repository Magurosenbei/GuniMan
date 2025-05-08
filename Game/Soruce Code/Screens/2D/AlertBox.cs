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
    public class AlertBox : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        
        //////////////////
        /// PopUp types
        /////////////////
        //None
        //Pickup   - pickup item
        //Curse    - pickup curse
        //Empty    - pickup empty
        //Shortage - Insufficient materials to fuse
        //Save     - saves the game
        //Saved    - saved

        protected enum PopupTypes { NONE, PICKUP, CURSE, EMPTY, SHORTAGE, SAVE, SAVED, LACKITEM, NOTATHOUSE, BOLUI, GETCASH }
        PopupTypes PopState = PopupTypes.NONE;
        
        ////////////////// 
        /// Lists
        /////////////////
        private List<AlertDef> AlertList;
        private List<AlertDef> SaveList;
        private List<AlertDef> EmptyList;
        
        
        
        //////////////////
        ///Textures
        /////////////////
        private Texture2D Picture;
        private Texture2D Yes;
        private Texture2D No;
        private Texture2D BackGround;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();


        ////////////////
        // Time
        ///////////////
        private float CurrentTime;
        private float PreviousTime;

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle PictureRectangle;
        private Vector2 PictureOrigin;
        private Vector2 PicturePosition;

        private Rectangle ConfirmRectangle;
        private Vector2 ConfirmOrigin;
        private Vector2 YesPosition;
        private Vector2 NoPosition;

        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundPosition;
        private Vector2 BackGroundScale;

        private Vector2 DialoguePosition;
        private Vector2 DialogueOrigin;
        
        ////////////////
        /// Font
        ////////////////
        SpriteFont WriteText;

        ///////////////////
        /// Display Text 
        ///////////////////
        private string DisplayText;

        ///////////////////
        /// Variables 
        ///////////////////
        private float FadeAndSize;
        private bool Play;
        private bool TextAppear;
        private bool Choice;                    //Choice for Yes & No
        private bool EnteredChoice;             //Choice has been entered or not
        private bool Done;                      //Bool to check whether action has finished
        private bool Faster;                    //Not pickup fastward variable

        /////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;

        /////////////////////////
        /// Effect Thingy
        /////////////////////////
        private Effect myEffect;


        ////////////////
        /// Sound
        ////////////////
        Cue SoundThingy = null;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public AlertBox(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {
            WriteText = spritefont;
            random = new Random(); // random is random /// lol

            Picture = GameEngine.Content.Load<Texture2D>("Content/Icons/Paper");
            Yes = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Tick");
            No = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Cross");

            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");

            AlertList = GameEngine.Content.Load<List<AlertDef>>(@"Content/Alert/XMLAlertPickup");
            SaveList = GameEngine.Content.Load<List<AlertDef>>(@"Content/Alert/XMLAlertSaving");
            EmptyList = GameEngine.Content.Load<List<AlertDef>>(@"Content/Alert/XMLAlertEmpty");

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");

            PictureRectangle = new Rectangle(0, 0, Picture.Width, Picture.Height);
            PictureOrigin = new Vector2(Picture.Width * 0.5f, Picture.Height * 0.5f);

            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BackGroundPosition = PicturePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 0.3f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.4f));

            ConfirmRectangle = new Rectangle(0, 0, Yes.Width, Yes.Height);
            ConfirmOrigin = new Vector2(Yes.Width * 0.5f, Yes.Height * 0.5f);
            YesPosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y + (BackGround.Height * 0.4f * BackGroundScale.Y));
            NoPosition = new Vector2(BackGroundPosition.X + BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y + (BackGround.Height * 0.4f * BackGroundScale.Y));

            DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);
            DialogueOrigin = new Vector2(0, 0);
            
            Play = true;
            TextAppear = false;
            Choice = true;
            EnteredChoice = false;
            FadeAndSize = 0.0f;
            Done = false;
            Faster = false;

            CurrentTime = PreviousTime = 0;
           
        }
        public void ScreenUpdate()
        {
            if(PopState == PopupTypes.PICKUP)
            {
                BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 0.3f), ((Resizer.GetRealHeight(1.0f) / (float)BackGround.Height) * 0.4f));
                DialoguePosition = new Vector2(BackGroundPosition.X - (BackGround.Width * 0.4f * BackGroundScale.X), BackGroundPosition.Y - (BackGround.Height * 0.4f * BackGroundScale.Y));
                YesPosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y + (BackGround.Height * 0.4f * BackGroundScale.Y));
                NoPosition = new Vector2(BackGroundPosition.X + BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y + (BackGround.Height * 0.4f * BackGroundScale.Y));

            }

            else if (PopState == PopupTypes.EMPTY)
            {
                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.35f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.1f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - (BackGround.Height * 0.4f * BackGroundScale.Y));
            }  
            //BackGroundPosition = PicturePosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            //BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 0.3f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.1f));

            //DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);    
        }
        private string RandomText()
        {
            if (PopState == PopupTypes.PICKUP)
            {
                return AlertList[random.Next(0, AlertList.Count)].AlertText;
            }
            else if (PopState == PopupTypes.EMPTY)
            {
                return EmptyList[random.Next(0, EmptyList.Count)].AlertText;
            }
            else if (PopState == PopupTypes.LACKITEM)
            {
                return "You need at least 5 items to sell!";
            }
            else if (PopState == PopupTypes.GETCASH)
            {
                return "";
            }
            else if (PopState == PopupTypes.NOTATHOUSE)
            {
                return "You need to be at your house!";
            }
            else if (PopState == PopupTypes.BOLUI)
            {
                return "You need 100,000G!";
            }
            else if (PopState == PopupTypes.SAVE)
            {
                return SaveList[random.Next(0, SaveList.Count)].AlertText;
            }
            else
            {
                return "";
            }
        }
        public void PopUp()
        {
            if (FadeAndSize < 1.0f)
            {
                FadeAndSize += 0.05f;
            }
            else
            {
                Play = false;
                TextAppear = true;
                SoundThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("Beeps");
                SoundThingy.Play();
            }
            PreviousTime = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds;
        }
        public void PopDown()
        {
            if (PopState != PopupTypes.PICKUP)
            {
                if (Faster)
                {
                    CurrentTime = 5.0f;
                }
                else
                {
                    CurrentTime = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds - PreviousTime;
                }
                if (CurrentTime < 2.0f)
                {
                    return;
                }
                if (FadeAndSize > 0.0f)
                {
                    TextAppear = false;
                    FadeAndSize -= 0.05f;
                }
                else
                {
                    Reset();
                }
            }
            else if (EnteredChoice)
            {
                if (FadeAndSize > 0.0f)
                {
                    TextAppear = false;
                    FadeAndSize -= 0.05f;
                }
                else
                {
                    Reset();
                }
            }
        }
        public bool GetDone()
        {
            return Done;
        }
        public void SetDone(bool temp)
        {
            Done = temp;
        }
        public bool GetChoice()
        {
            return Choice;
        }
        public void Reset()
        {
            if (PopState == PopupTypes.SAVE)
            {
                Play = true;
                FadeAndSize = 0.0f;
                CurrentTime = PreviousTime = 0;
                //Visible = false;
                PopUpBox(6, "Game Saved");
                Done = false;
                Faster = false;
            }
            else
            {
                Play = true;
                FadeAndSize = 0.0f;
                CurrentTime = PreviousTime = 0;
                Visible = false;
                Choice = true;
                EnteredChoice = false;
                Done = false;
                PopState = PopupTypes.NONE;
                Faster = false;
            }
            

            //to be revised or sumthing like that
            PlayerState.Currently = PlayerState.State.NORMAL;

        }
        private void SetState(int state)
        {
            if (state == 1)
            {
                PopState = PopupTypes.PICKUP;

                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f),Resizer.GetHeight(0.5f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.3f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.4f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);
                
            }
            if (state == 2)
            {
                PopState = PopupTypes.CURSE;
            }
            if (state == 3)
            {
                PopState = PopupTypes.EMPTY;
                
                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.35f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.1f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);
               
            }
            if (state == 4)
            {
                PopState = PopupTypes.SHORTAGE;
            }
            if (state == 5)
            {
                PopState = PopupTypes.SAVE;
            }
            if (state == 6)
            {
                PopState = PopupTypes.SAVED;
            }
            if (state == 7)
            {
                PopState = PopupTypes.LACKITEM;

                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.35f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.1f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);
               
            }
            if (state == 8)
            {
                PopState = PopupTypes.NOTATHOUSE;

                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.35f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.1f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);

            }
            if (state == 9)
            {
                PopState = PopupTypes.BOLUI;

                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.35f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.1f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);

            }
            if (state == 10)
            {
                PopState = PopupTypes.GETCASH;

                BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.4f));
                BackGroundScale = new Vector2(((Resizer.GetWithoutBorderWidth(1.0f) / (float)BackGround.Width) * 0.35f), ((Resizer.GetWithoutBorderHeight(1.0f) / BackGround.Height) * 0.1f));
                DialoguePosition = new Vector2(BackGroundPosition.X - BackGround.Width * 0.4f * BackGroundScale.X, BackGroundPosition.Y - BackGround.Height * 0.4f * BackGroundScale.Y);

            }
        }

        public void PopUpBox(int state, string Text)
        {
            SetState(state);
            DisplayText = RandomText();
            DisplayText += "\n";
            DisplayText += Text;
            //DialogueOrigin = new Vector2((float)DisplayText.Length * 0.0f, 3.5f);    
        }
        private void KeyboardInput()
        {
            //if (GP.Button_Pressed(Buttons.A))
            //{
            //    return;
            //}
            if (KB.Key_Pressed(Keys.Left) || KB.Key_Pressed(Keys.Right) || GP.Button_Pressed(Buttons.DPadLeft) || GP.Button_Pressed(Buttons.DPadRight) )
            {
                Choice = !Choice;
            }
            if (KB.Key_Pressed(Keys.Z) && TextAppear || GP.Button_Pressed(Buttons.A) && TextAppear)
            {
                if (!EnteredChoice && !Done)
                {
                    EnteredChoice = true;
                    Done = true;
                }
                if (PopState != PopupTypes.PICKUP)
                {
                    Faster = true;
                }
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (PopState != PopupTypes.NONE && Play)
            {
                PopUp();
            }
            else if (PopState != PopupTypes.NONE && !Play)
            {
                PopDown();
            }
            KeyboardInput();
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
                //BG
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, new Color(Color.White, FadeAndSize),
                           0.0f, BackGroundOrigin, BackGroundScale * FadeAndSize, SpriteEffects.None, 0f);

                if (TextAppear)
                {
                    //Text
                    GameEngine.SpriteBatch.DrawString(WriteText, DisplayText, DialoguePosition, new Color(Color.White, 1.0f),
                          0.0f, DialogueOrigin, 1.0f, SpriteEffects.None, 0.0f);

                    if (PopState == PopupTypes.PICKUP)
                    {
                        //Picture
                        GameEngine.SpriteBatch.Draw(Picture, PicturePosition, PictureRectangle, Color.White,
                                   0.0f, PictureOrigin, 1.5f, SpriteEffects.None, 0f);
                    }
                }
            }
            GameEngine.SpriteBatch.End();

            if (TextAppear && PopState == PopupTypes.PICKUP)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        if (i == 0 && !Choice || i == 1 && Choice)
                        {
                            myEffect.Parameters["Intensity"].SetValue(0.3f);
                            myEffect.Begin();
                            myEffect.CurrentTechnique.Passes[0].Begin();
                        }
                        if (i == 0)
                        {
                            //Yes
                            GameEngine.SpriteBatch.Draw(Yes, YesPosition, ConfirmRectangle, Color.White,
                                       0.0f, ConfirmOrigin, 1.5f, SpriteEffects.None, 0f);
                        }
                        else if (i == 1)
                        {
                            //No
                            GameEngine.SpriteBatch.Draw(No, NoPosition, ConfirmRectangle, Color.White,
                                       0.0f, ConfirmOrigin, 1.5f, SpriteEffects.None, 0f);
                        }

                        if (i == 0 && !Choice || i == 1 && Choice)
                        {
                            myEffect.CurrentTechnique.Passes[0].End();
                            myEffect.End();
                        }
                    }
                    GameEngine.SpriteBatch.End();
                }

            }


        }
    }
}
