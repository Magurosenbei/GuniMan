using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace Game
{
    public class GaussianBlur : PostProcessor
    {
        Vector2[] sampleOffsetsH = new Vector2[15];
        Vector2[] sampleOffsetsV = new Vector2[15];

        float[] sampleWeightsH = new float[15];
        float[] sampleWeightsV = new float[15];

        float intensity = 0.8f;
        //float intensity = 10.8f;
        public GaussianBlur(int Width, int Height)
            : base(GameEngine.Content.Load<Effect>("Content/Shader Fx/GaussianBlur"), Width, Height)
        {
            Setup(Width, Height);
        }

        public GaussianBlur(int Width, int Height, GameScreen Parent)
            : base(GameEngine.Content.Load<Effect>("Content/Shader Fx/GaussianBlur"), Width, Height, Parent)
        {
            Setup(Width, Height);
        }

        void Setup(int Width, int Height)
        {
            DrawOrder = 0;
            Vector2 texelSize = new Vector2(1f / Width, 1f / Height);

            SetBlurParameters(texelSize.X, 0, ref sampleOffsetsH, ref sampleWeightsH);
            SetBlurParameters(0, texelSize.Y, ref sampleOffsetsV, ref sampleWeightsV);
        }

        public float Intensity
        {
            get { return intensity; }
            set
            {
                intensity = value;
                if (intensity < 0) intensity = 0;
                Vector2 texelSize = new Vector2(1f / base.Width, 1f / base.Height);
                SetBlurParameters(texelSize.X, 0, ref sampleOffsetsH, ref sampleWeightsH);
                SetBlurParameters(0, texelSize.Y, ref sampleOffsetsV, ref sampleWeightsV);
            }
        }
        void SetBlurParameters(float dx, float dy, ref Vector2[] vSampleOffsets, ref float[] fSampleWeights)
        {
            // The first sample always has a zero offset.
            fSampleWeights[0] = ComputeGaussian(0);
            vSampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = fSampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < 15 / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                fSampleWeights[i * 2 + 1] = weight;
                fSampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                vSampleOffsets[i * 2 + 1] = delta;
                vSampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < fSampleWeights.Length; i++)
                fSampleWeights[i] /= totalWeights;
        }

        private float ComputeGaussian(float n)
        {
            float theta = intensity + float.Epsilon;
            return theta = (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) * Math.Exp(-(n * n) / (2 * theta * theta)));
        }

        public void Draw(GaussianBlurDirection Direction, Texture2D Input)
        {
            this.Input = Input;
            SetParameters(Direction);
            base.Draw();
        }

        public override void Draw()
        {
            GetInputFromFrameBuffer(); // Set Input texture
            GameEngine.GraphicDevice.Clear(Color.Black);
            SetParameters(GaussianBlurDirection.Horizontal); // Set horizontal parameters
            base.Draw(); // Apply blur

            GetInputFromFrameBuffer(); // Set Input texture again
            GameEngine.GraphicDevice.Clear(Color.Black);
            SetParameters(GaussianBlurDirection.Vertical); // Set vertical parameters
            base.Draw(); // Apply blur
        }

        void SetParameters(GaussianBlurDirection Direction)
        {
            if (Direction == GaussianBlurDirection.Horizontal)
            {
                Effect.Parameters["sampleWeights"].SetValue(sampleWeightsH);
                Effect.Parameters["sampleOffsets"].SetValue(sampleOffsetsH);
            }
            else
            {
                Effect.Parameters["sampleWeights"].SetValue(sampleWeightsV);
                Effect.Parameters["sampleOffsets"].SetValue(sampleOffsetsV);
            }
        }
        public enum GaussianBlurDirection { Horizontal, Vertical };
    }
}
