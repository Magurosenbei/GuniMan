using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

namespace Game
{
    public class WorldMap : Component
    {
        List<Texture2D> Places = new List<Texture2D>();
        List<Vector2> PlacePos = new List<Vector2>();
        //Texture2D Background;
        Vector2 CurrentlyAt = Vector2.Zero;
        KeyboardDevice KB;

        protected override void InitializeComponent(GameScreen Parent)
        {
            /*Background = GameEngine.Content.Load<Texture2D>("Content/Textures/World");
            for (int i = 0; i < 4; i++)
            {
                Vector2 Pos;
                Pos.X = (float)Math.Sin((float)Math.PI * 0.5f * i);
                Pos.Y = (float)Math.Cos((float)Math.PI * 0.5f * i);
                Places.Add(GameEngine.Content.Load<Texture2D>("Content/Textures/Place" + i.ToString()));
                PlacePos.Add(Pos);
            }*/
            KB = GameEngine.Services.GetService<KeyboardDevice>();
            base.InitializeComponent(Parent);
        }

        public override void Update()
        {
            if (KB.Key_Pressed(Keys.Up) && CurrentlyAt.Y < 1 && CurrentlyAt.X == 0)
                CurrentlyAt.Y++;
            if (KB.Key_Pressed(Keys.Down) && CurrentlyAt.Y > -1 && CurrentlyAt.X == 0)
                CurrentlyAt.Y--;
            if (KB.Key_Pressed(Keys.Right) && CurrentlyAt.X < 1 && CurrentlyAt.Y == 0)
                CurrentlyAt.X++;
            if (KB.Key_Pressed(Keys.Left) && CurrentlyAt.X > -1 && CurrentlyAt.Y == 0)
                CurrentlyAt.X--;

            //Debug.Write("\n Choice" + CurrentlyAt.ToString());
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
