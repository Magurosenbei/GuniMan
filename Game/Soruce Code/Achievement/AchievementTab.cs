using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Engine;
using XmlContentExtension;

namespace Game
{
    public class AchievementTab
    {
        public Texture2D Texture2 { get; set; }        // The texture that will be drawn to represent the background
        public Texture2D Texture { get; set; }        // The texture that will be drawn to represent the icon
        public string Title { get; set; }             // The desciption of the achievement
        public string Description { get; set; }       // The desciption of the achievement
        public bool Unlocked { get; set; }            // The unlock state of the achievement
        public float Counter { get; set; }              // The counter of the achievement
        public Vector2 Position { get; set; }         // The current position of the achievement
        public SpriteFont WriteText;                  // The Font
        public SpriteFont WriteText2;                 // The other Font

        public bool Animation;                        // The animation of the tabs
        public bool AnimationIn;
        public bool AnimationOut;
        public float AnimateValue;                    // The value of the tabs to move
        public float AnimateLimit;                    // The value of the tabs to move
        public float AnimateCounter;
        public int Seconds;                           // The duration of the tab
        public int PreviousSecond;                    // Store Previous seconds


        Rectangle sourceRectangle;
        Rectangle sourceRectangle2;
        Vector2 origin2;
        Vector2 origin;
        Vector2 origin3;
        Vector2 origin4;

        Vector2 Position2;
        Vector2 Position3;
        Vector2 Position4;

        /////////////////////
        // Resizer
        /////////////////////
        GameScreenResizer Resizer = GameEngine.Services.GetService<GameScreenResizer>();

        public AchievementTab(Texture2D texture, Texture2D texture2, SpriteFont spritefont, SpriteFont spritefont2, 
            string title, string description, float count,
            float animatevalue, int seconds)
        {
            WriteText = spritefont;
            WriteText2 = spritefont2;
            Texture = texture;
            Texture2 = texture2;
            Position = new Vector2(Resizer.GetWidth(1.0f) + ((float)Texture2.Width *0.5f), Resizer.GetHeight(0.9f) - ((float)Texture2.Height * 0.5f));
            Title = title;
            Description = description;
            Counter = count;
            Unlocked = false;

            Animation = false;
            AnimationIn = false;
            AnimationOut = false;
            AnimateValue = animatevalue;
            AnimateLimit = (float)Texture2.Width;
            AnimateCounter = 0;
            Seconds = seconds + 2;
            PreviousSecond = 0;


            sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            sourceRectangle2 = new Rectangle(0, 0, Texture2.Width, Texture2.Height);

            origin2 = new Vector2(Texture2.Width / 2, Texture2.Height / 2);
            origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            origin3 = new Vector2(Title.Length / 2, Texture.Height / 2);
            origin4 = new Vector2(Description.Length / 2, Texture.Height / 2);

            Position2 = new Vector2(Position.X - Texture2.Width / 2 + Texture.Width / 2 + 5.0f, Position.Y);
            Position3 = new Vector2(Position2.X + Texture.Width / 2 + 5.0f, Position.Y + 10.0f);
            Position4 = new Vector2(Position3.X, Position.Y + 20.0f);
        }
        public void ScreenUpdate()
        {
            Position = new Vector2(Resizer.GetWidth(1.0f) + ((float)Texture2.Width * 0.5f), Resizer.GetHeight(0.9f) - ((float)Texture2.Height * 0.5f));
            Position2 = new Vector2(Position.X - Texture2.Width / 2 + Texture.Width / 2 + 5.0f, Position.Y);
            Position3 = new Vector2(Position2.X + Texture.Width / 2 + 5.0f, Position.Y + 10.0f);
            Position4 = new Vector2(Position3.X, Position.Y + 20.0f);
        }
        public void Reset()
        {
            Unlocked = false;
        }
        public void Set(bool temp)
        {
            Unlocked = temp;
        }
        public int Update()
        {
            if (!Animation)
            {
                return 0;
            }
            if (Animation && AnimateCounter == 0)
            {
                AnimationIn = true;
            }
            if (AnimationIn)
            {
                if (AnimateCounter < AnimateLimit)
                {
                    Position = new Vector2(Position.X - AnimateValue, Position.Y);
                    Position2 = new Vector2(Position.X - Texture2.Width / 2 + Texture.Width / 2 + 5.0f, Position.Y);
                    Position3 = new Vector2(Position2.X + Texture.Width / 2 + 5.0f, Position.Y + 10.0f);
                    Position4 = new Vector2(Position3.X, Position.Y + 20.0f);
                    AnimateCounter += AnimateValue;
                }
                if (AnimateCounter >= AnimateLimit)
                {
                    if (PreviousSecond != GameEngine.GameTime.TotalGameTime.Seconds)
                    {
                        Seconds--;
                        PreviousSecond = GameEngine.GameTime.TotalGameTime.Seconds;
                    }
                    if (Seconds == 0)
                    {
                        AnimationIn = false;
                        AnimationOut = true;
                    }
                }
            }
            if (AnimationOut)
            {
                Position = new Vector2(Position.X + AnimateValue, Position.Y);
                Position2 = new Vector2(Position.X - Texture2.Width / 2 + Texture.Width / 2 + 5.0f, Position.Y);
                Position3 = new Vector2(Position2.X + Texture.Width / 2 + 5.0f, Position.Y + 10.0f);
                Position4 = new Vector2(Position3.X, Position.Y + 20.0f);
                AnimateCounter -= AnimateValue;
                
                if (AnimateCounter <= 0)
                {
                    AnimationOut = false;
                    Animation = false;
                    return -1;
                }
            }
            return 1;
        }
        public void Draw2(Vector2 temp)
        {
            Vector2 Position21 = new Vector2(temp.X - Texture2.Width / 2 + Texture.Width / 2 + 5.0f, temp.Y);
            Vector2 Position31 = new Vector2(Position21.X + Texture.Width / 2 + 5.0f, temp.Y + 10.0f);
            Vector2 Position41 = new Vector2(Position31.X, temp.Y + 20.0f);
            
            //background
            GameEngine.SpriteBatch.Draw(Texture2, temp, sourceRectangle2, Color.White,
                0.0f, origin2, 1.0f, SpriteEffects.None, 0f);
            //icon
            GameEngine.SpriteBatch.Draw(Texture, Position21, sourceRectangle, Color.White,
                0.0f, origin, 1.0f, SpriteEffects.None, 0f);
            //title
            GameEngine.SpriteBatch.DrawString(WriteText2, Title, Position31, Color.Navy,
                0.0f, origin3, 1.0f, SpriteEffects.None, 0.0f);
            //desciption
            GameEngine.SpriteBatch.DrawString(WriteText, Description, Position41, Color.White,
                0.0f, origin4, 0.7f, SpriteEffects.None, 0.0f);
        }
        public void Draw()
        {
       
            //background
            GameEngine.SpriteBatch.Draw(Texture2, Position, sourceRectangle2, Color.White,
                0.0f, origin2, 1.0f, SpriteEffects.None, 0f);
            //icon
            GameEngine.SpriteBatch.Draw(Texture, Position2, sourceRectangle, Color.White,
                0.0f, origin, 1.0f, SpriteEffects.None, 0f);
            //title
            GameEngine.SpriteBatch.DrawString(WriteText2, Title, Position3, Color.Navy,
                0.0f, origin3, 1.0f, SpriteEffects.None, 0.0f);
            //desciption
            GameEngine.SpriteBatch.DrawString(WriteText, Description, Position4, Color.White,
                0.0f, origin4, 0.7f, SpriteEffects.None, 0.0f);
        }

    }
}
