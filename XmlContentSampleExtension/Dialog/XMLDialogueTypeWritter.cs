using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using DialogueDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class DialogueDefTypeWritter : ContentTypeWriter<DialogueDef>
    {
        protected override void Write(ContentWriter output, DialogueDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Text);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(DialogueDef.DialogueDefContentReader).AssemblyQualifiedName;
        }
    }
}