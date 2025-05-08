using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
using System.Diagnostics;


namespace Game
{
    static public class SceneControl
    {
        public static Rendering RenderMode = Rendering.NORMAL;
        public static bool WireframeMode = false;
        public enum Rendering { NORMAL, WIREFRAME, SHADOW }
        public enum SceneArea { NORMAL, SAVEPOINT, BUILDING, OUTOFMAP }
        public static SceneArea NowAt = SceneArea.NORMAL;
    }
    public class World3D: GameScreen
    {
        IEContentManager ContentManager;
        public static TimeSpan  PlayTime = new TimeSpan();
        protected Camera Camera;
        protected Shadow Shadows;
        protected Humaniod_Buffer       HumanBuffer;
        protected EmoIconSystem         EmoSys;
        protected PathSystem            PathSys;
        protected PatrolCats            PatCats;
        protected SellCutScene          Cutscene;
        protected ObtainHouseCut        ObtainHouseCut;
        protected Decoration            Decos;
        protected FixedDecoration       FDeco;

        protected List <Component>      DrawingWaitList = new List<Component>();
        protected List <NewBuilding>    Buildings   = new List<NewBuilding>();
        protected SDHHandle             SDHouse;

        protected VegetationGenerator   VegeGenerator;
        protected PickableGenerator     PickableGenerator;
        protected DustbinGenerator      DustbinGenerator;

        protected BlockingWalls         Blocker;
        protected PhysicsStaticObject   FloorPlane;
        protected StaticObject          SkyDome;

        protected PlayerHouse           PHouse;
        protected Guniman               Player;

        protected float                 DisappearRange;
        protected Vector3               DisappearPoint;

        protected KeyboardDevice KB;
        protected GamepadDevice GPD;

        public World3D(string Name) : base(Name) { Visible = false; }
        public World3D() : base("World3D") { Visible = false; }

        public virtual void LoadContents()
        {
            PlayTime = new TimeSpan();
            DrawingWaitList.Capacity = 100;
            GPD = GameEngine.Services.GetService<GamepadDevice>();
            KB = GameEngine.Services.GetService<KeyboardDevice>();
            GameEngine.Services.GetService<Physics>().UpdatePhysics = false;

            Debug.Write("\nNumber of Physics Body : " + GameEngine.Services.GetService<Physics>().PhysicsSystem.Bodies.Count);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Number of Physics Body : " + GameEngine.Services.GetService<Physics>().PhysicsSystem.Bodies.Count.ToString(), 2);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine(Name + " Map", 0);
            Debug.Write("\nLoading Map : " + Name);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine(Name + " Map", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Map Camera and Shadow Defined", 0);
            Camera = new Static3rdCam(this);
            Camera.Position = new Vector3(10, 15, 10);
            Camera.View = new Vector3(0, 0, 0);
            Camera.Offset = new Vector3(20, 7, 20);
            Camera.Visible = false;
            GameEngine.Services.AddService(typeof(Camera), Camera);
            Shadows = new Shadow();
            GameEngine.Services.AddService(typeof(Shadow), Shadows);

            ContentManager = new IEContentManager(GameEngine.Services);
            ContentManager.PreserveAsset = true;
            ContentManager.UseDefaultLoad = true;
            GameEngine.Services.AddService(typeof(IEContentManager), ContentManager);

            HumanBuffer = new Humaniod_Buffer();
            HumanBuffer.LoadPlayerMDLBuffer(ContentManager);
            HumanBuffer.LoadNPCMDLBuffer(ContentManager);
            GameEngine.Services.AddService(typeof(Humaniod_Buffer), HumanBuffer);
            EmoSys = new EmoIconSystem(ContentManager);
            GameEngine.Services.AddService(typeof(EmoIconSystem), EmoSys);

            Debug.Write("\nMap Camera , Billboard and Shadow Defined");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Map Camera and Shadow Defined", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Map Plot Script Read", 0);
            XMLMapPlot Plot = ContentManager.Load<XMLMapPlot>("Content/Map/" + Name);
            Debug.Write("\nMap Plot Script Read");

            Cutscene = new SellCutScene(Plot.SellSpawn, Plot.Sell_CenterPoint, Plot.Sell_PlayerSitOffSet, Plot.Sell_RadiusOfEffect,
                                            Plot.Sell_RadiusOutBounce, Plot.Sell_StartAngle, Plot.Sell_EndAngle, Plot.Sell_IntervalAngle, this);
            Cutscene.GenerateGoalPointDir();
            ObtainHouseCut = new ObtainHouseCut(ContentManager, this);

            GameEngine.Services.AddService(typeof(ObtainHouseCut), ObtainHouseCut);
            GameEngine.Services.AddService(typeof(SellCutScene), Cutscene);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Map Plot Script Read", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Waypoint Loaded", 0);
            PathSys = new PathSystem("Content/Map/" + Name + "Waypoints", ContentManager, this, false);
            PatCats = new PatrolCats("Content/Map/" + Name + "CatWayPoints", ContentManager, this);
            GameEngine.Services.AddService(typeof(PathSystem), PathSys);
            GameEngine.Services.AddService(typeof(PatrolCats), PatCats);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Waypoint Loaded", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("FloorPlane and Skydome", 0);
            Blocker = new BlockingWalls("Content/Map/wallsCol", ContentManager);
            DisappearPoint = Plot.DisappearPoint;
            DisappearRange = (float)(Plot.DisappearRadius * Plot.DisappearRadius);

