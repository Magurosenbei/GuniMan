using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public abstract class InputDevice <T> : Component
    {
        public abstract T State { get; }
    }

    public class InputDeviceEventArgs <O,S> : EventArgs
    {
        // the type that trigger event
        public O Object;

        public InputDevice<S> Device; // the device that owns this

        public S State;

        public InputDeviceEventArgs(O Object, InputDevice<S> Device)
        {
            this.Object = Object;
            this.Device = Device;
            this.State = ((InputDevice<S>)Device).State;
        }
    }
    public delegate void InputEventHandler<O, S>(object sender, InputDeviceEventArgs<O, S> e);
}
