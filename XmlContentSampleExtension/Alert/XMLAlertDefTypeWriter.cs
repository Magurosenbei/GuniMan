using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using AlertDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class AlertDefTypeWritter : ContentTypeWriter<AlertDef>
    {
        protected override void Write(ContentWriter output, AlertDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.AlertText);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AlertDef.AlertDefContentReader).AssemblyQualifiedName;
        }
    }
}