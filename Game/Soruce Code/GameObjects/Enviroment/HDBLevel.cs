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
    /* Holds doors windows and decos etc */
    public class HDBLevel : Component
    {
        PhysicsStaticObject OriginBody;
        List <PhysicsStaticObject> Decoration = new List<PhysicsStaticObject>();
        List <House> Houses = new List<House>();
        //-----------------------
        public Vector3 Position = Vector3.Zero;
        public Vector3 HalfExtents = Vector3.Zero;

        public float Bearing = 0;
        float a = 0;
        public float Height = 0.0f;
        public HDBLevel(float BearingRadians) : base() { a = Bearing = BearingRadians; }
        public HDBLevel(float BearingRadians, GameScreen Parent) : base(Parent) { a = Bearing = BearingRadians; }

        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        /* Load here */
        public void Load(string BuildingName, Matrix Rotation, float aBearing)
        {
            Bearing = aBearing;
            a = Bearing;
            OriginBody = new PhysicsStaticObject(Parent);
            HDBLevelDef Store = GameEngine.Content.Load<HDBLevelDef>("Content/Buildings/" + BuildingName + "/Script/Level");
            
            Height = (float)Store.Height;
            OriginBody.Setup(Store.Model, Store.Shader);
            List<HDBLevelColBox> ColList = GameEngine.Content.Load<List<HDBLevelColBox>>(Store.CollisionDef);
            
            OriginBody.mPhyObj = new BoxShape();
            OriginBody.mPhyObj.CollisionSkin.RemoveAllPrimitives();        
            for (int i = 0; i < ColList.Count; i++)
            {
                OriginBody.mPhyObj.CollisionSkin.AddPrimitive(
                    PhysicsObject.CreateBoxFromCenter(ColList[i].Position, ColList[i].HalfExtents, MathsUtility.Vector3ToMatrix(ColList[i].PitchYawRoll)),
                    (int)MaterialTable.MaterialID.UserDefined, new MaterialProperties(0.8f, 0.8f, 0.7f));
            }
            OriginBody.mPhyObj.Immovable = true;
            OriginBody.mStaticObj.Scale = Store.Scale;
            OriginBody.mStaticObj.OutlineScale = Store.OutlineScale;
            OriginBody.mStaticObj.CenterOffset = Store.ModelOffset;
            OriginBody.mStaticObj.ViewRange = 50.0f;
            OriginBody.mPhyObj.Position = Position;
            OriginBody.mPhyObj.Rotation = Rotation;
            OriginBody.mPhyObj.Body.SetInactive();

            //-------- Find View Range
            HalfExtents = GraphicUtility.LargestHalfExtent(OriginBody.mStaticObj.GetModel());
            OriginBody.mStaticObj.ViewRange = MathHelper.Max(MathHelper.Max(HalfExtents.X, HalfExtents.Z), HalfExtents.Y);

            //-------- Setup Houses
            ColList = GameEngine.Content.Load<List<HDBLevelColBox>>(Store.HouseDefinition);
            
            for (int i = 0; i < ColList.Count; i++)
            {
                Houses.Add(new House(VariableAsset.Doors[GameEngine.RandomValue.Next(0, VariableAsset.Doors.Count)], Store.Shader, Position,
                                        ColList[i].Position, ColList[i].HalfExtents, Rotation, aBearing)); 
            }
        }
        public void Setbody(bool disable)
        {
            if (disable)
                OriginBody.mPhyObj.Body.DisableBody();
            else
                OriginBody.mPhyObj.Body.EnableBody();
        }
        public void Unload()
        {
            for (int i = 0; i < Decoration.Count; i++)
                Decoration[i].DisableComponent();

            Decoration.Clear();
            Houses.Clear();
            OriginBody.DisableComponent();
        }
        public override void DisableComponent()
        {
            Unload();
            base.DisableComponent();
        }
        public override void Draw()
        {
            for (int i = 0; i < Houses.Count; i++)
                Houses[i].Draw();
            base.Draw();
        }
        public override void Update()
        {
            Visible = OriginBody.Visible;
           
            for (int i = 0; i < Houses.Count; i++)
            {
                if (GameEngine.Services.GetService<KeyboardDevice>().KeyDown(Keys.End))
                    Houses[i].OpenDoor = true;
                if (GameEngine.Services.GetService<KeyboardDevice>().KeyDown(Keys.Home))
                    Houses[i].OpenDoor = false;
                Houses[i].Update();
            }
            base.Update();
        }
    }
}
