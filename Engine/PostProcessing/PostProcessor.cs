using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Engine
{
    public class PostProcessor : Component, I2DComponent
    {
        public Effect Effect;

        protected int Width, Height;

        public Texture2D input; // the image to be processed

        public Texture2D Input
        {
            get { return input; }
            set
            {
                input = value;
                if (Effect.Parameters["InputTexture"] != null)
                    Effect.Parameters["InputTexture"].SetValue(value);
            }
        }

        public Rectangle Rectangle { get { return new Rectangle(0, 0, Width, Height); } set { this.Width = value.Width; this.Height = value.Height; } }
        public PostProcessor()
            : base()
        {
            Setup(null, GameEngine.GraphicDevice.Viewport.Width, GameEngine.GraphicDevice.Viewport.Height);
        }
        public PostProcessor(Effect fx, int w, int h)
            :base()
        {
            Setup(fx, w, h);
        }
        public PostProcessor(Effect fx, int w, int h, GameScreen Parent)
            : base(Parent)
        {
            Setup(fx, w, h);
        }
        void Setup(Effect fx, int width, int height)
        {
            this.Effect = fx;
            Input = new Texture2D(GameEngine.GraphicDevice, 1, 1);
            Input.SetData<Color>(new Color[]{Color.White});
            this.Width = width;
            this.Height = height;
        }

        public void GetInputFromFrameBuffer()
        {
            if (!(Input is ResolveTexture2D))
                Input = GraphicUtility.CreateResolveTexture();
            GameEngine.GraphicDevice.ResolveBackBuffer((ResolveTexture2D)Input);
        }
        public override void Draw()
        {
            GameEngine.GraphicDevice.Clear(Color.Black);
            GameEngine.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            Effect.Begin();
            foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                GameEngine.SpriteBatch.Draw(Input, Rectangle, Color.White);
                pass.End();
            }
            Effect.End();
            GameEngine.SpriteBatch.End();
            base.Draw();
        }
    }
}
