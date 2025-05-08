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

using XmlContentExtension;

namespace Game
{
    public class PlayerStats : Component
    {
        /////////////////
        /// KeyBoard
        ////////////////
        //?? KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        /////////////////
        /// Variables
        ////////////////
        private int Popularity;         //Affects customers
        public int Money;              //G
        private int Sales;              //Sales

        private int DelayValue;         //displaying text

        public PlayerStats() : base() 
        {
            Popularity = 30;
            Money = 0;
            Sales = 0;

            DelayValue = 3;
        }
       
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public void LoadFromFileMoney(int money)
        {
            Money = money;
            GameEngine.Services.GetService<HealthHud>().MoneyUpdate(Money);
        }
        public void PopularityChange(int value)
        {
            Popularity += value;
            if (Popularity + value > 100)
            {
                Popularity = 100;
            }
        }
        public void DelayValueChange(int value)
        {
            if (value == 5)
            {
                value = 1;
            }
            else if (value == 1)
            {
                value = 5;
            }
            DelayValue = value;
            GameEngine.Services.GetService<DialogueEngine>().DelayValueChange(DelayValue);
            GameEngine.Services.GetService<InventoryDisplay>().DelayValueChange(DelayValue);
        }
        public void ResetSales()
        {
            Sales = 0;
        }
        public int GetSales()
        {
            return Sales;
        }
        public void SetSales(int temp)
        {
            Sales += temp;
        }
        public int GetPopularity()
        {
            return Popularity;
        }
        public int GetMoney()
        {
            return Money;
        }
        public void MoneyChange(int amount)
        {
            if (Money + amount > 999999)
            {
                Money = 999999;
            }
            else
            {
                Money += amount;
            }
            GameEngine.Services.GetService<HealthHud>().MoneyUpdate(Money);
        }
        public void ResetMoney()
        {
            Money = 0;
        }
        public int GetDelayValue()
        {
            return DelayValue;
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
           
            //if (KB.Key_Pressed(Keys.R))
            //{
                //GameEngine.Services.GetService<DialogueEngine>().Visible = true;
                //GameEngine.Services.GetService<DialogueEngine>().StartConversation("Konata_fanart", "Konata_fanart", true, 1);
            //}
          
            base.Update();
        }
       
        public override void Draw()
        {
            base.Draw();
        }
        protected void DrawButtonOnChoice(string ButtonName)
        {
            //GameEngine.SpriteBatch.Draw(AssetImages[ButtonName], AssetPosition[ButtonName], Color.White);
        }
    }
}
