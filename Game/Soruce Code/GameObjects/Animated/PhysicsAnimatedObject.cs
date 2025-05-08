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

using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;
using XNAnimation.Pipeline;

namespace Game
{
    public class PhysicsAnimatedObject : Component
    {
        public PhysicsObject mPhyObj = null;
        public AnimatedObject mAnimatedObj = null;

        public PhysicsAnimatedObject() : base() { this.Parent = GameEngine.DefaultScreen; }
        public PhysicsAnimatedObject(GameScreen Parent) : base(Parent) { this.Parent = Parent; }

        protected override void InitializeComponent(GameScreen Parent)
        {
            base.InitializeComponent(Parent);
            DrawOrder = 2;
        }

        public virtual void Setup(string ModelPath, string ShaderPath, IEContentManager ContentManager)
        {
            mAnimatedObj = new AnimatedObject(GameEngine.Content.Load<SkinnedModel>(ModelPath), Vector3.Zero);
            mAnimatedObj.LoadShader(ShaderPath, ContentManager);
        }
        public virtual void SetCollision(PhysicsObject type)
        {
            mPhyObj = type;
        }

        public void AddToScreen() { base.InitializeComponent(GameEngine.DefaultScreen); }
        public void AddToScreen(GameScreen Parent) { base.InitializeComponent(Parent); }
        public override void DisableComponent()
        {
            mAnimatedObj.Shader.Dispose();
            mPhyObj.DisableComponent();
            base.DisableComponent();
        }

        public override void Update()
        {
            if (mAnimatedObj == null || mPhyObj == null) return;
            
            mAnimatedObj.Position = mPhyObj.Position;
            mAnimatedObj.Rotation = mPhyObj.Rotation;
            float blend = 1;
            if (mAnimatedObj.Cullable)
            {
                Visible = GameEngine.Services.GetService<Camera>().InView(mAnimatedObj.Position, mAnimatedObj.ViewRange, out blend, out mAnimatedObj.DistanceFromCamera);
            }
            mAnimatedObj.Color.W = blend;

            if (!Visible && mPhyObj.Body.IsBodyEnabled)
                mPhyObj.Body.DisableBody();
            else if (Visible && !mPhyObj.Body.IsBodyEnabled)
                mPhyObj.Body.EnableBody();

            if(Visible)
                mAnimatedObj.Update();

            base.Update();
        }
        public override void Draw()
        {
            if (mAnimatedObj == null || mPhyObj == null) return;
            mAnimatedObj.Draw("Normal");
            base.Draw();
        }
    }
}
