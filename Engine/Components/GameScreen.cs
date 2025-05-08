using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class GameScreen
    {
        public ComponentCollection Components;

        public bool Visible = true;

        // For pausemenus etc...
        public bool BlocksUpdate = false;
        // Whether overridable.
        public bool OverrideUpdateBlocked;

        public bool BlocksDraw = false;
        public bool OverrideDrawBlocked = false;

        public bool BlockInput = false;
        public bool OverrideInputBlocked = false;

        // Block Inputs
        public bool InputDisabled = false;
        // by default if input is even allowed
        public bool IsInputAllowed = true;

        // Name the component
        public string Name;

        // Activated when Initialize is finished or hook for syncro load screen
        public event EventHandler OnInitialized;

        bool initialized = false;

        public bool Initialized
        {
            get { return initialized; }
            set
            {
                initialized = true;
                // Shoot msg to indicate that this is ready
                if (OnInitialized != null)
                    OnInitialized(this, new EventArgs());
            }
        }
        public GameScreen(string Name)
        {
            Components = new ComponentCollection(this);
            this.Name = Name;
            GameEngine.GameScreens.Add(this);

            if (!Initialized)
                Initialize();
        }
        public virtual void Initialize()
        {
            this.Initialized = true;
        }
        // Update Screen and child components
        public virtual void Update()
        {
            //List <Component> Updating = new List<Component>();
            for (int i = 0; i < Components.Count; i++)
            {
                //Updating.Add(Components[i]);
                if (Components[i].Initialized)
                    Components[i].Update();
            }
            /*for(int i = 0; i < Updating.Count; i++)
                if (Updating[i].Initialized)
                    Updating[i].Update();*/
        }

        public virtual void Draw(ComponentType RenderType)
        {
            List<Component> Drawing = new List<Component>();

            foreach (Component comp in Components.InDrawOrder)
            {
                switch (RenderType)
                {
                    case ComponentType.Both:
                        if (comp is I2DComponent || comp is I3DComponent)
                            Drawing.Add(comp);
                        break;
                    case ComponentType.Component2D:
                        if (comp is I2DComponent)
                            Drawing.Add(comp);
                        break;
                    case ComponentType.Component3D:
                        if (comp is I3DComponent)
                            Drawing.Add(comp);
                        break;
                    default:
                        Drawing.Add(comp);
                        break;
                }
            }
            // Whole list of 2D stuff, sorting to draw
            List<Component> defer2D = new List<Component>();
            for (int i = 0; i < Drawing.Count; i++)
            {
                if (Drawing[i].Visible && Drawing[i].Initialized)
                    if (Drawing[i] is I2DComponent)
                        defer2D.Add(Drawing[i]);
                    else
                        Drawing[i].Draw();    // Draw immediate if not 2D
            }

            for (int i = 0; i < defer2D.Count; i++)
                defer2D[i].Draw();
        }

        public virtual void Disable()
        {
            Components.Clear();
            GameEngine.GameScreens.Remove(this);
            if (GameEngine.DefaultScreen == this)
                GameEngine.DefaultScreen = GameEngine.BackgroundScreen;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