            SkyDome = new StaticObject(ContentManager.Load<Model>("Content/Models/SkyDome"), Vector3.Zero);
            SkyDome.Scale = new Vector3(600, 400, 600);
            SkyDome.LoadShader("Content/Shader Fx/CelFx", ContentManager);
            SkyDome.Cullable = false;

            // Floor
            FloorPlane = new PhysicsStaticObject(this);
            FloorPlane.Setup(Plot.MapModel, "Content/Shader Fx/CelFx", ContentManager);
            FloorPlane.SetCollision(new PlaneShape(new Vector3(0, 0.0f, 0), this));
            FloorPlane.mStaticObj.CenterOffset = new Vector3(0, -0.05f, 0);
            FloorPlane.mStaticObj.Scale = new Vector3(1.0f);
            FloorPlane.mStaticObj.Cullable = false;
            FloorPlane.mStaticObj.DisplayShadow = false;
            FloorPlane.mPhyObj.Immovable = true;
            FloorPlane.Visible = false;
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("FloorPlane and Skydome", 1);
            Debug.Write("\nFloorPlane and Skydome Succeed");

            Debug.Write("\nGenerating Vegetation");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Vegetation", 0);
            VegeGenerator = new VegetationGenerator(Plot.Trees, ContentManager, this);
            Debug.Write("\nCompleted");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Vegetation", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Player", 0);

