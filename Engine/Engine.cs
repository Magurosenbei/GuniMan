using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;


using System.IO;
using System.Xml.Serialization;

namespace Engine
{
    public static class GameEngine
    {
        public static IEServiceContainer    Services;
        public static IEContentManager      Content;
        // Game Screens
        public static GameScreenCollection  GameScreens = new GameScreenCollection();
        public static GameScreen            BackgroundScreen;
        public static GameScreen            DefaultScreen;
        // Standard XNA stuff
        public static SpriteBatch           SpriteBatch;
        public static GraphicsDevice        GraphicDevice;

        public static GameTime              GameTime;
        public static bool                  IsInitialized = false;

        public static Random                RandomValue = new Random();

        public static void BootUpEngine(IGraphicsDeviceService GraphicService)
        {
            GameEngine.GraphicDevice = GraphicService.GraphicsDevice;
            GameEngine.SpriteBatch = new SpriteBatch(GraphicService.GraphicsDevice);
            GameEngine.Services = new IEServiceContainer();
            GameEngine.Services.AddService(typeof(IGraphicsDeviceService), GraphicService);
            GameEngine.Content = new IEContentManager(Services);
            
            BackgroundScreen = new GameScreen("GameEngine.BackgroundScreen");
            BackgroundScreen.OverrideUpdateBlocked = true;
            BackgroundScreen.OverrideDrawBlocked = true;
            BackgroundScreen.OverrideInputBlocked = true;
            DefaultScreen = BackgroundScreen;
            GameEngine.IsInitialized = true;
        }
        public static void Update(GameTime gameTime)
        {
            GameEngine.GameTime = gameTime;

            List<GameScreen> Updating = new List<GameScreen>();
            foreach (GameScreen gs in GameScreens)
                Updating.Add(gs);

            for (int i = GameScreens.Count - 1; i >= 0; i--)
            {
                if (!GameScreens[i].BlocksUpdate) continue;
                if (i <= 0) break;
                for (int j = i - 1; j >= 0; j--)
                    if (GameScreens[j].OverrideUpdateBlocked)
                        Updating.Remove(GameScreens[j]);

                break;
            }
            // Update all the things inside
            foreach (GameScreen gs in Updating)
                if (gs.Initialized)
                    gs.Update();
            // clear all
            Updating.Clear();
            // Add all again
            foreach (GameScreen gs in GameScreens)
                Updating.Add(gs);

            for (int i = GameScreens.Count - 1; i >= 0; i--)
            {
                if (!GameScreens[i].BlockInput) continue;
                if (i <= 0) break;
                for (int j = i - 1; j >= 0; j--)
                    if (GameScreens[j].OverrideInputBlocked)
                        Updating.Remove(GameScreens[j]);

                break;
            }
            foreach (GameScreen gs in GameScreens)
                if (!gs.InputDisabled)
                    gs.IsInputAllowed = Updating.Contains(gs);
                else
                    gs.IsInputAllowed = true;
        }

        public static void Draw(GameTime dt, ComponentType Type)
        {
            GameEngine.GameTime = dt;

            List<GameScreen> drawing = new List<GameScreen>();
            GraphicDevice.Clear(Color.Black);

            foreach (GameScreen gs in GameScreens)
                if(gs.Visible)
                    drawing.Add(gs);

            for (int i = GameScreens.Count - 1; i >= 0; i--)
            {
                if (!GameScreens[i].BlocksDraw) continue;
                if (i <= 0) break;
                for (int j = i - 1; j >= 0; j--)
                    if (GameScreens[j].OverrideDrawBlocked)
                        drawing.Remove(GameScreens[j]);

                break;
            }
            foreach (GameScreen gs in drawing)
                if (gs.Initialized)
                    gs.Draw(Type);
        }
    }
}