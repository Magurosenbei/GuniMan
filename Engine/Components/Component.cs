using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public class Component
    {
        public GameScreen Parent;           // The game Screen that has this thing

        public bool Initialized = false;    // Booted up
        public bool ManuelUpdate = false;
        public bool Visible = true;         // Draw or no Draw

        public Component(GameScreen Parent) // Set Parent
        {
            InitializeComponent(Parent);
        }
        public Component()                  // Use Default
        {
            InitializeComponent(GameEngine.DefaultScreen);
        }

        protected virtual void InitializeComponent(GameScreen Parent)
        {
            if (!GameEngine.IsInitialized)
                throw new Exception("Engine Not Inited via Engine Setup");
            Parent.Components.Add(this);
            Initialized = true;
            this.Parent = Parent;
        }
        // Called by user
        public virtual void Update()
        {
        }
        // Called by user
        public virtual void Draw()
        {
        }
        public virtual void AdditionalUpdate()
        {

        }
        // unregister component
        public virtual void DisableComponent()
        {
            Parent.Components.Remove(this);
        }
        // The draw order of the component. Lower values draw first
        int drawOrder = 1;
        // Draw order changed event
        public event ComponentDrawOrderChangedEventHandler DrawOrderChanged;
        // Public draw order. If the value is changed, we fire the draw
        // order change event
        public int DrawOrder
        {
            get { return drawOrder; }
            set
            {
                // Save a copy of the old value and set the new one
                int last = drawOrder;
                drawOrder = value;
                // Fire DrawOrderChanged
                if (DrawOrderChanged == null) return;
                DrawOrderChanged(this, new ComponentDrawOrderChangedEventArgs(this, last, this.Parent.Components));
            }
        }
    }

    public class ComponentDrawOrderChangedEventArgs : EventArgs
    {
        // Component that was modified
        public Component Component;

        // The old draw order
        public int LastDrawOrder;

        // The collection that owns the component
        public ComponentCollection ParentCollection;

        public ComponentDrawOrderChangedEventArgs(Component Component, int LastDrawOrder, ComponentCollection ParentCollection)
        {
            this.Component = Component;
            this.LastDrawOrder = LastDrawOrder;
            this.ParentCollection = ParentCollection;
        }
    }
    public delegate void ComponentDrawOrderChangedEventHandler(
            object sender, ComponentDrawOrderChangedEventArgs e);


}
