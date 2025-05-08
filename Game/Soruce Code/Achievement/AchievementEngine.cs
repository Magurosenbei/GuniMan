using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Engine;
using XmlContentExtension;

namespace Game
{
    public class AchievementEngine : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }

        private List<AchievementTabDef> tabsdef;        //Achievements form xml
        private List<AchievementTab> tabs;              //Achievements
        public int OnScreen;                            //How many tabs is displayed on the screen

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        /////////////////////
        // Position
        /////////////////////

        private Vector2 StartPosition;

        private Effect myEffect;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public void AchievementsReSet()
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].Reset();
            }
        }
        public void AchievementsSet(int index, bool temp)
        {
            tabs[index - 1].Set(temp);
        }
        public void AchievementsInit()
        {
            for (int i = 0; i < tabsdef.Count; i++)
            {
                tabs.Add(new AchievementTab(
                    GameEngine.Content.Load<Texture2D>("Content/Achievements/Sprites/" + tabsdef[i].Picture),
                    GameEngine.Content.Load<Texture2D>("Content/Achievements/Sprites/" + tabsdef[i].Background),
                    GameEngine.Content.Load<SpriteFont>("Content/Fonts/" + tabsdef[i].NormalText),
                    GameEngine.Content.Load<SpriteFont>("Content/Fonts/" + tabsdef[i].BoldText),
                    tabsdef[i].Title, tabsdef[i].Description, tabsdef[i].Counter,
                    2.0f, 3));

            }
            
            //tabs.Add(new AchievementTab(GameEngine.Content.Load<Texture2D>("Content/Achievements/Sprites/Glass"), GameEngine.Content.Load<Texture2D>("Content/Achievements/Sprites/cement"), GameEngine.Content.Load<SpriteFont>("Content/Fonts/Comic"), GameEngine.Content.Load<SpriteFont>("Content/Fonts/ComicBold"),
            //    "Glass Award", "Obtain 100 glass", -10, 2.0f, 200.0f, 3));
            //tabs.Add(new AchievementTab(GameEngine.Content.Load<Texture2D>("Content/Achievements/Sprites/Paper"), GameEngine.Content.Load<Texture2D>("Content/Achievements/Sprites/cement"), GameEngine.Content.Load<SpriteFont>("Content/Fonts/Comic"), GameEngine.Content.Load<SpriteFont>("Content/Fonts/ComicBold"),
            //   "Paper Award", "Obtain 100 paper", -10, 2.0f, 200.0f, 3));
        }
        
        public AchievementEngine(GameScreen Parent)
            : base(Parent)
        {
            Visible = true;
            tabs = new List<AchievementTab>();
            tabsdef = GameEngine.Content.Load<List<AchievementTabDef>>(@"Content/Achievements/XML/XMLAchievements");
            AchievementsInit();
            OnScreen = 0;
            StartPosition = new Vector2(Resizer.GetWidth(0.25f), Resizer.GetHeight(0.55f));

            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");        
        }
        private void KeyboardInput()
        {
            if (KB.Key_Pressed(Keys.T))
            {
                UnlockAchievement(1);
            }
            if (KB.Key_Pressed(Keys.Y))
            {
                UnlockAchievement(0);
            }
        }
        public void Exit()
        {
            Visible = false;
        }
        public void SetPosition(Vector2 change)
        {
            StartPosition = change;
        }
        public void FullDraw()
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    if (!tabs[i].Unlocked)
                    {
                        myEffect.Parameters["Intensity"].SetValue(0.3f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[4].Begin();
                    }
                    if (i % 3 == 0)
                    {
                        tabs[i].Draw2(new Vector2(StartPosition.X , StartPosition.Y + ((i / 3) * 100.0f)));
                    }
                    else if (i % 3 == 1)
                    {
                        tabs[i].Draw2(new Vector2(StartPosition.X + Resizer.GetRealWidth(0.25f), StartPosition.Y + (((i - 1) / 3) * 100.0f)));
                    }
                    else if (i % 3 == 2)
                    {
                        tabs[i].Draw2(new Vector2(StartPosition.X + Resizer.GetRealWidth(0.5f), StartPosition.Y + (((i - 1) / 3) * 100.0f)));
                    }
                    if (!tabs[i].Unlocked)
                    {
                        myEffect.CurrentTechnique.Passes[4].End();
                        myEffect.End();
                    }
                }
                GameEngine.SpriteBatch.End();
            }
        }
        public void UnlockAchievement(int index)
        {
           if (!tabs[index].Unlocked)
           {
               tabs[index].Position = new Vector2(tabs[index].Position.X, tabs[index].Position.Y - (OnScreen * 50.0f)); 
               tabs[index].Unlocked = true;
               tabs[index].Animation = true;
               OnScreen++;
           }
           
        }
        public bool IsUnlocked(int index)
        {
            return tabs[index].Unlocked;
        }
        public void ScreenUpdate()
        {
            for (int i = 0; i < tabsdef.Count; i++)
            {
                tabs[i].ScreenUpdate();
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            for (int index = 0; index < tabs.Count; index++)
            {
                if (tabs[index].Update() == -1)
                {
                    OnScreen--;
                }
            }
            KeyboardInput();
        }

        public override void Draw()
        {
            GameEngine.SpriteBatch.Begin();
            for (int index = 0; index < tabs.Count; index++)
            {
                if (tabs[index].Unlocked && tabs[index].Animation)
                {
                    tabs[index].Draw();
                }
            }
            GameEngine.SpriteBatch.End();
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
    }
}
