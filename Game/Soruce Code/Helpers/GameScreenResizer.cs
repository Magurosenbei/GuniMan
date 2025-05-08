using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Engine;

namespace Game
{
    public class GameScreenResizer : Component
    {
        private float CurrentScreenWidth;
        private float CurrentScreenHeight;

        private bool Border;

        public GameScreenResizer() : base() { }
        public GameScreenResizer(GameScreen Parent) : base(Parent) { }

        protected override void InitializeComponent(GameScreen Parent)
        {
            Border = false;
            CurrentScreenWidth = GameEngine.GraphicDevice.Viewport.Width * 1.0f;
            CurrentScreenHeight = GameEngine.GraphicDevice.Viewport.Height * 1.0f;
            

            base.InitializeComponent(Parent);
        }
        public void ScreenChange()
        {
            if (Border)
            {
                CurrentScreenWidth = GameEngine.GraphicDevice.Viewport.Width * 0.8f;
                CurrentScreenHeight = GameEngine.GraphicDevice.Viewport.Height * 0.8f;
            }
            else
            {
                CurrentScreenWidth = GameEngine.GraphicDevice.Viewport.Width * 1.0f;
                CurrentScreenHeight = GameEngine.GraphicDevice.Viewport.Height * 1.0f;
            }
        }
        public float GetWithoutBorderWidth(float percent)
        {
            return GameEngine.GraphicDevice.Viewport.Width * percent;
        }
        public float GetWithoutBorderHeight(float percent)
        {
            return GameEngine.GraphicDevice.Viewport.Height * percent;
        }
        public float GetWithBorderWidth(float percent)
        {
            return GameEngine.GraphicDevice.Viewport.Width * 0.8f * percent;
        }
        public float GetWithBorderHeight(float percent)
        {
            return GameEngine.GraphicDevice.Viewport.Height * 0.8f * percent;
        }
        public float GetRealWidth(float percent)
        {
            if (Border)
            {
                return ((CurrentScreenWidth * percent)); // + GameEngine.GraphicDevice.Viewport.Width * 0.1f);
            }
            else
            {
                return (CurrentScreenWidth * percent);
            }
        }
        public float GetRealHeight(float percent)
        {
            if (Border)
            {
                return ((CurrentScreenHeight * percent)); // + GameEngine.GraphicDevice.Viewport.Height * 0.1f);
            }
            else
            {
                return (CurrentScreenHeight * percent);
            }
        }
        public float GetWidth(float percent)
        {
            if (Border)
            {
                return ((CurrentScreenWidth * percent) + GameEngine.GraphicDevice.Viewport.Width * 0.1f);
            }
            else
            {
                return (CurrentScreenWidth * percent);
            }
        }
        public float GetHeight(float percent)
        {
            if (Border)
            {
                return ((CurrentScreenHeight * percent) + GameEngine.GraphicDevice.Viewport.Height * 0.1f);
            }
            else
            {
                return (CurrentScreenHeight * percent);
            }
        }
        private void GrandUpdate()
        {
            //MainMenu
            GameEngine.Services.GetService<MainMenu>().ScreenUpdate();
            //Loading
            GameEngine.Services.GetService<LoadingScreen>().ScreenUpdate();
            //Transition
            GameEngine.Services.GetService<TransitionScreen>().ScreenUpdate();
            
            //achievments
            GameEngine.Services.GetService<AchievementEngine>().ScreenUpdate();
            
            //HUD
            GameEngine.Services.GetService<HealthHud>().ScreenUpdate();
            //Dialogue
            GameEngine.Services.GetService<DialogueEngine>().ScreenUpdate();
            //InGameMainMenu
            GameEngine.Services.GetService<InGameMainMenu>().ScreenUpdate();

            //Lift
            GameEngine.Services.GetService<LiftSelection>().ScreenUpdate();
            //AlertBox
            GameEngine.Services.GetService<AlertBox>().ScreenUpdate();
            //winlose
            GameEngine.Services.GetService<Winlose>().ScreenUpdate();
            //console
            GameEngine.Services.GetService<ConsoleMenu>().ScreenUpdate();
            
        }
        public override void Update()
        {
            if (Border == GameEngine.Services.GetService<SafeZone>().Visible)
            {
                return;
            }
            else
            {
                if (GameEngine.Services.GetService<SafeZone>().Visible)
                {
                    Border = true;
                    ScreenChange();
                    GrandUpdate();
                }
                else
                {
                    Border = false;
                    ScreenChange();
                    GrandUpdate();
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            return;
        }
    }
}
