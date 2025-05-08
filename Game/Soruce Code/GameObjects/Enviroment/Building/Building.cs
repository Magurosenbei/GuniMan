using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using XmlContentExtension;
using Engine;

namespace Game
{
    public class NewBuilding : Component
    {
        float VoidDeckHt;
        float LevelHt;
        int TotalLevel;
        float RangeRadius = 550.0f;

        List <BuidlingBlock>    Parts = new List<BuidlingBlock>();
        List <RandomStuffThrown>Stuffs = new List<RandomStuffThrown>();
        List <DoorToDoor> DoorA = new List<DoorToDoor>();
        Lift Elevator;

        DangerZone dangerZone;

        KeyboardDevice KB;
        GamepadDevice GPD;

        Interactive     CallEvent;
        LiftSelection   LiftPanel;

        public int thisResultThingy = 0;
        bool ActivateHornSpawn = false;
        bool AllowReHorn = true;
        bool Horn = false;
        bool UsePhysics = true;
        bool DrawShadow = true;
        public NewBuilding() : base() { this.Parent = GameEngine.DefaultScreen; }
        public NewBuilding(GameScreen Parent) : base(Parent) { this.Parent = Parent; }

        public bool LoadBuilding(string AssetXML, IEContentManager ContentManager, bool Physics, float Range)
        {
            LiftPanel = GameEngine.Services.GetService<LiftSelection>();
            CallEvent = GameEngine.Services.GetService<Interactive>();
            KB = GameEngine.Services.GetService<KeyboardDevice>();
            GPD = GameEngine.Services.GetService<GamepadDevice>();
            UsePhysics = Physics;
            RangeRadius = Range;
            BuildingAsset BASET;
            BuildingDef VoidDeck , Level, Roof;
            HDBLevelColBox Lift;
            try
            {
                BASET = ContentManager.Load<BuildingAsset>(AssetXML);
                /* Load Scripts */
                VoidDeck = ContentManager.Load<BuildingDef>(BASET.VoidDeckDef);
                Level = ContentManager.Load<BuildingDef>(BASET.LevelDef);
                Roof = ContentManager.Load<BuildingDef>(BASET.RoofDef);
                Lift = ContentManager.Load<HDBLevelColBox>(BASET.LiftDef);
            }
            catch (Exception e)
            {
                Debug.Write("\nBuilding Load Failed " + e.Message);
                return false;
            }
            StaticObject Levelpiece = new StaticObject(ContentManager.Load<Model>(Level.ModelPath), Vector3.Zero);
            Levelpiece.LoadShader(Level.ShaderPath, ContentManager);
            StaticObject RoofPiece = new StaticObject(ContentManager.Load<Model>(Roof.ModelPath), Vector3.Zero);
            RoofPiece.LoadShader(Level.ShaderPath, ContentManager);
            StaticObject LiftDoor = new StaticObject(ContentManager.Load<Model>(Level.DoorPath), Vector3.Zero);
            LiftDoor.LoadShader(Level.ShaderPath, ContentManager);
            
            /* Defstart */
            TotalLevel = GameEngine.RandomValue.Next(4, 10);
            Parts.Capacity = TotalLevel + 2;
            VoidDeckHt = (float)VoidDeck.Height;
            LevelHt = (float)Level.Height;
            Parts.Add(new BuidlingBlock(VoidDeck, ContentManager, Physics, true));

            List<StaticObject> HouseDoor = new List<StaticObject>();
            HouseDoor.Capacity = VariableAsset.Doors.Count;
            for (int i = 0; i < VariableAsset.Doors.Count; i++)
            {
                HouseDoor.Add(new StaticObject(ContentManager.Load<Model>(VariableAsset.Doors[i]), Vector3.Zero));
                HouseDoor[i].LoadShader(Level.ShaderPath, ContentManager);
            }
            float Ht = VoidDeckHt;
            for (int i = 0; i < TotalLevel + 1; i++)    // include roof
            {
                if (i < TotalLevel)
                {
                    if (UsePhysics)
                    {
                        Stuffs.Add(new RandomStuffThrown(Level, ContentManager, Parent, Ht, i));
                        DoorA.Add(new DoorToDoor(Level, ContentManager,  Parent, Ht, i));
                    }
                    Parts.Add(new LevelBlock(Level, Levelpiece, LiftDoor, HouseDoor, Physics, true));   
                }
                else
                    Parts.Add(new BuidlingBlock(Roof, RoofPiece, LiftDoor, false, false));
                Parts[i + 1].Set3DStruct(new Vector3(0, Ht, 0), Vector3.Zero);
                Ht += LevelHt;
            }
            if (UsePhysics)
            {
                dangerZone = new DangerZone();
                Elevator = new Lift(VoidDeckHt, LevelHt, TotalLevel, UsePhysics);
                Elevator.MoveTo(Lift.Position, Lift.PitchYawRoll);
            }
            Levelpiece = null;
            RoofPiece = null;
            HouseDoor.Clear();
            LiftDoor = null;
            return true;
        }
        public void MoveTo(Vector3 Position, Vector3 PitchYawRoll) // Call Once only Please
        {
            for (int i = 0; i < Parts.Count; i++)
                Parts[i].MoveTo(Position, PitchYawRoll);
            if (Elevator == null) return;
            Matrix Trans = Matrix.CreateTranslation(Elevator.GetPosition()) * Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z);
            Elevator.MoveTo(Trans.Translation + Position, Elevator.GetPitchYawRoll() + PitchYawRoll);
            for (int i = 0; i < Stuffs.Count; i++)
                Stuffs[i].MoveTo(Position, Stuffs[i].mPitchYawRoll + PitchYawRoll);
            if(dangerZone != null)
            dangerZone.MoveTo(Position, PitchYawRoll);
            for (int i = 0; i < DoorA.Count; i++)
                DoorA[i].GetWorld(Position, PitchYawRoll);
            
        }
        public override void DisableComponent()
        {
            for (int i = 0; i < Parts.Count; i++)
                Parts[i].Disable();
            if (Elevator != null)
                Elevator.ForceDisable();
            for (int i = 0; i < Stuffs.Count; i++)
                Stuffs[i].DisableComponent();
            if(dangerZone != null)
                dangerZone.DisableComponent();
            for (int i = 0; i < DoorA.Count; i++)
                DoorA[i].DisableComponent();
            base.DisableComponent();
        }
        
