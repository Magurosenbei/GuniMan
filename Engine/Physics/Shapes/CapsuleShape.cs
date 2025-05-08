using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;

namespace Engine
{
    // A capsule physics simulation object
    public class CapsuleShape : PhysicsObject
    {
        float radius;
        float length;

        // Radius of capsule
        public float Radius
        {
            get { return radius; }
            set { SetupSkin(Length, value); }
        }

        // Length of capsule
        public float Length
        {
            get { return length; }
            set { SetupSkin(value, Radius); }
        }

        // Constructors

        public CapsuleShape() : base()
        {
            InitializeBody();
        }

        public CapsuleShape(float Length, float Radius)
        {
            InitializeBody();
            SetupSkin(Length, Radius);
        }

        public CapsuleShape(float Length, float Radius, Vector3 Position, 
            Vector3 Rotation) : base()
        {
            InitializeBody();
            SetupSkin(Length, Radius);
            this.Position = Position;
            this.EulerRotation = Rotation;
        }

        public CapsuleShape(float Length, float Radius, Vector3 Position, 
            Vector3 Rotation, GameScreen Parent) : base(Parent)
        {
            InitializeBody();
            SetupSkin(Length, Radius);
            this.Position = Position;
            this.EulerRotation = Rotation;
        }

        // Sets up the collision skin
        void SetupSkin(float length, float radius)
        {
            // Set new size values
            this.length = length;
            this.radius = radius;

            // Update the collision skin
            CollisionSkin.RemoveAllPrimitives();
            CollisionSkin.AddPrimitive(new Capsule(
                Vector3.Transform(new Vector3(-0.5f, 0, 0), Rotation), 
                Rotation, radius, length), 
                (int)MaterialTable.MaterialID.UserDefined, 
                new MaterialProperties(0.8f, 0.7f, 0.6f));
        }
    }
}