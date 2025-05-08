using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace XmlContentExtension
{
    public class IOStorage
    {
        protected string Pathname;

        public string PlayerName;
        public Vector2 PlayerPosition;
        public int Level;
        public int Score;
        public string SaveDataAsset;

        public class IOStorageContentReader : ContentTypeReader<IOStorage>
        {
            protected override IOStorage Read(
                    ContentReader input,
                    IOStorage existingInstance)
            {
                IOStorage storage = new IOStorage();

                storage.PlayerName = input.ReadString();
                storage.PlayerPosition = input.ReadVector2();
                storage.Level = input.ReadInt32();
                storage.Score = input.ReadInt32();
                storage.SaveDataAsset = input.ReadString();

                return storage;
            }
        }
    }

    public class SaveData
    {
        public int Equipment = 0;
        public int Money = 0;

        public class SaveDataContentReader : ContentTypeReader<SaveData>
        {
            protected override SaveData Read(ContentReader input, SaveData existingInstance)
            {
                if (existingInstance == null)
                    existingInstance = new SaveData();
                // Add On here 
                existingInstance.Equipment  = input.ReadInt32();
                existingInstance.Money      = input.ReadInt32();


                return existingInstance;
            }
        }
    }
    [ContentTypeWriter]
    public class SaveDataContentTypeWriter : ContentTypeWriter<SaveData>
    {
        protected override void Write(ContentWriter output, SaveData value)
        {
            output.Write(value.Equipment);
            output.Write(value.Money);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SaveData.SaveDataContentReader).AssemblyQualifiedName;
        }
    }
}
