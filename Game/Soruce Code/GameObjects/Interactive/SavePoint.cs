using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using XmlContentExtension;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;

namespace Game
{
    public class PlayerHouse : PhysicsObject
    {
        StaticObject    mStaticObj;
        Body            Trigger;
        CollisionSkin   TriggerSkin;
        public bool     bInSavePoint;
        public bool     bAllowSell;

        public PlayerHouse() : base() { }
        public PlayerHouse(GameScreen Parent) : base(Parent) { Parent.Components.Add(this); }

        public void Load(BuildingDef Def, IEContentManager ContentManager)
        {
            Visible = true;
            mStaticObj = new StaticObject(ContentManager.Load<Model>(Def.ModelPath), Vector3.Zero);
            mStaticObj.LoadShader(Def.ShaderPath, ContentManager);
            mStaticObj.Scale = Def.Scale;
            //mStaticObj.OutlineScale = Def.OutlineScale;
            mStaticObj.CenterOffset = Def.ModelOffset;
            Vector3 Extents = GraphicUtility.LargestHalfExtent(mStaticObj.GetModel(), true);
            mStaticObj.ViewRange = MathHelper.Max(Extents.X,  Extents.Z) * 1.73f;
            InitializeBody();
            CollisionSkin.RemoveAllPrimitives();
            for (int i = 0; i < Def.ColStore.Count; i++)
            {
                CollisionSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(
                                            Def.ColStore[i].Position,
                                            Def.ColStore[i].HalfExtents,
                                            Matrix.CreateFromYawPitchRoll(Def.ColStore[i].PitchYawRoll.Y, Def.ColStore[i].PitchYawRoll.X, Def.ColStore[i].PitchYawRoll.Z)),
                                            (int)ObjectFilter.BUILDINGS, new MaterialProperties(0.4f, 0.8f, 0.8f));
            }
            Trigger = new Body();
            TriggerSkin = new CollisionSkin(Trigger);
            Trigger.CollisionSkin = TriggerSkin;
            TriggerSkin.RemoveAllPrimitives();
            for (int i = 0; i < Def.HouseSlot.Count; i++)
            {
                TriggerSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(
                                            Def.HouseSlot[i].Position,
                                            Def.HouseSlot[i].HalfExtents,
                                            Matrix.CreateFromYawPitchRoll(Def.HouseSlot[i].PitchYawRoll.Y, Def.HouseSlot[i].PitchYawRoll.X, Def.HouseSlot[i].PitchYawRoll.Z)),
                                            (int)ObjectFilter.SAVEPOINT, new MaterialProperties(0.4f, 0.8f, 0.8f));
            }
            //CollisionSkin.NonCollidables.Add(TriggerSkin);
            Trigger.DisableBody();
            Trigger.Immovable = Immovable = true;
        }
        public override void DisableComponent()
        {
            Trigger.DisableBody();
            base.DisableComponent();
        }
        public void MoveTo(Vector3 Position, Vector3 PitchYawRoll)
        {
            Matrix Trans = Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z);
            Body.MoveTo(Position, Trans);
            Trigger.MoveTo(Position, Trans);
            mStaticObj.Position = Body.Position;
            mStaticObj.Rotation = Trans;
        }
        public Vector3 GetDoorStep()
        {
            return Trigger.CollisionSkin.GetPrimitiveNewWorld(0).TransformMatrix.Translation;
        }
        public override void Update()
        {
            //mStaticObj.Update();

            if (mStaticObj.Cullable)
                Visible = mStaticObj.camera.InView(mStaticObj.Position + new Vector3(0, 10, 0), mStaticObj.ViewRange, out mStaticObj.Color.W, out mStaticObj.DistanceFromCamera);
            mStaticObj.Color.W += 0.3f;
            Guniman Player = GameEngine.Services.GetService<Guniman>();
            if(!Visible) return;
            //if(new Vector2(Player.mPhyObj.Position.X - Body.Position.X, Player.mPhyObj.Position.Z - Body.Position.Z).LengthSquared() > mStaticObj.ViewRange * mStaticObj.ViewRange) return;
            if (PlayerState.Currently != PlayerState.State.NORMAL)
            {
                bInSavePoint = false;
                return;
            }
            if (mStaticObj.DistanceFromCamera < 50)
            {
                if (Player.mPhyObj.BoundingBox.Intersects(TriggerSkin.WorldBoundingBox))
                    bInSavePoint = true;
                else
                    bInSavePoint = false;
                bAllowSell = bInSavePoint;
            }
            else
                bInSavePoint = false;
        }
        public override void Draw()
        {
            mStaticObj.Draw("Normal");
            base.Draw();
        }
    }
}
