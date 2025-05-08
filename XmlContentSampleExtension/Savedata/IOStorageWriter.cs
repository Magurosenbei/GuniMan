using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using IOStorage = System.String;

namespace XmlContentExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class IOStorageContentTypeWriter : ContentTypeWriter<IOStorage>
    {
        protected override void Write(ContentWriter output, IOStorage value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.PlayerName);
            output.Write(value.PlayerPosition);
            output.Write(value.Level);
            output.Write(value.Score);
            output.Write(value.SaveDataAsset);
            //throw new NotImplementedException();
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            //return "MyNamespace.MyContentReader, MyGameAssembly";
            return typeof(IOStorage.IOStorageContentReader).AssemblyQualifiedName;
        }
    }
}
