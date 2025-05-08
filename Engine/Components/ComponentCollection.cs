
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine
{
    public class ComponentCollection : Collection<Component>
    {
        GameScreen owner;
        // The list containing each component's index in the
        // component list, in the order we want them to draw
        List<int> inDrawOrder = new List<int>();

        // Constructor
        public ComponentCollection(GameScreen Owner)
        {
            owner = Owner;
        }
        protected override void ClearItems()
        {
            inDrawOrder.Clear();
            base.ClearItems();
        }

        // Override InsertItem so we can set the parent of the
        // component to the owner and manage the DrawOrder
        protected override void InsertItem(int index, Component item)
        {
            // Remove component from it's parent's list
            if (item.Parent != null && item.Parent != owner)
                item.Parent.Components.Remove(item);

            // Set the new parent
            item.Parent = owner;

            // Tell what to do when the item's draw order changes
            item.DrawOrderChanged += new ComponentDrawOrderChangedEventHandler(ComponentDrawOrderChangeEventHandler);

            base.InsertItem(index, item);

            // Update its position in the draw order list
            UpdateDrawPosition(item);
        }

        // Draw order changed event handler
        void ComponentDrawOrderChangeEventHandler(object sender, ComponentDrawOrderChangedEventArgs e)
        {
            // We simply update the component's position using the
            // existing method
            UpdateDrawPosition(e.Component);
        }

        // Updates the position of the component in the draw order list
        void UpdateDrawPosition(Component Component)
        {
            // Save the draw order and index location in the component list
            int ord = Component.DrawOrder;
            int loc = Items.IndexOf(Component);

            // Remove the index from the in order list
            if (inDrawOrder.Contains(loc))
                inDrawOrder.Remove(loc);

            // Create our index variable
            int i = 0;

            // Search through the ordered list until we find a component of
            // lesser or equal draw order value
            if (ord > 0)
            {
                while (i < inDrawOrder.Count)
                    // If the current item's draw order is greator or
                    // equal to the one we are working with...
                    if (Items[inDrawOrder[i]].DrawOrder >= ord)
                    {
                        // If it is greator, decrement it so it is
                        // above the component we are moving's draw order...
                        if (Items[inDrawOrder[i]].DrawOrder > ord)
                            i--;

                        // And stop looping
                        break;
                    }
                    // Otherwise, keep going (until we reach the end of the
                    // list)
                    else
                        i++;
            }

            // Insert the location of the component in the component list
            // into the ordered list
            inDrawOrder.Insert(i, Items.IndexOf(Component));
        }

        // Tells what enumerator to use when we want to loop through
        // components by draw order
        public Engine.ComponentEnumerator InDrawOrder
        {
            get { return new ComponentEnumerator(this, inDrawOrder); }
        }

        // Override RemoveItem so we can set the parent of
        // the component to null (no parent)
        protected override void RemoveItem(int index)
        {
            Items[index].Parent = null;

            // Unhook the draw order change event
            Items[index].DrawOrderChanged -= ComponentDrawOrderChangeEventHandler;

            // Remove the component from the collection
            base.RemoveItem(index);

            // Rebuild inDrawOrder
            inDrawOrder.Clear();
            foreach (Component component in Items)
                UpdateDrawPosition(component);
        }
    }
}
