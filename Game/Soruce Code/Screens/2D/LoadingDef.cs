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

namespace Game
{
    public class LoadingDef
    {
        private string Text;                // Text
        private int Index;                  // Index  
        private Vector2 Position;           // Position 

        public LoadingDef(string text, Vector2 position, int index)
        {
            Text = text;
            Position = position;
            Index = index;
        }
        public void SetText(string text)
        {
            Text = text;
        }
        public string GetText()
        {
            return Text;
        }
        public void GoUpOneLine()
        {
            Index++;
            Position = new Vector2(Position.X, Position.Y - 20.0f);
        }
        public void PositionCheck(float width, float height)
        {
            Position = new Vector2(width, height - 20.0f * (Index+1) );
        }
        public Vector2 GetPosition()
        {
            return Position;
        }
       

    }
}