            // Add in Player     
            Player = new Guniman(this);
            Player.Setup("Content/Models/Humaniod/Player/GMAN", "Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
            Player.mAnimatedObj.Animator.StartClip(Player.mAnimatedObj.GetAnimatedModel().AnimationClips["Idle" + PlayerState.EquipKey[(int)PlayerState.Equipedwith]]);
            Player.SetBoundingPhysicsBasedOnModel(Vector3.Zero, 0.4f, Vector3.Zero, true, (int)ObjectFilter.PLAYER, true);
            Player.mAnimatedObj.CopyBodeTransform();

            Player.ChangeEquipment(ContentManager);
            /*Player.HoldItem.Add(new Equipment(new StarHead(4, 1.5f, ContentManager)));
            Player.HoldItem[2].Item.CenterOffset = new Vector3(0, 1.0f, 0);
            Player.HoldItem[2].Item.Rotation = Matrix.CreateRotationZ(-1.202f);
            Player.mAnimatedObj.FindBonePosition("character_gman_set01_v02_01:character_proto2:head", out Player.HoldItem[2].ABS_ParentID, out Player.HoldItem[2].Skin_ParentID);
            Player.HoldItem[2].ABS_Parent = Player.mAnimatedObj.Transformation[Player.HoldItem[2].ABS_ParentID];
            Player.HoldItem[2].UseBone = true;*/
            GameEngine.Services.AddService(typeof(Guniman), Player);

            Debug.Write("\nAdded Player as service and active");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Player", 1);

            Debug.Write("\nLoading Dummy Buildings");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Dummy Buildings", 0);

            for (int i = 0; i < Plot.StaticBuilding.Count; i++)
            {
                Buildings.Add(new NewBuilding(this));
                Buildings[Buildings.Count - 1].LoadBuilding(VariableAsset.BuildingNames[GameEngine.RandomValue.Next(0, VariableAsset.BuildingNames.Count)], ContentManager, false, 0);
                Buildings[Buildings.Count - 1].MoveTo(Plot.StaticBuilding[i].Position, Plot.StaticBuilding[i].PitchYawRoll);
            }
            Debug.Write("\nCompleted");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Dummy Buildings", 1);

            Debug.Write("\nLoading Interactive Buildings");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Interactive Buildings", 0);

            for (int i = 0; i < Plot.ActiveHouse.Count; i++) 
            {
                Buildings.Add(new NewBuilding(this));
                Buildings[Buildings.Count - 1].LoadBuilding(VariableAsset.BuildingNames[GameEngine.RandomValue.Next(0, VariableAsset.BuildingNames.Count)], ContentManager, true, 850);
                //B.MoveTo(Plot.ActiveHouse[i].Position, new Vector3(0, -1.57f, 0));
                Buildings[Buildings.Count - 1].MoveTo(Plot.ActiveHouse[i].Position, Plot.ActiveHouse[i].PitchYawRoll);
            }

            SDHouse = new SDHHandle(Plot.FlatHouse, ContentManager, this);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Interactive Buildings", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Decorations", 0);
            // Interactive Cluster objects
            // Decoration Script
            Decos = new Decoration(ContentManager, this);
            GameEngine.Services.AddService(typeof(Decoration), Decos);
            FDeco = new FixedDecoration(ContentManager, this);
            GameEngine.Services.AddService(typeof(FixedDecoration), FDeco);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Decorations", 1);

            Debug.Write("\nCompleted");
            
            Debug.Write("\nFilling in Interactive Object");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Loading AI", 0);
            PathSys.FillPassers(ContentManager);
            PatCats.FillPasserByCats(ContentManager);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Loading AI", 1);

            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Interactive Objects", 0);
            PickableGenerator = new PickableGenerator(Plot.PickZones, ContentManager, this);
            DustbinGenerator = new DustbinGenerator(this);
            DustbinGenerator.Load(Plot.Dustbin, ContentManager);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Interactive Objects", 1);

            VegeGenerator.GenerateColliCluster(Plot.ColTrees, ContentManager, this);

            PHouse = new PlayerHouse(this);
            BuildingDef PHouseDef = ContentManager.Load<BuildingDef>("Content/Buildings/SavePoint/StoreRoom/StoreRoomDef");
            PHouse.Load(PHouseDef, ContentManager);
            PHouse.MoveTo(Plot.SavePoint.Position, Plot.SavePoint.PitchYawRoll);
            Player.mPhyObj.Position += (PHouse.GetDoorStep() - Plot.SavePoint.Position) * 2.0f;
            Debug.Write("\nSave Point Defined");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Save Point Defined", 2);

            Debug.Write("\nCompleted");

            ContentManager.ClearDustbin();
            Debug.Write("\nCleared ContentDustbin, Load Completed\n");
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Cleared Content Dustbin", 2);
            GameEngine.Services.GetService<LoadingScreen>().AddNewLine("Load is completed", 2);
            Debug.Write("\nNumber of Physics Body : " + GameEngine.Services.GetService<Physics>().PhysicsSystem.Bodies.Count);

