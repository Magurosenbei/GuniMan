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
    public class AudioButtonDefTypeWritter : ContentTypeWriter<AudioButtonDef>
    {
        protected override void Write(ContentWriter output, AudioButtonDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Button);
            output.Write(value.Sound);
            output.Write(value.Timing);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AudioButtonDef.AudioButtonDefContentReader).AssemblyQualifiedName;
        }
    }
}