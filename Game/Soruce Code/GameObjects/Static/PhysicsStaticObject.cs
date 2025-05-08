using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Engine;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;

namespace Game
{
    public class PhysicsStaticObject : Component , I3DComponent
    {
        public bool ChangePhysicsViaCull = true;
        public PhysicsObject   mPhyObj = null;
        public StaticObject    mStaticObj = null;

        public PhysicsStaticObject() : base() { }
        public PhysicsStaticObject(GameScreen Parent) : base(Parent) { }

        #region I3DComponent stuff
        public virtual Vector3 Position { get { return mStaticObj.Position; } set { mStaticObj.Position = value; } }
        public Vector3 EulerRotation
        {
            get { return MathsUtility.MatrixToVector3(mStaticObj.Rotation); }
            set { mStaticObj.Rotation = MathsUtility.Vector3ToMatrix(value); }
        }
        public virtual Matrix Rotation { get { return mStaticObj.Rotation; } set { mStaticObj.Rotation = value; } }
        public virtual Vector3 Scale { get { return mStaticObj.Scale; } set { mStaticObj.Scale = value; } }
        public virtual BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(
                    Position - (Scale / 2),
                    Position + (Scale / 2)
                );
            }
        }
        #endregion

        protected override void InitializeComponent(GameScreen Parent)
        {
            base.InitializeComponent(Parent);
            DrawOrder = 1;
        }

        public void Setup(string ModelPath, string ShaderPath, IEContentManager ContentManager)
        {
            mStaticObj = new StaticObject(GameEngine.Content.Load<Model>(ModelPath), Vector3.Zero);
            mStaticObj.LoadShader(ShaderPath, ContentManager);
        }
        public void SetCollision(PhysicsObject type)
        {
            mPhyObj = type;
        }
        public void SetBoundingPhysicsBasedOnModel(Vector3 Position, float Scale, Vector3 Rotation, bool aboveground)
        {
            mStaticObj.Scale = Vector3.One * Scale;
            Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(mStaticObj.GetModel(), aboveground) * Scale;
            Position.Y += HalfExtents.Y;
            mPhyObj = new BoxShape(HalfExtents, Position, Rotation, base.Parent);
        }
        public void SetBoundingPhysicsBasedOnModel(Vector3 Position, float Scale, Vector3 Rotation, bool aboveground, int FilterID)
        {
            mStaticObj.Scale = Vector3.One * Scale;
            Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(mStaticObj.GetModel(), aboveground) * Scale;
            Position.Y += HalfExtents.Y;
            mPhyObj = new BoxShape(HalfExtents, Position, Rotation, FilterID, base.Parent);
        }

        public void AddToScreen() { base.InitializeComponent(GameEngine.DefaultScreen); }
        public void AddToScreen(GameScreen Parent) { base.InitializeComponent(Parent); }
        public override void DisableComponent()
        {
            mPhyObj.DisableComponent();
            base.DisableComponent();
        }

        public override void Update()
        {
            if (mStaticObj == null || mPhyObj == null) return;
            //mStaticObj.Update();
            mStaticObj.Position = mPhyObj.Position;// +mStaticObj.CenterOffset;
            mStaticObj.Rotation = mPhyObj.Rotation;

            mStaticObj.CalculatedOffset = (Matrix.CreateTranslation(mStaticObj.CenterOffset) * mStaticObj.Rotation).Translation;
            float blend = 1;
            if (mStaticObj.Cullable)
            {
                Visible = mStaticObj.camera.InView(Position, mStaticObj.ViewRange, out blend, out mStaticObj.DistanceFromCamera);
                if (ChangePhysicsViaCull)
                {
                 //   System.Threading.Thread.BeginCriticalRegion();
                    if (Visible && mStaticObj.DistanceFromCamera > 80 && mPhyObj.Body.IsBodyEnabled)
                        mPhyObj.Body.DisableBody();
                    else if (!Visible && mPhyObj.Body.IsBodyEnabled)
                        mPhyObj.Body.DisableBody();
                    else if (Visible && !mPhyObj.Body.IsBodyEnabled)
                        mPhyObj.Body.EnableBody();
                 //   System.Threading.Thread.EndCriticalRegion();
                }
            }
            if(!mStaticObj.CustomFade)
                mStaticObj.Color.W = blend;            
            base.Update();
        }

        public override void Draw()
        {
            if (mStaticObj == null || mPhyObj == null) return;
            mStaticObj.Draw("Normal");
            base.Draw();
        }
    }
}
