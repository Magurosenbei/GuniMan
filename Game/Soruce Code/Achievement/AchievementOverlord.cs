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
    public class AchievementOverlord : Component
    {
        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        //////////////////////////
        /// Achievement Engine
        //////////////////////////
        AchievementEngine AE = GameEngine.Services.GetService<AchievementEngine>();
        
        // 0 - Paper
        // 1 - Plastic
        // 2 - Metal
        // 3 - Glass
        // 4 - Cat
        // 5 - Headshot

        public AchievementOverlord()
        {
            Visible = true;
        }
        private void AchievementChecker()
        {
            //Paper Award
            if (GameEngine.Services.GetService<Inventory>().GetAmount(1) > 100)
            {
                AE.UnlockAchievement(0);
            }
            //Plastic Award
            if (GameEngine.Services.GetService<Inventory>().GetAmount(2) > 100)
            {
                AE.UnlockAchievement(1);
            }
            //Metal Award
            if (GameEngine.Services.GetService<Inventory>().GetAmount(3) > 100)
            {
                AE.UnlockAchievement(2);
            }
            //Glass Award
            if (GameEngine.Services.GetService<Inventory>().GetAmount(4) > 100)
            {
                AE.UnlockAchievement(3);
            }
            // Cat Award
            if (GameEngine.Services.GetService<PatrolCats>().GetCount() > 5)
            {
                AE.UnlockAchievement(4);
            }
            // Headshot Flowerpot Award
            //if (GameEngine.Services.GetService<PatrolCats>().GetCount() > 5)
            //{
            //    AE.UnlockAchievement(5);
            //}
            if (GameEngine.Services.GetService<PlayerStats>().GetSales() > 5000 && PlayerState.Currently == PlayerState.State.NORMAL )
            {
                AE.UnlockAchievement(8);
            }

        }
        public void Headshot()
        {
            AE.UnlockAchievement(5);
        }
        public void TalkToEveryPasserBy()
        {
            AE.UnlockAchievement(7);
            
        }
        public override void Update()
        {
            if(MainGame.APP_STATE == APP_STATE.MAP)
            AchievementChecker();
        }

        public override void Draw()
        {
            
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = true;
            base.InitializeComponent(Parent);
        }
    }
}
