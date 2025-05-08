using Engine;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;

using Microsoft.Xna.Framework;

namespace Engine
{
    public class BoxShape : PhysicsObject
    {
        Vector3 sideLengths;
        int id = (int)MaterialTable.MaterialID.UserDefined;
        public Vector3 SideLengths
        {
            get { return sideLengths; }
            set 
            { 
                sideLengths = value;
                CollisionSkin.RemoveAllPrimitives();
                CollisionSkin.AddPrimitive( new Box(-value, Body.Orientation, value * 2.0f), 
                                            id, 
                                            new MaterialProperties(0.8f, 0.8f, 0.7f));
                
                this.Mass = this.Mass;
            }
        }
        public BoxShape()
            : base()
        {
            InitializeBody();
            sideLengths = Vector3.One;
        }
        public BoxShape(Vector3 SideLengths)
            : base()
        {
            SetupSkin(SideLengths, Vector3.Zero, Vector3.Zero);
        }
 
        public BoxShape(Vector3 SideLengths, Vector3 Position,
            Vector3 Rotation)
            : base()
        {
            SetupSkin(SideLengths, Position, Rotation);
        }

        public BoxShape(Vector3 SideLengths, Vector3 Position,
            Vector3 Rotation, GameScreen Parent)
            : base(Parent)
        {
            SetupSkin(SideLengths, Position, Rotation);
        }

        public BoxShape(Vector3 SideLengths, Vector3 Position,
            Vector3 Rotation, int ID, GameScreen Parent)
            : base(Parent)
        {
            id = ID;
            SetupSkin(SideLengths, Position, Rotation);
        }
 
        // Sets up the object with the specified parameters
        void SetupSkin(Vector3 SideLengths, Vector3 Position,
            Vector3 Rotation)
        {
            // Setup the body
            InitializeBody();
 
            // Set properties
            this.SideLengths = SideLengths;
            this.Position = Position;
            this.EulerRotation = Rotation;
        }
    }
}
