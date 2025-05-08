using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using Engine;
using System;

namespace Game
{
    class HealthHud : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        Texture2D LifubarSpiral, LifubarRect, HPLogo, Outlinebar;
        Vector2 Position;
        Vector2 ItemDisplayPosition;
        Vector2 NumberDisplayPosition;

        Effect FillBar;

        ////////////////
        /// Font
        ////////////////
        SpriteFont WriteText;


        ////////////////
        /// Effects
        ////////////////
        private Effect myEffect;

        ////////////////
        /// Textures
        ////////////////
        Texture2D Paper;
        Texture2D Plastic;
        Texture2D Glass;
        Texture2D Metal;
        Texture2D Digit;
        private Texture2D Reminder;

        //////////////////////////
        /// Variables for sprites
        /////////////////////////

        private Rectangle IconRectangle;
        private Vector2 IconOrigin;

        private Rectangle DigitRectangle;
        private Vector2 DigitOrigin;

        private Rectangle ReminderRectangle;
        private Vector2 ReminderOrigin;
        private Vector2 ReminderPosition;

        /////////////////
        /// Money (Guni)
        /////////////////
        private int Money;
        string[] DigitValue = new string[7];          //limit is 999999 // last is used for "G"

        private int TempValue;

        float LifePoint = 100.0f;


        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();


        public HealthHud() : base() { Load(); }
        public HealthHud(SpriteFont spritefont, GameScreen Parent) : base(Parent) 
        {
            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
            WriteText = spritefont;
            Money = GameEngine.Services.GetService<PlayerStats>().GetMoney();
            TempValue = 0;

            for (int i = 0; i < DigitValue.Length; i++)
            {
                if (i == (DigitValue.Length - 1))
                {
                    DigitValue[i] = "G";
                }
                else
                {
                    DigitValue[i] = "0";
                }
            }
            Load(); 
        }
        public void MoneyUpdate(int temp)
        {
            Money = temp;
        }
        public void Load()
        {
            Visible = false;

            Reminder = GameEngine.Content.Load<Texture2D>("Content/GuniUI/HUD/Gamegoal");
            LifubarSpiral = GameEngine.Content.Load<Texture2D>("Content/GuniUI/HUD/Lifubar");
            LifubarRect = GameEngine.Content.Load<Texture2D>("Content/GuniUI/HUD/LifubarRect");
            HPLogo = GameEngine.Content.Load<Texture2D>("Content/GuniUI/HUD/Life_Hud");
            FillBar = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
            Outlinebar = GameEngine.Content.Load<Texture2D>("Content/GuniUI/HUD/OutlineBar");

            Paper = GameEngine.Content.Load<Texture2D>("Content/Icons/Paper");
            Plastic = GameEngine.Content.Load<Texture2D>("Content/Icons/Plastic");
            Metal = GameEngine.Content.Load<Texture2D>("Content/Icons/Metal");
            Glass = GameEngine.Content.Load<Texture2D>("Content/Icons/Glass");
            Digit = GameEngine.Content.Load<Texture2D>("Content/Numbers/0");

            DigitRectangle = new Rectangle(0, 0, Digit.Width, Digit.Height);
            DigitOrigin = new Vector2(Digit.Width * 0.5f, Digit.Height * 0.5f);

            IconRectangle = new Rectangle(0, 0, Paper.Width, Paper.Height);
            IconOrigin = new Vector2(Paper.Width * 0.5f, Paper.Height * 0.5f);

            ReminderRectangle = new Rectangle(0, 0, Reminder.Width, Reminder.Height);
            ReminderOrigin = new Vector2(Reminder.Width * 0.5f, Reminder.Height * 0.5f);
            ReminderPosition = new Vector2(Resizer.GetWidth(0.0f) + ReminderOrigin.X, Resizer.GetHeight(0.0f) + ReminderOrigin.Y);
            
            Position = new Vector2(Resizer.GetWidth(0.01f), Resizer.GetHeight(1.0f) - 150.0f);
            ItemDisplayPosition = new Vector2(Resizer.GetWidth(1.0f) - 100.0f, Resizer.GetHeight(1.0f) - 200.0f);
            NumberDisplayPosition = new Vector2(Resizer.GetWidth(1.0f) - (DigitOrigin.X * DigitValue.Length) * 0.5f, Resizer.GetHeight(0.0f) + Digit.Height);
        }
        public void ScreenUpdate()
        {
            Position = new Vector2(Resizer.GetWidth(0.01f), Resizer.GetHeight(1.0f) - 150.0f);
            ItemDisplayPosition = new Vector2(Resizer.GetWidth(1.0f) - 100.0f, Resizer.GetHeight(1.0f) - 200.0f);
            NumberDisplayPosition = new Vector2(Resizer.GetWidth(1.0f) - (DigitOrigin.X * DigitValue.Length) * 0.5f, Resizer.GetHeight(0.0f) + Digit.Height);
            ReminderPosition = new Vector2(Resizer.GetWidth(0.0f) + ReminderOrigin.X, Resizer.GetHeight(0.0f) + ReminderOrigin.Y);
        }
        private void NumbersUpdate()
        {
            for (int i = 0; i < DigitValue.Length - 1; i++)
            {
                if (i != 0)
                {
                    //Debug.Write(((int)(Math.Pow(10, i + 1))).ToString());
                    TempValue = ((Money - TempValue) % (int)(Math.Pow(10,i+1)));
                    TempValue /= (int)(Math.Pow(10, i));
                    DigitValue[i] = TempValue.ToString();
                    TempValue = Money % (int)(Math.Pow(10, i + 1));
                }
                else
                {
                    TempValue = Money % 10;
                    DigitValue[i] = TempValue.ToString();
                }
            }
            TempValue = 0;
            //TempValue = (Money % 10);
            //OnesValue = TempValue.ToString();

            //OnesValue = (Money.ToString()).Substring(4, 1);
            //TensValue = (Money.ToString()).Substring(3, 1);
            //HundValue = (Money.ToString()).Substring(2, 1);
            //ThouValue = (Money.ToString()).Substring(1, 1);
            //TenThouValue = (Money.ToString()).Substring(0, 1);
        }

