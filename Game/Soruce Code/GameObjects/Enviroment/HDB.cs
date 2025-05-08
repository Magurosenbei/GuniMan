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
    // Enviroment Section, contains Building models and collision box
    public class HDB : Component, Building
    {
        ElevatorLift Lift;
        PhysicsStaticObject Roof, VoidDeck;
        List <HDBLevel> Levels;
        //List <House> Apartments;
        float RangeRadius = 510.0f;
        float VoidDeckHt, LvlHt;
        string LabelTag = "";

        public HDB() : base() { Visible = false; }
        public HDB(GameScreen Parent) : base(Parent) { Visible = false; }

        public void Load(string buildingName, Vector3 Position, Matrix Rotate, float Bearing, float Range)
        {
            Visible = false;
            RangeRadius = Range;
            LabelTag = buildingName;
            float Height = 0;
            //-----------Void Deck
            VoidDeck = new PhysicsStaticObject(Parent);
            HDBLevelDef Store = GameEngine.Content.Load<HDBLevelDef>("Content/Buildings/" + LabelTag + "/Script/VoidDeck");
            VoidDeck.Setup(Store.Model, Store.Shader);
            VoidDeck.mPhyObj = new BoxShape();
            VoidDeck.mPhyObj.CollisionSkin.RemoveAllPrimitives();

            List<HDBLevelColBox> ColList = GameEngine.Content.Load<List<HDBLevelColBox>>(Store.CollisionDef);
            for (int i = 0; i < ColList.Count; i++)
            {
                VoidDeck.mPhyObj.CollisionSkin.AddPrimitive(
                    PhysicsObject.CreateBoxFromCenter(ColList[i].Position, ColList[i].HalfExtents, MathsUtility.Vector3ToMatrix(ColList[i].PitchYawRoll)),
                    (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.8f, 0.8f, 0.7f));
            }
            VoidDeck.mPhyObj.Rotation = Rotate;
            VoidDeck.mPhyObj.Position += Position;
            VoidDeck.mPhyObj.Immovable = true;
            VoidDeck.mPhyObj.Body.SetInactive();
            VoidDeck.mStaticObj.Scale = Store.Scale;
            VoidDeck.mStaticObj.OutlineScale = Store.OutlineScale;
            VoidDeck.mStaticObj.CenterOffset = Store.ModelOffset;

            Vector3 ExtentChecker = GraphicUtility.LargestHalfExtent(VoidDeck.mStaticObj.GetModel());
            VoidDeck.mStaticObj.ViewRange = MathHelper.Max(MathHelper.Max(ExtentChecker.X, ExtentChecker.Z), ExtentChecker.Y);

            VoidDeckHt = (float)Store.Height;
            Height += (float)Store.Height;
            //-----------Levels
            int LevelCount = GameEngine.RandomValue.Next(4, 10);
            Levels = new List<HDBLevel>();
            for(int i = 0; i < LevelCount; i++)
                Levels.Add(new HDBLevel(0, Parent));
            for (int i = 0; i < LevelCount; i++)
            {
                Levels[i].Position = new Vector3(0, Height, 0) + Position;
                Levels[i].Load(LabelTag, Rotate, Bearing);
                Height += Levels[i].Height;
            }
            LvlHt = Levels[0].Height;
            //-----------Roof
            Roof = new PhysicsStaticObject(Parent);
            Store = GameEngine.Content.Load<HDBLevelDef>("Content/Buildings/" + LabelTag + "/Script/Roof");
            Roof.Setup(Store.Model, Store.Shader);
            Roof.mPhyObj = new BoxShape();
            Roof.mPhyObj.CollisionSkin.RemoveAllPrimitives();

            Roof.mPhyObj.Immovable = true;
            Roof.mPhyObj.Body.SetInactive();
            Roof.mStaticObj.Scale = Store.Scale;
            Roof.mStaticObj.OutlineScale = Store.OutlineScale;
            Roof.mStaticObj.CenterOffset = Store.ModelOffset;
            Roof.mPhyObj.Rotation = Rotate;
            Roof.mPhyObj.Position = new Vector3(0, Height, 0) + Position;
            ExtentChecker = GraphicUtility.LargestHalfExtent(Roof.mStaticObj.GetModel());
            Roof.mStaticObj.ViewRange = MathHelper.Max(MathHelper.Max(ExtentChecker.X, ExtentChecker.Z), ExtentChecker.Y);
            Roof.mPhyObj.Body.DisableBody();

            Store = GameEngine.Content.Load<HDBLevelDef>("Content/Buildings/" + LabelTag + "/Script/Lift");
            Lift = new ElevatorLift(Store.ModelOffset + Position, Rotate, VoidDeckHt, LvlHt, Levels.Count);
            Lift.Rotation = Rotate;

            for (int i = 0; i < Levels.Count; i++)
                Levels[i].Setbody(true);
            Lift.Body.DisableBody();
            VoidDeck.mPhyObj.Body.DisableBody();
        }
        public override void DisableComponent()
        {
            VoidDeck.DisableComponent();
            for (int i = 0; i < Levels.Count; i++)
                Levels[i].DisableComponent();
            Lift.DisableComponent();
            Roof.DisableComponent();
            base.DisableComponent();
        }

        public override void Update()
        {
            Guniman Player = GameEngine.Services.GetService<Guniman>();            
            if ((new Vector2(
                    Player.mPhyObj.Position.X - VoidDeck.mPhyObj.Position.X,
                    Player.mPhyObj.Position.Z - VoidDeck.mPhyObj.Position.Z)
                    .LengthSquared() > RangeRadius))
            {
                if (VoidDeck.mPhyObj.Body.IsBodyEnabled)
                {
                    VoidDeck.mPhyObj.Body.DisableBody();
                    for (int i = 0; i < Levels.Count; i++)
                        Levels[i].Setbody(true);
                    Lift.Body.DisableBody();
                }
                return;
            }
            else if (!VoidDeck.mPhyObj.Body.IsBodyEnabled)
            {
                VoidDeck.mPhyObj.Body.EnableBody();
                for (int i = 0; i < Levels.Count; i++)
                    Levels[i].Setbody(false);
                Lift.Body.EnableBody();
            }
            if (Lift.Body.IsBodyEnabled)
                Lift.Update();

            int atlevel = Lift.InLift(Player.mPhyObj.Position.Y);
            if (atlevel > -1)
            {
                if(PlayerState.Currently != PlayerState.State.LIFT)
                    GameEngine.Services.GetService<Interactive>().Visible = true;
                LiftSelection.CurrentFloor = atlevel + 1;   // Count start from 1
                LiftSelection.FloorMax = Levels.Count + 1;
                if (PlayerState.Currently == PlayerState.State.NORMAL && GameEngine.Services.GetService<KeyboardDevice>().Key_Released(Keys.V))
                    PlayerState.Currently = PlayerState.State.LIFT;
                else if (PlayerState.Currently == PlayerState.State.LIFT && GameEngine.Services.GetService<KeyboardDevice>().Key_Pressed(Keys.B) && Player.GoalHeight < 0)
                    PlayerState.Currently = PlayerState.State.NORMAL;
                else if (atlevel < 0)
                {
                    PlayerState.Currently = PlayerState.State.NORMAL;
                    Player.GoalHeight = -1.0f;
                }
            }
            else
                GameEngine.Services.GetService<Interactive>().Visible = false;

            if (PlayerState.Currently == PlayerState.State.LIFT && Player.GoalHeight < 0)
            {
                int keep = GameEngine.Services.GetService<LiftSelection>().GetTargetFloor();
                if(keep > -1)
                    Player.GoalHeight = Lift.GetLevelHeight(keep - 1);
            }
            base.Update();
        }
    }
}
