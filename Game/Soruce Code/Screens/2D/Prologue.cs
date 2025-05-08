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
    public class Prologue : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
        public SpriteFont WriteText;                  // The Font
        private bool PrologueThingy;
        private bool PrologueAdd;
        private bool PrologueMinus;
        private float counter;
        private string Title;

        private Cue MikuCall = null;
        private Cue MusicThingy = null;
        private Vector2 origin;
        private Vector2 center;

        //////////////////
        /// Keyboard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        public Prologue() : base() { }
        public Prologue(SpriteFont spritefont, string title, GameScreen Parent )  : base(Parent)
        {
            Title = title;
            WriteText = spritefont;
            PrologueAdd = true;
            PrologueMinus = false;
            PrologueThingy = true;
            counter = 0.0f;

            origin = new Vector2((float)(Title.Length * 3.5f), 0.0f);
            center = new Vector2((float)(GameEngine.GraphicDevice.Viewport.Width * 0.5f),(float)GameEngine.GraphicDevice.Viewport.Height * 0.5f);

            MikuCall = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("MikuGarangGuni");
            MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("PreMenuTracK");
        }
        
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = true;
            base.InitializeComponent(Parent);
        }
        protected void AddPrologue(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % 10 == 0)
            {
                counter += 0.03f;
            }
        }
        protected void MinusPrologue(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Milliseconds % 20 == 0)
            {
                counter -= 0.02f;
            }
            if (counter <= 0.0f)
            {
                counter = 0.0f;
                PrologueThingy = false;
            }
        }
        public void Reset()
        {
            Visible = false;
            PrologueAdd = true;
            PrologueMinus = false;
            PrologueThingy = true;
            counter = 0.0f;
            
            MikuCall.Stop(AudioStopOptions.Immediate);
            MikuCall.Dispose();

            MusicThingy.Stop(AudioStopOptions.Immediate);
            MusicThingy.Dispose();
            
        }
        private void PlayBGM()
        {
            if (!MusicThingy.IsPlaying)
            {
                MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("PreMenuTracK");
                MusicThingy.Play();
            }

        }
        private void KeyBoardInput()
        {
            if (KB.Key_Pressed(Keys.N) || GP.Button_Pressed(Buttons.Start))
            {
                //GameEngine.Services.GetService<MainMenu>().Visible = true;
                GameEngine.Services.GetService<TransitionScreen>().Visible = true;
                GameEngine.Services.GetService<TransitionScreen>().StartFading(1,2);
                
                //Reset();
                return;
            }
                        
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            PlayBGM();
            KeyBoardInput();
            
            GameTime gameTime = GameEngine.GameTime;
            if (!PrologueThingy)
            {
                GameEngine.Services.GetService<MainMenu>().Visible = true;
                //GameEngine.Services.GetService<AudioGame>().Visible = true;
                Visible = false;
                Reset();
                return;
            }
            if (PrologueThingy)
            {
                if (PrologueAdd)
                {
                    AddPrologue(gameTime);
                }
                if (PrologueMinus)
                {
                    MinusPrologue(gameTime);
                }
                if (counter > 1.0f)
                {
                    PrologueAdd = false;
                    PrologueMinus = true;
                }

                if (counter > 0.6f && PrologueAdd && !MikuCall.IsPlaying)
                {
                    MikuCall = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("MikuGarangGuni");
                    MikuCall.Play();
                }
                
            }
            base.Update();
        }
        public override void Draw()
        {
            //GameEngine.GraphicDevice.Clear(Color.Black);
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //Title
                GameEngine.SpriteBatch.DrawString(WriteText, Title, center, new Color(Color.White, counter),
                    0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
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
