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
    public class Sprite
    {
        Vector2 position;
        float rotation;
        Vector2 scale;

        string textureAsset;
        Texture2D texture;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public string TextureAsset
        {
            get { return textureAsset; }
            set { textureAsset = value; }
        }

        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return texture; }
        }

        public void Load(ContentManager content)
        {
            texture = content.Load<Texture2D>(Path.Combine(@"Content/Textures", textureAsset));
        }
        public class SpriteContentReader : ContentTypeReader<Sprite>
        {
            protected override Sprite Read(
                    ContentReader input,
                    Sprite existingInstance)
            {
                Sprite sprite = new Sprite();

                sprite.Position = input.ReadVector2();
                sprite.Rotation = input.ReadSingle();
                sprite.Scale = input.ReadVector2();
                sprite.TextureAsset = input.ReadString();

                sprite.Load(input.ContentManager);

                return sprite;
            }

        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(
                texture,
                position,
                null,
                Color.White,
                rotation,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
    }

}
