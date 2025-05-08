using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using AudioButtonDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class ItemDefTypeWritter : ContentTypeWriter<ItemDef>
    {
        protected override void Write(ContentWriter output, ItemDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.ItemName);
            output.Write(value.Description);          
            output.Write(value.ID);
            output.Write(value.Type);
            output.Write(value.Amount);
            output.Write(value.Index);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ItemDef.ItemDefContentReader).AssemblyQualifiedName;
        }
    }
}