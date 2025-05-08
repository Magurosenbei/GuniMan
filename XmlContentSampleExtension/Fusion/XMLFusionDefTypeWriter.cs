using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using FusionDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class FusionDefTypeWritter : ContentTypeWriter<FusionDef>
    {
        protected override void Write(ContentWriter output, FusionDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.ItemName);
            output.Write(value.Level);          
            output.Write(value.Paper);
            output.Write(value.Plastic);
            output.Write(value.Metal);
            output.Write(value.Glass);
            output.Write(value.Money);
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(FusionDef.FusionDefContentReader).AssemblyQualifiedName;
        }
    }
}