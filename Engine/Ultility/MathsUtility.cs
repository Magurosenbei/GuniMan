using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class MathsUtility
    {
        public static Matrix CreateProjectionMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)GameEngine.GraphicDevice.Viewport.Width
                / (float)GameEngine.GraphicDevice.Viewport.Height,
                .1f, 10000);
        }

        // Creates a world matrix
        public static Matrix CreateWorldMatrix(Vector3 Translation)
        {
            return CreateWorldMatrix(Translation, Matrix.Identity);
        }

        // Creates a world matrix
        public static Matrix CreateWorldMatrix(Vector3 Translation,
            Matrix Rotation)
        {
            return CreateWorldMatrix(Translation, Rotation, Vector3.One);
        }

        // Creates a world matrix
        public static Matrix CreateWorldMatrix(Vector3 Translation,
            Matrix Rotation, Vector3 Scale)
        {
            return Matrix.CreateScale(Scale) *
                Rotation *
                Matrix.CreateTranslation(Translation);
        }

        // Converts a rotation vector into a rotation matrix
        public static Matrix Vector3ToMatrix(Vector3 Rotation)
        {
            return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
        }

        // Converts a rotation matrix into a rotation vector
        public static Vector3 MatrixToVector3(Matrix Rotation)
        {
            Quaternion q = Quaternion.CreateFromRotationMatrix(Rotation);
            return new Vector3(q.X, q.Y, q.Z);
        }

    }
}
