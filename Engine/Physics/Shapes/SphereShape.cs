using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Engine
{
    // A sphere physics object
    public class SphereShape : PhysicsObject
    {
        float radius;

        // The radius of the sphere
        public float Radius
        {
            get { return radius; }
            set
            {
                // Set the new value
                radius = value;

                // Update the collision skin
                CollisionSkin.RemoveAllPrimitives();
                CollisionSkin.AddPrimitive(new Sphere(Vector3.Zero * 5.0f, value), 
                    (int)MaterialTable.MaterialID.UserDefined, 
                    new MaterialProperties(0.5f, 0.7f, 0.6f));
            }
        }

        // Constructors

        public SphereShape() : base()
        {
            SetupSkin(Radius, Vector3.Zero, Vector3.Zero);
        }

        public SphereShape(float Radius)
        {
            SetupSkin(Radius, Position, EulerRotation);
        }

        public SphereShape(float Radius, Vector3 Position, Vector3 Rotation)
            : base()
        {
            SetupSkin(Radius, Position, Rotation);
        }

        public SphereShape(float Radius, Vector3 Position, Vector3 Rotation,
            GameScreen Parent) : base(Parent)
        {
            SetupSkin(Radius, Position, Rotation);
        }

        // Sets up the object
        void SetupSkin(float Radius, Vector3 Position, Vector3 Rotation)
        {
            // Setup the body
            InitializeBody();

            // Set parameters
            this.Radius = Radius;
            this.Position = Position;
            this.EulerRotation = Rotation;
        }
    }
}
