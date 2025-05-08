using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using AdminDef = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class AdminDefTypeWritter : ContentTypeWriter<AdminDef>
    {
        protected override void Write(ContentWriter output, AdminDef value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Name);
            output.Write(value.Pass);

        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AdminDef.AdminDefContentReader).AssemblyQualifiedName;
        }
    }
}