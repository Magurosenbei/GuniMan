using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using CommandDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class CommandDefTypeWritter : ContentTypeWriter<CommandDef>
    {
        protected override void Write(ContentWriter output, CommandDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Command);
            output.Write(value.ID);

        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(CommandDef.CommandDefContentReader).AssemblyQualifiedName;
        }
    }
}