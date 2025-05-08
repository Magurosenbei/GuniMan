using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Engine;


namespace Game
{
    public class Pickable
    {
        Vector3 Position = Vector3.Zero;
        Vector3 RandomAngular = Vector3.Zero;
        float   RangeEffect = 4.0f;    // squared
        public float   ViewRange = 25.0f;
        float HoverValue;
        float HoverFact = 0.5f;

        public int Id = 0;
        public int Amt = 0;
        public bool Visible = false;
        public bool PickedUp = false;
        public bool Remove = false;
        public bool Respawn = false;

        public TimeSpan RespawnElapsed = new TimeSpan();
        public TimeSpan RespawnWait = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(30, 45));
        public float ModelAlpha = 0.0f;
        StaticObject Model;
        public float DistanceFromCamera = 0.0f;
        List <ParticleEmitor> Emitors = new List<ParticleEmitor>();  

        public Pickable() { }
        public void Setup(Vector3 Position, IEContentManager ContentManager)
        {
            Emitors.Add(new ParticleEmitor(50, false, ContentManager));
            Emitors[0].Position = Position;
            Emitors.Add(new DanmakuStyle_Spiral(ContentManager));
            Emitors[1].Position = Position;

            Model = new StaticObject(ContentManager.Load<Model>(VariableAsset.Pickables[GameEngine.RandomValue.Next(0, VariableAsset.Pickables.Count)]), Vector3.Zero);
            Model.Scale = Vector3.One;
            //Model.OutlineScale = new Vector3(1.03f, 1.03f, 1.03f);
            Model.CenterOffset = new Vector3(0, -0.5f, 0);
            Model.LoadShader("Content/Shader Fx/CelFx", ContentManager);
            RandomAngular = new Vector3((float)GameEngine.RandomValue.Next(-3, 3), (float)GameEngine.RandomValue.Next(-3, 3), (float)GameEngine.RandomValue.Next(-3, 3));
            RandomAngular *= 0.01f;
            HoverFact = GameEngine.RandomValue.Next(1, 10) * 0.1f;
            Model.Color.W = 0;
        }
        public bool InRange(Vector3 PlayerPosition, float PlayerRange)
        {
            if((PlayerPosition - Position).LengthSquared() > (PlayerRange + RangeEffect) * (PlayerRange + RangeEffect))
                return false;   // Not in place at all
            return true;
        }
        public void SetPosition(Vector3 Position)
        {
            this.Position = Position;
            for (int i = 0; i < Emitors.Count; i++)
                Emitors[i].Position = Position;
        }
        public Vector3 GetPosition() { return this.Position; }
        public void Reset()
        {
            for (int i = 0; i < Emitors.Count; i++)
            {
                Emitors[i].StopSpawn = false;
                Emitors[i].Done = false;
            }
        }
        public void Update()
        {
            if (!Visible) return;
            if (PickedUp)
            {
                RespawnElapsed += GameEngine.GameTime.ElapsedGameTime;
                if (RespawnElapsed > RespawnWait && Emitors[1].Done)
                    Respawn = true; 
                // Temp
                Emitors[0].StopSpawn = true;
                Emitors[0].Update();
                Emitors[1].Update();
                if (Emitors[1].Done)
                    Remove = true;
            }
            else
            {
                Emitors[0].Update();
                if (Visible && ModelAlpha < 1.0f)
                {
                    ModelAlpha += 0.5f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                    Model.Color.W = ModelAlpha;
                }
                else
                    Emitors[0].StopSpawn = false;
            }

            Model.Position = Position + new Vector3(0, (float)Math.Sin(HoverValue), 0) * 0.5f;
            HoverValue += (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds * HoverFact;
            if (HoverValue > 2.0f * Math.PI)
                HoverValue = 0;
            else if (HoverValue < 0)
                HoverValue = 2.0f * (float)Math.PI;

            Vector3 tmp = RandomAngular * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
            Model.Rotation *= Matrix.CreateFromYawPitchRoll(RandomAngular.Y, RandomAngular.X, RandomAngular.Z);
            Model.CalculatedOffset = (Matrix.CreateTranslation(Model.CenterOffset) * Model.Rotation).Translation;
        }
        public void Draw()
        {
            if(!PickedUp)
                Model.Draw("Normal");
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW) return;
            //for (int i = 0; i < Emitors.Count; i++)
            Emitors[0].Draw("Normal");
            Emitors[1].Draw("Normal");
        }
        public void Disable()
        {
            Model.Shader.Dispose();
            for (int i = 0; i < Emitors.Count; i++)
                Emitors[i].Disable();
        }
    }
}
