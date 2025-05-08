using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace Engine
{
    public class FPSCamera : Camera
    {
        // Keeps track of the rotation and translation that have been
        // added via RotateTranslate()
        Vector3 rotation;
        Vector3 translation;

        KeyboardDevice keyboard = null;
        MouseDevice mouse = null;

        public FPSCamera(GameScreen Parent) : base(Parent) { DrawOrder = 0; }
        public FPSCamera() : base() { DrawOrder = 0; }

        // This adds to rotation and translation to change the camera view
        public void RotateTranslate(Vector3 Rotation, Vector3 Translation)
        {
            translation += Translation;
            rotation += Rotation;
        }
        public void CameraSTDOldUpdate()
        {
            base.Update();
        }
        public override void Update()
        {
            Rotation = MathsUtility.Vector3ToMatrix(rotation); // Update the rotation matrix using the rotation vector
            translation = Vector3.Transform(translation, Rotation);// Update the position in the direction of rotation
            Position += translation;
            translation = Vector3.Zero;             // Reset translation 
            View = Vector3.Add(Position, Rotation.Forward);   // Calculate the new target
            // Have the base Camera update all the matrices, etc.
            base.Update();

            if(keyboard == null)
                keyboard = GameEngine.Services.GetService<KeyboardDevice>();
            if(mouse == null)
                mouse = GameEngine.Services.GetService<MouseDevice>();
            
            Vector3 inputModifier = new Vector3(
                (keyboard.KeyDown(Keys.A) ? -3 : 0) + (keyboard.KeyDown(Keys.D) ? 3 : 0),
                (keyboard.KeyDown(Keys.Q) ? -3 : 0) + (keyboard.KeyDown(Keys.E) ? 3 : 0),
                (keyboard.KeyDown(Keys.W) ? -3 : 0) + (keyboard.KeyDown(Keys.S) ? 3 : 0)
            );
            if(keyboard.KeyDown(Keys.Space))
                this.RotateTranslate(new Vector3(mouse.Delta.Y * -0.004f, mouse.Delta.X * -0.006f, 0), inputModifier * 0.5f);
        }
    }
}
