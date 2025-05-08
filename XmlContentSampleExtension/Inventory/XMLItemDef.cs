using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace XmlContentExtension
{
    public class ItemDef
    {
        public string ItemName;        // The image
        public string Description;     // The item description 
        public int ID;                 // The item ID
        public int Type;               // The item type 
        public int Amount;             // The amount
        public int Index;              // The item index
  
        ///Type
        //1 - raw
        //2 - good
        //3 - useless
        //4 - key
        
        //Index - Used for displaying ( good and bad only)

        //-1 - dun have
        //0 - first
        //1 - second

        public ItemDef() { }


        public class ItemDefContentReader : ContentTypeReader<ItemDef>
        {
            protected override ItemDef Read(ContentReader input, ItemDef ItemStuff)
            {
                ItemDef ItemThingy = new ItemDef();
                ItemThingy.ItemName = input.ReadString();
                ItemThingy.Description = input.ReadString();
                ItemThingy.ID = input.ReadInt32();
                ItemThingy.Type = input.ReadInt32();
                ItemThingy.Amount = input.ReadInt32();
                ItemThingy.Index = input.ReadInt32();

                return ItemThingy;
            }
        }

    }
}