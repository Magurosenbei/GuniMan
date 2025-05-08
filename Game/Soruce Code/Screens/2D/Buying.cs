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
    public class BuyingStuff : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }

        //////////////////
        ///Textures
        /////////////////
        private Texture2D OverLayBackGround;
        private Texture2D BackGround;
        private Texture2D OnesDigits;
        private Texture2D TensDigits;
        private Texture2D HundDigits;
        private Texture2D Arrow;
        private Texture2D GThingy;
        private Texture2D OkButton;


        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        ////////////////
        /// Font
        ////////////////
        SpriteFont WriteText;

        /////////////////////
        /// Sprites Stuff
        //////////////////////

        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundPosition;
        private Vector2 BackGroundScale;

        //private Rectangle OkRectangle;
        //private Vector2 OkOrigin;
        //private Vector2 OkPosition;

        private Rectangle DigitRectangle;
        private Vector2 DigitOrigin;
        private Vector2 OnesPosition;
        private Vector2 TensPosition;
        private Vector2 HundPosition;

        private Rectangle ArrowRectangle;
        private Vector2 ArrowOrigin;

        private Vector2 ArrowPositionUp;
        private Vector2 ArrowPositionDown;

        /////////////////////
        /// buying Variables
        //////////////////////
        int Selection;
        int OnesValue;
        int TensValue;
        int HundValue;

        int NumOfGoodies;
        int TypeOfGoodies;
        int NumOfTries; // To be randomed

        float Allowance;
        string DisplayStuff;

        /////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;


        ////////////////////////////
        /// Some other variables
        ///////////////////////////

        //gender
        bool Guy;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public BuyingStuff(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {
            NumOfTries = 1;
            WriteText = spritefont;
            OverLayBackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/purplebackground");
            Arrow = GameEngine.Content.Load<Texture2D>("Content/Buying/arrow");
            OnesDigits = GameEngine.Content.Load<Texture2D>("Content/Numbers/0");
            TensDigits = GameEngine.Content.Load<Texture2D>("Content/Numbers/0");
            HundDigits = GameEngine.Content.Load<Texture2D>("Content/Numbers/0");
            GThingy = GameEngine.Content.Load<Texture2D>("Content/Numbers/G");
            OkButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/AButton");

            BackGroundRectangle = new Rectangle(0, 0, 450, 450);
            BackGroundOrigin = new Vector2(BackGroundRectangle.Width * 0.5f, BackGroundRectangle.Height * 0.5f);
            BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 0.4f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.3f));

            DigitRectangle = new Rectangle(0, 0, OnesDigits.Width, OnesDigits.Height);
            DigitOrigin = new Vector2(OnesDigits.Width * 0.5f, OnesDigits.Height * 0.5f);

            TensPosition = new Vector2(BackGroundPosition.X, BackGroundPosition.Y);
            OnesPosition = new Vector2(TensPosition.X + DigitOrigin.X * 3.0f, TensPosition.Y);
            HundPosition = new Vector2(TensPosition.X - DigitOrigin.X * 3.0f, TensPosition.Y);

            ArrowRectangle = new Rectangle(0, 0, Arrow.Width, Arrow.Height);
            ArrowOrigin = new Vector2(Arrow.Width * 0.5f, Arrow.Height * 0.5f);

            ArrowPositionUp = new Vector2(HundPosition.X, HundPosition.Y - Arrow.Height * 1.5f);
            ArrowPositionDown = new Vector2(HundPosition.X, HundPosition.Y + Arrow.Height * 1.5f);

            OnesValue = 0;
            TensValue = 0;
            HundValue = 0;
            Selection = 3;

            NumOfGoodies = 0;
            NumOfTries = 2;
            Allowance = 0;

            Guy = false;
            DisplayStuff = "";
            random = new Random(); // random is random /// lol
        }
        public void DisplayWhat(int type)
        {
            DisplayStuff += NumOfGoodies.ToString() + " ";
            if (type == 1)
            {
                DisplayStuff += "Paper";
            }
            if (type == 2)
            {
                DisplayStuff += "Plastic";
            }
            if (type == 3)
            {
                DisplayStuff += "Metal";
            }
            if (type == 4)
            {
                DisplayStuff += "Glass";
            }
        }
        public void Reset()
        {
            DisplayStuff = "";
            OnesValue = 0;
            TensValue = 0;
            HundValue = 0;
        }
        public void RandomStuff(bool guy)
        {
            Guy = guy;
            TypeOfGoodies = random.Next(1, 4);
            NumOfGoodies = random.Next(5, 20);
            Allowance = TypeOfGoodies * NumOfGoodies * 0.1f;
            DisplayWhat(TypeOfGoodies);
        }
        private void AddValue()
        {
            if (Selection == 1)
            {
                if (HundValue < 9)
                {
                    HundValue++;
                }
                else
                {
                    HundValue = 0;
                }
            }
            else if (Selection == 2)
            {
                if (TensValue < 9)
                {
                    TensValue++;
                }
                else
                {
                    TensValue = 0;
                }
            }
            else if (Selection == 3)
            {
                if (OnesValue < 9)
                {
                    OnesValue++;
                }
                else
                {
                    OnesValue = 0;
                }
            }
        }
        private void MinusValue()
        {
            if (Selection == 1)
            {
                if (HundValue > 0)
                {
                    HundValue--;
                }
                else
                {
                    HundValue = 9;
                }
            }
            else if (Selection == 2)
            {
                if (TensValue > 0)
                {
                    TensValue--;
                }
                else
                {
                    TensValue = 9;
                }
            }
            else if (Selection == 3)
            {
                if (OnesValue > 0)
                {
                    OnesValue--;
                }
                else
                {
                    OnesValue = 9;
                }
            }
        }
        private void AddSelection()
        {
            if (Selection == 3)
            {
                Selection = 1;
            }
            else
            {
                Selection++;
            }
        }
        private void MinusSelection()
        {
            if (Selection == 1)
            {
                Selection = 3;
            }
            else
            {
                Selection--;
            }
        }
        private void SelectionArrowUpdate()
        {
            if (Selection == 1)
            {
                ArrowPositionUp = new Vector2(HundPosition.X, HundPosition.Y - Arrow.Height * 1.5f);
                ArrowPositionDown = new Vector2(HundPosition.X, HundPosition.Y + Arrow.Height * 1.5f);
            }
            if (Selection == 2)
            {
                ArrowPositionUp = new Vector2(TensPosition.X, HundPosition.Y - Arrow.Height * 1.5f);
                ArrowPositionDown = new Vector2(TensPosition.X, HundPosition.Y + Arrow.Height * 1.5f);
            }
            if (Selection == 3)
            {
                ArrowPositionUp = new Vector2(OnesPosition.X, HundPosition.Y - Arrow.Height * 1.5f);
                ArrowPositionDown = new Vector2(OnesPosition.X, HundPosition.Y + Arrow.Height * 1.5f);
            }
        }
        private void DigitsUpdate()
        {
            OnesDigits = GameEngine.Content.Load<Texture2D>("Content/Numbers/" + OnesValue.ToString());
            TensDigits = GameEngine.Content.Load<Texture2D>("Content/Numbers/" + TensValue.ToString());
            HundDigits = GameEngine.Content.Load<Texture2D>("Content/Numbers/" + HundValue.ToString());
        }
        public int GetValue()
        {
            return ((HundValue * 100) + (TensValue * 10) + (OnesValue));
        }
        private void Compare()
        {
            Visible = false;
            //bo lui
            if (GetValue() > GameEngine.Services.GetService<PlayerStats>().GetMoney())
            {
                if (NumOfTries == 0)
                {
                    NumOfTries = 2;
                    Reset();
                    GameEngine.Services.GetService<PlayerStats>().PopularityChange(-1);
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 4);
                }
                else
                {
                    NumOfTries--;
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 5);
                }
            }

            //if less than real value + allowance
            else if ((float)(TypeOfGoodies * NumOfGoodies) > (float)(GetValue() + Allowance))
            {
                if (NumOfTries == 0)
                {
                    NumOfTries = 2;
                    Reset();
                    GameEngine.Services.GetService<PlayerStats>().PopularityChange(-1);
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 4);
                }
                else
                {
                    NumOfTries--;
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 1);
                }
            }
            //if okay okay value
            else if ((float)(TypeOfGoodies * NumOfGoodies) < (float)(GetValue()) + Allowance &&
                (float)(TypeOfGoodies * NumOfGoodies) > (float)(GetValue()) - Allowance)
            {
                if (NumOfGoodies + GameEngine.Services.GetService<Inventory>().GetItemsOnHand() >
                    GameEngine.Services.GetService<Inventory>().GetMaxCapacity())
                {
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 6);
                }
                else
                {
                    GameEngine.Services.GetService<PlayerStats>().MoneyChange(GetValue() * -1);
                    GameEngine.Services.GetService<Inventory>().ItemUpdate(TypeOfGoodies, NumOfGoodies);
                    Reset();
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 2);
                }
            }
            //if a lot more
            else if ((float)(TypeOfGoodies * NumOfGoodies) < (float)(GetValue()) - Allowance)
            {
                if (NumOfGoodies + GameEngine.Services.GetService<Inventory>().GetItemsOnHand() >
                    GameEngine.Services.GetService<Inventory>().GetMaxCapacity())
                {
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 6);
                }
                else
                {
                    GameEngine.Services.GetService<PlayerStats>().MoneyChange(GetValue() * -1);
                    GameEngine.Services.GetService<Inventory>().ItemUpdate(TypeOfGoodies, NumOfGoodies);
                    Reset();
                    GameEngine.Services.GetService<DialogueEngine>().StartConversation("-", "-", Guy, 3);
                }
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (KB.Key_Pressed(Keys.Left) || GP.Button_Pressed(Buttons.DPadLeft))
            {
                MinusSelection();
            }
            if (KB.Key_Pressed(Keys.Right) || GP.Button_Pressed(Buttons.DPadRight))
            {
                AddSelection();
            }
            if (KB.Key_Pressed(Keys.Down) || GP.Button_Pressed(Buttons.DPadDown))
            {
                MinusValue();
            }
            if (KB.Key_Pressed(Keys.Up) || GP.Button_Pressed(Buttons.DPadUp))
            {
                AddValue();
            }
            if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A))
            {
                Compare();
            }
            SelectionArrowUpdate();
            DigitsUpdate();


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
                //Background overlay
                //GameEngine.SpriteBatch.Draw(OverLayBackGround, BackGroundPosition, BackGroundRectangle, Color.White,
                //          0.0f, BackGroundOrigin, 1.0f, SpriteEffects.None, 0f);

                //Background
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, Color.White,
                          0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);


                //G 
                GameEngine.SpriteBatch.Draw(GThingy, new Vector2(OnesPosition.X + 60.0f ,OnesPosition.Y), DigitRectangle, Color.White,
                          0.0f, DigitOrigin, 1.0f, SpriteEffects.None, 0f);

                //Ones 
                GameEngine.SpriteBatch.Draw(OnesDigits, OnesPosition, DigitRectangle, Color.White,
                          0.0f, DigitOrigin, 1.5f, SpriteEffects.None, 0f);

                //Tens 
                GameEngine.SpriteBatch.Draw(TensDigits, TensPosition, DigitRectangle, Color.White,
                          0.0f, DigitOrigin, 1.5f, SpriteEffects.None, 0f);

                //Hund 
                GameEngine.SpriteBatch.Draw(HundDigits, HundPosition, DigitRectangle, Color.White,
                          0.0f, DigitOrigin, 1.5f, SpriteEffects.None, 0f);

                //Stuff
                GameEngine.SpriteBatch.DrawString(WriteText, DisplayStuff, new Vector2(BackGroundPosition.X, BackGroundPosition.Y - 50.0f), Color.White,
                       0.0f, new Vector2(((DisplayStuff.Length * 3.5f)), 0.0f), 1.0f, SpriteEffects.None, 0.0f);


            }
            GameEngine.SpriteBatch.End();

            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //Hund Arrow Up 
                GameEngine.SpriteBatch.Draw(Arrow, ArrowPositionUp, ArrowRectangle, Color.White,
                          0.0f, ArrowOrigin, 1.0f, SpriteEffects.None, 0f);

                //Hund Arrow Down
                GameEngine.SpriteBatch.Draw(Arrow, ArrowPositionDown, ArrowRectangle, Color.White,
                          0.0f, ArrowOrigin, 1.0f, SpriteEffects.FlipVertically, 0f);

            }
            GameEngine.SpriteBatch.End();

        }
    }
}
