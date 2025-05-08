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
    public class LevelBlock : BuidlingBlock
    {
        List<NewHouse>  Houses = new List<NewHouse>();
        public LevelBlock(BuildingDef Def, IEContentManager ContentManager, bool Physics, bool hasdoor)
            : base(Def, ContentManager, Physics, hasdoor)
        {
            Houses.Capacity = Def.HouseSlot.Count;
            for (int i = 0; i < Def.HouseSlot.Count; i++)
                Houses.Add(new NewHouse(
                            Def.ShaderPath,
                            Def.HouseSlot[i].Position, Def.HouseSlot[i].HalfExtents, Def.HouseSlot[i].PitchYawRoll,
                            ContentManager));
        }
        public LevelBlock(BuildingDef Def, StaticObject Piece, StaticObject LDoor, List<StaticObject> Doors, bool Physics, bool hasdoor)
            : base(Def, Piece, LDoor, Physics, hasdoor)
        {
            Houses.Capacity = Def.HouseSlot.Count;
            for (int i = 0; i < Def.HouseSlot.Count; i++)
            {
                int select = GameEngine.RandomValue.Next(0, Doors.Count);

                Houses.Add(new NewHouse(Doors[select],
                            Def.HouseSlot[i].Position, Def.HouseSlot[i].HalfExtents, Def.HouseSlot[i].PitchYawRoll));
            }
        }
        public override void Disable()
        {
            for (int i = 0; i < Houses.Count; i++)
                Houses[i].Disable();
            base.Disable();
        }
        public override void Set3DStruct(Vector3 Position, Vector3 PitchYawRoll)
        {
            base.Set3DStruct(Position, PitchYawRoll);
            for (int i = 0; i < Houses.Count; i++)
                Houses[i].Set3DStruct(Position, PitchYawRoll);
        }
        public override void MoveTo(Vector3 Position, Vector3 PitchYawRoll)
        {
            base.MoveTo(Position, PitchYawRoll);
            for (int i = 0; i < Houses.Count; i++)
                Houses[i].Set3DStruct(Position, PitchYawRoll);
        }
        public override void OpenByIndex(int index)
        {
            for (int i = 0; i < Houses.Count; i++)
            {
                if (index == i)
                {
                    if (!Houses[i].OpenDoor)
                        Houses[i].OpenDoor = true;
                    else
                        continue;
                }
                Houses[i].Update();
            }
            base.OpenByIndex(index);
        }
        public override void CloseByIndex(int index)
        {
            for (int i = 0; i < Houses.Count; i++)
            {
                if (index == i)
                {
                    if (Houses[i].OpenDoor)
                        Houses[i].OpenDoor = false;
                    else
                        continue;
                }
                Houses[i].Update();
            }
            base.CloseByIndex(index);
        }
        public override void Update()
        {
            for (int i = 0; i < Houses.Count; i++)
            {
                //if (GameEngine.Services.GetService<KeyboardDevice>().KeyDown(Keys.End))
                //    Houses[i].OpenDoor = true;
                //if (GameEngine.Services.GetService<KeyboardDevice>().KeyDown(Keys.Home))
                //    Houses[i].OpenDoor = false;
                Houses[i].Update();
            }

            base.Update();
        }
        public override void Draw()
        {
            for (int i = 0; i < Houses.Count; i++)
                Houses[i].Draw();
            base.Draw();
        }
    }
    public class BuidlingBlock
    {
        public Body Body;
        public CollisionSkin CollisionSkin;
        public StaticObject mStaticObject;
        protected float Yoff = 1;
        protected Vector3 mPitchYawRoll;
        public bool Visible;
        public bool ForceDisable = false;
        protected bool UsePhysics;
        // Lift Door
        protected StaticObject LiftDoor;
        protected Body LiftDoorBody;
        protected CollisionSkin LiftDoorSkin;
        protected bool OpenDoor = false;
        protected bool Havedoor;

        protected Guniman Player;

        public BuidlingBlock(BuildingDef Def, StaticObject obj, StaticObject Ldoor, bool Physics, bool hasdoor)
        {
            UsePhysics = Physics;
            Havedoor = hasdoor;
            Body = new Body();

            mStaticObject = new StaticObject(obj.GetModel(), Vector3.Zero);
            mStaticObject.Shader = obj.Shader.Clone(GameEngine.GraphicDevice);
            mStaticObject.camera = obj.camera;

            mStaticObject.Scale = Def.Scale;
            mStaticObject.CenterOffset = Def.ModelOffset;
            Vector3 HalfExtent = GraphicUtility.LargestHalfExtent(mStaticObject.GetModel(), true);
            mStaticObject.ViewRange = MathHelper.Max(HalfExtent.X, HalfExtent.Z);
            Yoff = HalfExtent.Y * 3.0f;
            if (Yoff < 5.0)
                Yoff = 5.0f;
            if (UsePhysics)
            {
                CollisionSkin = new CollisionSkin(Body);
                Body.CollisionSkin = CollisionSkin;
                CollisionSkin.RemoveAllPrimitives();
                for (int i = 0; i < Def.ColStore.Count; i++)
                {
                    CollisionSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(
                                                Def.ColStore[i].Position,
                                                Def.ColStore[i].HalfExtents,
                                                Matrix.CreateFromYawPitchRoll(Def.ColStore[i].PitchYawRoll.Y, Def.ColStore[i].PitchYawRoll.X, Def.ColStore[i].PitchYawRoll.Z)),
                                                (int)ObjectFilter.BUILDINGS, new MaterialProperties(0.4f, 0.8f, 0.8f));
                }
                CollisionSkin.callbackFn += new CollisionCallbackFn(CollisionSkin_callbackFn);
                Body.EnableBody();
            }
            else
                Body.DisableBody();
            Body.Immovable = true;
            if (hasdoor)
            {
                LiftDoorBody = new Body();
                LiftDoorBody.Immovable = true;
                if (UsePhysics)
                {
                    LiftDoorSkin = new CollisionSkin(LiftDoorBody);
                    LiftDoorBody.CollisionSkin = LiftDoorSkin;
                    LiftDoorBody.DisableBody();
                    LiftDoorSkin.RemoveAllPrimitives();
                    LiftDoorSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(Def.LiftSlot.Position, Def.LiftSlot.HalfExtents, Matrix.Identity),
                                                (int)ObjectFilter.BUILDINGS, new MaterialProperties(0, 0, 0));

                }
                LiftDoor = new StaticObject(Ldoor.GetModel(), Def.LiftSlot.PitchYawRoll);
                LiftDoor.Shader = Ldoor.Shader.Clone(GameEngine.GraphicDevice);
                LiftDoor.camera = Ldoor.camera;
            }
        }

        public BuidlingBlock(BuildingDef Def, IEContentManager ContentManager, bool Physics, bool hasdoor)
        {
            UsePhysics = Physics;
            Havedoor = hasdoor;
            Body = new Body();

            mStaticObject = new StaticObject(ContentManager.Load<Model>(Def.ModelPath), Vector3.Zero);
            mStaticObject.LoadShader(Def.ShaderPath, ContentManager);
            mStaticObject.Scale = Def.Scale;
            //mStaticObject.OutlineScale = Def.OutlineScale;
            mStaticObject.CenterOffset = Def.ModelOffset;
            Vector3 HalfExtent = GraphicUtility.LargestHalfExtent(mStaticObject.GetModel(), true);
            mStaticObject.ViewRange = MathHelper.Max(HalfExtent.X, HalfExtent.Z);
            Yoff = HalfExtent.Y * 3.0f;
            if (Yoff < 5.0) 
                Yoff = 5.0f;
            if (UsePhysics)
            {
                CollisionSkin = new CollisionSkin(Body);
                Body.CollisionSkin = CollisionSkin;
                CollisionSkin.RemoveAllPrimitives();

                for (int i = 0; i < Def.ColStore.Count; i++)
                {
                    CollisionSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(
                                                Def.ColStore[i].Position,
                                                Def.ColStore[i].HalfExtents,
                                                Matrix.CreateFromYawPitchRoll(Def.ColStore[i].PitchYawRoll.Y, Def.ColStore[i].PitchYawRoll.X, Def.ColStore[i].PitchYawRoll.Z)),
                                                (int)ObjectFilter.BUILDINGS, new MaterialProperties(0.4f, 0.8f, 0.8f));
                }
                CollisionSkin.callbackFn += new CollisionCallbackFn(CollisionSkin_callbackFn);
                Body.EnableBody();
            }
            else
                Body.DisableBody();
            Body.Immovable = true;
            if (hasdoor)
            {
                LiftDoorBody = new Body();
                LiftDoorBody.Immovable = true;
                if (UsePhysics)
                {
                    LiftDoorSkin = new CollisionSkin(LiftDoorBody);
                    LiftDoorBody.CollisionSkin = LiftDoorSkin;
                    LiftDoorBody.DisableBody();
                    LiftDoorSkin.RemoveAllPrimitives();
                    LiftDoorSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(Def.LiftSlot.Position, Def.LiftSlot.HalfExtents, Matrix.Identity),
                                                (int)ObjectFilter.BUILDINGS, new MaterialProperties(0, 0, 0));
                }
                LiftDoor = new StaticObject(ContentManager.Load<Model>(Def.DoorPath), Def.LiftSlot.PitchYawRoll);
                LiftDoor.LoadShader(Def.ShaderPath, ContentManager);
            }
        }
        public bool CollisionSkin_callbackFn(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.GetMaterialID(0) != (int)ObjectFilter.BUILDINGS) // Ignore Collision between buildings
                return true;
            return false;   // Report Not Colliding anything
        }
        public Vector3 SetMass(float Mass)
        {
            PrimitiveProperties primitiveProperties = new PrimitiveProperties(PrimitiveProperties.MassDistributionEnum.Solid,
                    PrimitiveProperties.MassTypeEnum.Density, Mass);
            float junk; Vector3 com; Matrix it, itCom;
            CollisionSkin.GetMassProperties(primitiveProperties, out junk, out com, out it, out itCom);
            Body.BodyInertia = itCom;
            Body.Mass = junk;
            return com;
        }
        virtual public void Disable()
        {
            Player = null;
            Body.DisableBody();
            if(Havedoor)
            {
                LiftDoorBody.DisableBody();
                LiftDoor.Shader.Dispose();
            }
            mStaticObject.Shader.Dispose();
        }

        virtual public void Set3DStruct(Vector3 Position, Vector3 PitchYawRoll)
        {
            Body.MoveTo(Position, Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z));
            if (Havedoor)
            {
                LiftDoorBody.MoveTo(Position, Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z));
                LiftDoor.Position += Position;
            }
            mStaticObject.Position = Body.Position;
            mStaticObject.Rotation = Body.Orientation;
            mPitchYawRoll = PitchYawRoll;
        }
        virtual public void MoveTo(Vector3 Position, Vector3 PitchYawRoll)
        {
            Body.MoveTo(Body.Position + Position, Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z));     
            mStaticObject.Position = Body.Position;
            mStaticObject.Rotation = Body.Orientation;
            mPitchYawRoll = PitchYawRoll;

            if (Havedoor)
            {
                LiftDoorBody.MoveTo(LiftDoorBody.Position + Position, Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z));
                LiftDoor.Position = (Matrix.CreateTranslation(LiftDoor.Position) * Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z)).Translation;
                LiftDoor.Position += Position;
                LiftDoor.Rotation = mStaticObject.Rotation;
            }
        }
        public virtual void OpenByIndex(int index)
        {
        }
        public virtual void CloseByIndex(int index)
        {
        }
        virtual public void Update()
        {
            if (UsePhysics && Havedoor && !ForceDisable)
            {
                if (Player == null)
                    Player = GameEngine.Services.GetService<Guniman>();
                if (PlayerState.Currently == PlayerState.State.LIFT)
                {
                    if (LiftDoor.Scale.X < 1.0f)
                        LiftDoor.Scale += new Vector3(2.0f, 0, 0) * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    else
                        LiftDoor.Scale = Vector3.One;
                }
                else if (Player.mPhyObj.BoundingBox.Intersects(LiftDoorSkin.WorldBoundingBox))
                    if (LiftDoor.Scale.X > 0)
                        LiftDoor.Scale -= new Vector3(2.0f, 0, 0) * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    else
                        LiftDoor.Scale = new Vector3(0, 1, 1);
                else
                    if (LiftDoor.Scale.X < 1.0f)
                        LiftDoor.Scale += new Vector3(2.0f, 0, 0) * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    else
                        LiftDoor.Scale = Vector3.One;
            }
            mStaticObject.Position = Body.Position;
            mStaticObject.Rotation = Body.Orientation;
            mStaticObject.CalculatedOffset = mStaticObject.CenterOffset;
            //mStaticObject.Update();
            float blend = 1.0f;
            if (mStaticObject.Cullable)
                Visible = mStaticObject.camera.InView(mStaticObject.Position, mStaticObject.ViewRange, Yoff, 0, out blend, out mStaticObject.DistanceFromCamera);
            mStaticObject.Color.W = blend;
            if(Havedoor)
                LiftDoor.Color.W = blend;

            if (!ForceDisable)
            {
                if (UsePhysics && Visible && !Body.IsBodyEnabled)
                    Body.EnableBody();
                else if (UsePhysics && !Visible && Body.IsBodyEnabled)
                    Body.DisableBody();
            }
            else if (Body.IsBodyEnabled)
                Body.DisableBody();
        }
        virtual public void Draw()
        {
            mStaticObject.Draw("Normal");
            if(Havedoor)
                LiftDoor.Draw("Normal");
        }
    }
}