        public void ChangeLifeBy(float Amt)
        {
            LifePoint += Amt;
            if(LifePoint > 100)
                LifePoint = 100;
            if (LifePoint < 0)
                LifePoint = 0;
        }
        public void ChangeLifeByPercentage(float Amt)
        {
            LifePoint += Amt * LifePoint;
            if (LifePoint > 100)
                LifePoint = 100;
            if (LifePoint < 0)
                LifePoint = 0;
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            NumbersUpdate();
        }
        public override void Draw()
        {
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                FillBar.Begin();

                GameEngine.SpriteBatch.Draw(Outlinebar, new Vector2(Position.X - 15.0f, Position.Y - 10.0f), new Microsoft.Xna.Framework.Rectangle(0, 0, Outlinebar.Width, Outlinebar.Height), Color.White,
                        0.0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);

                if (((LifePoint * 0.5f) / 100 * 2) >= 0.4f)
                {
                    GameEngine.SpriteBatch.Draw(LifubarRect, new Vector2(Position.X - 15.0f, Position.Y - 10.0f), new Microsoft.Xna.Framework.Rectangle(0, 0, (int)(((LifePoint * 0.5f) / 100 * 2) * LifubarRect.Width), LifubarRect.Height), Color.White,
                        0.0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);
                }
                GameEngine.SpriteBatch.Draw(LifubarSpiral, new Vector2(Position.X - 15.0f, Position.Y - 9.0f), new Microsoft.Xna.Framework.Rectangle(0, 0, LifubarSpiral.Width, (int)((LifePoint / 100 * 2) * LifubarSpiral.Height)), Color.White,
                        0.0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);

                GameEngine.SpriteBatch.Draw(HPLogo, new Vector2(Position.X - 25.0f, Position.Y - 25.0f), new Microsoft.Xna.Framework.Rectangle(0, 0, HPLogo.Width, HPLogo.Height), Color.White,
                        0.0f, new Vector2(0, 0), 1.7f, SpriteEffects.None, 0f);

                FillBar.End();
            }
            GameEngine.SpriteBatch.End();


