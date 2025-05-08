using System;
using Microsoft.Xna.Framework.Input;

#pragma warning disable 67

namespace Engine
{
    public class KeyboardDevice : InputDevice<KeyboardState>
    {
        KeyboardState last, current;

        Keys[] currentKeys;

        public override KeyboardState State { get { return current; } }
        public Keys[] PressedKeys { get { return currentKeys; } }
        public event InputEventHandler<Keys, KeyboardState> KeyPressed;
        public event InputEventHandler<Keys, KeyboardState> KeyReleased;
        public event InputEventHandler<Keys, KeyboardState> KeyHeld;

        public KeyboardDevice()
        {
            current = Keyboard.GetState();
            Update();
            Visible = false;
        }

        public override void Update()
        {
            last = current;
            current = Keyboard.GetState();
            currentKeys = current.GetPressedKeys();

            foreach (Keys k in InputUltility.GetEnumValues<Keys>())
            {
                

            }

        }

        public bool KeyUp(Keys Key)
        {
            return current.IsKeyUp(Key);
        }
        public bool KeyDown(Keys Key)
        {
            return current.IsKeyDown(Key);
        }
        public bool Key_Pressed(Keys Key)
        {
            if (last.IsKeyUp(Key) && current.IsKeyDown(Key))
                return true;
            return false;
        }
        public bool Key_Released(Keys Key)
        {
            if (current.IsKeyUp(Key) && last.IsKeyDown(Key))
                return true;
            return false;
        }
        public bool Key_Held(Keys Key)
        {
            if (current.IsKeyDown(Key) && last.IsKeyDown(Key))
                return true;
            return false;
        }
    }
}
