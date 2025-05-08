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
    public class PlayerEvent : Component
    {       
        ///////////////////
        /// Lists
        ///////////////////
        private List<ItemDef> ItemList;
       
        ///////////////////
        /// Variables
        ///////////////////
        private int Amount;
        private int ItemID;
        private string DisplayText;
        int PickCall = -1;
        int ComfirmCall = -1;
        int CallSector = -1;

        ////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;
        private int RandomCounter;

        //////////////
        // Inventory 
        //////////////
        Inventory Invt = GameEngine.Services.GetService<Inventory>();
        
        public PlayerEvent()
            : base()
        {
            ItemList = GameEngine.Content.Load<List<ItemDef>>(@"Content/Inventory/XML/XMLItemList");

            random = new Random(); // random is random /// lol
            RandomCounter = 0;

            Amount = 0;
            ItemID = 0;
        }
        private void DefineText(int amount, int itemtype)
        {
            DisplayText = amount.ToString() + " ";
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].ID == itemtype)
                {
                    DisplayText += ItemList[i].ItemName;
                    break;
                }
            }
        }
        public int RandomStuffID() // All
        {
            RandomCounter = random.Next(0, 100);

            //Good stuff 10%
            if (RandomCounter < 10)
            {
                return random.Next(Invt.GetGoodMin(), Invt.GetGoodMax() + 1);
            }
            //Useless 40%
            else if (RandomCounter < 50)
            {
                return random.Next(Invt.GetUselessMin(), Invt.GetUselessMax() + 1);
            }
            //Raw 50%
            else
            {
                RandomCounter = random.Next(0, 100);
                //Paper 45%
                if (RandomCounter < 45)
                {
                    return 1;
                }
                //Plastic 25%
                else if (RandomCounter < 70)
                {
                    return 2;
                }
                //Metal 15%
                else if (RandomCounter < 85)
                {
                    return 3;
                }
                //Glass 15%
                else if (RandomCounter < 100)
                {
                    return 4;
                }
            }
            return -1;
        }
        public int RandomStuffIDFloor() //Useless + Raw + (Level1 + Level4)Items
        {
            RandomCounter = random.Next(0, 100);
            //Good stuff 5%
            if (RandomCounter < 5)
            {
                RandomCounter = random.Next(0,5);

                if (RandomCounter == 0)
                {
                    return 10; // notebook
                }
                if (RandomCounter == 1)
                {
                    return 11; // Stapler
                }
                if (RandomCounter == 2)
                {
                    return 12; // Pen
                }
                if (RandomCounter == 3)
                {
                    return 13; // Origami
                }
                if (RandomCounter == 4)
                {
                    return 20; // Ring =D
                }
            }
            //Useless 45%
            else if (RandomCounter < 50)
            {
                return random.Next(Invt.GetUselessMin(), Invt.GetUselessMax() + 1);
            }
            //Raw 50%
            else
            {
                RandomCounter = random.Next(1, 100);
                //Paper 45%
                if (RandomCounter <= 45)
                {
                    return 1;
                }
                //Plastic 25%
                else if (RandomCounter <= 70)
                {
                    return 2;
                }
                //Metal 15%
                else if (RandomCounter <= 85)
                {
                    return 3;
                }
                //Glass 15%
                else if (RandomCounter <= 100)
                {
                    return 4;
                }
            }
            return -1;
        }
        public void PickUp()
        {
            Visible = true;
            //if alert box is not there
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                Amount = random.Next(1, 4);
                ItemID = RandomStuffID();

                DefineText(Amount, ItemID);

                PlayerState.Currently = PlayerState.State.PICK;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(1, DisplayText);               
            }
        }
        public void PickUpDustBin()
        {
            Visible = true;
            //if alert box is not there
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                Amount = random.Next(1, 4);
                ItemID = random.Next(1, 5);

                DefineText(Amount, ItemID);

                PlayerState.Currently = PlayerState.State.PICK;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(1, DisplayText);
            }
        }
        public void PickUpFloor(int i, int seg, int id, int amt)
        {
            CallSector = seg;
            PickCall = i;
            Visible = true;
            //if alert box is not there
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                Amount = amt;//random.Next(1, 4);
                ItemID = id;

                DefineText(Amount, ItemID);

                PlayerState.Currently = PlayerState.State.PICK;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(1, DisplayText);
            }
        }
        public void GetPickCall(int seg, out int index, out int segment)
        {
            segment = CallSector;
            if (seg == CallSector)
            {
                index = ComfirmCall;
                ComfirmCall = -1;
                return;
            }
            index = -1;
        }
        public void Lackitem()
        {
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                //PlayerState.Currently = PlayerState.State;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(7, "");
            }
        }
        public void NoHouse()
        {
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                //PlayerState.Currently = PlayerState.State;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(8, "");
            }
        }
        
        public void PlayerBoLui()
        {
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                //PlayerState.Currently = PlayerState.State;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(9, "");
            }
        }
        public void Empty()
        {
            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                PlayerState.Currently = PlayerState.State.PICK;
                GameEngine.Services.GetService<AlertBox>().Visible = true;
                GameEngine.Services.GetService<AlertBox>().PopUpBox(3, "");
            }
        }
        public void Reset()
        {
            Visible = false;
        }
        public void AlertBoxChecking()
        {
            //if alert box is there
            if (GameEngine.Services.GetService<AlertBox>().Visible)
            {
                if (GameEngine.Services.GetService<AlertBox>().GetDone())
                {
                    if (GameEngine.Services.GetService<AlertBox>().GetChoice())
                    {
                        // pass value to inventory here
                        ComfirmCall = PickCall;
                        GameEngine.Services.GetService<Inventory>().ItemUpdate(ItemID, Amount);
                    }
                    GameEngine.Services.GetService<AlertBox>().SetDone(false);
                    Visible = false;
                }
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            AlertBoxChecking();
        }
        
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public override void Draw()
        {
            return;
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                

            }
            GameEngine.SpriteBatch.End();

        }
    }
}
