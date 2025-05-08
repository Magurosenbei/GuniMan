using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class FusionDef
    {
        public string ItemName;         // The name
        public int Level;               // The item level (hidden from player)
        public int Paper;               // Paper required
        public int Plastic;             // Plastic required
        public int Metal;               // Metal required
        public int Glass;               // Glass required
        public int Money;               // Money required


        public FusionDef() { }

        public class FusionDefContentReader : ContentTypeReader<FusionDef>
        {
            protected override FusionDef Read(ContentReader input, FusionDef FusionStuff)
            {
                FusionDef FusionThingy = new FusionDef();
                FusionThingy.ItemName = input.ReadString();
                FusionThingy.Level = input.ReadInt32();
                FusionThingy.Paper = input.ReadInt32();
                FusionThingy.Plastic = input.ReadInt32();
                FusionThingy.Metal = input.ReadInt32();
                FusionThingy.Glass = input.ReadInt32();
                FusionThingy.Money = input.ReadInt32();
                
                return FusionThingy;
            }
        }

    }
}