using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using EconomyDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class EconomyDefTypeWritter : ContentTypeWriter<EconomyDef>
    {
        protected override void Write(ContentWriter output, EconomyDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Name);
            output.Write(value.ID);
            output.Write(value.MinPrice);
            output.Write(value.MaxPrice);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(EconomyDef.EconomyDefContentReader).AssemblyQualifiedName;
        }
    }
}