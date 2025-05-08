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
    public class DialogueEngine : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }

        ////////////////////
        /// Dialogue State
        ////////////////////
        protected enum DialogueState { PLAYERBEGIN, PERSONRESPONSE, PERSONCOMMENT, PERSONDONTWANT, PLAYERNOMONEY, 
                                        PERSONCOLLIDE, PERSONSLEEP, CATMEOW, PLAYERFULL }
        DialogueState DaState = DialogueState.PLAYERBEGIN;

        protected enum CustomerState { NONE, MALELITTLE, MALEMUCH, MALEOK, FEMALELITTLE, FEMALEMUCH, FEMALEOK }
        CustomerState CustState = CustomerState.NONE;


        ////////////////
        /// Dialogues
        ////////////////
        private List<DialogueDef> PlayerGreetDialogueList;

        private List<DialogueDef> MalePlayerReplyDialogueList;
        private List<DialogueDef> MalePlayerCommentLittleDialogueList;
        private List<DialogueDef> MalePlayerCommentOkDialogueList;
        private List<DialogueDef> MalePlayerCommentMuchDialogueList;


        private List<DialogueDef> FemalePlayerReplyDialogueList;
        private List<DialogueDef> FemalePlayerCommentLittleDialogueList;
        private List<DialogueDef> FemalePlayerCommentOkDialogueList;
        private List<DialogueDef> FemalePlayerCommentMuchDialogueList;

        private List<DialogueDef> PlayerDontWantDialogueList;
        private List<DialogueDef> PlayerNoMoneyDialogueList;
        private List<DialogueDef> PlayerCollideDialogueList;

        private List<DialogueDef> CatDialogueList;
        private List<DialogueDef> PlayerFullDialogueList;
        ////////////////
        /// Font
        ////////////////
        SpriteFont WriteText;

        /////////////////
        /// Stuff 
        /////////////////
        private int Counter;                        //Counter for displaying Text
        private int Index;                          //Index to get the dialogue text from xml
        private int Delay;                          //Delay 
        private int DelayValue;                     //DelayValue
        private bool DialogueEnd;                   //Indicate dialogue has ended

        //////////////////
        ///Textures
        /////////////////
        private Texture2D RightPerson;
        private Texture2D LeftPerson;
        private Texture2D BackGround;
        private Texture2D OkButton;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
        GamepadDevice GP = GameEngine.Services.GetService<GamepadDevice>();

        //////////////////////////
        /// Variables for sprites
        /////////////////////////
        private Rectangle BGRectangle;
        private Vector2 BGOrigin;

        private Rectangle PortraitRectangle;
        private Vector2 PortraitOrigin;

        private Rectangle OkRectangle;
        private Vector2 OkOrigin;

        private Vector2 RightPortraitOriginPosition;
        private Vector2 LeftPortraitOriginPosition;

        private Vector2 RightPortraitPosition;
        private Vector2 LeftPortraitPosition;

        private Vector2 BGPosition;
        private Vector2 OkPosition;

        private Vector2 DialoguePosition;

        //////////////////////////
        /// Variables for Dialogues
        /////////////////////////
        private bool Start;
        private bool Guy;
        private bool Skipper;
        private string TextDisplay;

        /////////////////////////
        /// Power of Random
        /////////////////////////
        private Random random;

        //scaling
        private Vector2 DialogueBoxScale;
        private Vector2 PortraitScale;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public DialogueEngine(SpriteFont spritetext, GameScreen Parent)
            : base(Parent)
        {
            random = new Random(); // random is random /// lol

            Delay = 0;
            DelayValue = GameEngine.Services.GetService<PlayerStats>().GetDelayValue();
            Counter = 0;
            Index = 0;

            DialogueEnd = false;
            Skipper = false;
            TextDisplay = "";

            WriteText = spritetext;
            PlayerGreetDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLPlayerGreetingDialogue");

            MalePlayerReplyDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLMalePlayerReplyDialogue");
            MalePlayerCommentLittleDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLMalePlayerCommentLittleDialogue");
            MalePlayerCommentOkDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLMalePlayerCommentOkDialogue");
            MalePlayerCommentMuchDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLMalePlayerCommentMuchDialogue");

            FemalePlayerReplyDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLFemalePlayerReplyDialogue");
            FemalePlayerCommentLittleDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLFemalePlayerCommentLittleDialogue");
            FemalePlayerCommentOkDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLFemalePlayerCommentOkDialogue");
            FemalePlayerCommentMuchDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLFemalePlayerCommentMuchDialogue");

            PlayerDontWantDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLPlayerDontWantDialogue");
            PlayerNoMoneyDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLPlayerNotEnoughMoneyDialogue");
            PlayerCollideDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLPlayerCollideDialogue");
            PlayerFullDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLFullPlayerDialogue");
            CatDialogueList = GameEngine.Content.Load<List<DialogueDef>>(@"Content/Dialogue/XML/XMLCatDialogue");

            RightPerson = GameEngine.Content.Load<Texture2D>("Content/Dialogue/Sprites/Portrait_Guniman");
            LeftPerson = GameEngine.Content.Load<Texture2D>("Content/Dialogue/Sprites/Portrait_Guniman");
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Dialogue/Sprites/DialogueBackground");
            OkButton = GameEngine.Content.Load<Texture2D>("Content/Buttons/Small/Button/AButton");

            OkRectangle = new Rectangle(0, 0, OkButton.Width, OkButton.Height);
            OkOrigin = new Vector2(OkButton.Width * 0.5f, OkButton.Height * 0.5f);

            BGRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BGOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);

            PortraitRectangle = new Rectangle(0, 0, RightPerson.Width, RightPerson.Height);
            PortraitOrigin = new Vector2(RightPerson.Width * 0.5f, RightPerson.Height * 0.5f);

            RightPortraitOriginPosition = RightPortraitPosition = new Vector2(Resizer.GetWidth(1.0f) + PortraitOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.4f);
            LeftPortraitOriginPosition = LeftPortraitPosition = new Vector2(Resizer.GetWidth(0.0f) - PortraitOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.4f);
            BGPosition = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.8f);

            DialogueBoxScale = new Vector2(((Resizer.GetWithBorderWidth(1.0f) / (float)BackGround.Width) * 1.0f), (((Resizer.GetWithBorderHeight(1.0f) / BackGround.Height) * 0.25f)));
            PortraitScale = new Vector2(((Resizer.GetWithBorderWidth(1.0f) / (float)LeftPerson.Width) * 0.4f), (((Resizer.GetWithBorderHeight(1.0f) / LeftPerson.Height) * 0.6f)));
            OkPosition = new Vector2(BGPosition.X + BackGround.Width * 0.5f * DialogueBoxScale.X - OkOrigin.X * 1.5f, BGPosition.Y + BackGround.Height * 0.5f * DialogueBoxScale.Y - OkOrigin.Y * 1.5f);
            DialoguePosition = new Vector2(BGPosition.X - BackGround.Width * 0.4f * DialogueBoxScale.X, BGPosition.Y - BackGround.Height * 0.5f * DialogueBoxScale.Y);
        }
        public void ScreenUpdate()
        {
            RightPortraitOriginPosition = new Vector2(Resizer.GetWidth(1.0f) + PortraitOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.4f);
            LeftPortraitOriginPosition = new Vector2(Resizer.GetWidth(0.0f) - PortraitOrigin.X, GameEngine.GraphicDevice.Viewport.Height * 0.4f);
        }
        public void DelayValueChange(int value)
        {
            DelayValue = value;
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (DaState == DialogueState.CATMEOW && !Skipper || DaState == DialogueState.PERSONCOLLIDE && !Skipper)
            {
                Skipper = true;
                return;
            }
            if (Start)
            {
                DisplayText();
                if (DaState == DialogueState.PLAYERFULL)
                {
                    if (Counter == PlayerFullDialogueList[Index].Text.Length)
                    {
                        DialogueEnd = true;
                    }
                }
                if (DaState == DialogueState.CATMEOW)
                {
                    if (Counter == CatDialogueList[Index].Text.Length)
                    {
                        DialogueEnd = true;
                    }
                    if ((RightPortraitPosition - RightPortraitOriginPosition).Length() < RightPerson.Width)
                    {
                        RightPortraitPosition = new Vector2(RightPortraitPosition.X - 3.0f, RightPortraitPosition.Y);
                    }
                }
                if (DaState == DialogueState.PLAYERNOMONEY)
                {
                    if (Counter == PlayerNoMoneyDialogueList[Index].Text.Length)
                    {
                        DialogueEnd = true;
                    }
                }
                if (DaState == DialogueState.PERSONDONTWANT)
                {
                    if (Counter == PlayerDontWantDialogueList[Index].Text.Length)
                    {
                        DialogueEnd = true;
                    }
                }
                if (DaState == DialogueState.PERSONCOMMENT)
                {
                    if (CustState == CustomerState.MALELITTLE)
                    {
                        if (Counter == MalePlayerCommentLittleDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if (CustState == CustomerState.MALEOK)
                    {
                        if (Counter == MalePlayerCommentOkDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if (CustState == CustomerState.MALEMUCH)
                    {
                        if (Counter == MalePlayerCommentMuchDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if (CustState == CustomerState.FEMALELITTLE)
                    {
                        if (Counter == FemalePlayerCommentLittleDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if (CustState == CustomerState.FEMALEOK)
                    {
                        if (Counter == FemalePlayerCommentOkDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if (CustState == CustomerState.FEMALEMUCH)
                    {
                        if (Counter == FemalePlayerCommentMuchDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                }
                else if (DaState == DialogueState.PLAYERBEGIN)
                {
                    if (Counter == PlayerGreetDialogueList[Index].Text.Length)
                    {
                        DialogueEnd = true;
                    }
                    if ((LeftPortraitPosition - LeftPortraitOriginPosition).Length() < LeftPerson.Width)
                    {
                        LeftPortraitPosition = new Vector2(LeftPortraitPosition.X + 3.0f, LeftPortraitPosition.Y);
                    }
                }
                else if (DaState == DialogueState.PERSONRESPONSE)
                {
                    if (Guy)
                    {
                        if (Counter == MalePlayerReplyDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if (!Guy)
                    {
                        if (Counter == FemalePlayerReplyDialogueList[Index].Text.Length)
                        {
                            DialogueEnd = true;
                        }
                    }
                    if ((RightPortraitPosition - RightPortraitOriginPosition).Length() < RightPerson.Width)
                    {
                        RightPortraitPosition = new Vector2(RightPortraitPosition.X - 3.0f, RightPortraitPosition.Y);
                    }
                    else
                    {

                    }
                }
                else if (DaState == DialogueState.PERSONCOLLIDE)
                {
                    if (Counter == PlayerCollideDialogueList[Index].Text.Length)
                    {
                        DialogueEnd = true;
                    }
                    if ((RightPortraitPosition - RightPortraitOriginPosition).Length() < RightPerson.Width)
                    {
                        RightPortraitPosition = new Vector2(RightPortraitPosition.X - 3.0f, RightPortraitPosition.Y);
                    }

                }
            }

            KeyBoardInput();

        }
        public void GetCustState(int state)
        {
            //-2
            //collide with neko
            //-1
            //collide with humand

            //0
            //Guni greet
            //1
            //Too little
            //2
            //Okay okay
            //3
            //Too much

            //4
            //Tries overload
            //5
            //No Money

            if (state == -2)
            {
                Index = random.Next(0, CatDialogueList.Count);
            }
            else if (state == -1)
            {
                Index = random.Next(0, PlayerCollideDialogueList.Count);
            }
            else if (state == 0)
            {
                Index = random.Next(0, PlayerGreetDialogueList.Count);
            }
            else if (state == 1)
            {
                if (Guy)
                {
                    CustState = CustomerState.MALELITTLE;
                    Index = random.Next(0, MalePlayerCommentLittleDialogueList.Count);
                }
                else
                {
                    CustState = CustomerState.FEMALELITTLE;
                    Index = random.Next(0, FemalePlayerCommentLittleDialogueList.Count);
                }
            }
            else if (state == 2)
            {
                if (Guy)
                {
                    CustState = CustomerState.MALEOK;
                    Index = random.Next(0, MalePlayerCommentOkDialogueList.Count);
                }
                else
                {
                    CustState = CustomerState.FEMALEOK;
                    Index = random.Next(0, FemalePlayerCommentOkDialogueList.Count);
                }
            }
            else if (state == 3)
            {
                if (Guy)
                {
                    CustState = CustomerState.MALEMUCH;
                    Index = random.Next(0, MalePlayerCommentMuchDialogueList.Count);
                }
                else
                {
                    CustState = CustomerState.FEMALEMUCH;
                    Index = random.Next(0, FemalePlayerCommentMuchDialogueList.Count);
                }
            }
            else if (state == 4)
            {
                Index = random.Next(0, PlayerDontWantDialogueList.Count);
            }
            else if (state == 5)
            {
                Index = random.Next(0, PlayerNoMoneyDialogueList.Count);
            }
            else if (state == 6)
            {
                Index = random.Next(0, PlayerFullDialogueList.Count);
            }
        }
        public void StartConversation(string leftPerson, string rightPerson, bool guy, int state)
        {
            PlayerState.Currently = PlayerState.State.TALK;

            if (leftPerson != "-" && rightPerson != "-")
            {
                RightPerson = GameEngine.Content.Load<Texture2D>("Content/Dialogue/Sprites/" + rightPerson);
                LeftPerson = GameEngine.Content.Load<Texture2D>("Content/Dialogue/Sprites/" + leftPerson);
            }
            if (state == -1 || state == -2)
            {
                RightPortraitPosition = RightPortraitOriginPosition;
                LeftPortraitPosition = new Vector2(LeftPortraitOriginPosition.X + LeftPerson.Width, LeftPortraitOriginPosition.Y);
            }
            if (state == 0 )
            {
                RightPortraitPosition = RightPortraitOriginPosition;
                LeftPortraitPosition = LeftPortraitOriginPosition;
            }
            else
            {
                //RightPortraitPosition = new Vector2(RightPortraitOriginPosition.X - RightPerson.Width, RightPortraitOriginPosition.Y);
                //LeftPortraitPosition = new Vector2(LeftPortraitOriginPosition.X + LeftPerson.Width, LeftPortraitOriginPosition.Y);
            }
            Visible = true;
            if (state == 0)
            {
                DaState = DialogueState.PLAYERBEGIN;
            }
            else if (state == -1)
            {
                DaState = DialogueState.PERSONCOLLIDE;
            }
            else if (state == -2)
            {
                DaState = DialogueState.CATMEOW;
            }
            else if(state == 1 || state == 2 || state == 3)
            {
                DaState = DialogueState.PERSONCOMMENT;
            }
            else if (state == 4)
            {
                DaState = DialogueState.PERSONDONTWANT;
            }
            else if (state == 5)
            {
                DaState = DialogueState.PLAYERNOMONEY;
            }
            else if (state == 6)
            {
                DaState = DialogueState.PLAYERFULL;
            }
            Start = true;
            DialogueEnd = false;
            Counter = 0;
            Guy = guy;
            Delay = 0;
            GetCustState(state);

        }
        private void Reset()
        {
            if (CustState == CustomerState.FEMALEOK || CustState == CustomerState.MALEOK
                || CustState == CustomerState.FEMALEMUCH || CustState == CustomerState.MALEMUCH  
                || DaState == DialogueState.PERSONCOLLIDE || DaState == DialogueState.PERSONDONTWANT
                || DaState == DialogueState.CATMEOW  || DaState == DialogueState.PLAYERFULL)
            {
                Visible = false;
                PlayerState.Currently = PlayerState.State.NORMAL;
            }

            TextDisplay = "";
            Start = false;
            Skipper = false;

            Counter = 0;
            Index = 0;
            Delay = 0;

        }
        private void KeyBoardInput()
        {
            if (KB.Key_Pressed(Keys.Z) || GP.Button_Pressed(Buttons.A))
            {
                if (DialogueEnd)
                {
                    if (DaState == DialogueState.PERSONRESPONSE)
                    {
                        GameEngine.Services.GetService<BuyingStuff>().Visible = true;
                        GameEngine.Services.GetService<BuyingStuff>().RandomStuff(Guy);
                        Reset();
                    }
                    else if (DaState == DialogueState.PERSONCOMMENT)
                    {
                        if (CustState == CustomerState.FEMALELITTLE || CustState == CustomerState.MALELITTLE)
                        {
                            GameEngine.Services.GetService<BuyingStuff>().Visible = true;
                        }
                        Reset();
                    }
                    else if (DaState == DialogueState.PERSONCOLLIDE)
                    {
                        Reset();
                    }
                    else if (DaState == DialogueState.CATMEOW)
                    {
                        Reset();
                    }
                    else if (DaState == DialogueState.PLAYERFULL)
                    {
                        Reset();
                    }
                    else if (DaState == DialogueState.PERSONDONTWANT)
                    {
                        Reset();
                    }
                    else if (DaState == DialogueState.PLAYERNOMONEY)
                    {
                        GameEngine.Services.GetService<BuyingStuff>().Visible = true;
                        Reset();
                    }
                    else if (DaState == DialogueState.PLAYERBEGIN)
                    {
                        DaState = DialogueState.PERSONRESPONSE;
                        if (Guy)
                        {
                            Index = random.Next(0, MalePlayerReplyDialogueList.Count);
                        }
                        else
                        {
                            Index = random.Next(0, FemalePlayerReplyDialogueList.Count);
                        }
                    }
                    DialogueEnd = false;
                    Counter = 0;

                }
                else if (!DialogueEnd)
                {
                    if (DaState == DialogueState.PERSONCOMMENT)
                    {
                        if (CustState == CustomerState.MALELITTLE)
                        {
                            Counter = MalePlayerCommentLittleDialogueList[Index].Text.Length;
                        }
                        else if (CustState == CustomerState.FEMALELITTLE)
                        {
                            Counter = FemalePlayerCommentLittleDialogueList[Index].Text.Length;
                        }
                        else if (CustState == CustomerState.MALEOK)
                        {
                            Counter = MalePlayerCommentOkDialogueList[Index].Text.Length;
                        }
                        else if (CustState == CustomerState.FEMALEOK)
                        {
                            Counter = FemalePlayerCommentOkDialogueList[Index].Text.Length;
                        }
                        else if (CustState == CustomerState.MALEMUCH)
                        {
                            Counter = MalePlayerCommentMuchDialogueList[Index].Text.Length;
                        }
                        else if (CustState == CustomerState.FEMALEMUCH)
                        {
                            Counter = FemalePlayerCommentMuchDialogueList[Index].Text.Length;
                        }
                    }
                    else if (DaState == DialogueState.PERSONCOLLIDE)
                    {
                        Counter = PlayerCollideDialogueList[Index].Text.Length;
                    }
                    else if (DaState == DialogueState.PERSONDONTWANT)
                    {
                        Counter = PlayerDontWantDialogueList[Index].Text.Length;
                    }
                    else if (DaState == DialogueState.PLAYERNOMONEY)
                    {
                        Counter = PlayerNoMoneyDialogueList[Index].Text.Length;
                    }
                    else if (DaState == DialogueState.CATMEOW)
                    {
                        Counter = CatDialogueList[Index].Text.Length;
                    }
                    else if (DaState == DialogueState.PLAYERFULL)
                    {
                        Counter = PlayerFullDialogueList[Index].Text.Length;
                    }
                    else if (DaState == DialogueState.PLAYERBEGIN)
                    {
                        LeftPortraitPosition = new Vector2(LeftPortraitOriginPosition.X + LeftPerson.Width, LeftPortraitOriginPosition.Y);
                        Counter = PlayerGreetDialogueList[Index].Text.Length;
                    }
                    else if (DaState == DialogueState.PERSONRESPONSE)
                    {
                        RightPortraitPosition = new Vector2(RightPortraitOriginPosition.X - RightPerson.Width, RightPortraitOriginPosition.Y);
                        if (Guy)
                        {
                            Counter = MalePlayerReplyDialogueList[Index].Text.Length;
                        }
                        else
                        {
                            Counter = FemalePlayerReplyDialogueList[Index].Text.Length;
                        }
                    }
                }

            }

        }
        private void DisplayText()
        {
            Delay++;
            if (DaState == DialogueState.PERSONCOMMENT)
            {
                if (Delay >= DelayValue)
                {
                    if (CustState == CustomerState.MALELITTLE)
                    {
                        if (Counter < MalePlayerCommentLittleDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        TextDisplay = MalePlayerCommentLittleDialogueList[Index].Text.Substring(0, Counter);

                    }
                    if (CustState == CustomerState.MALEOK)
                    {
                        if (Counter < MalePlayerCommentOkDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        TextDisplay = MalePlayerCommentOkDialogueList[Index].Text.Substring(0, Counter);

                    }
                    if (CustState == CustomerState.MALEMUCH)
                    {
                        if (Counter < MalePlayerCommentMuchDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        TextDisplay = MalePlayerCommentMuchDialogueList[Index].Text.Substring(0, Counter);

                    } if (CustState == CustomerState.FEMALELITTLE)
                    {
                        if (Counter < FemalePlayerCommentLittleDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        TextDisplay = FemalePlayerCommentLittleDialogueList[Index].Text.Substring(0, Counter);

                    }
                    if (CustState == CustomerState.FEMALEOK)
                    {
                        if (Counter < FemalePlayerCommentOkDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        TextDisplay = FemalePlayerCommentOkDialogueList[Index].Text.Substring(0, Counter);

                    }
                    if (CustState == CustomerState.FEMALEMUCH)
                    {
                        if (Counter < FemalePlayerCommentMuchDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        TextDisplay = FemalePlayerCommentMuchDialogueList[Index].Text.Substring(0, Counter);

                    }
                    Delay = 0;
                }

            }

            else if (DaState == DialogueState.PLAYERBEGIN)
            {
                if (Delay >= DelayValue)
                {
                    if (Counter < PlayerGreetDialogueList[Index].Text.Length)
                    {
                        Counter++;
                    }
                    Delay = 0;
                }

                TextDisplay = PlayerGreetDialogueList[Index].Text.Substring(0, Counter);
            }

            else if (DaState == DialogueState.PERSONCOLLIDE)
            {
                if (Delay >= DelayValue)
                {
                    if (Counter < PlayerCollideDialogueList[Index].Text.Length)
                    {
                        Counter++;
                    }
                    Delay = 0;
                }

                TextDisplay = PlayerCollideDialogueList[Index].Text.Substring(0, Counter);
            }
            else if (DaState == DialogueState.PERSONDONTWANT)
            {
                if (Delay >= DelayValue)
                {
                    if (Counter < PlayerDontWantDialogueList[Index].Text.Length)
                    {
                        Counter++;
                    }
                    Delay = 0;
                }

                TextDisplay = PlayerDontWantDialogueList[Index].Text.Substring(0, Counter);
            }
            else if (DaState == DialogueState.PLAYERNOMONEY)
            {
                if (Delay >= DelayValue)
                {
                    if (Counter < PlayerNoMoneyDialogueList[Index].Text.Length)
                    {
                        Counter++;
                    }
                    Delay = 0;
                }

                TextDisplay = PlayerNoMoneyDialogueList[Index].Text.Substring(0, Counter);
            }
            else if (DaState == DialogueState.CATMEOW)
            {
                if (Delay >= DelayValue)
                {
                    if (Counter < CatDialogueList[Index].Text.Length)
                    {
                        Counter++;
                    }
                    Delay = 0;
                }

                TextDisplay = CatDialogueList[Index].Text.Substring(0, Counter);
            }
            else if (DaState == DialogueState.PLAYERFULL)
            {
                if (Delay >= DelayValue)
                {
                    if (Counter < PlayerFullDialogueList[Index].Text.Length)
                    {
                        Counter++;
                    }
                    Delay = 0;
                }

                TextDisplay = PlayerFullDialogueList[Index].Text.Substring(0, Counter);
            }

            else if (DaState == DialogueState.PERSONRESPONSE)
            {
                if (Guy)
                {
                    if (Delay >= DelayValue)
                    {
                        if (Counter < MalePlayerReplyDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        Delay = 0;
                    }

                    TextDisplay = MalePlayerReplyDialogueList[Index].Text.Substring(0, Counter);
                }
                else
                {
                    if (Delay >= DelayValue)
                    {
                        if (Counter < FemalePlayerReplyDialogueList[Index].Text.Length)
                        {
                            Counter++;
                        }
                        Delay = 0;
                    }

                    TextDisplay = FemalePlayerReplyDialogueList[Index].Text.Substring(0, Counter);
                }
            }
        }
        public void SetDelayValue(short temp)
        {
            DelayValue = temp;
        }
        public int GetDelayValue()
        {
            return DelayValue;
        }
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public override void Draw()
        {
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                //Right Portrait
                GameEngine.SpriteBatch.Draw(RightPerson, RightPortraitPosition, PortraitRectangle, Color.White,
                           0.0f, PortraitOrigin, 1.0f, SpriteEffects.None, 0f);

                //Left Portrait
                GameEngine.SpriteBatch.Draw(LeftPerson, LeftPortraitPosition, PortraitRectangle, Color.White,
                           0.0f, PortraitOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0f);

                //BG
                GameEngine.SpriteBatch.Draw(BackGround, BGPosition, BGRectangle, new Color(Color.White, 0.7f),
                           0.0f, BGOrigin, DialogueBoxScale, SpriteEffects.None, 0f);

                //Dialogue 
                GameEngine.SpriteBatch.DrawString(WriteText, TextDisplay, DialoguePosition, Color.White,
                    0.0f, new Vector2(0.0f, 0.0f), 1.0f, SpriteEffects.None, 0.0f);

                //ok button
                GameEngine.SpriteBatch.Draw(OkButton, OkPosition, OkRectangle, Color.White,
                           0.0f, OkOrigin, 1.0f, SpriteEffects.None, 0f);
            }
            GameEngine.SpriteBatch.End();
        }
    }
}
