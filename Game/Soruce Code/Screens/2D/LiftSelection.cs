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
    public class LiftSelection : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        //////////////////
        ///Textures
        /////////////////
        private Texture2D LeftGrid;
        private Texture2D RightGrid;
        private Texture2D LiftButton;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GPD = GameEngine.Services.GetService<GamepadDevice>();

        ////////////////
        /// Lift variables 
        ////////////////
        public static int CurrentFloor = 0; // Floor which the player is currently on
        public static int FloorMax = 0;     // Highest Floor
        public int TargetFloor;                    // Floor which the player wants to go
        bool Start;                         // Determines the Start of Lift moving

        /////////////////////
        /// Sprites Stuff
        //////////////////////
        private Rectangle GridRectangle;
        private Vector2 GridOrigin;


        private Rectangle LiftButtonRectangle;
        private Vector2 LiftButtonOrigin;

        private Vector2 LeftGridPosition;
        private Vector2 RightGridPosition;
        private Vector2 LiftButtonPosition;
        private Vector2 ButtonSpacing;
        
        
        private Effect myEffect;


        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public LiftSelection(GameScreen Parent)
            : base(Parent)
        {
            Visible = false;
            TargetFloor = CurrentFloor = 1;
            Start = false;
            FloorMax = 10;

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");

            LeftGrid = GameEngine.Content.Load<Texture2D>("Content/Lift/Sprites/0");
            RightGrid = GameEngine.Content.Load<Texture2D>("Content/Lift/Sprites/1");
            LiftButton = GameEngine.Content.Load<Texture2D>("Content/Lift/Sprites/LiftButton");

            GridRectangle = new Rectangle(0, 0, LeftGrid.Width , LeftGrid.Height);
            GridOrigin = new Vector2(LeftGrid.Width * 0.5f, LeftGrid.Height * 0.5f);

            LiftButtonRectangle = new Rectangle(0, 0, LiftButton.Width, LiftButton.Height);
            LiftButtonOrigin = new Vector2(LiftButton.Width * 0.5f, LiftButton.Height * 0.5f);

            LeftGridPosition = new Vector2(Resizer.GetWidth(0.5f) - GridOrigin.X * 0.7f,
                Resizer.GetHeight(0.01f) + GridOrigin.Y);

            RightGridPosition = new Vector2(Resizer.GetWidth(0.5f) + GridOrigin.X * 0.7f, LeftGridPosition.Y);

            LiftButtonPosition = new Vector2(Resizer.GetWidth(0.25f), LeftGridPosition.Y + GridOrigin.Y);
            ButtonSpacing = new Vector2(Resizer.GetRealWidth(0.5f), 0);
        }
        public void ScreenUpdate()
        {
            LeftGridPosition = new Vector2(Resizer.GetWidth(0.5f) - GridOrigin.X * 0.7f,
                Resizer.GetHeight(0.01f) + GridOrigin.Y);

            RightGridPosition = new Vector2(Resizer.GetWidth(0.5f) + GridOrigin.X * 0.7f, LeftGridPosition.Y);

            LiftButtonPosition = new Vector2(Resizer.GetWidth(0.25f), LeftGridPosition.Y + GridOrigin.Y);
            ButtonSpacing = new Vector2(Resizer.GetRealWidth(0.5f), 0);
        }
        public override void Update()
        {
            if (PlayerState.Currently != PlayerState.State.LIFT)
            {
                Visible = false;
                return;
            }
            else
            {
                Visible = true;
            }
            if (CurrentFloor == TargetFloor)
            {
                Start = false;
            }

            KeyBoardInput();
            NumbersUpdate();
        }
        private void Reset()
        {
            Visible = false;
        }
        private void TargetChange(int Tens, int Ones)
        {
            LeftGrid = GameEngine.Content.Load<Texture2D>("Content/Lift/Sprites/" + Tens.ToString());
            RightGrid = GameEngine.Content.Load<Texture2D>("Content/Lift/Sprites/" + Ones.ToString());
        }
        private void NumbersUpdate()
        {
            int Ones = TargetFloor % 10;
            int Tens = (TargetFloor - Ones) / 10 ;
            TargetChange(Tens, Ones);
        }
        public void LiftActivate()
        {
            Start = true;
        }
        public int GetTargetFloor()
        {
            if (Start)
            {
                return TargetFloor;
            }
            return -1;
        }
        private void AddFloor()
        {
            if (TargetFloor + 1 > FloorMax)
            {
                TargetFloor = 1;
            }
            else
            {
                TargetFloor++;
            }

        }
        private void MinusFloor()
        {
            if (TargetFloor - 1 < 1)
            {
                TargetFloor = FloorMax;
            }
            else
            {
                TargetFloor--;
            }
        }
        private void KeyBoardInput()
        {
            if (Start)
            {
                return;
            }
            //GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);

            if (GPD.IsConnected)
            {
                Vector2 Dir = GPD.LeftStickDelta;
                if (GPD.Button_Pressed(Buttons.DPadLeft) || GPD.Button_Pressed(Buttons.DPadDown) || Dir.X < 0 || Dir.Y < 0)
                    MinusFloor();
                else if (GPD.Button_Pressed(Buttons.DPadRight) || GPD.Button_Pressed(Buttons.DPadUp) || Dir.X > 0 || Dir.Y > 0)
                    AddFloor();
                else if (GPD.Button_Pressed(Buttons.A))
                    LiftActivate();
            }
            if (KB.Key_Pressed(Keys.Left )) //minus
                MinusFloor();
            else if (KB.Key_Pressed(Keys.Right)) //add
                AddFloor();
            else if (KB.Key_Pressed(Keys.Z)) // confirm
                LiftActivate();
        }
        
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = true;
            base.InitializeComponent(Parent);
        }
        public override void Draw()
        {
            if (!Start)
            {
                GameEngine.SpriteBatch.Begin();
                {
                    //Left Grid
                    GameEngine.SpriteBatch.Draw(LeftGrid, LeftGridPosition, GridRectangle, Color.White,
                               0.0f, GridOrigin, 1.0f, SpriteEffects.None, 0f);

                    //Right Grid
                    GameEngine.SpriteBatch.Draw(RightGrid, RightGridPosition, GridRectangle, Color.White,
                               0.0f, GridOrigin, 1.0f, SpriteEffects.None, 0f);

                    //Button
                    //GameEngine.SpriteBatch.Draw(LiftButton, LiftButtonPosition, LiftButtonRectangle, Color.White,
                    //           0.0f, LiftButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                }
                GameEngine.SpriteBatch.End();
            }
            for (int i = 1; i <= FloorMax ; i++)
            {
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    if (CurrentFloor == i)
                    {
                        myEffect.Parameters["Intensity"].SetValue(0.3f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[1].Begin();

                        //Button
                        GameEngine.SpriteBatch.Draw(LiftButton, LiftButtonPosition + new Vector2(((ButtonSpacing.X) / (FloorMax - 1)) * (i - 1), 0), LiftButtonRectangle, Color.White,
                               0.0f, LiftButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                        myEffect.CurrentTechnique.Passes[1].End();
                        myEffect.End();
                    }
                    else
                    {
                        //Button
                        GameEngine.SpriteBatch.Draw(LiftButton, LiftButtonPosition + new Vector2(((ButtonSpacing.X) / (FloorMax - 1)) * (i - 1), 0), LiftButtonRectangle, Color.White,
                               0.0f, LiftButtonOrigin, 1.0f, SpriteEffects.None, 0f);
                    }
                }
                GameEngine.SpriteBatch.End();
            }

         
        }
        //public int GetTargetLvl() { return TargetFloor; }
    }
}
