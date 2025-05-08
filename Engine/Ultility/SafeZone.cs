using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public class SafeZone : Component, I2DComponent
    {
        Texture2D safeZoneTexture;
        Texture2D failZoneTexture;
        KeyboardDevice KB;
        public Rectangle Rectangle { get; set; }

        static readonly Dictionary<float, Rectangle> prevValues = new Dictionary<float, Rectangle>();

        public SafeZone()
            : base()
        {
            BootUp();
        }

        public SafeZone(GameScreen Parent)
            : base(Parent)
        {
            BootUp();
        }

        public static Rectangle GetTitleSafeArea(GraphicsDevice device, float percent)
        {
            Rectangle retval;

            if (prevValues.TryGetValue(percent, out retval))
                return retval;

            retval = new Rectangle(
                device.Viewport.X,
                device.Viewport.Y,
                device.Viewport.Width,
                device.Viewport.Height);

            float border = (1 - percent) / 2;

            retval.X = (int)(border * retval.Width);
            retval.Y = (int)(border * retval.Height);
            retval.Width = (int)(percent * retval.Width);
            retval.Height = (int)(percent * retval.Height);

            prevValues.Add(percent, retval);

            return retval;
        }

        void BootUp()
        {
            int screenWidth = GameEngine.GraphicDevice.Viewport.Width;
            int screenHeight = GameEngine.GraphicDevice.Viewport.Height;

            safeZoneTexture = new Texture2D(
                GameEngine.GraphicDevice,
                screenWidth,
                screenHeight,
                0,
                TextureUsage.None,
                SurfaceFormat.Color);
            failZoneTexture = new Texture2D(
                GameEngine.GraphicDevice,
                screenWidth,
                screenHeight,
                0,
                TextureUsage.None,
                SurfaceFormat.Color);

            Color[] safePixels = new Color[screenWidth * screenHeight];
            Color[] failPixels = new Color[screenWidth * screenHeight];

            Rectangle safe = SafeZone.GetTitleSafeArea(GameEngine.GraphicDevice, .8f);
            Rectangle fail = SafeZone.GetTitleSafeArea(GameEngine.GraphicDevice, .9f);

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    int i = y * screenWidth + x;

                    safePixels[i] = (safe.Contains(x, y))?
                       Color.TransparentWhite       : (fail.Contains(x, y))?
                       new Color(255, 255, 0, 150)  : Color.TransparentWhite;
                    failPixels[i] = (fail.Contains(x, y))?
                        Color.TransparentWhite : new Color(255, 0, 0, 150);
                }
            }

            safeZoneTexture.SetData(safePixels);
            failZoneTexture.SetData(failPixels);
        }

        public override void Update()
        {
            if(KB == null)
                KB = GameEngine.Services.GetService<KeyboardDevice>();
            if (KB.Key_Pressed(Keys.F2))
                Visible = !Visible;
            base.Update();
        }
        public override void Draw()
        {
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            GameEngine.SpriteBatch.Draw(safeZoneTexture, Vector2.Zero, Color.White);
            GameEngine.SpriteBatch.Draw(failZoneTexture, Vector2.Zero, Color.White);
            GameEngine.SpriteBatch.End();
            base.Draw();
        }
    }
}
