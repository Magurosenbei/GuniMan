using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;


namespace Engine
{
    public class Static3rdCam : FPSCamera
    {
        // Keeps track of the rotation and translation that have been
        // added via RotateTranslate()       
        KeyboardDevice kb = null;
        GamepadDevice GPD = null;
        public bool  AllowControl = true;
        public float Limit = 0.7855f;
        public float OffsetAngle = 0.0f;
        public Static3rdCam(GameScreen Parent) : base(Parent) { }
        public Static3rdCam() : base() { }

        public override void Update()
        {
            if (kb == null)
                kb = GameEngine.Services.GetService<KeyboardDevice>();
            if (GPD == null)
                GPD = GameEngine.Services.GetService<GamepadDevice>();
            if (kb.KeyDown(Keys.Space))
                base.Update();
            else
            {
                if (AllowControl && (GPD.RightStickPosition.X > 0 || kb.KeyDown(Keys.S)) && OffsetAngle < 0.244299978f)
                    OffsetAngle += 0.01745f;
                else if (AllowControl && (GPD.RightStickPosition.X < 0 || kb.KeyDown(Keys.A)) && OffsetAngle > -0.7855f)
                    OffsetAngle -= 0.01745f;

                if (OffsetAngle > Math.PI)
                    OffsetAngle = -(float)Math.PI;
                else if (OffsetAngle < -Math.PI)
                    OffsetAngle = (float)Math.PI;

                Position = (Matrix.CreateTranslation(Offset) * Matrix.CreateRotationY(OffsetAngle)).Translation + View;
                CameraSTDOldUpdate();
            }
        }
    }
}
