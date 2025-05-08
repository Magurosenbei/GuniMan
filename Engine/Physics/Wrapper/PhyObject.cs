using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

using Microsoft.Xna.Framework;

namespace Engine
{
    public abstract class PhysicsObject : Component
    {
        float fMass = 1;

        public Body Body;
        public CollisionSkin CollisionSkin;

        public float Mass
        {
            get { return fMass; }
            set
            {
                fMass = value;
                Vector3 com = SetMass(value);
                if (CollisionSkin == null) return;
                CollisionSkin.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
            }
        }
        public Vector3 Position
        {
            get { return Body.Position; }
            set { Body.MoveTo(value, Body.Orientation); }
        }
        public Vector3 EulerRotation
        {
            get { return MathsUtility.MatrixToVector3(Rotation); }
            set { Rotation = MathsUtility.Vector3ToMatrix(value); }
        }
        public Matrix Rotation
        {
            get { return Body.Orientation; }
            //set { Body.Orientation = value; }
            set { Body.MoveTo(Body.Position, value); }
        }
        public bool Immovable
        {
            get { return Body.Immovable; }
            set { Body.Immovable = value; }
        }
        public BoundingBox BoundingBox
        {
            get
            {
                if (Body.CollisionSkin != null)
                    return Body.CollisionSkin.WorldBoundingBox;
                else
                    return new BoundingBox(Position - Vector3.One, Position + Vector3.One);
            }
        }
        public Vector3 Scale
        {
            get { return Vector3.One; }
            set { }
        }

        public Vector3 Velocity
        {
            get { return Body.Velocity; }
            set { Body.Velocity = value; }
        }

        public PhysicsObject() : base() { }
        public PhysicsObject(GameScreen Parent) : base(Parent) { }

        protected override void InitializeComponent(Engine.GameScreen Parent)
        {
            if (!GameEngine.IsInitialized)
                throw new Exception("Engine Not Inited via Engine Setup");
            //Parent.Components.Add(this);
            Initialized = true;
            Visible = false;
            //base.InitializeComponent(Parent);
        }
        protected void InitializeBody()
        {
            Body = new Body();
            CollisionSkin = new CollisionSkin(Body);
            Body.CollisionSkin = this.CollisionSkin;
            Body.EnableBody();
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
        public void OffsetModel(Vector3 PositionOffset, Matrix RotationOffset)
        {
            CollisionSkin.ApplyLocalTransform(new JigLibX.Math.Transform(PositionOffset, RotationOffset));
        }
        public override void DisableComponent()
        {
            Body.DisableBody();
            //base.DisableComponent();
        }

        public static Box CreateBoxFromCenter(Vector3 Position, Vector3 HalfSize, Matrix Orientation)
        {
            return new Box(-HalfSize + Position, Orientation, HalfSize * 2.0f);
        }
        public static Box CreateBoxFromCorner(Vector3 Position, Vector3 Extents, Matrix Orientation)
        {
            return new Box(Position, Orientation, Extents);
        }
    }
}