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
    public class Economy : Component
    {
        ////////////////// 
        /// Lists
        /////////////////
        private List<EconomyDef> EconomyList;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        /////////////////
        /// Variables
        ////////////////
        private int[] MaterialPrice;

        private int RandomCounter;

        ////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;


        public Economy()
            : base() 
        {
            random = new Random(); // random is random /// lol
            RandomCounter = 0;

            EconomyList = GameEngine.Content.Load<List<EconomyDef>>(@"Content/Economy/XMLEconomy");
            MaterialPrice = new int[EconomyList.Count];
            
            PriceInit();
            //DropInit();
        }
       
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        private void PriceInit()
        {           
            //starting standard price
            for (int i = 0; i < EconomyList.Count; i++)
            {
                MaterialPrice[i] = (int)((EconomyList[i].MaxPrice + EconomyList[i].MinPrice) / 2);
            }
        }
        public void Reset()
        {
            
        }
        private void RandomItem()
        {

        }
        private void EconomyRate()
        {
            //GameEngine.Services.GetService<Inventory>().
        }
        private void EconomyVary()
        {
            for (int i = 0; i < EconomyList.Count; i++)
            {
                MaterialPrice[i] += random.Next(0, 2);
                if (MaterialPrice[i] < EconomyList[i].MinPrice)
                {
                    MaterialPrice[i] = EconomyList[i].MinPrice;
                }
                else if (MaterialPrice[i] > EconomyList[i].MaxPrice)
                {
                    MaterialPrice[i] = EconomyList[i].MaxPrice;
                }
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            
            //Fixed fluaction
            EconomyRate();

            //random fluaction
            EconomyVary();
          
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
