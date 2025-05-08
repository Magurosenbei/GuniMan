using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

using System;

using System.Threading;

namespace Engine
{
    public class Physics : Component
    {
        public PhysicsSystem PhysicsSystem = new PhysicsSystem();

        public bool UpdatePhysics = false;

        public Physics()
        {
            this.PhysicsSystem.EnableFreezing = false;
            this.PhysicsSystem.SolverType = PhysicsSystem.Solver.Combined;
            this.PhysicsSystem.CollisionSystem = new CollisionSystemSAP();
            
            this.PhysicsSystem.CollisionSystem.UseSweepTests = false;
            /*this.PhysicsSystem.NumCollisionIterations = 5;
            this.PhysicsSystem.NumContactIterations = 15;
            this.PhysicsSystem.NumPenetrationRelaxtionTimesteps = 100;*/
            this.PhysicsSystem.AllowedPenetration = 0.0f;
            this.PhysicsSystem.Gravity = new Vector3(0,-9.8f,0);
            Visible = false;
        }
        public override void DisableComponent()
        {
            base.DisableComponent();
        }
        public override void Update()
        {
            if (!UpdatePhysics) return;
            try
            {
                PhysicsSystem.CurrentPhysicsSystem.Integrate((float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write("\n" + "Exception in Physics + Beware" + e.Message);
            }
            base.Update();
        }
    }
}