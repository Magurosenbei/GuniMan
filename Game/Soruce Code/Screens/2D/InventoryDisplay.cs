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
    public class InventoryDisplay : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }

        ////////////////////////////////
        /// Top Selection States (L&R)
        ///////////////////////////////
        protected enum TopMenu { ITEMS, RAW, KEY }
        TopMenu TopMenuChoice = TopMenu.ITEMS;

        //////////////////////////////////////
        /// Side Selection States (Up & down)
        //////////////////////////////////////
        protected enum SideMenu { NONE, VIEW, FUSE, SELL }
        SideMenu SideMenuChoice = SideMenu.NONE;

        ///////////////////
        /// Font
        //////////////////
        SpriteFont WriteText;

        ////////////////
        /// Lists
        ///////////////
        private List<ItemDef> ItemList;
        private List<FusionDef> FusionList;

        //////////////////
        ///Textures
        /////////////////
        private Texture2D[] ItemTexture;
        private Texture2D BackGround;
        private Texture2D Selector;
        private Texture2D DialogueBackGround;
        private Texture2D ConfirmBackGround;
        private Texture2D[] SideMenuTexture;
        private Texture2D[] TopMenuTexture;
        private Texture2D[] MiniRawTexture;
        private Texture2D PurpleBackGroundSide;
        private Texture2D PurpleBackGroundTop;  


        private Texture2D UpDownButton;
        //private Texture2D LeftRightButton;
        private Texture2D AButton;
        private Texture2D BButton;
        private Texture2D XButton;
        private Texture2D LRButton;
        private Texture2D YButton;

        private Texture2D Yes;
        private Texture2D No;


        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        //////////////////////////////
        /// Variables 
        /////////////////////////////
        private int SideMenuSelection;                  //used to indicate SideMenu selected
        //1 - View
        //2 - Fuse
        //3 - Sell


        private string TextDisplay;
        private int Counter;
        private int Delay;
        private int DelayLimit;
        private int ItemSelection;                      //used for transversing

        private int FusionIndex;                        //used for fusion listing
        private bool Confirming;                        //whether confirm screen is on the screen
        private bool ConfirmSelection;                  //used for YES & NO thingy
        private int MaxItemSelection;                   //used for getting the max item in one cetegory
        private bool MaxItemChecked;                    //check whether the item max has been checked

        private string[] HelpText;
        private string PopUpText;                       //used for delete and fuse 

        private int ViewID;                             //used for passing the ID to be deleted
        private int[] SellPrice;                        //used for actual sell price
        private int[] PlayerPrice;                      //used for player sell price
        private int SellIndex;                          //used for checking sell price

        private int ItemSelling;                        //used to indicate how many items the player are selling
        private int SellSelected;                       //used to indicate which item is pointing to in SEll mode
        private int SellPercent;                        //used for selling percentage

        /////////////////////
        /// Sprites Stuff
        //////////////////////

        private Rectangle MiniRawRectangle;
        private Vector2 MiniRawOrigin;
        private Vector2 MiniRawPosition;

        private Rectangle SideMenuRectangle;
        private Vector2 SideMenuOrigin;
        private Vector2 SideMenuPosition;

        private Rectangle TopMenuRectangle;
        private Vector2 TopMenuOrigin;
        private Vector2 TopMenuPosition;

        private Rectangle ItemRectangle;
        private Vector2 ItemOrigin;
        private Vector2 ItemPosition;

        private Rectangle SelectorRectangle;
        private Vector2 SelectorOrigin;
        private Vector2 SelectorPosition;

        private Rectangle ConfirmRectangle;
        private Vector2 ConfirmOrigin;
        private Rectangle ConfirmBackGroundRectangle;
        private Vector2 ConfirmBackGroundOrigin;
        private Vector2 ConfirmBackGroundPosition;
        private Vector2 ConfirmBackGroundScale;
        private Vector2 YesPosition;
        private Vector2 NoPosition;

        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundPosition;
        private Vector2 BackGroundScale;

        private Rectangle DialogueBackGroundRectangle;
        private Vector2 DialogueBackGroundOrigin;
        private Vector2 DialogueBackGroundPosition;
        private Vector2 DialogueBackGroundScale;

        private Rectangle ButtonRectangle;
        private Vector2 ButtonOrigin;
        private Vector2 ButtonPosition;

        private Vector2 DialoguePosition;
        private Vector2 PopUpPosition;


        //////////////
        // Inventory 
        //////////////
        Inventory Invt = GameEngine.Services.GetService<Inventory>();

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        ////////////////
        // Timer
        //////////////
        private int PreviousSecond;

        //////////////////
        // Analog Control
        //////////////////
        private bool AnalogUp;
        private bool AnalogDown;
        private bool AnalogLeft;
        private bool AnalogRight;
        private bool CoolDown;
        private int CoolDownValue;

        private int AnalogUpValue;
        private int AnalogDownValue;
        private int AnalogLeftValue;
        private int AnalogRightValue;

        private Effect myEffect;

        /////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;

        public InventoryDisplay(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {
            random = new Random(); // random is random /// lol

            WriteText = spritefont;
            ItemList = GameEngine.Content.Load<List<ItemDef>>(@"Content/Inventory/XML/XMLItemList");
            FusionList = GameEngine.Content.Load<List<FusionDef>>(@"Content/Fusion/XML/XMLFusionList");
            TextDisplay = "Danightmare <3 Konata-chan";
            Counter = 0;
            Delay = 0;
            DelayLimit = GameEngine.Services.GetService<PlayerStats>().GetDelayValue();
            ItemSelection = -1;
            SideMenuSelection = 1;

            FusionIndex = 0;
            ViewID = 0;

            SellPriceInit();
            SellIndex = 0;
            SellSelected = 0;
            ItemSelling = 0;
            SellPercent = 0;

            PreviousSecond = 0;

            MaxItemSelection = 0;
            MaxItemChecked = false;
            ConfirmSelection = true;
            Confirming = false;

            HelpText = new string[5];
            HelpTextInit();
            PopUpText = "";

            AnalogUp = false;
            AnalogDown = false;
            AnalogLeft = false;
            AnalogRight = false;
            CoolDown = false;
            CoolDownValue = 0;

            AnalogUpValue = 0;
            AnalogDownValue = 0;
            AnalogLeftValue = 0;
            AnalogRightValue = 0;

            PurpleBackGroundSide = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/SmallSel");
            PurpleBackGroundTop = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/BigSel");
            Selector = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/HornSelector");
            UpDownButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/AButton");
            LRButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/LRButton");
            AButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/AButton");
            BButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/BButton");
            XButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/XButton");
            YButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/YButton");
            Yes = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Tick");
            No = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Cross");

            BackGround = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/InventoryScreen");
            DialogueBackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background2");
            ConfirmBackGround = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/BigSel");

            SideMenuTexture = new Texture2D[3];
            SideMenuTextureInit();
            SideMenuRectangle = new Rectangle(0, 0, SideMenuTexture[0].Width, SideMenuTexture[0].Height);
            SideMenuOrigin = new Vector2(SideMenuTexture[0].Width * 0.5f, SideMenuTexture[0].Height * 0.5f);
            SideMenuPosition = new Vector2(Resizer.GetWidth(0.1f), Resizer.GetHeight(0.3f));

            TopMenuTexture = new Texture2D[3];
            TopMenuTextureInit();
            TopMenuRectangle = new Rectangle(0, 0, TopMenuTexture[0].Width, TopMenuTexture[0].Height);
            TopMenuOrigin = new Vector2(TopMenuTexture[0].Width * 0.5f, TopMenuTexture[0].Height * 0.5f);
            TopMenuPosition = new Vector2(Resizer.GetWidth(0.2f), Resizer.GetHeight(0.2f));

            MiniRawTexture = new Texture2D[4];
            MiniRawTextureInit();
            MiniRawRectangle = new Rectangle(0, 0, MiniRawTexture[0].Width, MiniRawTexture[0].Height);
            MiniRawOrigin = new Vector2(MiniRawTexture[0].Width * 0.5f, MiniRawTexture[0].Height * 0.5f);
            MiniRawPosition = new Vector2(Resizer.GetWidth(0.2f), Resizer.GetHeight(0.1f));

            ItemTexture = new Texture2D[ItemList.Count];
            ItemTextureInit();
            //Item = GameEngine.Content.Load<Texture2D>("Content/Interactive/LightBulb");

            ItemRectangle = new Rectangle(0, 0, ItemTexture[0].Width, ItemTexture[0].Height);
            ItemOrigin = new Vector2(ItemTexture[0].Width * 0.5f, ItemTexture[0].Height * 0.5f);
            ItemPosition = new Vector2(Resizer.GetWidth(0.25f), Resizer.GetHeight(0.3f));
            //ItemPosition = new Vector2(Resizer.GetRealWidth(0.1f), Resizer.GetRealHeight(0.1f));

            SelectorRectangle = new Rectangle(0, 0, Selector.Width, Selector.Height);
            SelectorOrigin = new Vector2(Selector.Width * 0.5f, Selector.Height * 0.5f);
            SelectorPosition = new Vector2(ItemPosition.X - 50.0f, ItemPosition.Y);

            ConfirmRectangle = new Rectangle(0, 0, Yes.Width, Yes.Height);
            ConfirmOrigin = new Vector2(Yes.Width * 0.5f, Yes.Height * 0.5f);
            ConfirmBackGroundRectangle = new Rectangle(0, 0, ConfirmBackGround.Width, ConfirmBackGround.Height);
            ConfirmBackGroundOrigin = new Vector2(ConfirmBackGround.Width * 0.5f, ConfirmBackGround.Height * 0.5f);
            ConfirmBackGroundPosition = new Vector2(SelectorPosition.X + 450.0f, SelectorPosition.Y + ConfirmBackGroundOrigin.Y);
            ConfirmBackGroundScale = new Vector2(((Resizer.GetWidth(1.0f) / ConfirmBackGround.Width) * 0.2f), ((Resizer.GetHeight(1.0f) / ConfirmBackGround.Height) * 0.1f));

            YesPosition = new Vector2(ConfirmBackGroundPosition.X - ConfirmBackGroundOrigin.X * ConfirmBackGroundScale.X + ConfirmOrigin.X + 10.0f,
                ConfirmBackGroundPosition.Y + ConfirmBackGroundOrigin.Y * ConfirmBackGroundScale.Y - ConfirmOrigin.Y - 10.0f);
            NoPosition = new Vector2(ConfirmBackGroundPosition.X + ConfirmBackGroundOrigin.X * ConfirmBackGroundScale.X - ConfirmOrigin.X - 10.0f,
                ConfirmBackGroundPosition.Y + ConfirmBackGroundOrigin.Y * ConfirmBackGroundScale.Y - ConfirmOrigin.Y - 10.0f);

            PopUpPosition = new Vector2(ConfirmBackGroundPosition.X - ConfirmBackGroundOrigin.X * ConfirmBackGroundScale.X + 12.0f,
                ConfirmBackGroundPosition.Y - ConfirmBackGroundOrigin.Y * ConfirmBackGroundScale.Y + 20.0f);


            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));

            ButtonRectangle = new Rectangle(0, 0, AButton.Width, AButton.Height);
            ButtonOrigin = new Vector2(AButton.Width * 0.5f, AButton.Height * 0.5f);
            ButtonPosition = new Vector2((int)Resizer.GetWidth(0.8f), (int)Resizer.GetHeight(0.6f));

            DialogueBackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / DialogueBackGround.Width) * 0.9f), ((Resizer.GetRealHeight(1.0f) / DialogueBackGround.Height) * 0.1f));
            DialogueBackGroundRectangle = new Rectangle(0, 0, DialogueBackGround.Width, DialogueBackGround.Height);
            DialogueBackGroundOrigin = new Vector2(DialogueBackGround.Width * 0.5f, DialogueBackGround.Height * 0.5f);
            DialogueBackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.94f) - DialogueBackGroundOrigin.Y * DialogueBackGroundScale.Y);

            DialoguePosition = new Vector2(DialogueBackGroundPosition.X - (DialogueBackGround.Width * DialogueBackGroundScale.X) * 0.5f,
                DialogueBackGroundPosition.Y - (DialogueBackGround.Height * DialogueBackGroundScale.Y) * 0.5f );


            myEffect = GameEngine.Content.Load<Effect>("Content/Shader Fx/2DEffects");
        }
        private void TopMenuTextureInit()
        {
            TopMenuTexture[0] = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Items");
            TopMenuTexture[1] = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Materials");
            TopMenuTexture[2] = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Keyitems");
        }
        private void SideMenuTextureInit()
        {
            SideMenuTexture[0] = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/View");
            SideMenuTexture[1] = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Fuse");
            SideMenuTexture[2] = GameEngine.Content.Load<Texture2D>("Content/Inventory/Sprites/Sell");
        }
        private void SellPriceInit()
        {
            Delay = 0;  //used temporarily
            for (int j = 0; j < ItemList.Count; j++)
            {
                if (ItemList[j].Type == 2)
                {
                    Delay++;
                }
            }
            SellPrice = new int[Delay];
            PlayerPrice = new int[Delay];
            Delay = 0;
            for (int i = 0; i < FusionList.Count; i++)
            {
                SellPrice[i] = FusionList[i].Money;
                PlayerPrice[i] = FusionList[i].Money;
            }

            //special - platinum ring
            SellPrice[FusionList.Count] = 5000;
            PlayerPrice[FusionList.Count] = 5000;

        }
        private void HelpTextInit()
        {
            HelpText[0] = "Tabs";
            HelpText[1] = "Next";
            HelpText[2] = "Cancel";
            HelpText[3] = "Discard";
            HelpText[4] = "Close";
        }
        private void MiniRawTextureInit()
        {
            MiniRawTexture[0] = GameEngine.Content.Load<Texture2D>("Content/Icons/Paper");
            MiniRawTexture[1] = GameEngine.Content.Load<Texture2D>("Content/Icons/Plastic");
            MiniRawTexture[2] = GameEngine.Content.Load<Texture2D>("Content/Icons/Metal");
            MiniRawTexture[3] = GameEngine.Content.Load<Texture2D>("Content/Icons/Glass");
        }
        public void ScreenUpdate()
        {
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));
        }
        public void Reset()
        {
            SideMenuChoice = SideMenu.NONE;
            TopMenuChoice = TopMenu.ITEMS;

            SideMenuSelection = 1;
            ItemSelection = 0;
            SetText();
            ItemSelection = -1;

            MaxItemChecked = false;
            MaxItemSelection = 0;

            Counter = 0;
            for (int i = 0; i < FusionList.Count; i++)
            {
                PlayerPrice[i] = FusionList[i].Money;
            }

            //special - platinum ring
            PlayerPrice[FusionList.Count] = 5000;
            Visible = false;
        }
        private void ItemTextureInit()
        {
            for (int i = 0; i < ItemTexture.Length; i++)
            {
                ItemTexture[i] = GameEngine.Content.Load<Texture2D>("Content/Interactive/LightBulb");
            }
        }
      
        private void TopMenuSelectionUp()
        {
            if ((int)TopMenuChoice < 2)
            {
                TopMenuChoice++;
            }
            else
            {
                TopMenuChoice = TopMenu.ITEMS;
            }
            SetText();
            MaxItemChecked = false;

            ItemSelection = 0;
            MaxItemSelection = 0;
        }
        private void TopMenuSelectionDown()
        {
            if ((int)TopMenuChoice > 0)
            {
                TopMenuChoice--;
            }
            else
            {
                TopMenuChoice = TopMenu.KEY;
            }
            SetText();
            MaxItemChecked = false;

            ItemSelection = 0;
            MaxItemSelection = 0;
        }
        private void ItemSelectionUp()
        {
            if (MaxItemSelection > 1)
            {
                if (ItemSelection < MaxItemSelection - 1)
                {
                    ItemSelection++;
                }
                else
                {
                    ItemSelection = 0;
                }
                SetText();
            }
        }
        private void ItemSelectionDown()
        {
            if (MaxItemSelection > 1)
            {
                if (ItemSelection > 0)
                {
                    ItemSelection--;
                }
                else
                {
                    ItemSelection = MaxItemSelection - 1;
                }
                SetText();
            }
        }
        private void SideMenuSelectionUp()
        {
            if (SideMenuSelection < 3)
            {
                SideMenuSelection++;
            }
            else
            {
                SideMenuSelection = 1;
            }
        }
        private void SideMenuSelectionDown()
        {
            if (SideMenuSelection > 1)
            {
                SideMenuSelection--;
            }
            else
            {
                SideMenuSelection = 3;
            }
        }
        private void SideMenuSelected()
        {
            MaxItemChecked = false;

            ItemSelection = 0;
            MaxItemSelection = 0;
            if (SideMenuSelection == 1)
            {
                SideMenuChoice = SideMenu.VIEW;
                MaxItemChecked = false;
                SetText();
            }
            else if (SideMenuSelection == 2)
            {
                SideMenuChoice = SideMenu.FUSE;
                MaxItemChecked = false;
                SetText();
            }
            else if (SideMenuSelection == 3)
            {
                SideMenuChoice = SideMenu.SELL;
                MaxItemChecked = false;
                SetText();
            }
        }
        private void FuseItem()
        {
            FusionIndex = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                for (int j = 0; j < FusionList.Count; j++)
                {
                    if (ItemList[i].ItemName == FusionList[j].ItemName)
                    {
                        if (FusionList[j].Paper <= Invt.GetAmount("Paper") && FusionList[j].Plastic <= Invt.GetAmount("Plastic")
                        && FusionList[j].Metal <= Invt.GetAmount("Metal") && FusionList[j].Glass <= Invt.GetAmount("Glass"))
                        {
                            if (FusionIndex == ItemSelection)
                            {
                                GameEngine.Services.GetService<Inventory>().DeleteMaterials(1, FusionList[j].Paper);
                                GameEngine.Services.GetService<Inventory>().DeleteMaterials(2, FusionList[j].Plastic);
                                GameEngine.Services.GetService<Inventory>().DeleteMaterials(3, FusionList[j].Metal);
                                GameEngine.Services.GetService<Inventory>().DeleteMaterials(4, FusionList[j].Glass);
                                GameEngine.Services.GetService<Inventory>().ItemUpdate(ItemList[i].ID, 1);
                                return;
                            }
                            else
                            {
                                FusionIndex++;
                            }
                        }
                    }
                }
            }
        }
        private int CalculateItemSelling()
        {
            ItemSelling = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type == 2)
                {
                    ItemSelling += ItemList[i].Amount;
                }
            }
            return ItemSelling;
        }
        private void PopUpTextChange()
        {
            if (SideMenuChoice == SideMenu.VIEW)
            {
                if (TopMenuChoice == TopMenu.ITEMS)
                {
                    PopUpText = "Discard this item?";
                }
                else if (TopMenuChoice == TopMenu.RAW)
                {
                    PopUpText = "Discard this material?";
                }
            }
            else if (SideMenuChoice == SideMenu.FUSE)
            {
                PopUpText = "Create this item?";
            }
            else if (SideMenuChoice == SideMenu.SELL)
            {
                PopUpText = "Begin selling?";
            }
        }
        private void HelpTextChange()
        {
            //HelpText[0] = "Tabs";
            //HelpText[1] = "Confirm";
            //HelpText[2] = "Cancel";
            //HelpText[3] = "Discard";
            //HelpText[4] = "Close";
            if (SideMenuChoice == SideMenu.NONE)
            {
                HelpText[0] = "-";
                HelpText[1] = "Next";
                HelpText[2] = "-";
                HelpText[3] = "-";
                HelpText[4] = "Close";
            }
            else if (SideMenuChoice == SideMenu.VIEW)
            {
                if (TopMenuChoice == TopMenu.ITEMS)
                {
                    HelpText[0] = "Tabs";
                    HelpText[1] = "Next";
                    HelpText[2] = "Cancel";
                    HelpText[3] = "Discard";
                    HelpText[4] = "Close";
                }
                else if (TopMenuChoice == TopMenu.RAW)
                {
                    HelpText[0] = "Tabs";
                    HelpText[1] = "Next";
                    HelpText[2] = "Cancel";
                    HelpText[3] = "Discard";
                    HelpText[4] = "Close";
                }
                else if (TopMenuChoice == TopMenu.KEY)
                {
                    HelpText[0] = "Tabs";
                    HelpText[1] = "Next";
                    HelpText[2] = "Cancel";
                    HelpText[3] = "-";
                    HelpText[4] = "Close";
                }
            }
            else if (SideMenuChoice == SideMenu.FUSE)
            {
                HelpText[0] = "-";
                HelpText[1] = "Next";
                HelpText[2] = "Cancel";
                HelpText[3] = "Fuse";
                HelpText[4] = "Close";
            }
            else if (SideMenuChoice == SideMenu.SELL)
            {
                HelpText[0] = "-";
                HelpText[1] = "Next";
                HelpText[2] = "Cancel";
                HelpText[3] = "Sell";
                HelpText[4] = "Close";
            }
        }
        private void AnalogInput()
        {
            //if (PreviousSecond != GameEngine.GameTime.TotalGameTime.Seconds)
            //{
            //    Seconds++;
            //    PreviousSecond = GameEngine.GameTime.TotalGameTime.Seconds;
            //}
            PreviousSecond++;
            AnalogUp = false;
            AnalogDown = false;
            AnalogLeft = false;
            AnalogRight = false;

            if (GP.LeftStickPosition.Y == 1 && CoolDown)
            {
                AnalogUpValue++;
                CoolDown = false;
            }
            else if (GP.LeftStickPosition.Y > 0.0f && PreviousSecond % 20 == 0)
            {
                AnalogUpValue++;
            }
            if (GP.LeftStickPosition.Y == -1 && CoolDown)
            {
                AnalogDownValue++;
                CoolDown = false;
            }
            else if (GP.LeftStickPosition.Y < 0.0f && PreviousSecond % 20 == 0)
            {
                AnalogDownValue++;
            }

            //if (GP.LeftStickPosition.Y < -0.5f && PreviousSecond % 10 == 6)
            //{
            //    AnalogDown = true;
            //}
            if (AnalogUpValue > 0)
            {
                AnalogUp = true;
                AnalogUpValue--;
            }
            if (AnalogDownValue > 0)
            {
                AnalogDown = true;
                AnalogDownValue--;
            }


            if (!CoolDown)
            {
                CoolDownValue++;
            }
            if (CoolDownValue >= 15 && GP.LeftStickPosition.Y != 1 && GP.LeftStickPosition.Y != -1)
            {
                CoolDown = true;
                CoolDownValue = 0;
            }
            if (PreviousSecond >= 60)
            {
                PreviousSecond = 0;
            }

        }
        private void PercentageSellThingy()
        {
            SellIndex = 0;
            SellSelected = 0;
            SellPercent = 0;

            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type == 2)
                {
                    if (ItemList[i].Amount > 0)
                    {
                        SellPercent += (int)(((float)PlayerPrice[SellIndex] / (float)SellPrice[SellIndex]) * 100.0f);
                    }
                    SellIndex++;
                }
            }
            //for (int i = 0; i < SellPrice.Length; i++)
            //{
            //    
            //    SellPercent += (int)(PlayerPrice[i] /SellPrice[i]);
            //}

            SellSelected = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type == 2)
                {
                    if (ItemList[i].Amount > 0)
                    {
                        SellSelected++; //get number of items
                    }
                }
            }
            SellPercent = (int)(((float)SellPercent / (float)(SellSelected * 100)) * 100);
            SellPercent = SellPercent * 1;
            
            PlayerState.Currently = PlayerState.State.SELL;
            SellIndex = 0;
            SellSelected = 0;
            GameEngine.Services.GetService<PlayerStats>().ResetSales();
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type == 2)
                {
                    if (ItemList[i].Amount > 0)
                    {
                        if (SellSelected == ItemSelection)
                        {
                            for (int j = 0; j < ItemList[i].Amount; j++)
                            {
                                if (random.Next(0, 110) < GameEngine.Services.GetService<PlayerStats>().GetPopularity())
                                {
                                    ItemList[i].Amount--;
                                    GameEngine.Services.GetService<PlayerStats>().MoneyChange(PlayerPrice[SellIndex]);
                                    GameEngine.Services.GetService<PlayerStats>().SetSales(PlayerPrice[SellIndex]);
                                    break;
                                }
                            }                       
                        }
                        else
                        {
                            SellSelected++;
                        }
                    }
                    SellIndex++;
                }
            }
                
            
            //pass sellpercent to andrea sell here

        }
        public int GetSellPercent()
        {
            return SellPercent;
        }
        private void SellItemSelected(int value)
        {
            SellIndex = 0;
            SellSelected = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type == 2)
                {
                    if (ItemList[i].Amount > 0)
                    {
                        if (SellSelected == ItemSelection)
                        {
                            if (SellPrice[SellIndex] * 1.5f < PlayerPrice[SellIndex] + value ||
                                SellPrice[SellIndex] * 0.5f > PlayerPrice[SellIndex] + value)
                            {
                                return;
                            }
                            else
                            {
                                PlayerPrice[SellIndex] += value;
                                return;
                            }
                        }
                        else
                        {
                            SellSelected++;
                        }
                    }
                    SellIndex++;
                }
            }
        }
        private void KeyBoardInput()
        {
            if (KB.Key_Pressed(Keys.V) || GP.Button_Pressed(Buttons.Y))
            {
                if (!Confirming)
                {
                    Reset();
                    PlayerState.Currently = PlayerState.State.NORMAL;
                }
            }

            if (SideMenuChoice != SideMenu.NONE && !Confirming)
            {
                if (KB.Key_Pressed(Keys.Left) || GP.Button_Pressed(Buttons.DPadLeft))
                {
                    if (SideMenuChoice == SideMenu.SELL)
                    {
                        SellItemSelected(-10);
                    }
                }
                if (KB.Key_Pressed(Keys.Right) || GP.Button_Pressed(Buttons.DPadRight))
                {
                    if (SideMenuChoice == SideMenu.SELL)
                    {
                        SellItemSelected(10);
                    }
                }
                if (KB.Key_Pressed(Keys.D) || GP.Button_Pressed(Buttons.RightShoulder))
                {
                    if (SideMenuChoice == SideMenu.VIEW)
                    {
                        TopMenuSelectionUp();
                        HelpTextChange();
                    }
                }
                else if (KB.Key_Pressed(Keys.S) || GP.Button_Pressed(Buttons.LeftShoulder))
                {
                    if (SideMenuChoice == SideMenu.VIEW)
                    {
                        TopMenuSelectionDown();
                        HelpTextChange();
                    }
                }
                else if (KB.Key_Pressed(Keys.Down) || GP.Button_Pressed(Buttons.DPadDown) || AnalogDown)
                {
                    ItemSelectionUp();
                }
                else if (KB.Key_Pressed(Keys.Up) || GP.Button_Pressed(Buttons.DPadUp) || AnalogUp)
                {
                    ItemSelectionDown();
                }
                else if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A))
                {
                    Counter = TextDisplay.Length;
                }
                else if (KB.Key_Pressed(Keys.X) || GP.Button_Pressed(Buttons.B))
                {
                    SideMenuChoice = SideMenu.NONE;
                    SetText();
                    HelpTextChange();

                    MaxItemChecked = false;
                    ItemSelection = 0;
                    MaxItemSelection = 0;
                }
                else if (KB.Key_Pressed(Keys.C) || GP.Button_Pressed(Buttons.X))
                {
                    PopUpTextChange();
                    if (SideMenuChoice == SideMenu.VIEW && MaxItemSelection > 0)
                    {
                        if (TopMenuChoice == TopMenu.ITEMS)
                        {
                            if (!Confirming)
                            {
                                Confirming = true;
                            }
                        }
                        else if (TopMenuChoice == TopMenu.RAW)
                        {
                            if (!Confirming)
                            {
                                Confirming = true;
                            }
                        }
                    }
                    else if (SideMenuChoice == SideMenu.FUSE && MaxItemSelection > 0)
                    {
                        if (!Confirming)
                        {
                            Confirming = true;
                        }
                    }
                    else if (SideMenuChoice == SideMenu.SELL && MaxItemSelection > 0)
                    {
                        if (!Confirming)
                        {
                            Confirming = true;
                        }
                    }
                }
            }
            else if (Confirming)        //delete item or fuse item
            {
                if (KB.Key_Pressed(Keys.Left) || KB.Key_Pressed(Keys.Right) || GP.Button_Pressed(Buttons.DPadLeft) || GP.Button_Pressed(Buttons.DPadRight))
                {
                    ConfirmSelection = !ConfirmSelection;
                }
                else if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A))
                {
                    if (SideMenuChoice == SideMenu.VIEW)
                    {
                        if (ConfirmSelection)
                        {
                            if (TopMenuChoice == TopMenu.ITEMS)
                            {
                                GameEngine.Services.GetService<Inventory>().DeleteItem(ViewID);
                            }
                            else if (TopMenuChoice == TopMenu.RAW)
                            {
                                GameEngine.Services.GetService<Inventory>().DeleteMaterials(ViewID, 1);
                            }
                            SetText();
                            MaxItemChecked = false;
                            MaxItemSelection = 0;

                            Confirming = false;
                        }
                        else
                        {
                            ConfirmSelection = true;
                            Confirming = false;
                        }
                    }
                    else if (SideMenuChoice == SideMenu.FUSE)
                    {
                        if (ConfirmSelection)
                        {
                            FuseItem();
                            SetText();
                            MaxItemChecked = false;
                            MaxItemSelection = 0;

                            Confirming = false;
                        }
                        else
                        {
                            ConfirmSelection = true;
                            Confirming = false;
                        }
                    }
                    else if (SideMenuChoice == SideMenu.SELL)
                    {
                        if (ConfirmSelection)
                        {
                            if (CalculateItemSelling() < 5)
                            {
                                GameEngine.Services.GetService<PlayerEvent>().Lackitem();
                            }
                            else
                            {
                                if (SceneControl.NowAt == SceneControl.SceneArea.SAVEPOINT)
                                {
                                    PercentageSellThingy();
                                }
                                else
                                {
                                    //cannot sell becoz not at house
                                    GameEngine.Services.GetService<PlayerEvent>().NoHouse();
                                    //return;
                                }//sell function

                                //Cutscene.Initialize_Scene(20, 0);
                            }
                            MaxItemChecked = false;
                            MaxItemSelection = 0;

                            Confirming = false;
                        }
                        else
                        {
                            ConfirmSelection = true;
                            Confirming = false;
                        }
                    }
                }
            }
            else                       //Side menu select
            {
                if (KB.Key_Pressed(Keys.Down) || GP.Button_Pressed(Buttons.DPadDown))
                {
                    SideMenuSelectionUp();
                }
                else if (KB.Key_Pressed(Keys.Up) || GP.Button_Pressed(Buttons.DPadUp))
                {
                    SideMenuSelectionDown();
                }
                else if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A))
                {
                    SideMenuSelected();
                    HelpTextChange();
                }

            }

        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (ItemSelection == -1)
            {
                ItemSelection = 0;
                SetText();
                return;
            }
            DisplayText();
            AnalogInput();

            if (!GameEngine.Services.GetService<AlertBox>().Visible)
            {
                KeyBoardInput();
            }
            if (!MaxItemChecked)
            {
                Checker();
            }
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        private void DisplayText()
        {
            Delay++;
            if (Delay >= DelayLimit)
            {
                if (Counter < TextDisplay.Length)
                {
                    Counter++;
                }
                Delay = 0;
            }
        }
        private void SetEmptyText()
        {
            if (SideMenuChoice == SideMenu.NONE)
            {
                Counter = 0;
                TextDisplay = "Select a page";
            }
            else if (SideMenuChoice == SideMenu.SELL)
            {
                Counter = 0;
                TextDisplay = "You have nothing to sell...";
            }
            else if (SideMenuChoice == SideMenu.FUSE)
            {
                Counter = 0;
                TextDisplay = "You have insufficent materials to fuse.";
            }
            else if (SideMenuChoice == SideMenu.VIEW)
            {
                if (TopMenuChoice == TopMenu.RAW)
                {
                    Counter = 0;
                    TextDisplay = "You have no raw materials.";
                }
                if (TopMenuChoice == TopMenu.ITEMS)
                {
                    Counter = 0;
                    TextDisplay = "You have no items";
                }
            }
        }
        public void DelayValueChange(int temp)
        {
            DelayLimit = temp;
        }
        private void SetText()
        {
            //for (int i = 0; i < ItemList.Count; i++)
            //{
            //    if (RealItemSelection == i)
            //    {
            //        Counter = 0;
            //        TextDisplay = ItemList[i].Description;
            //       return;
            //    }
            //}
            FusionIndex = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (SideMenuChoice == SideMenu.NONE)
                {
                    SetEmptyText();
                }
                else if (SideMenuChoice == SideMenu.SELL)
                {
                    if (ItemList[i].Type == 2 && ItemList[i].Amount > 0)
                    {
                        if (FusionIndex == ItemSelection)
                        {
                            Counter = 0;
                            TextDisplay = ItemList[i].Description;
                            return;
                        }
                        else
                        {
                            FusionIndex++;
                        }
                    }
                }
                else if (SideMenuChoice == SideMenu.FUSE)
                {
                    for (int j = 0; j < FusionList.Count; j++)
                    {
                        if (ItemList[i].ItemName == FusionList[j].ItemName)
                        {
                            if (FusionList[j].Paper <= Invt.GetAmount("Paper") && FusionList[j].Plastic <= Invt.GetAmount("Plastic")
                            && FusionList[j].Metal <= Invt.GetAmount("Metal") && FusionList[j].Glass <= Invt.GetAmount("Glass"))
                            {
                                if (FusionIndex == ItemSelection)
                                {
                                    Counter = 0;
                                    TextDisplay = ItemList[i].Description;
                                    return;
                                }
                                else
                                {
                                    FusionIndex++;
                                }
                            }
                        }
                    }
                    //if (ItemList[i].Type == 1)
                    //{
                    //    if (ItemList[i].Index == ItemSelection)
                    //    {
                    //        Counter = 0;
                    //        TextDisplay = ItemList[i].Description;
                    //        return;
                    //    }
                    //}
                }
                else if (SideMenuChoice == SideMenu.VIEW)
                {
                    if (TopMenuChoice == TopMenu.RAW)
                    {
                        if (ItemList[i].Type == 1)
                        {
                            if (ItemList[i].Index == ItemSelection)
                            {
                                ViewID = ItemList[i].ID;
                                Counter = 0;
                                TextDisplay = ItemList[i].Description;
                                return;
                            }
                        }
                    }
                    else if (TopMenuChoice == TopMenu.ITEMS)
                    {
                        if (ItemList[i].Type == 2 || ItemList[i].Type == 3)
                        {
                            if (ItemList[i].Index == ItemSelection)
                            {
                                ViewID = ItemList[i].ID;
                                Counter = 0;
                                TextDisplay = ItemList[i].Description;
                                return;
                            }
                        }
                    }
                    else if (TopMenuChoice == TopMenu.KEY)
                    {
                        if (ItemList[i].Type == 4)
                        {
                            if (ItemList[i].Index == ItemSelection)
                            {
                                Counter = 0;
                                TextDisplay = ItemList[i].Description;
                                return;
                            }
                        }
                    }
                }
            }
        }
        private void Checker()
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (SideMenuChoice == SideMenu.NONE)
                {

                }
                else if (SideMenuChoice == SideMenu.SELL)
                {
                    if (ItemList[i].Type == 2)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            if (!MaxItemChecked)
                            {
                                MaxItemSelection++;
                            }
                        }
                    }
                }
                else if (SideMenuChoice == SideMenu.FUSE)
                {
                    for (int j = 0; j < FusionList.Count; j++)
                    {
                        if (FusionList[j].Paper <= Invt.GetAmount("Paper") && FusionList[j].Plastic <= Invt.GetAmount("Plastic")
                            && FusionList[j].Metal <= Invt.GetAmount("Metal") && FusionList[j].Glass <= Invt.GetAmount("Glass"))
                        {
                            if (!MaxItemChecked)
                            {
                                MaxItemSelection++;
                            }
                        }
                    }
                    if (MaxItemSelection == 0 && !MaxItemChecked)
                    {
                        SetEmptyText();
                    }
                    if (!MaxItemChecked)
                    {
                        if (ItemSelection + 1 > MaxItemSelection && ItemSelection != 0)
                        {
                            ItemSelection = MaxItemSelection - 1;
                            SetText();
                        }
                        MaxItemChecked = true;
                    }
                }
                else if (TopMenuChoice == TopMenu.RAW)
                {
                    if (ItemList[i].Type == 1)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            if (!MaxItemChecked)
                            {
                                MaxItemSelection++;
                            }
                        }
                    }
                }
                else if (TopMenuChoice == TopMenu.ITEMS)
                {
                    if (ItemList[i].Type == 2 || ItemList[i].Type == 3)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            if (!MaxItemChecked)
                            {
                                MaxItemSelection++;
                            }
                        }
                    }
                }
                else if (TopMenuChoice == TopMenu.KEY)
                {
                    if (ItemList[i].Type == 4)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            if (!MaxItemChecked)
                            {
                                MaxItemSelection++;
                            }
                        }
                    }
                }
            }
            if (MaxItemSelection == 0 && !MaxItemChecked)
            {
                SetEmptyText();
            }
            if (!MaxItemChecked)
            {
                if (ItemSelection + 1 > MaxItemSelection && ItemSelection != 0)
                {
                    ItemSelection = MaxItemSelection - 1;
                    SetText();
                }
                MaxItemChecked = true;
            }
        }
        private void ViewSelection()
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (TopMenuChoice == TopMenu.RAW)
                {
                    if (ItemList[i].Type == 1)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            //Picture
                            GameEngine.SpriteBatch.Draw(ItemTexture[i], new Vector2(ItemPosition.X, ItemPosition.Y + ItemList[i].Index * 25.0f), ItemRectangle, Color.White,
                                   0.0f, ItemOrigin, 1.0f, SpriteEffects.None, 0f);

                            //Name
                            GameEngine.SpriteBatch.DrawString(WriteText, ItemList[i].ItemName, new Vector2(ItemPosition.X + 20.0f, ItemPosition.Y + ItemList[i].Index * 25.0f), Color.White,
                                    0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);

                            //Amount
                            GameEngine.SpriteBatch.DrawString(WriteText, "X " + ItemList[i].Amount.ToString(), new Vector2(ItemPosition.X + 200.0f, ItemPosition.Y + ItemList[i].Index * 25.0f), Color.White,
                                    0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                        }
                    }
                }
                else if (TopMenuChoice == TopMenu.ITEMS)
                {
                    if (ItemList[i].Type == 2 || ItemList[i].Type == 3)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            //Picture
                            GameEngine.SpriteBatch.Draw(ItemTexture[i], new Vector2(ItemPosition.X, ItemPosition.Y + ItemList[i].Index * 25.0f), ItemRectangle, Color.White,
                                   0.0f, ItemOrigin, 1.0f, SpriteEffects.None, 0f);

                            //Name
                            GameEngine.SpriteBatch.DrawString(WriteText, ItemList[i].ItemName, new Vector2(ItemPosition.X + 20.0f, ItemPosition.Y + ItemList[i].Index * 25.0f), Color.White,
                                    0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);

                            //Amount
                            GameEngine.SpriteBatch.DrawString(WriteText, "X " + ItemList[i].Amount.ToString(), new Vector2(ItemPosition.X + 200.0f, ItemPosition.Y + ItemList[i].Index * 25.0f), Color.White,
                                    0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                        }
                    }
                }
                else if (TopMenuChoice == TopMenu.KEY)
                {
                    if (ItemList[i].Type == 4)
                    {
                        if (ItemList[i].Amount > 0)
                        {
                            //Picture
                            GameEngine.SpriteBatch.Draw(ItemTexture[i], new Vector2(ItemPosition.X, ItemPosition.Y + ItemList[i].Index * 25.0f), ItemRectangle, Color.White,
                                   0.0f, ItemOrigin, 1.0f, SpriteEffects.None, 0f);

                            //Name
                            GameEngine.SpriteBatch.DrawString(WriteText, ItemList[i].ItemName, new Vector2(ItemPosition.X + 20.0f, ItemPosition.Y + ItemList[i].Index * 25.0f), Color.White,
                                    0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);

                            //Amount
                            GameEngine.SpriteBatch.DrawString(WriteText, "X " + ItemList[i].Amount.ToString(), new Vector2(ItemPosition.X + 200.0f, ItemPosition.Y + ItemList[i].Index * 25.0f), Color.White,
                                    0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                        }
                    }
                }
            }
        }
        private void FuseSelection()
        {
            FusionIndex = 0;
            for (int i = 0; i < FusionList.Count; i++)
            {
                if (FusionList[i].Paper <= Invt.GetAmount("Paper") && FusionList[i].Plastic <= Invt.GetAmount("Plastic")
                    && FusionList[i].Metal <= Invt.GetAmount("Metal") && FusionList[i].Glass <= Invt.GetAmount("Glass"))
                {
                    //Picture
                    GameEngine.SpriteBatch.Draw(ItemTexture[i], new Vector2(ItemPosition.X, ItemPosition.Y + FusionIndex * 25.0f), ItemRectangle, Color.White,
                           0.0f, ItemOrigin, 1.0f, SpriteEffects.None, 0f);

                    //Name
                    GameEngine.SpriteBatch.DrawString(WriteText, FusionList[i].ItemName, new Vector2(ItemPosition.X + 20.0f, ItemPosition.Y + FusionIndex * 25.0f), Color.White,
                            0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);

                    FusionIndex++;
                }
            }
        }
        private void SellSelection()
        {
            FusionIndex = 0;
            SellIndex = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Type == 2)
                {
                    if (ItemList[i].Amount > 0)
                    {
                        //Picture
                        GameEngine.SpriteBatch.Draw(ItemTexture[i], new Vector2(ItemPosition.X, ItemPosition.Y + FusionIndex * 25.0f), ItemRectangle, Color.White,
                               0.0f, ItemOrigin, 1.0f, SpriteEffects.None, 0f);

                        //Name
                        GameEngine.SpriteBatch.DrawString(WriteText, ItemList[i].ItemName, new Vector2(ItemPosition.X + 20.0f, ItemPosition.Y + FusionIndex * 25.0f), Color.White,
                                0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);

                        //Price
                        GameEngine.SpriteBatch.DrawString(WriteText, PlayerPrice[SellIndex].ToString(), new Vector2(ItemPosition.X + 100.0f, ItemPosition.Y + FusionIndex * 25.0f), Color.White,
                                 0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                        FusionIndex++;
                    }
                    SellIndex++;
                }
            }
        }
        public bool GetConfirming()
        {
            return Confirming;
        }
        private void RawMaterials()
        {
            //Raw materials
            for (int i = 0; i < 4; i++)
            {
                //Picture
                GameEngine.SpriteBatch.Draw(MiniRawTexture[i], new Vector2(MiniRawPosition.X + (i * 120.0f), MiniRawPosition.Y), MiniRawRectangle, Color.White,
                              0.0f, MiniRawOrigin, 1.0f, SpriteEffects.None, 0f);

                //Amount
                GameEngine.SpriteBatch.DrawString(WriteText, Invt.GetAmount(i + 1).ToString(), new Vector2(MiniRawPosition.X + (i * 120.0f) + 30.0f, MiniRawPosition.Y), Color.White,
                        0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);

            }
        }
        private void SideMenuDis()
        {
            //Side Menu
            for (int i = 0; i < 3; i++)
            {
                GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    if (SideMenuChoice == SideMenu.VIEW && i == 0 || SideMenuSelection == 1 && i == 0 ||
                    SideMenuChoice == SideMenu.FUSE && i == 1 || SideMenuSelection == 2 && i == 1 ||
                    SideMenuChoice == SideMenu.SELL && i == 2 || SideMenuSelection == 3 && i == 2)
                    {
                        myEffect.Parameters["Intensity"].SetValue(1.5f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[0].Begin();
                    }
                    else
                    {
                        myEffect.Parameters["Intensity"].SetValue(0.5f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[0].Begin();
                    }
                    //BG
                    GameEngine.SpriteBatch.Draw(PurpleBackGroundSide, new Vector2(SideMenuPosition.X, SideMenuPosition.Y + (i * 80.0f)), SideMenuRectangle, Color.White,
                          0.0f, SideMenuOrigin, 1.0f, SpriteEffects.None, 0f);

                    //Picture
                    GameEngine.SpriteBatch.Draw(SideMenuTexture[i], new Vector2(SideMenuPosition.X, SideMenuPosition.Y + (i * 80.0f)), SideMenuRectangle, Color.White,
                          0.0f, SideMenuOrigin, 1.0f, SpriteEffects.None, 0f);
                
                    myEffect.CurrentTechnique.Passes[0].End();
                    myEffect.End();
                    

                }
                GameEngine.SpriteBatch.End();
            }
        }
        private void TopMenuDis()
        {
            if (SideMenuChoice == SideMenu.VIEW)
            {
                //Top Menu
                for (int i = 0; i < 3; i++)
                {
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        if (TopMenuChoice == TopMenu.ITEMS && i == 0 ||
                            TopMenuChoice == TopMenu.RAW && i == 1 ||
                            TopMenuChoice == TopMenu.KEY && i == 2)
                        {
                            myEffect.Parameters["Intensity"].SetValue(1.5f);
                            myEffect.Begin();
                            myEffect.CurrentTechnique.Passes[0].Begin();
                        }
                        else
                        {
                            myEffect.Parameters["Intensity"].SetValue(0.5f);
                            myEffect.Begin();
                            myEffect.CurrentTechnique.Passes[0].Begin();
                        }
                        //BG
                        GameEngine.SpriteBatch.Draw(PurpleBackGroundTop, new Vector2(TopMenuPosition.X + (i * 120.0f), TopMenuPosition.Y), TopMenuRectangle, Color.White,
                                  0.0f, TopMenuOrigin, 1.0f, SpriteEffects.None, 0f);

                        //Picture
                        GameEngine.SpriteBatch.Draw(TopMenuTexture[i], new Vector2(TopMenuPosition.X + (i * 120.0f), TopMenuPosition.Y), TopMenuRectangle, Color.White,
                                  0.0f, TopMenuOrigin, 1.0f, SpriteEffects.None, 0f);

                        myEffect.CurrentTechnique.Passes[0].End();
                        myEffect.End();

                    }
                    GameEngine.SpriteBatch.End();
                }
            }
        }
        private void ConfirmingThingy()
        {
            if (Confirming)
            {
                if (ConfirmSelection)
                {
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        myEffect.Parameters["Intensity"].SetValue(1.5f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[0].Begin();
                        //Yes
                        GameEngine.SpriteBatch.Draw(Yes, new Vector2(YesPosition.X, YesPosition.Y + ItemSelection * 25.0f), ConfirmRectangle, Color.White,
                               0.0f, ConfirmOrigin, 1.5f, SpriteEffects.None, 0f);

                        myEffect.CurrentTechnique.Passes[0].End();
                        myEffect.End();
                    }
                    GameEngine.SpriteBatch.End();
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        //No
                        GameEngine.SpriteBatch.Draw(No, new Vector2(NoPosition.X, NoPosition.Y + ItemSelection * 25.0f), ConfirmRectangle, Color.White,
                           0.0f, ConfirmOrigin, 1.0f, SpriteEffects.None, 0f);

                    }
                    GameEngine.SpriteBatch.End();
                }
                else if (!ConfirmSelection)
                {
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        myEffect.Parameters["Intensity"].SetValue(1.5f);
                        myEffect.Begin();
                        myEffect.CurrentTechnique.Passes[0].Begin();

                        //No
                        GameEngine.SpriteBatch.Draw(No, new Vector2(NoPosition.X, NoPosition.Y + ItemSelection * 25.0f), ConfirmRectangle, Color.White,
                           0.0f, ConfirmOrigin, 1.5f, SpriteEffects.None, 0f);


                        myEffect.CurrentTechnique.Passes[0].End();
                        myEffect.End();
                    }
                    GameEngine.SpriteBatch.End();
                    GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                    {
                        //Yes
                        GameEngine.SpriteBatch.Draw(Yes, new Vector2(YesPosition.X, YesPosition.Y + ItemSelection * 25.0f), ConfirmRectangle, Color.White,
                               0.0f, ConfirmOrigin, 1.0f, SpriteEffects.None, 0f);
                    }
                    GameEngine.SpriteBatch.End();
                }
            }
        }
        public override void Draw()
        {    
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //Background
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, Color.White,
                           0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);

                //Dialogue Background
                GameEngine.SpriteBatch.Draw(DialogueBackGround, DialogueBackGroundPosition, DialogueBackGroundRectangle, new Color(Color.Black, 0.7f),
                           0.0f, DialogueBackGroundOrigin, DialogueBackGroundScale, SpriteEffects.None, 0f);

                //Dialogue
                GameEngine.SpriteBatch.DrawString(WriteText, TextDisplay.Substring(0, Counter), DialoguePosition, Color.White,
                        0.0f, new Vector2(0.0f, 3.0f), 1.0f, SpriteEffects.None, 0.0f);

                //Raw Materials
                RawMaterials();

                if (MaxItemSelection > 0)
                {
                    //Selector
                    GameEngine.SpriteBatch.Draw(Selector, new Vector2(SelectorPosition.X, SelectorPosition.Y + ItemSelection * 25.0f), SelectorRectangle, Color.White,
                           0.0f, SelectorOrigin, 1.0f, SpriteEffects.None, 0f);
                }
                if (Confirming)
                {
                    if (SideMenuChoice == SideMenu.SELL)
                    {
                        //Confirm Background
                        GameEngine.SpriteBatch.Draw(ConfirmBackGround, new Vector2(ConfirmBackGroundPosition.X, ConfirmBackGroundPosition.Y + ItemSelection * 25.0f), ConfirmBackGroundRectangle, Color.White,
                               0.0f, ConfirmBackGroundOrigin, ConfirmBackGroundScale, SpriteEffects.None, 0f);

                        //Confirm Text
                        GameEngine.SpriteBatch.DrawString(WriteText, PopUpText, new Vector2(PopUpPosition.X, PopUpPosition.Y + ItemSelection * 25.0f), new Color(Color.Black, 0.8f),
                                0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                      
                    }
                    else
                    {
                        //Confirm Background
                        GameEngine.SpriteBatch.Draw(ConfirmBackGround, new Vector2(ConfirmBackGroundPosition.X, ConfirmBackGroundPosition.Y + ItemSelection * 25.0f), ConfirmBackGroundRectangle, Color.White,
                               0.0f, ConfirmBackGroundOrigin, ConfirmBackGroundScale, SpriteEffects.None, 0f);

                        //Confirm Text
                        GameEngine.SpriteBatch.DrawString(WriteText, PopUpText, new Vector2(PopUpPosition.X, PopUpPosition.Y + ItemSelection * 25.0f), new Color(Color.Black, 0.8f),
                                0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                      
                    }
                }
                //View
                if (SideMenuChoice == SideMenu.VIEW)
                {
                    ViewSelection();
                }
                //Fuse
                else if (SideMenuChoice == SideMenu.FUSE)
                {
                    FuseSelection();
                }
                //Sell
                else if (SideMenuChoice == SideMenu.SELL)
                {
                    SellSelection();
                }

                //LR - Tabs
                GameEngine.SpriteBatch.Draw(LRButton, ButtonPosition, ButtonRectangle, Color.White,
                       0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                //AButton - confirm
                GameEngine.SpriteBatch.Draw(AButton, new Vector2(ButtonPosition.X, ButtonPosition.Y + 25.0f), ButtonRectangle, Color.White,
                       0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                //BButton - cancel
                GameEngine.SpriteBatch.Draw(BButton, new Vector2(ButtonPosition.X, ButtonPosition.Y + 50.0f), ButtonRectangle, Color.White,
                       0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                //XButton - discard
                GameEngine.SpriteBatch.Draw(XButton, new Vector2(ButtonPosition.X, ButtonPosition.Y + 75.0f), ButtonRectangle, Color.White,
                       0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                //YButton - close
                GameEngine.SpriteBatch.Draw(YButton, new Vector2(ButtonPosition.X, ButtonPosition.Y + 100.0f), ButtonRectangle, Color.White,
                       0.0f, ButtonOrigin, 1.0f, SpriteEffects.None, 0f);

                //Help stuff
                //Dialogue
                for (int i = 0; i < HelpText.Length; i++)
                {
                    GameEngine.SpriteBatch.DrawString(WriteText, HelpText[i], new Vector2(ButtonPosition.X + 20.0f, ButtonPosition.Y + i * 25.0f), new Color(Color.White, 0.8f),
                            0.0f, new Vector2(0.0f, 12.0f), 1.0f, SpriteEffects.None, 0.0f);
                }

            }
            GameEngine.SpriteBatch.End();
            //Side Menu
            SideMenuDis();

            //Top Menu
            TopMenuDis();

            //Yes No
            ConfirmingThingy();

        }
    }
}