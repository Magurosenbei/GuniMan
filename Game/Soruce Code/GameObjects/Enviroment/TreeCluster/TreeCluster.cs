using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;
using XmlContentExtension;


namespace Game
{
    public class TreeCluster
    {
        int TotalTree = 5;
        List <StaticObject> Trees = new List<StaticObject>();
        Shadow gShadow = null;
        Vector3 CenterPt;

        public TreeCluster(Vector3 Point, float RangeX, float RangeZ, List<StaticObject> Asset, Shadow shadow)
        {
            gShadow = shadow;
            CenterPt = Point;
            if (RangeX > 10)
                TotalTree += (int)(RangeX / 3.0f);
            if (RangeZ > 10)
                TotalTree += (int)(RangeZ / 3.0f);
            if (TotalTree < 6)
                TotalTree = GameEngine.RandomValue.Next(2, 5);
            else
                TotalTree = GameEngine.RandomValue.Next((int)RangeX / 5 + (int)RangeZ / 5, TotalTree);

            float RandomX, RandomZ;

            for (int i = 0; i < TotalTree; i++)
            {
                int select = GameEngine.RandomValue.Next(0, VariableAsset.Trees.Count);
                Vector3 Extents = GraphicUtility.LargestHalfExtent(Asset[select].GetModel(), true);
            Regen:
                RandomX = (float)GameEngine.RandomValue.Next(-(int)(RangeX), (int)(RangeX));
                RandomZ = (float)GameEngine.RandomValue.Next(-(int)(RangeZ), (int)(RangeZ));

                if (RandomZ - Extents.Z * 0.5f < -RangeZ || RandomZ + Extents.Z * 0.5f > RangeZ)
                    goto Regen;
                if (RandomX - Extents.X * 0.5f < -RangeX || RandomX + Extents.X * 0.5f > RangeX)
                    goto Regen;
                Vector3 newPoint = new Vector3(RandomX, 0, RandomZ) + Point;

                for (int j = 0; j < Trees.Count; j++)
                    if ((newPoint - Trees[j].Position).LengthSquared() < 25.0f)
                        goto Regen;

                Trees.Add(new StaticObject(Asset[select].GetModel(), newPoint));
                Trees[Trees.Count - 1].Rotation = Matrix.CreateRotationY(MathHelper.ToRadians((float)GameEngine.RandomValue.Next(0, 360)));
                Trees[Trees.Count - 1].camera = Asset[select].camera;
                Trees[Trees.Count - 1].ViewRange = MathHelper.Max(MathHelper.Max(Extents.X, Extents.Z), Extents.Y);
                Trees[Trees.Count - 1].Shader = Asset[select].Shader.Clone(GameEngine.GraphicDevice);
                Trees[Trees.Count - 1].Scale = new Vector3(1, GameEngine.RandomValue.Next(5, 15) * 0.1f, 1);
                Trees[Trees.Count - 1].CenterOffset = Vector3.Zero;
            }
        }
        public TreeCluster(Vector3 Point, float RangeX, float RangeZ, IEContentManager ContentManager)
        {
            gShadow = GameEngine.Services.GetService<Shadow>();
            CenterPt = Point;
            if (RangeX > 10)
                TotalTree += (int)(RangeX / 2.0f);
            if (RangeZ > 10)
                TotalTree += (int)(RangeZ / 2.0f);
            if(TotalTree < 6)
                TotalTree = GameEngine.RandomValue.Next(2, 5);
            else
                TotalTree = GameEngine.RandomValue.Next((int)RangeX / 5 + (int)RangeZ / 5, TotalTree);

            float RandomX, RandomZ;

            for (int i = 0; i < TotalTree; i++)
            {
                Model MDL = ContentManager.Load<Model>(
                            VariableAsset.Trees[
                            GameEngine.RandomValue.Next(0, VariableAsset.Trees.Count)]);

                Vector3 Extents = GraphicUtility.LargestHalfExtent(MDL, true);

            Regen:
                RandomX = (float)GameEngine.RandomValue.Next(-(int)(RangeX), (int)(RangeX));
                RandomZ = (float)GameEngine.RandomValue.Next(-(int)(RangeZ), (int)(RangeZ));

                if (RandomZ - Extents.Z * 0.5f < -RangeZ || RandomZ + Extents.Z * 0.5f > RangeZ)
                    goto Regen;
                if (RandomX - Extents.X * 0.5f < -RangeX || RandomX + Extents.X * 0.5f > RangeX)
                    goto Regen;
                Vector3 newPoint = new Vector3(RandomX, 0, RandomZ) + Point;

                for (int j = 0; j < Trees.Count; j++)
                    if ((newPoint - Trees[j].Position).LengthSquared() < 25.0f)
                        goto Regen;
           
                Trees.Add(new StaticObject(MDL, newPoint));
                Trees[Trees.Count - 1].Rotation = Matrix.CreateRotationY(MathHelper.ToRadians((float)GameEngine.RandomValue.Next(0, 360)));

                Trees[Trees.Count - 1].ViewRange = MathHelper.Max(MathHelper.Max(Extents.X, Extents.Z), Extents.Y);
                Trees[Trees.Count - 1].LoadShader("Content/Shader Fx/CelFx", ContentManager);
                //Trees[Trees.Count - 1].OutlineScale = new Vector3(1.02f, 1.005f, 1.02f);
                Trees[Trees.Count - 1].Scale = new Vector3(1, GameEngine.RandomValue.Next(5, 15) * 0.1f, 1);
                Trees[Trees.Count - 1].CenterOffset = Vector3.Zero;
            }
        }

        public void RotateWholeSale(Matrix Rotation)
        {
            for (int i = 0; i < Trees.Count; i++)
            {
                Trees[i].Position = (Matrix.CreateTranslation(Trees[i].Position - CenterPt) * Rotation).Translation + CenterPt;
            }
        }
        public void MoveWholeSale(Vector3 Position)
        {
            Vector3 OldPt = CenterPt;
            for (int i = 0; i < Trees.Count; i++)
            {
                Trees[i].Position = Trees[i].Position - OldPt + Position;
            }
            CenterPt = Position;
        }

        public void Update()
        {
            for (int i = 0; i < Trees.Count; i++)
            {
                //Trees[i].Update();
                float blend = Trees[i].Color.W;
                float d = 0.0f;
                Trees[i].Cullable = Trees[i].camera.InView(Trees[i].Position, Trees[i].ViewRange * 2, out blend, out d);
                Trees[i].Color.W = blend;
            }
        }

        public void Draw()
        {
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW) return;
            for (int i = 0; i < Trees.Count; i++)
            {
                GameEngine.GraphicDevice.RenderState.DepthBias = -0.0000001f * i;
                if (Trees[i].Cullable)
                    gShadow.Draw(Trees[i].Position, Vector3.Zero, Vector3.One * Trees[i].ViewRange * 2.0f, Trees[i].Rotation);
            }
            GameEngine.GraphicDevice.RenderState.DepthBias = -0.000001f;
            for (int i = 0; i < Trees.Count; i++)
                if (Trees[i].Cullable)
                    Trees[i].Draw("Normal");
            GameEngine.GraphicDevice.RenderState.DepthBias = 0;
        }
    }
}
