using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace Engine
{
    // A flat plane physics object
    public class PlaneShape : PhysicsObject
    {
        // Constructors
        public PlaneShape(): base()
        {
            Setup(Vector3.Zero);
        }
        public PlaneShape(Vector3 Position) : base()
        {
            Setup(Position);
        }
        public PlaneShape(GameScreen Parent) : base(Parent)
        {
            Setup(Vector3.Zero);
        }
        public PlaneShape(Vector3 Position, GameScreen Parent) : base(Parent)
        {
            Setup(Position);
        }

        public PlaneShape(Vector3 Position, GameScreen Parent, int type)
            : base(Parent)
        {
            Setup(Position, type);
        }

        void Setup(Vector3 Position, int Type)
        {
            // We can't use InitializeBody() here because we want to add a 
            // plane and not have it fall
            Body = new Body();
            CollisionSkin = new CollisionSkin(Body);
            CollisionSkin.AddPrimitive(
                new JigLibX.Geometry.Plane(Vector3.Up, Position),
                Type,
                new MaterialProperties(0.2f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(CollisionSkin);
            this.Position = Position;
        }

        // Setup everything
        void Setup(Vector3 Position)
        {
            // We can't use InitializeBody() here because we want to add a 
            // plane and not have it fall
            Body = new Body();
            CollisionSkin = new CollisionSkin(Body);
            CollisionSkin.AddPrimitive(
                new JigLibX.Geometry.Plane(Vector3.Up, Position), 
                (int)MaterialTable.MaterialID.UserDefined, 
                new MaterialProperties(0.2f, 0.7f, 0.6f));
            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(CollisionSkin);
            this.Position = Position;
        }
    }
}
