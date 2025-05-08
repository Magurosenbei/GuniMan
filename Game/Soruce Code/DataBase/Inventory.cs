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
    public class Inventory : Component
    {
        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();

        /////////////////
        /// Variables
        ////////////////
        private List<ItemDef> ItemList;
        private int ItemsOnHand;
        private int MaxCapacity; // 100 // 300 // 900

        private int RawMin;
        private int RawMax;

        private int GoodMin;
        private int GoodMax;
        
        private int UselessMin;
        private int UselessMax;
        
        private int RawIndex;               //used for displaying in order
        private int ItemsIndex;             //used for displaying in order

        private int Temp;                   //used for temp storage
        //////////////
        // Item Type
        //////////////
        //1 - raw       ID 1-4
        //2 - Good      ID 10-20
        //3 - Useless   ID 30-39
        //4 - KeyItem   ID 101-102

        public Inventory() : base() 
        {
            ItemList = GameEngine.Content.Load<List<ItemDef>>(@"Content/Inventory/XML/XMLItemList");
            ItemsOnHand = 0;
            RawIndex = ItemsIndex =0;
            MaxCapacity = 100;

            ItemMaxMinInit();
            RecalculateItemsOnHand();
        }
        public int GetMaxCapacity()
        {
            return MaxCapacity;
        }
        public int GetRawMin()
        {
            return RawMin;
        }
        public int GetRawMax()
        {
            return RawMax;
        }
        public int GetGoodMin()
        {
            return GoodMin;
        }
        public int GetGoodMax()
        {
            return GoodMax;
        }
        public int GetUselessMin()
        {
            return UselessMin;
        }
        public int GetUselessMax()
        {
            return UselessMax;
        }
        private void RecalculateItemsOnHand()
        {
            ItemsOnHand = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type != 4)
                {
                    ItemsOnHand += ItemList[i].Amount;
                }
            }
        }
        private void ItemMaxMinInit()
        {
            RawMin = 10000;
            RawMax = 0;

            GoodMin = 10000;
            GoodMax = 0;
        
            UselessMin = 10000;
            UselessMax = 0;

            for(int i = 0;i < ItemList.Count;i++)
            {
                // Raw
                if (ItemList[i].Type == 1)
                {
                    if (ItemList[i].ID < RawMin)
                    {
                        RawMin = ItemList[i].ID;
                    }
                    if (ItemList[i].ID > RawMax)
                    {
                        RawMax = ItemList[i].ID;
                    }
                }
                // Good
                if (ItemList[i].Type == 2)
                {
                    if (ItemList[i].ID < GoodMin)
                    {
                        GoodMin = ItemList[i].ID;
                    }
                    if (ItemList[i].ID > GoodMax)
                    {
                        GoodMax = ItemList[i].ID;
                    }
                }
                // Useless
                if (ItemList[i].Type == 3)
                {
                    if (ItemList[i].ID < UselessMin)
                    {
                        UselessMin = ItemList[i].ID;
                    }
                    if (ItemList[i].ID > UselessMax)
                    {
                        UselessMax = ItemList[i].ID;
                    }
                }
            }
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public int GetAmount(int IDchecker)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ID == IDchecker)
                {
                    return ItemList[i].Amount;
                }
            }
            return -1;
        }
        public int GetAmount(string Namechecker)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ItemName == Namechecker)
                {
                    return ItemList[i].Amount;
                }
            }
            return -1;
        }
        public void OrderChangeRaw()
        {
            Temp = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                //Paper
                if (ItemList[i].ID == 1 && ItemList[i].Amount > 0)
                {
                    ItemList[i].Index = Temp;
                    Temp++;
                }
                //Plastic
                else if (ItemList[i].ID == 2 && ItemList[i].Amount > 0)
                {
                    ItemList[i].Index = Temp;
                    Temp++;
                }
                //Metal
                else if (ItemList[i].ID == 3 && ItemList[i].Amount > 0)
                {
                    ItemList[i].Index = Temp;
                    Temp++;
                }
                //Glass
                else if (ItemList[i].ID == 4 && ItemList[i].Amount > 0)
                {
                    ItemList[i].Index = Temp;
                    Temp++;
                }
            }
        }
        public void OrderChange(int index, string type)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Index == index)
                {
                    if (type == "Raw")
                    {
                        if (ItemList[i].Type == 1)
                        {
                            ItemList[i].Index--;
                            OrderChangeRaw();
                            return;
                        }
                    }
                    else if (type == "Items")
                    {
                        if (ItemList[i].Type == 2 || ItemList[i].Type == 3)
                        {
                            ItemList[i].Index--;
                            OrderChange(index + 1, type);
                            return;
                        }
                    }
                }
            }
        }
        public void DeleteMaterials(int ID, int amount)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ID == ID)
                {
                    ItemList[i].Amount -= amount;
                    if (ItemList[i].Amount == 0)
                    {
                        OrderChange(ItemList[i].Index + 1, "Raw");     //move everything up
                        ItemList[i].Index = -1;                 //reset to default
                    }
                    ItemsOnHand -= amount;
                    RawIndex--;
                    return;
                }
            }
        }
        public void DeleteItem(int selection)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ID == selection)
                {
                    ItemList[i].Amount--;
                    if (ItemList[i].Amount == 0)
                    {
                        OrderChange(ItemList[i].Index + 1, "Items");     //move everything up
                        ItemList[i].Index = -1;                         //reset to default
                    }
                    ItemsOnHand--;
                    ItemsIndex--;
                    return;
                }
            }
        }
        public void ItemUpdate(int type, int amount)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ID == type)
                {
                    if (ItemList[i].Amount == 0)
                    {
                        if (ItemList[i].Type == 1)
                        {
                            ItemList[i].Index = RawIndex;
                            OrderChangeRaw();
                            RawIndex++;
                        }
                        if (ItemList[i].Type == 2 || ItemList[i].Type == 3)
                        {
                            ItemList[i].Index = ItemsIndex;
                            ItemsIndex++;
                        }
                    } 
                    ItemList[i].Amount += amount;
                    break;
                }
            }
            ItemsOnHand += amount;
        }
        public int GetItemsOnHand()
        {
            return ItemsOnHand;
        }
        public void Set()
        {

        }
        public void Reset()
        {
            
        }
       
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
           
            if (KB.Key_Pressed(Keys.R))
            {
                //GameEngine.Services.GetService<DialogueEngine>().Visible = true;
                //GameEngine.Services.GetService<DialogueEngine>().StartConversation("Konata_fanart", "Konata_fanart", true, 1);
            }
          
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
