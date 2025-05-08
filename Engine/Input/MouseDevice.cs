using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#pragma warning disable 67

namespace Engine
{
    public enum MouseButton { LEFT, RIGHT, MIDDLE, X1, X2 }

    public class MouseDevice : InputDevice<MouseState>
    {
        MouseState last, current;
        MouseButton[] currentButton = null;

        public override MouseState State { get { return current; } }
        public MouseButton[] PressedButtons { get { return currentButton; } }

        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                Mouse.SetPosition((int)value.X, (int)value.Y);
            }
        }

        public Vector2 Delta = Vector2.Zero;

        public bool ResetMouseToCenter = false;
        public float ScrollPosition { get { return current.ScrollWheelValue; } }

        public float ScrollDelta = 0;

        public event InputEventHandler<MouseButton, MouseState> ButtonPressed;
        public event InputEventHandler<MouseButton, MouseState> ButtonReleased;
        public event InputEventHandler<MouseButton, MouseState> ButtonHeld;

        public MouseDevice()
        {
            if (ResetMouseToCenter)
                Position = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
            current = Mouse.GetState();
            Update();
            Visible = false;
        }

        List<MouseButton> Pressed = new List<MouseButton>();
        public override void Update()
        {
            last = current;
            float LastScrollWheel = last.ScrollWheelValue;

            current = Mouse.GetState();
            Pressed.Clear();

            ScrollDelta = ScrollPosition - LastScrollWheel;

            if (ResetMouseToCenter)
            {
                Vector2 center = new Vector2(GameEngine.GraphicDevice.Viewport.Width * 0.5f, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
                Delta = new Vector2(current.X - center.X, current.Y - center.Y);

                Position = center;
            }
            else
                Delta = new Vector2(current.X - last.X, current.Y - last.Y);

            foreach (MouseButton Mb in InputUltility.GetEnumValues<MouseButton>())
            {


            }
        }

        public bool ButtonDown(MouseButton Button)
        {
            return ButtonDown(Button, current);
        }

        // An internal version of IsButtonDown that also allows us
        // to specify which state the check against
        bool ButtonDown(MouseButton Button, MouseState State)
        {
            return GetButtonState(Button, State) == ButtonState.Pressed ? true : false;
        }

        // Whether the specified button is currently up
        public bool ButtonUp(MouseButton Button)
        {
            return ButtonUp(Button, current);
        }

        // An internal version of IsButtonUp that also allows us
        // to specify which state the check against
        bool ButtonUp(MouseButton Button, MouseState State)
        {
            return GetButtonState(Button, State) ==
                ButtonState.Released ? true : false;
        }

        // Whether the specified button is down for the time this frame
        public bool WasButtonPressed(MouseButton Button)
        {
            if (ButtonUp(Button, last) && ButtonDown(Button, current))
                return true;
            return false;
        }

        // Whether the specified button is up for the first this frame
        public bool WasButtonReleased(MouseButton Button)
        {
            if (ButtonDown(Button, last) && ButtonUp(Button, current))
                return true;
            return false;
        }

        // Whether the specified button has been down for more than one frame
        public bool WasButtonHeld(MouseButton Button)
        {
            if (ButtonDown(Button, last) && ButtonDown(Button, current))
                return true;
            return false;
        }
        ButtonState GetButtonState(MouseButton Button, MouseState State)
        {
            if (Button == MouseButton.LEFT)
                return State.LeftButton;
            else if (Button == MouseButton.MIDDLE)
                return State.MiddleButton;
            else if (Button == MouseButton.RIGHT)
                return State.RightButton;
            else if (Button == MouseButton.X1)
                return State.XButton1;
            else if (Button == MouseButton.X2)
                return State.XButton2;

            return ButtonState.Released;
        }

    }
}
