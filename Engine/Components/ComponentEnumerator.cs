using System.Collections;
using System.Collections.Generic;


namespace Engine
{
    public class ComponentEnumerator : IEnumerator
    {
        // The current position
        int position = -1;

        // The collection we are enumerating
        ComponentCollection collection;

        // The order in which to return items from 'collection'
        List<int> ordered = new List<int>();

        // The current item, which is the item who's index is
        // at the current position in the order list
        public object Current
        {
            get { return collection[ordered[position]]; }
        }

        // Constructor sets the local component list and order
        public ComponentEnumerator(ComponentCollection Collection,
            List<int> Order)
        {
            this.collection = Collection;
            this.ordered = Order;
        }

        // Moves to the next item in the list
        public bool MoveNext()
        {
            position++;

            // If we have reached the end of the list, stop
            // enumerating
            if (position == ordered.Count)
                return false;

            // Otherwise keep going
            return true;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        // Resets to the beginning of the list
        public void Reset()
        {
            position = -1;
        }
    }
}
