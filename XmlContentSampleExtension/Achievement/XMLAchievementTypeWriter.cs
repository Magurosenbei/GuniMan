using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using AchievementTabDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class AchievementTabDefTypeWritter : ContentTypeWriter<AchievementTabDef>
    {
        protected override void Write(ContentWriter output, AchievementTabDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Picture);
            output.Write(value.Background);
            output.Write(value.Title);
            output.Write(value.Description);
            output.Write(value.NormalText);
            output.Write(value.BoldText);
            output.Write(value.Counter);
            //output.Write(value.Title);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AchievementTabDef.AchievementTabDefContentReader).AssemblyQualifiedName;
        }
    }
}