using Microsoft.Xna.Framework;

namespace Engine
{
    // Represents 3D object
    public interface I3DComponent
    {
        Vector3 Position{get; set;}
        Vector3 Scale { get; set; }
        Vector3 EulerRotation { get; set; }
        Matrix  Rotation { get; set; }

        BoundingBox BoundingBox { get; }
    }
    // Represent 2D object
    public interface I2DComponent
    {
        Rectangle Rectangle { get; set; }
    }

    public enum ComponentType
    {
        // represent type
        Component2D, Component3D, Both, All
    }
}