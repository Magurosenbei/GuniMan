using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class EconomyDef
    {
        public string Name;           // The Material Name
        public int ID;                // Material ID
        public int MinPrice;          // The Material Price (Min)
        public int MaxPrice;          // The Material Price (Max)

        public EconomyDef() { }


        public class EconomyDefContentReader : ContentTypeReader<EconomyDef>
        {
            protected override EconomyDef Read(ContentReader input, EconomyDef EconomyStuff)
            {
                EconomyDef EconomyThingy = new EconomyDef();
                EconomyThingy.Name = input.ReadString();
                EconomyThingy.ID = input.ReadInt32();
                EconomyThingy.MinPrice = input.ReadInt32();
                EconomyThingy.MaxPrice = input.ReadInt32();
                
                return EconomyThingy;
            }
        }

    }
}