        public override void Update()
        {
            DrawShadow = true;
            Guniman Player = null;
            Vector3 Rel_Point = Parts[0].mStaticObject.Position - Parts[0].mStaticObject.camera.Position;
            float D_F = Vector3.Dot(Parts[0].mStaticObject.camera.Rotation.Forward, Rel_Point);
            if (D_F + Parts[0].mStaticObject.ViewRange > 150 || D_F + Parts[0].mStaticObject.ViewRange < - 100)
            {
                if (!UsePhysics)
                    DrawShadow = false; // Too far Don't draw shadow
                //return;
            }
            float Height = D_F * 0.82842f;  // Based on 2 * tan(PI / 8)
            D_F = Vector3.Dot(Rel_Point, Parts[0].mStaticObject.camera.Rotation.Up);
            Height *= GameEngine.GraphicDevice.Viewport.AspectRatio;
            D_F = Vector3.Dot(Rel_Point, Parts[0].mStaticObject.camera.Rotation.Left);
            if (D_F > Height * 0.5f + Parts[0].mStaticObject.ViewRange * 2.0f || D_F < -Height * 0.5f - Parts[0].mStaticObject.ViewRange * 2.0f)
            {
                if (!UsePhysics)
                {
                    DrawShadow = false;
                    return;
                }
            }
            if (!UsePhysics)
            {
                for (int i = 0; i < Parts.Count; i++)
                    Parts[i].Update();
                return;
            }
            try
            {
                Player = GameEngine.Services.GetService<Guniman>();

                //for (int i = 0; i < LoopCount; i++)
                AudioGame refAgame = GameEngine.Services.GetService<AudioGame>();
                
                Parallel.For(0, Stuffs.Count, delegate(int i)
                {
                    if (dangerZone.InDangerZone() && !refAgame.Visible)
                        Stuffs[i].Update();
                     Stuffs[i].UpdateFade();
                });
                if (refAgame.GetResult() && AllowReHorn)
                {
                    if (ActivateHornSpawn) { Horn = true; thisResultThingy = refAgame.GetResultThingy(); }
                    //PlayerState.Currently = PlayerState.State.NORMAL;
                }
                else Horn = false;
                /*for (int i = 0; i < Stuffs.Count; i++)      // Zone Check
                {
                    if (dangerZone.InDangerZone())
                        Stuffs[i].Update();
                    Stuffs[i].UpdateFade();
                }*/
                AllowReHorn = true;
                for (int i = 0; i < DoorA.Count; i++)      // Zone Check
                {
                    if (Horn) {
                        DoorA[i].CreateOnSpot(Parts[i + 1], thisResultThingy);
                    }
                    DoorA[i].UpdateAI(Parts[i + 1]);
                    DoorA[i].UpdateAfterBorder();
                    if (DoorA[i].getOccupiedBuilding())
                        AllowReHorn = false;
                }
                if (Horn)
                    ActivateHornSpawn = false;
                //float Distance = (Player.mPhyObj.Position.X - Parts[0].Body.Position.X) * (Player.mPhyObj.Position.X - Parts[0].Body.Position.X) 
                //                    + (Player.mPhyObj.Position.Z - Parts[0].Body.Position.Z) * (Player.mPhyObj.Position.Z - Parts[0].Body.Position.Z);
                if ((Player.mPhyObj.Position.X - Parts[0].Body.Position.X) * (Player.mPhyObj.Position.X - Parts[0].Body.Position.X)
                    + (Player.mPhyObj.Position.Z - Parts[0].Body.Position.Z) * (Player.mPhyObj.Position.Z - Parts[0].Body.Position.Z)
                    > RangeRadius)
                {
                    for (int i = 0; i < Parts.Count; i++)
                    {
                        Parts[i].ForceDisable = true;
                        Parts[i].Update();
                    }
                    Elevator.SetBodyEnable(false);         
                }
                else
                {
                    if (refAgame.Visible && PlayerState.Currently == PlayerState.State.HORN)
                        ActivateHornSpawn = true;
                    /*if (PlayerState.Equipedwith != PlayerState.Player_Eq.BAG && PlayerState.Equipedwith != PlayerState.Player_Eq.TRUCK)
                    {
                        if (!refAgame.Visible && PlayerState.Currently == PlayerState.State.NORMAL)
                        {
                            if (KB.Key_Pressed(Keys.H) || GPD.Button_Pressed(Buttons.LeftShoulder))
                            {
                                refAgame.Visible = true;
                                PlayerState.Currently = PlayerState.State.HORN;
                                ActivateHornSpawn = true;
                            }
                        }
                    }*/
                    SceneControl.NowAt = SceneControl.SceneArea.BUILDING;
                    Elevator.SetBodyEnable(true);
                    //Parallel.For(0, Parts.Count - 1, delegate(int i)
                    for (int i = 0; i < Parts.Count; i++)
                    {
                        Parts[i].ForceDisable = false;
                        Parts[i].Update();
                    }
                    if (Elevator.InLift())
                    {
                        int atLevel = Elevator.AtLevel(Player.mPhyObj.Position.Y);
                        if(atLevel > -1)
                        {
                            if(PlayerState.Currently != PlayerState.State.LIFT)
                                CallEvent.Visible = true;
                            LiftSelection.CurrentFloor = atLevel + 1;   // Count start from 1
                            LiftSelection.FloorMax = Parts.Count - 1;
                            if (PlayerState.Currently == PlayerState.State.NORMAL && (KB.Key_Pressed(Keys.Z) || GPD.Button_Pressed(Buttons.A)))
                            {
                                Player.mPhyObj.Velocity = Vector3.Zero;
                                PlayerState.Currently = PlayerState.State.LIFT;
                            }
                            else if (PlayerState.Currently == PlayerState.State.LIFT && (KB.Key_Pressed(Keys.X) || GPD.Button_Pressed(Buttons.B)) && Player.GoalHeight < 0)
                            {
                                PlayerState.Currently = PlayerState.State.NORMAL;
                                LiftPanel.TargetFloor = atLevel + 1;
                            }
                            else if (atLevel < 0)
                            {
                                PlayerState.Currently = PlayerState.State.NORMAL;
                                Player.GoalHeight = -1.0f;
                            }
                        }
                    }
                    if (PlayerState.Currently == PlayerState.State.LIFT && Player.GoalHeight < 0)
                    {
                        int keep = LiftPanel.GetTargetFloor();
                        if (keep > -1)
                            Player.GoalHeight = Elevator.GetLevelHeight(keep - 1);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write("\n Error occured in Building Update " + e.Message);
            }
            dangerZone.Update();
            base.Update();
        }
        public override void Draw()
        {
            //GameEngine.GraphicDevice.RenderState.DepthBias = -0.00001f;
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW && DrawShadow)
            {
                for (int i = 0; i < Parts.Count; i++)
                    Parts[i].Draw();
            }
            else if(SceneControl.RenderMode != SceneControl.Rendering.SHADOW)
            {
                for (int i = 0; i < Parts.Count; i++)
                {
                    if (Parts[i].Visible)
                    {
                        Parts[i].Draw();
                        if(i < Stuffs.Count)
                            Stuffs[i].Draw();
                    }
                }
            }
            base.Draw();
        }
    }
}


            
