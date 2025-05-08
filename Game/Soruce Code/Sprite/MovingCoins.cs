using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XmlContentExtension;
using Engine;

namespace Game
{
    class MovingCoins
    {
        List <SpriteScripting> SpriteScript;
        List <Sprite> sprite;

        public bool Visible;

        public void Load(string XMLFile)
        {
            Visible = false;
            SpriteScript = GameEngine.Content.Load<List<SpriteScripting>>(XMLFile);
            sprite = new List<Sprite>();

            if (SpriteScript.Count > 0)
            {
                for (int i = 0; i < SpriteScript.Count; i++)
                {
                    sprite.Add(new Sprite());
                    sprite[i].Position = SpriteScript[i].Attributes[0].position;
                    sprite[i].Position = new Vector2(GameEngine.GraphicDevice.Viewport.Width * (sprite[i].Position.X/1000)
                    , GameEngine.GraphicDevice.Viewport.Height * (sprite[i].Position.X / 1000));
                    sprite[i].Rotation = SpriteScript[i].Attributes[0].rotation;
                    sprite[i].Scale = SpriteScript[i].Attributes[0].scale;
                    sprite[i].TextureAsset = SpriteScript[i].textureAsset;
                    sprite[i].Load(GameEngine.Content);
                    SpriteScript[i].angle = 0;
                    SpriteScript[i].index = 0;
                    SpriteScript[i].limit = SpriteScript[i].Attributes.Count() - 1;
                }
            }             
        }
        public void Draw(SpriteBatch batch)
        {
            if (Visible)
            {
                batch.Begin();
                for (int i = 0; i < sprite.Count; i++)
                {
                    //sprite[i].Position = new Vector2(GameEngine.GraphicDevice.Viewport.Width * (0.25f + sprite[i].Position.X/1000)
                    //, GameEngine.GraphicDevice.Viewport.Height * 0.5f);
                    sprite[i].Scale = new Vector2(sprite[i].Scale.X * GameEngine.GraphicDevice.Viewport.Width / 1024, sprite[i].Scale.Y * GameEngine.GraphicDevice.Viewport.Height / 768);
                    batch.Draw(sprite[i].Texture, sprite[i].Position, null, Color.White, sprite[i].Rotation, new Vector2(0, 0), sprite[i].Scale, SpriteEffects.None, 0);
                }
                batch.End();
            }
        }
    
        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                for (int i = 0; i < SpriteScript.Count; i++)
                {
                    sprite[i].Rotation = SpriteScript[0].Attributes[SpriteScript[0].index].rotation;
                    sprite[i].Scale = SpriteScript[0].Attributes[SpriteScript[0].index].scale;
                    sprite[i].Position = new Vector2(sprite[i].Position.X + (float)SpriteScript[i].speedFactor / 180.0f * (GameEngine.GraphicDevice.Viewport.Width * (SpriteScript[i].Attributes[SpriteScript[i].limit].position.X - SpriteScript[i].Attributes[0].position.X) / 1000)
                    , (GameEngine.GraphicDevice.Viewport.Height * (SpriteScript[i].Attributes[SpriteScript[i].limit].position.Y / 1000)) - (float)Math.Sin(SpriteScript[i].angle) * 100 * (float)SpriteScript[i].angleFactor);


                    SpriteScript[i].angle += (float)SpriteScript[i].speedFactor / 180.0f * (float)Math.PI;


                    if (SpriteScript[i].angle > (float)Math.PI)
                    {
                        SpriteScript[i].angle = 0;
                        if (++SpriteScript[i].index > SpriteScript[i].limit - 1)
                        {
                            SpriteScript[i].index = 0;
                            sprite[i].Position = SpriteScript[i].Attributes[0].position;
                            sprite[i].Position = new Vector2(GameEngine.GraphicDevice.Viewport.Width * (sprite[i].Position.X / 1000)
                            , GameEngine.GraphicDevice.Viewport.Height * (sprite[i].Position.Y / 1000));
                        }
                    }

                }
            }
        }
    }
}