            GameEngine.SpriteBatch.Begin();
            {

                GameEngine.SpriteBatch.Draw(Reminder, ReminderPosition, ReminderRectangle, new Color(Color.White, 0.7f),
                    0.0f, ReminderOrigin, 1.0f, SpriteEffects.None, 0f);
                //Items on hand
                //GameEngine.SpriteBatch.DrawString(WriteText, "X " + GameEngine.Services.GetService<Inventory>().GetItemsOnHand(), new Vector2(ItemDisplayPosition.X, ItemDisplayPosition.Y - 50.0f), Color.White,
                //        0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);

                //inventory display
                //    for (int i = 1; i < 5; i++)
                //    {
                //        //Paper     1
                //        //Plastic   2
                //        //Metal     3
                //        //Glass     4

                //        GameEngine.SpriteBatch.DrawString(WriteText, GameEngine.Services.GetService<Inventory>().GetAmount(i).ToString(), new Vector2(ItemDisplayPosition.X, ItemDisplayPosition.Y + ((i - 1) * 50.0f)), Color.White,
                //            0.0f, new Vector2(0.0f, 0.0f), 1.3f, SpriteEffects.None, 0.0f);

                //        if (i == 1)
                //        {
                //            GameEngine.SpriteBatch.Draw(Paper, new Vector2(ItemDisplayPosition.X - 30.0f, ItemDisplayPosition.Y + ((i - 1) * 50.0f) + 7.0f), IconRectangle, Color.White,
                //            0.0f, IconOrigin, 0.75f, SpriteEffects.None, 0f);
                //        }
                //        if (i == 2)
                //        {
                //            GameEngine.SpriteBatch.Draw(Plastic, new Vector2(ItemDisplayPosition.X - 30.0f, ItemDisplayPosition.Y + ((i - 1) * 50.0f) + 7.0f), IconRectangle, Color.White,
                //            0.0f, IconOrigin, 0.75f, SpriteEffects.None, 0f);
                //        }
                //        if (i == 3)
                //        {
                //            GameEngine.SpriteBatch.Draw(Metal, new Vector2(ItemDisplayPosition.X - 30.0f, ItemDisplayPosition.Y + ((i - 1) * 50.0f) + 7.0f), IconRectangle, Color.White,
                //            0.0f, IconOrigin, 0.75f, SpriteEffects.None, 0f);
                //        }
                //        if (i == 4)
                //        {
                //            GameEngine.SpriteBatch.Draw(Glass, new Vector2(ItemDisplayPosition.X - 30.0f, ItemDisplayPosition.Y + ((i - 1) * 50.0f) + 7.0f), IconRectangle, Color.White,
                //            0.0f, IconOrigin, 0.75f, SpriteEffects.None, 0f);
                //        }
                //    }
                //}
            }
            GameEngine.SpriteBatch.End();

            //Money display
            for (int i = DigitValue.Length - 1; i > -1; i--)
            {
                Digit = GameEngine.Content.Load<Texture2D>("Content/Numbers/" + DigitValue[i]);
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    if (i == DigitValue.Length - 1)
                    {
                        myEffect.Parameters["Intensity"].SetValue(0.9f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[1].Begin();

                        GameEngine.SpriteBatch.Draw(Digit, new Vector2(NumberDisplayPosition.X + 30.0f, NumberDisplayPosition.Y), DigitRectangle, Color.White,
                            0.0f, DigitOrigin, 0.8f, SpriteEffects.None, 0f);

                        myEffect.CurrentTechnique.Passes[1].End();
                        myEffect.End();
                    }
                    else
                    {
                        myEffect.Parameters["Intensity"].SetValue(1.3f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[0].Begin();

                        GameEngine.SpriteBatch.Draw(Digit, new Vector2(NumberDisplayPosition.X - (i * 28.0f), NumberDisplayPosition.Y), DigitRectangle, Color.White,
                            0.0f, DigitOrigin, new Vector2(0.9f, 1.0f), SpriteEffects.None, 0f);

                        myEffect.CurrentTechnique.Passes[0].End();
                        myEffect.End();

                    }
                }
                GameEngine.SpriteBatch.End();
            }

            base.Draw();
        }
    }
}
