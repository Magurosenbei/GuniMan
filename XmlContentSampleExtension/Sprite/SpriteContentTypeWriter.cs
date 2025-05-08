using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using Sprite = System.String;

namespace XmlContentExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class SpriteContentTypeWriter : ContentTypeWriter<Sprite>
    {
        protected override void Write(ContentWriter output, Sprite value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Position);
            output.Write(value.Rotation);
            output.Write(value.Scale);
            output.Write(value.TextureAsset);
            //throw new NotImplementedException();
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            //return "MyNamespace.MyContentReader, MyGameAssembly";
            return typeof(Sprite.SpriteContentReader).AssemblyQualifiedName;
        }
    }
}
