using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Engine;

namespace Game
{
    public class AnimationFrame
    {
        List<Rectangle> Frames = new List<Rectangle>();
        float frameLength = 1.0f / 5.0f;
        float timer = 0;
        int currentFrame = 0;

        public int FPS
        {
            get { return (int)(1.0f / frameLength); }
            set { frameLength = 1.0f / (float)value; }
        }
        public Rectangle CurrentFrame
        {
            get { return Frames[currentFrame]; }
        }

        public AnimationFrame(int width, int height, int numFrames, int Xoffset, int Yoffset)
        {
            for (int i = 0; i < numFrames; i++)
                Frames.Add(new Rectangle());
            int framewidth = width / numFrames;
            for (int i = 0; i < numFrames; i++)
            {
                Frames[i] = new Rectangle(Xoffset + (framewidth * i),
                                            Yoffset, framewidth, height);
            }
        }
        public void Update(GameTime dt)
        {
            timer += (float)dt.ElapsedRealTime.TotalSeconds;
            if (timer < frameLength) return;
            timer = 0;
            currentFrame = (currentFrame + 1) % Frames.Count;
        }
        public void Reset()
        {
            currentFrame = 0;
            timer = 0;
        }
    }

    public class AnimatingSprite
    {
        public Texture2D Image;
        public Dictionary<string, AnimationFrame> Anime = new Dictionary<string, AnimationFrame>();
        bool updateAnime = true;

        string NowAnimating;

        public AnimatingSprite(string FileName)
        {
            Image = GameEngine.Content.Load<Texture2D>(FileName);
        }
        public string currentAnimation
        {
            get { return NowAnimating; }
            set
            {
                if (!Anime.ContainsKey(value))
                    throw new Exception("Invaild anime");
                if (NowAnimating == null || !NowAnimating.Equals(value))
                {
                    NowAnimating = value;
                    Anime[NowAnimating].Reset();
                }

            }
        }
        public void Play() { updateAnime = true; }
        public void Stop() { updateAnime = false; }

        public void Update(GameTime dt)
        {
            if (!updateAnime) return;
            if (currentAnimation == null)
            {
                if (Anime.Keys.Count == 0) return;
                string[] keys = new string[Anime.Keys.Count];
                Anime.Keys.CopyTo(keys, 0);
                currentAnimation = keys[0];
            }
            Anime[currentAnimation].Update(dt);
        }
        public void Draw(ref SpriteBatch sprBatch, Vector2 Position, float rotation)
        {
            Rectangle Pos = Anime[currentAnimation].CurrentFrame;
            Pos.X = (int)Position.X;
            Pos.Y = (int)Position.Y;
            sprBatch.Draw(Image, Pos, Anime[currentAnimation].CurrentFrame, Color.White, rotation,
                        new Vector2(Anime[currentAnimation].CurrentFrame.Width / 2, Anime[currentAnimation].CurrentFrame.Height / 2), SpriteEffects.None, 0);
        }
    }
}
