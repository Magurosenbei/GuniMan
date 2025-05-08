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
    public class ConsoleMenu : Component, I2DComponent
    {
        public Rectangle Rectangle { get; set; }
  

        ////////////////////
        /// Console State
        ////////////////////
        protected enum ConsoleState { NORMAL, LOGIN, PASSWORD, ADMIN }
        ConsoleState CommandState = ConsoleState.NORMAL;


        ///////////////////////////
        /// Command List Of Doom!!!
        //////////////////////////
        private List<CommandDef> CommandList;
        private List<AdminDef> AdminList;

        /////////////////
        /// KeyBoard
        ////////////////
        KeyboardDevice KB = GameEngine.Services.GetService<KeyboardDevice>();
 
        ////////////////
        /// Font
        ////////////////
        SpriteFont WriteText;

        ///////////////////
        /// Display Text 
        ///////////////////
        private string[] TextBox;
        private string InputText;

        ////////////////
        /// Textures
        ////////////////
        Texture2D BackGround;
        Texture2D Line;


        //////////////////////////
        /// Variables for sprites
        /////////////////////////
        private Vector2 LinePosition;
        private Rectangle LineRectangle;
        private Vector2 LineOrigin;
        private Vector2 LineScale;

        private Vector2 BackGroundPosition;
        private Rectangle BackGroundRectangle;
        private Vector2 BackGroundOrigin;
        private Vector2 BackGroundScale;

        private Vector2 OtherBackGroundPosition;
        private Vector2 OtherBackGroundScale;

        private Vector2 DialoguePosition;
        private Vector2 DialogueOrigin;

        /////////////////////////
        /// Variables
        /////////////////////////
        private bool Login;
        private bool DownAction;
        private bool UpAction;
        private bool TextAppear;
        private bool CapsOn;

        private int Index;
        private int NumOfTries;
        private bool Done;
        private int PreviousSecond;

        private float MoveLimit;
        private float MoveValue;

        private float OtherBGFadeLimit;
        private float OtherBGFadeValue;


        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public ConsoleMenu(SpriteFont spritefont, GameScreen Parent)
            : base(Parent)
        {
            //System.Enum.GetValues(GetType(Keys));
 
            WriteText = spritefont;
            InputText = "";
            Login = false;
            UpAction = DownAction = false;
            TextAppear = false;
            CapsOn = false;
            Index = 0;
            NumOfTries = 3;

            AdminList = GameEngine.Content.Load<List<AdminDef>>(@"Content/Commands/XMLAdminList");
            CommandList = GameEngine.Content.Load<List<CommandDef>>(@"Content/Commands/XMLCommandList");
            BackGround = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background");
            Line = GameEngine.Content.Load<Texture2D>("Content/Backgrounds/Background2");

            BackGroundRectangle = new Rectangle(0, 0, BackGround.Width, BackGround.Height);
            BackGroundOrigin = new Vector2(BackGround.Width * 0.5f, BackGround.Height * 0.5f);
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.7f));
            BackGroundPosition = new Vector2(Resizer.GetWithoutBorderWidth(0.5f), (BackGroundScale.Y * BackGroundOrigin.X) * -1.0f);

            LineRectangle = new Rectangle(0, 0, Line.Width, Line.Height);
            LineOrigin = new Vector2(0, 0);
            LineScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.001f));
            LinePosition = new Vector2(0,0);

            Done = false;
            PreviousSecond = 0;

            MoveLimit = Resizer.GetHeight(0.5f);
            MoveValue = 5.0f;

            OtherBGFadeLimit = 0.5f;
            OtherBGFadeValue = 0.0f;

            OtherBackGroundPosition = new Vector2(Resizer.GetWidth(0.5f),Resizer.GetHeight(0.5f));
            OtherBackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));

            DialoguePosition = new Vector2(BackGroundPosition.X - BackGroundOrigin.X * BackGroundScale.X, BackGroundPosition.Y + BackGroundOrigin.Y * BackGroundScale.Y);
            DialogueOrigin = new Vector2(0, 0);

            TextBox = new string[20];
            
            StartUp();
            
        }
        private void StartUp()
        {
            //Note
            //TextBox[20] is top
            //TextBox[0]  is bottom 
            for (int i = 0; i < TextBox.Length; i++)
            {
                TextBox[i] = "";
            }
            TextBox[0] = "";
            TextBox[1] = "---------------------------------------------------------";
            TextBox[2] = "Type 'exit' to quit console anytime.";
            TextBox[3] = "Welcome to GuniMan Console Menu.";
            TextBox[4] = "";
            TextBox[5] = "CommandList Version 1.0.0.1";
            TextBox[6] = "Admin List Loaded"; 
            TextBox[7] = "GuniMan Console Version 23.0.1.15467";
            TextBox[8] = "---------------------------------------------------------";   
        }
        public void ScreenUpdate()
        {
            BackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.7f));
            //BackGroundPosition = new Vector2(Resizer.GetWithoutBorderWidth(0.5f), (BackGroundScale.Y * BackGroundOrigin.X) * -1.0f);

            LineScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 0.001f));
            LinePosition = DialoguePosition = new Vector2(BackGroundPosition.X - BackGroundOrigin.X * BackGroundScale.X, BackGroundPosition.Y + BackGroundOrigin.Y * BackGroundScale.Y - 20.0f);

            OtherBackGroundPosition = new Vector2(Resizer.GetWidth(0.5f), Resizer.GetHeight(0.5f));
            OtherBackGroundScale = new Vector2(((Resizer.GetRealWidth(1.0f) / (float)BackGround.Width) * 1.0f), ((Resizer.GetRealHeight(1.0f) / BackGround.Height) * 1.0f));
        }
        public void Set()
        {
            string pw = "lol123";
            CharEnumerator charEnum = pw.GetEnumerator();
            int temp = Convert.ToInt32(pw[4]);
            Debug.Write(temp + "\n");
        }
        private int GetNumber(string temp)
        {
            //Number
            // 1 - 49
            // 2 - 50CharEnumerator charEnum = temp.GetEnumerator();

            int num = Convert.ToInt32(temp[0]);
            num -= 48;
            return num;
        }
        private void Push(string text)
        {
            for (int i = TextBox.Length - 1; i > -1; i--)
            {
                if (i == 0)
                {
                    TextBox[i] = text;
                }
                else
                {
                    TextBox[i] = TextBox[i - 1];
                }
            }
        }
        private void FlashingUnderScore()
        {
            if (PreviousSecond != GameEngine.GameTime.TotalGameTime.Seconds)
            {
                Done = true;
                PreviousSecond = GameEngine.GameTime.TotalGameTime.Seconds;
            }
            if (Done)
            {
                //add
                if (InputText.Length == 0)
                {
                    InputText += "|";
                    Done = false;
                }
                
                else if (InputText.Length > 0)
                {
                    //add
                    if (InputText.Substring(InputText.Length - 1, 1) != "|")
                    {
                        InputText += "|";
                        Done = false;
                    }
                    //gone
                    else if (InputText.Substring(InputText.Length - 1, 1) == "|")
                    {
                        InputText = InputText.Substring(0, InputText.Length - 1);
                        Done = false;
                    }
                }
 
            }
        }
        private void CommandRun(string input)
        {
            for (int i = 0; i < CommandList.Count; i++)
            {
                if (input.StartsWith(CommandList[i].Command))
                {
                    //exit
                    if (CommandList[i].ID == 0)
                    {
                        if (CommandList[i].Command == input)
                        {
                            //quit
                            TextAppear = false;
                            UpAction = true;
                        }
                        return;
                    }
                    if (CommandList[i].ID == 10)
                    {
                        if (CommandList[i].Command == input)
                        {
                            //{ BAG = 0, BAGHORN = 1, TRUCK = 2, TRUCKHORN = 3 }
                            Push("Enter a number after '" + CommandList[i].Command.ToString() + "'");
                            Push("( BAG = 0, BAGHORN = 1, TRUCK = 2, TRUCKHORN = 3)");
                        }
                        else if (CommandList[i].Command.Length + 1 == input.Length)
                        {
                            PlayerState.Equipedwith = (PlayerState.Player_Eq)(GetNumber(input.Substring(input.Length - 1, 1)));
                            GameEngine.Services.GetService<Guniman>().ChangeEquipment(GameEngine.Services.GetService<IEContentManager>());
                        }
                        else
                        {
                            Push("Error");
                        }
                        return;
                    }
                }
            }
        }
        private void CommandCheck(string input)
        {
            if (!Login)
            {
                for(int i = 0;i < CommandList.Count;i++)
                {
                    if(input.StartsWith(CommandList[i].Command))
                    {
                        //exit
                        if (CommandList[i].ID == 0)
                        {
                            if (CommandList[i].Command == input)
                            {
                                //quit
                                TextAppear = false;
                                UpAction = true;
                            }
                            return;
                        }
                        //login
                        if (CommandList[i].ID == 1)
                        {
                            if (CommandList[i].Command == input)
                            {
                                Login = true;
                                CommandState = ConsoleState.LOGIN;
                                Push(InputText);
                                Push("*Please enter your username*");
                                return;
                            }
                        }
                        else
                        {
                            Push("You do not have access rights.");
                            Push("Please type 'login' to login to admin account");
                            return;
                        }
                    }
                }  
                Push("'" + input + "' is an invalid command");
                return;
                
            }
            else if (CommandState == ConsoleState.LOGIN)
            {
                for (int i = 0; i < AdminList.Count; i++)
                {
                    if (AdminList[i].Name == input)
                    {
                        Index = i;
                        CommandState = ConsoleState.PASSWORD;
                        Push("-" + input + " Confirmed-");
                        Push("");
                        Push("*Please Enter your Password*");
                        NumOfTries = 3;
                        return;
                    }
                }
                if(NumOfTries > 1)
                {
                    NumOfTries--; 
                    Push("Username is not found.  (Tries: " + NumOfTries + ")");
                    Push("");
                }
                else
                {
                    Push("You have exceeded the maximum tries");
                    NumOfTries = 3;
                    CommandState = ConsoleState.NORMAL;
                    Login = false;
                }
            }
            else if (CommandState == ConsoleState.PASSWORD)
            {
                if (AdminList[Index].Pass == input)
                {
                    CommandState = ConsoleState.ADMIN;
                    Push("-Password Confirmed-");
                    Push("");
                    Push("You are now logged as " + AdminList[Index].Name);
                    NumOfTries = 3;
                    return;
                }
                else if(NumOfTries > 1)
                {
                    NumOfTries--; 
                    Push("Password is wrong.  (Tries: " + NumOfTries + ")");
                    Push("");
                }
                else
                {
                    Push("You have exceeded the maximum tries");
                    NumOfTries = 3;
                    CommandState = ConsoleState.NORMAL;
                    Login = false;
                }

            }
            else
            {
                for (int i = 0; i < CommandList.Count; i++)
                {
                    if (input.StartsWith(CommandList[i].Command))
                    {
                        CommandRun(input);
                        return;
                    }
                }
                Push("'" + input + "' is an invalid command");
            }
        }
        public void Reset()
        {
            InputText = "";
            StartUp();
            Visible = false;
            
        }
        private void MoveDown()
        {
            if ( (BackGroundPosition.Y + (BackGroundOrigin.Y * BackGroundScale.Y) ) < MoveLimit)
            {
                BackGroundPosition = new Vector2(BackGroundPosition.X, BackGroundPosition.Y + MoveValue);
                LinePosition = DialoguePosition = new Vector2(BackGroundPosition.X - BackGroundOrigin.X * BackGroundScale.X, BackGroundPosition.Y + BackGroundOrigin.Y * BackGroundScale.Y - 20.0f);
                if (OtherBGFadeValue < OtherBGFadeLimit)
                {
                    OtherBGFadeValue += 0.02f;
                }
            }
            else
            {
                DownAction = false;
                TextAppear = true;
            }
        }
        private void MoveUp()
        {
            if ((BackGroundPosition.Y + (BackGroundOrigin.Y * BackGroundScale.Y)) > ((BackGroundScale.Y * BackGroundOrigin.X) * -1.0f))
            {
                BackGroundPosition = new Vector2(BackGroundPosition.X, BackGroundPosition.Y - MoveValue);
                LinePosition = DialoguePosition = new Vector2(BackGroundPosition.X - BackGroundOrigin.X * BackGroundScale.X, BackGroundPosition.Y + BackGroundOrigin.Y * BackGroundScale.Y - 20.0f);
                
                if (OtherBGFadeValue > 0)
                {
                    OtherBGFadeValue -= 0.02f;
                }
            }
            else
            {
                UpAction = false;
                Reset();
            }
        }
        private void AddText(string input)
        {
            if (InputText.Length > 0)
            {
                if (InputText.Substring(InputText.Length - 1, 1) == "|")
                {
                    InputText = InputText.Substring(0, InputText.Length - 1);
                    InputText += input;
                    //InputText += "|";
                }
                else
                {
                    InputText += input;
                }
            }
            
        }
        private void KeyBoardInput()
        {
            CapsOn = false;
            if (KB.Key_Pressed(Keys.OemTilde) && !DownAction)
            {
                TextAppear = false;
                UpAction = true;
            }
            if (KB.Key_Held(Keys.LeftShift) || KB.Key_Held(Keys.RightShift)) 
            {
                CapsOn = true;
            }
            
            foreach (Keys Current in Keyboard.GetState().GetPressedKeys())
            {
                if(KB.Key_Pressed(Current) && (int)(Current) >= 65 && (int)(Current) <= 90 ||
                   KB.Key_Pressed(Current) && (int)(Current) >= 48 && (int)(Current) <= 57) 
                {
                    if ((int)(Current) >= 48 && (int)(Current) <= 57)
                    {
                        AddText(((char)((int)Current)).ToString());
                    }
                    else if (CapsOn)
                    {
                        AddText(((char)((int)Current)).ToString());
                    }
                    else
                    {
                        AddText(((char)((int)Current + 32)).ToString());
                    }
                }
            }
            if(KB.Key_Pressed(Keys.Space))
            {
                AddText(" ");
            }
            if(KB.Key_Pressed(Keys.Back))
            {
                if (InputText.Length > 0)
                {
                    if (InputText.Substring(InputText.Length - 1, 1) == "|")
                    {
                        if (InputText.Length > 1)
                        {
                            InputText = InputText.Substring(0, InputText.Length - 1);
                            InputText = InputText.Substring(0, InputText.Length - 1);
                            InputText += "|";
                        }
                    }
                    else if (InputText.Substring(0, 1) != "|")
                    {
                        InputText = InputText.Substring(0, InputText.Length - 1);
                        //InputText = InputText.Substring(0, InputText.Length - 1);
                        //InputText += "|";
                    }
                }
                
            }
            if (KB.Key_Pressed(Keys.Enter))
            {
                if (InputText.Length > 0)
                {
                    if (InputText.Substring(InputText.Length - 1, 1) == "|")
                    {
                        InputText = InputText.Substring(0, InputText.Length - 1);
                    }
                    if (InputText != "")
                    {
                        CommandCheck(InputText);
                        InputText = "";
                    }
                }
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }
            if (DownAction)
            {
                MoveDown();
            }
            if (UpAction)
            {
                MoveUp();
            }
            if (Visible)
            {
                if (!DownAction && !UpAction && !TextAppear )
                {
                    DownAction = true;
                }
            }
            if (TextAppear)
            {
                KeyBoardInput();
                FlashingUnderScore();
            }
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
                //OtherBG
                GameEngine.SpriteBatch.Draw(BackGround, OtherBackGroundPosition, BackGroundRectangle, new Color(Color.White, OtherBGFadeValue),
                           0.0f, BackGroundOrigin, OtherBackGroundScale, SpriteEffects.None, 0f);
                
                //BG
                GameEngine.SpriteBatch.Draw(BackGround, BackGroundPosition, BackGroundRectangle, new Color(Color.White, 0.8f),
                           0.0f, BackGroundOrigin, BackGroundScale, SpriteEffects.None, 0f);

                if (TextAppear)
                {
                    //Line
                    GameEngine.SpriteBatch.Draw(Line, LinePosition, LineRectangle, Color.White,
                          0.0f, LineOrigin, LineScale, SpriteEffects.None, 0f);

                    //Text
                    GameEngine.SpriteBatch.DrawString(WriteText, InputText, DialoguePosition, new Color(Color.White, 1.0f),
                          0.0f, DialogueOrigin, 1.0f, SpriteEffects.None, 0.0f);

                    //Text Box
                    for (int i = 0; i < TextBox.Length; i++)
                    {
                        GameEngine.SpriteBatch.DrawString(WriteText, TextBox[i], new Vector2(DialoguePosition.X, DialoguePosition.Y + ((i+1) * -20.0f) ), new Color(Color.White, 1.0f),
                          0.0f, DialogueOrigin, 1.0f, SpriteEffects.None, 0.0f);

                    }
                }
            
            }
            GameEngine.SpriteBatch.End();
        }
    }
}
