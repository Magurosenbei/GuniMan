using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public class ClearScreen : Component
    {
        public override void Draw()
        {
            GameEngine.GraphicDevice.Clear(Color.Black);
        }
    }
}
