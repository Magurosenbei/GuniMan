using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace Engine
{
    public class Camera : Component, I3DComponent
    {
        Vector3 CameraPosition  = Vector3.Zero;
        Vector3 CameraView      = Vector3.Zero;
        Vector3 CameraUp        = Vector3.Up;

        public int VisibleObject = 0;

        public Vector3 Offset = Vector3.One * 10.0f;
        
        Matrix RotationMatrix   = Matrix.Identity;
        Matrix ViewMatrix       = Matrix.Identity;
        Matrix ProjectMatrix    = Matrix.Identity;
        Matrix ViewProject      = Matrix.Identity;

        public virtual Vector3 View { get { return CameraView; } set { CameraView = value; } }
        public virtual Vector3 Up { get { return CameraUp; } set { CameraUp = value; } }
        public virtual Vector3 Position { get { return CameraPosition; } set { CameraPosition = value; } }
        public virtual Vector3 Scale { get { return Vector3.One; } set { } }
        public virtual Vector3 EulerRotation
        {
            get { return MathsUtility.MatrixToVector3(Rotation); }
            set { Rotation = MathsUtility.Vector3ToMatrix(value); }
        }
        public virtual Matrix MatrixViewProj { get { return ViewProject; } set { ViewProject = value; } }
        public virtual Matrix MatrixView { get { return ViewMatrix; } set { ViewMatrix = value; } }
        public virtual Matrix MatrixProjection { get { return ProjectMatrix; } set { ProjectMatrix = value; } }
        public virtual Matrix Rotation { get { return RotationMatrix; } set { RotationMatrix = value; } }
        public virtual BoundingBox BoundingBox { get { return new BoundingBox(CameraPosition - Vector3.One, CameraPosition + Vector3.One); } }

        public Camera(GameScreen Parent) : base(Parent) { ProjectMatrix = MathsUtility.CreateProjectionMatrix(); }
        public Camera() : base() { ProjectMatrix = MathsUtility.CreateProjectionMatrix(); }

        public override void Update()
        {
            Vector3 Forward = View - Position;
            Forward.Normalize();


            Matrix RotationProxy = this.Rotation;
            RotationProxy.Forward = Forward;

            Vector3 ReferenceVector = Vector3.Up;
            // On the slim chance that the camera is pointed perfectly parallel with
            // the Y Axis, we cannot use cross product with a parallel axis, so we
            // change the reference vector to the forward axis (Z).
            if (RotationProxy.Forward.Y == ReferenceVector.Y || RotationProxy.Forward.Y == -ReferenceVector.Y)
                ReferenceVector = Vector3.Backward;

            // Calculate the other parts of the rotation matrix
            RotationProxy.Right = Vector3.Cross(this.Rotation.Forward, ReferenceVector);
            RotationProxy.Up = Vector3.Cross(this.Rotation.Right, this.Rotation.Forward);

            this.Rotation = RotationProxy;

            // Use the rotation matrix to find the new up
            Up = Rotation.Up;

            // Recalculate View and Projection using the new Position, Target, and Up
            ViewMatrix = Matrix.CreateLookAt(Position, View, Up);
            ViewProject = ViewMatrix * ProjectMatrix;
            base.Update();
        }

        public bool InView(Vector3 Point, float RadiusRange, out float Alpha, out float distance)
        {
            Vector3 Rel_Point = Point - Position;
            float D_F = Vector3.Dot(Rotation.Forward, Rel_Point);
            distance = D_F;
            if (Point.Y + RadiusRange > Position.Y)
            {
                if (D_F - RadiusRange < 50.0f)
                    Alpha = D_F * 0.02f;
                else
                    Alpha = 1.0f;
                if (Alpha > 1.0)
                    Alpha = 1.0f;
            }
            else
                Alpha = 1.0f;
            if (Alpha < 0.4f)
                Alpha = 0.4f;
            if (D_F + RadiusRange > 400 || D_F + RadiusRange < 0.1f)
                return false;
            float Height = D_F * 0.82842f;  // Based on 2 * tan(PI / 8)
            D_F = Vector3.Dot(Rel_Point, Rotation.Up);
            if(D_F > Height * 0.5f + RadiusRange || D_F < -Height * 0.5f - RadiusRange)
		        return false;
  
            Height = Height * GameEngine.GraphicDevice.Viewport.AspectRatio;
             D_F = Vector3.Dot(Rel_Point, Rotation.Left);
            if(D_F > Height * 0.5f + RadiusRange || D_F < - Height * 0.5f - RadiusRange)
                return false;

            VisibleObject++;
            return true;
        }

        public bool InView(Vector3 Point, float RadiusRange, float Yoff, float AlphaBias, out float Alpha, out float Distance)
        {
            Vector3 Rel_Point = Point - Position;
            float D_F = Vector3.Dot(Rotation.Forward, Rel_Point);
            if (D_F + AlphaBias < 10)
                Alpha = (D_F + AlphaBias) * 0.1f;
            else
                Alpha = 1.0f;
            if (Alpha > 1.0)
                Alpha = 1.0f;
            if (Alpha < 0.4f)
                Alpha = 0.4f;

            Distance = D_F;
            if (D_F + RadiusRange > 400 || D_F + RadiusRange < 0.1f)
                return false;

            float Height = D_F * 0.82842f;  // Based on 2 * tan(PI / 8)
            D_F = Vector3.Dot(Rel_Point, Rotation.Up);
            if (D_F > Height * 0.5f + Yoff || D_F < -Height * 0.5f - Yoff)
                return false;

            Height = Height * GameEngine.GraphicDevice.Viewport.AspectRatio;
            D_F = Vector3.Dot(Rel_Point, Rotation.Left);
            if (D_F > Height * 0.5f + RadiusRange || D_F < -Height * 0.5f - RadiusRange)
                return false;

            VisibleObject++;
            return true;
        }
    }
}