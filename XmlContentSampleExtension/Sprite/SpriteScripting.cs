using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class PositionList
    {
        public Vector2 position;
        public float rotation;
        public Vector2 scale;

        public PositionList() { }

        public class PositionListContentReader : ContentTypeReader<PositionList>
        {
            protected override PositionList Read(ContentReader input, PositionList MultiPosition)
            {
                PositionList PosList = new PositionList();
                PosList.position = input.ReadVector2();
                PosList.rotation = input.ReadInt32();
                PosList.scale = input.ReadVector2();
                return PosList;
            }
        }
    }
    public class SpriteScripting
    {
        public List<PositionList> Attributes;

        public string textureAsset;
        public double angleFactor;
        public double speedFactor;

        [ContentSerializerIgnore]
            public double angle;
        [ContentSerializerIgnore]
            public int index;
        [ContentSerializerIgnore]
            public int limit;


        public void Load(ContentManager content)
        {
            textureAsset = (Path.Combine(@"Achievements", textureAsset)); 
        }

        public class SpriteScriptingContentReader : ContentTypeReader<SpriteScripting>
        {
            protected override SpriteScripting Read(
                    ContentReader input,
                    SpriteScripting existingInstance)
            {
                SpriteScripting script = new SpriteScripting();

                script.Attributes = input.ReadObject<List<PositionList>>();
                script.textureAsset = input.ReadString();
                script.angleFactor = input.ReadDouble();
                script.speedFactor = input.ReadDouble();               

                script.Load(input.ContentManager);

                return script;

            }

        }


         
        

    }
}