            //Cutscene = new SellCutScene();
            //Cutscene.GenerateGoalPointDir();
            //Cutscene.Initialize(10);
            Plot = null;
            PlayerState.Currently = PlayerState.State.NORMAL;
            GameEngine.Services.GetService<Physics>().UpdatePhysics = true;
            //lcm
            GameEngine.Services.GetService<SoundManager>().Visible = true;
        }
        public override void Disable()
        {
            Debug.Write("\nNumber of Physics Body : " + GameEngine.Services.GetService<Physics>().PhysicsSystem.Bodies.Count);
            GameEngine.Services.GetService<Physics>().UpdatePhysics = false;
            Visible = false;
            DrawingWaitList.Clear();
            Blocker.Disable();
            Blocker = null;
            FloorPlane.mPhyObj.Body.DisableBody();
            FloorPlane.DisableComponent();
            FloorPlane = null;
            PHouse.DisableComponent();
            PHouse = null;
            Player.DisableComponent();
            Player = null;
            SkyDome.Shader.Dispose();
            SkyDome = null;
            PathSys.Clear();
            PathSys = null;
            PatCats.Clear();
            PatCats = null;
            SDHouse.DisableComponent();
            SDHouse = null;
            ObtainHouseCut.DisableComponent();
            GameEngine.Services.RemoveService(typeof(ObtainHouseCut));
            GameEngine.Services.RemoveService(typeof(PatrolCats));
            GameEngine.Services.RemoveService(typeof(Decoration));
            GameEngine.Services.RemoveService(typeof(FixedDecoration));
            FDeco.DisableComponent();
            Decos.DisableComponent();

            GameEngine.Services.RemoveService(typeof(PathSystem));
            PickableGenerator.DisableComponent();
            for (int i = 0; i < Buildings.Count; i++)
                Buildings[i].DisableComponent();
            for (int i = 0; i < Components.Count; i++)
                Components[i].DisableComponent();
            Components.Clear();
            Buildings.Clear();
            Cutscene.Disable();
            GameEngine.Services.RemoveService(typeof(SellCutScene));
            Cutscene = null;
            EmoSys.Disable();
            EmoSys = null;
            HumanBuffer.Disable();
            HumanBuffer = null;
            GameEngine.Services.RemoveService(typeof(Humaniod_Buffer));
            
            GameEngine.Services.RemoveService(typeof(EmoIconSystem));
            GameEngine.Services.RemoveService(typeof(Camera));
            GameEngine.Services.RemoveService(typeof(Shadow));
            GameEngine.Services.RemoveService(typeof(Guniman));


            if (ContentManager != null)
            {
                ContentManager.Unload();
                ContentManager.ClearDustbin();
                ContentManager.Dispose();
                ContentManager = null;
            }       
            GameEngine.Services.RemoveService(typeof(IEContentManager));
            
            Debug.Write("\nNumber of Physics Body : " + GameEngine.Services.GetService<Physics>().PhysicsSystem.Bodies.Count);
            GameEngine.Services.GetService<Physics>().UpdatePhysics = true;   
        }
        public override void Update()
        {
            if (!Visible) return;
            if (KB.Key_Pressed(Keys.OemTilde))
                GameEngine.Services.GetService<ConsoleMenu>().Visible = true;

            PlayTime += GameEngine.GameTime.ElapsedRealTime;
            SceneControl.NowAt = SceneControl.SceneArea.NORMAL;
            GameEngine.Services.GetService<Interactive>().Visible = false;
            Camera.VisibleObject = 0;
            bool SendToDraw = (DrawingWaitList.Count < 1) ? true : false;

            if ((Player.mPhyObj.Position - DisappearPoint).LengthSquared() < DisappearRange)
                SceneControl.NowAt = SceneControl.SceneArea.OUTOFMAP;

            if(PHouse.bAllowSell)
                SceneControl.NowAt = SceneControl.SceneArea.SAVEPOINT;
            if (PHouse.bInSavePoint)
            {
                GameEngine.Services.GetService<Interactive>().Visible = true;
                
                if (KB.Key_Pressed(Keys.Z) || GPD.Button_Pressed(Buttons.A))//|| GPdevice.Button_Pressed(Buttons.Y))
                {
                    
                    MainGame.SaveFile.Data.Money = GameEngine.Services.GetService<PlayerStats>().GetMoney();
                    MainGame.SaveFile.Data.Equipment = (int)PlayerState.Equipedwith;
                    MainGame.SaveFile.Data.Achievement1 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(0);
                    MainGame.SaveFile.Data.Achievement2 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(1);
                    MainGame.SaveFile.Data.Achievement3 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(2);
                    MainGame.SaveFile.Data.Achievement4 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(3);
                    MainGame.SaveFile.Data.Achievement5 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(4);
                    MainGame.SaveFile.Data.Achievement6 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(5);
                    MainGame.SaveFile.Data.Achievement7 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(6);
                    MainGame.SaveFile.Data.Achievement8 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(7);
                    MainGame.SaveFile.Data.Achievement9 = GameEngine.Services.GetService<AchievementEngine>().IsUnlocked(8);

                    MainGame.SaveFile.Save();
                    //SaveDataAccess ACS = new SaveDataAccess();
                    //ACS.OpenData("Content/GameData/SaveData.xml");
                    ////int temp = Int32.Parse(ACS.ReadData("Money")[0].InnerText);
                    //ACS.WriteData("Money", GameEngine.Services.GetService<PlayerStats>().GetMoney(), 0);
                    //ACS.WriteData("Equipment", (int)PlayerState.Equipedwith, 0);
                    //ACS.CloseData();
                    ////GameEngine.Services.GetService<StoreRoomMenu>().Visible = true;
                    //PlayerState.Currently = PlayerState.State.GAMEMENU;
                    Player.mPhyObj.Velocity = Vector3.Zero;
                }
            }
            if (PlayerState.Currently == PlayerState.State.SELL && !Cutscene.Visible)
            {
                GameEngine.Services.GetService<TransitionScreen>().StartFading(-1, 6);
                //Cutscene.Initialize_Scene(GameEngine.Services.GetService<PlayerStats>().GetPopularity(),
                //    GameEngine.Services.GetService<InventoryDisplay>().GetSellPercent());
                //lcm
                GameEngine.Services.GetService<SoundManager>().ChangeSong("sell");
            }
            if (KB.Key_Pressed(Keys.NumPad9))
                Cutscene.Initialize_Scene(100, 100);
            //if (KB.Key_Pressed(Keys.NumPad0))
                //ObtainHouseCut.Initialize_Scene(new Vector3(120, 0, -210), new Vector3(0,0,25), Matrix.CreateRotationY(0.61094f));
            //if (KB.Key_Pressed(Keys.NumPad1))
                //ObtainHouseCut.EndScene();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Initialized && !Components[i].ManuelUpdate)
                    Components[i].Update();  
            }
            PathSys.Update();
            PatCats.Update();
            EmoSys.Update();
            SkyDome.Rotation *= Matrix.CreateRotationY(0.03f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds);
            PickableGenerator.Update();
            //SkyDome.Position = new Vector3(Camera.Position.X, 0, Camera.Position.Z);
        }
        public override void Draw(ComponentType RenderType)
        {    
            GameEngine.GraphicDevice.Clear(new Color(203, 203, 203, 255));
            FloorPlane.Draw();          
            int i = 0;
                
            for (i = 0; i < Components.Count; i++)
                if (Components[i].Visible)
                    Components[i].Draw();
            
            for (i = 0; i < LastDraw.LastList.Count; i++)
                LastDraw.LastList[i].Draw("Normal");
            

            SceneControl.Rendering LastMode = SceneControl.RenderMode;
            SceneControl.RenderMode = SceneControl.Rendering.SHADOW;

            GameEngine.GraphicDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 0);
            GameEngine.GraphicDevice.RenderState.StencilEnable = true;
            GameEngine.GraphicDevice.RenderState.ReferenceStencil = 0;
            GameEngine.GraphicDevice.RenderState.StencilFunction = CompareFunction.Equal;
            GameEngine.GraphicDevice.RenderState.StencilPass = StencilOperation.Increment;
            for (i = 0; i < Components.Count; i++)
                Components[i].Draw();
            GameEngine.GraphicDevice.RenderState.StencilEnable = false;
            SceneControl.RenderMode = LastMode;
            PickableGenerator.Draw();
            EmoSys.Draw("Normal");
            SkyDome.Draw("NOSP");
        }
    }
}
