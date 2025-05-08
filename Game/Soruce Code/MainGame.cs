using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;

using Engine;

using XmlContentExtension;


namespace Game
{
    public enum APP_STATE { MENU, LOAD, UNLOAD, NORMAL, WORLD, MAP }

    public class MainGame : Microsoft.Xna.Framework.Game
    {
        public static SaveGameStore SaveFile = new Game.SaveGameStore();
        /* Devices */
        KeyboardDevice KB;
        GamepadDevice GPD;
        GraphicsDeviceManager graphics;
        GameScreen HUD;

        List<World3D> Maps;

        Thread LoadThread = new Thread(delegate() { return; });

        static public APP_STATE APP_STATE = APP_STATE.NORMAL;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false; // or false;   
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = true;
            graphics.PreferredDepthStencilFormat = SelectStencilMode();
            this.IsFixedTimeStep = true;
            this.Window.Title = "The GuniMan";
            graphics.ApplyChanges();

            GameEngine.BootUpEngine(graphics);
            GameEngine.Content.PreserveAsset = false;   // save memory
            GameEngine.Content.UseDefaultLoad = true;
            GameEngine.GraphicDevice.RenderState.MultiSampleAntiAlias = true;
            GameEngine.GraphicDevice.RenderState.AlphaTestEnable = true;

            Maps = new List<World3D>();
            Maps.Add(new World3D("CenterMap"));
            //Maps.Add(new World3D("Map0"));
            //Maps.Add(new World3D("Map1"));
            //Maps.Add(new World3D("Map2"));
            //Maps.Add(new World3D("Map3"));
            HUD = new GameScreen("HUD");
            base.Initialize();
        }
        private static DepthFormat SelectStencilMode()
        {
            // Check stencil formats
            GraphicsAdapter adapter = GraphicsAdapter.DefaultAdapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil8))
                return DepthFormat.Depth24Stencil8;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil8Single))
                return DepthFormat.Depth24Stencil8Single;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil4))
                return DepthFormat.Depth24Stencil4;
            else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth15Stencil1))
                return DepthFormat.Depth15Stencil1;
            else
                throw new InvalidOperationException("Could Not Find Stencil Buffer for Default Adapter");
        }
        protected override void LoadContent()
        {
            GameEngine.Services.AddService(typeof(KeyboardDevice), KB = new KeyboardDevice());
            GameEngine.Services.AddService(typeof(MouseDevice), new MouseDevice());
            GameEngine.Services.AddService(typeof(GamepadDevice), GPD = new GamepadDevice(0));
            GameEngine.Services.AddService(typeof(GameScreenResizer), new GameScreenResizer());
            GameEngine.Services.AddService(typeof(ParticleOverlord), new ParticleOverlord(HUD));
            GameEngine.Services.AddService(typeof(AudioGame), new AudioGame(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(AudioComponent), new AudioComponent());
            GameEngine.Services.AddService(typeof(SoundManager), new SoundManager());
            GameEngine.Services.AddService(typeof(Physics), new Physics());
            GameEngine.Services.AddService(typeof(Economy), new Economy());
            GameEngine.Services.AddService(typeof(PlayerStats), new PlayerStats());

            ////////////////////////
            //Events && Inventory
            /////////////////////////

            GameEngine.Services.AddService(typeof(Inventory), new Inventory());
            GameEngine.Services.AddService(typeof(PlayerEvent), new PlayerEvent());
            GameEngine.Services.AddService(typeof(AlertBox), new AlertBox(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));

            ///////////////////////
            //Stuff to draw first
            //////////////////////     
            GameEngine.Services.AddService(typeof(RadialBlur), new RadialBlur(GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height, HUD));
            GameEngine.Services.AddService(typeof(GaussianBlur), new GaussianBlur(GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height, HUD));
            GameEngine.Services.AddService(typeof(OldTv), new OldTv(GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height, HUD));
            GameEngine.Services.AddService(typeof(TransitionScreen), new TransitionScreen(HUD));
            GameEngine.Services.AddService(typeof(FrameRateCounter), new FrameRateCounter(HUD));
            //GameEngine.Services.AddService(typeof(Clock), new Clock(HUD));
            GameEngine.Services.AddService(typeof(SafeZone), new SafeZone(HUD));
            GameEngine.Services.GetService<SafeZone>().Visible = false;
            GameEngine.Services.AddService(typeof(AchievementEngine), new AchievementEngine(HUD));
            GameEngine.Services.AddService(typeof(Interactive), new Interactive(HUD));
            GameEngine.Services.AddService(typeof(Winlose), new Winlose(HUD));

            GameEngine.BackgroundScreen.Visible = false;

            GameEngine.Services.AddService(typeof(ConsoleMenu), new ConsoleMenu(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(Prologue), new Prologue(Content.Load<SpriteFont>("Content/Fonts/Comic"), "Garang Guni", HUD));
            GameEngine.Services.AddService(typeof(MainMenu), new MainMenu(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(LoadingScreen), new LoadingScreen(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));

            GameEngine.Services.AddService(typeof(BuyingStuff), new BuyingStuff(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(DialogueEngine), new DialogueEngine(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(InventoryDisplay), new InventoryDisplay(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));

            //GameEngine.Services.AddService(typeof(StoreRoomMenu), new StoreRoomMenu(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(InGameMainMenu), new InGameMainMenu(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            //GameEngine.Services.AddService(typeof(GuniUI), new GuniUI(Content.Load<SpriteFont>("Content/Fonts/Comic"),HUD));


            GameEngine.Services.AddService(typeof(WorldMap), new WorldMap());

            GameEngine.Services.AddService(typeof(HealthHud), new HealthHud(Content.Load<SpriteFont>("Content/Fonts/Comic"), HUD));
            GameEngine.Services.AddService(typeof(LiftSelection), new LiftSelection(HUD));

            ///////////////////////
            //Stuff to run last
            //////////////////////
            //Checkers & must run at the very back stuff
            GameEngine.Services.AddService(typeof(AchievementOverlord), new AchievementOverlord());

            VariableAsset.ReadAssets("SomeXMLFile");

            
            MainGame.APP_STATE = APP_STATE.MENU;
            //Maps[0].LoadContents();
            Maps[0].Visible = false;
            GameEngine.Content.ClearDustbin();
        }
        protected override void UnloadContent()
        {
            Maps.Clear();
            GameEngine.Content.Unload();
            GameEngine.Content.Dispose();
        }
        protected override void Update(GameTime gameTime)
        {
            GameEngine.Update(gameTime);
            if (APP_STATE == APP_STATE.MENU)
            {
                switch (GameEngine.Services.GetService<MainMenu>().GetState())
                {
                    case 0:
                        SaveFile.LoadNew();
                        GameEngine.Services.GetService<PlayerStats>().LoadFromFileMoney(SaveFile.Data.Money);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(1, SaveFile.Data.Achievement1);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(2, SaveFile.Data.Achievement2);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(3, SaveFile.Data.Achievement3);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(4, SaveFile.Data.Achievement4);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(5, SaveFile.Data.Achievement5);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(6, SaveFile.Data.Achievement6);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(7, SaveFile.Data.Achievement7);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(8, SaveFile.Data.Achievement8);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(9, SaveFile.Data.Achievement9);
                        PlayerState.Equipedwith = (PlayerState.Player_Eq)SaveFile.Data.Equipment;

                        MainGame.APP_STATE = APP_STATE.LOAD;
                        GameEngine.Services.GetService<TransitionScreen>().StartFading(2, 3);
                        //GameEngine.Services.GetService<MainMenu>().Visible = false;
                        //GameEngine.Services.GetService<LoadingScreen>().Visible = true;
                        LoadThread = new Thread(delegate()
                        {
#if XBOX

                    t.SetProcessorAffinity(new[] { 5 });

#endif

                            if (Maps[0].Visible) return;
                            Maps[0].LoadContents();
                            Maps[0].Initialize();
                            Maps[0].Visible = true;
                        });
                        LoadThread.Start(); 
                        break;
                    case 1:
                        
                        SaveFile.Load();
                        GameEngine.Services.GetService<PlayerStats>().LoadFromFileMoney(SaveFile.Data.Money);
                        GameEngine.Services.GetService<AchievementEngine>().AchievementsSet(1, SaveFile.Data.Achievement1);
                        PlayerState.Equipedwith = (PlayerState.Player_Eq)SaveFile.Data.Equipment;


                        MainGame.APP_STATE = APP_STATE.LOAD;
                        GameEngine.Services.GetService<TransitionScreen>().StartFading(2, 3);
                        //GameEngine.Services.GetService<MainMenu>().Visible = false;
                        //GameEngine.Services.GetService<LoadingScreen>().Visible = true;
                        LoadThread = new Thread(delegate()
                        {
#if XBOX

                    t.SetProcessorAffinity(new[] { 5 });

#endif

                            if (Maps[0].Visible) return;
                            Maps[0].LoadContents();
                            Maps[0].Initialize();
                            Maps[0].Visible = true;
                        });
                        LoadThread.Start(); 
                        break;
                    case 2:
                        GameEngine.Services.GetService<MainMenu>().Reset();
                        GameEngine.Services.GetService<InGameMainMenu>().OutsideAchievements();
                        break;
                    case 3:
                        GameEngine.Services.GetService<MainMenu>().Reset();
                        GameEngine.Services.GetService<InGameMainMenu>().OutsideOptions();
                        break;
                    case 4:
                        if (LoadThread != null)
                            LoadThread.Abort();
                        this.Exit();
                        break;
                }
            }

            base.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (LoadThread != null)
                    LoadThread.Abort();
                this.Exit();
            }
            if (KB.Key_Pressed(Keys.F12))
            {
                if (SceneControl.RenderMode == SceneControl.Rendering.NORMAL)
                    SceneControl.RenderMode = SceneControl.Rendering.WIREFRAME;
                else
                    SceneControl.RenderMode = SceneControl.Rendering.NORMAL;
            }
            if (KB.Key_Pressed(Keys.F11))
                SceneControl.RenderMode = SceneControl.Rendering.SHADOW;
 
            if (MainGame.APP_STATE == APP_STATE.LOAD && LoadThread != null)
            {
                if (!LoadThread.IsAlive)
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Normal;
                    LoadThread.Abort();
                    LoadThread = null;
                    GameEngine.Services.GetService<LoadingScreen>().Reset();
                    GameEngine.Services.GetService<LoadingScreen>().Visible = false;
                    GameEngine.Services.GetService<SoundManager>().ChangeSong("normal");
                    MainGame.APP_STATE = APP_STATE.MAP;
                    GameEngine.Services.GetService<HealthHud>().Visible = true;
                }
            }
            if (MainGame.APP_STATE == APP_STATE.UNLOAD)
            {
                //GameEngine.Services.GetService<TransitionScreen>().StartFading(4, 2); 
                //if(GameEngine.Services.GetService<TransitionScreen>().
                Maps[0].Disable();
                GameEngine.Services.GetService<Physics>().UpdatePhysics = false;
                GameEngine.Services.GetService<Physics>().PhysicsSystem.CollisionSystem = new JigLibX.Collision.CollisionSystemSAP();
                GameEngine.Services.GetService<Physics>().PhysicsSystem.CollisionSystem.UseSweepTests = false;
                Debug.Write("\n Garbage : " + GC.GetTotalMemory(false).ToString());
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                Debug.Write("\n Garbage : " + GC.GetTotalMemory(true).ToString());
                Maps[0].Visible = false;
                GameEngine.Services.GetService<HealthHud>().Visible = false;

                //GameEngine.Services.GetService<MainMenu>().Visible = true;
                MainGame.APP_STATE = APP_STATE.MENU;
            }
            // Coin Script
            if (KB.Key_Pressed(Keys.F1))
            {
                bool on = GameEngine.Services.GetService<FrameRateCounter>().Visible;
                GameEngine.Services.GetService<FrameRateCounter>().Visible = !on;
            }
            if (KB.Key_Pressed(Keys.F3))
                GameEngine.Services.GetService<Physics>().UpdatePhysics = !GameEngine.Services.GetService<Physics>().UpdatePhysics;

        }
        protected override void Draw(GameTime gameTime)
        {
            GameEngine.Draw(gameTime, ComponentType.All);
            base.Draw(gameTime);
        }
    }
}
