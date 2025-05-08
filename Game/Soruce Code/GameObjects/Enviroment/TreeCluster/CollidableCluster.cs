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
    public class CollidableTreeCluster
    {
        int TotalTree = 5;
        List<PhysicsStaticObject> Trees = new List<PhysicsStaticObject>();
        Shadow gShadow = null;
        Vector3 CenterPt;

        public CollidableTreeCluster(   Vector3 Point, float RangeX, float RangeZ, IEContentManager ContentManager, GameScreen Parent,
                                        List<StaticObject> Asset, Shadow shadow)
        {
            gShadow = shadow;
            CenterPt = Point;
            if (RangeX > 10)
                TotalTree += (int)(RangeX / 5.0f);
            if (RangeZ > 10)
                TotalTree += (int)(RangeZ / 5.0f);
            if (TotalTree < 6)
                TotalTree = GameEngine.RandomValue.Next(2, 5);
            else
                TotalTree = GameEngine.RandomValue.Next((int)RangeX / 10 + (int)RangeZ / 10, TotalTree);

            float RandomX, RandomZ;
            
            for (int i = 0; i < TotalTree; i++)
            {
                int select = GameEngine.RandomValue.Next(0, VariableAsset.Trees.Count);
                Trees.Add(new PhysicsStaticObject(Parent));
                Trees[Trees.Count - 1].mStaticObj = new StaticObject(Asset[select].GetModel(), Vector3.Zero);
                Trees[Trees.Count - 1].mStaticObj.camera = Asset[select].camera;
                Trees[Trees.Count - 1].mStaticObj.Shader = Asset[select].Shader.Clone(GameEngine.GraphicDevice);
              
                Trees[Trees.Count - 1].ManuelUpdate = true;
                Vector3 Extents = GraphicUtility.LargestHalfExtent(Trees[Trees.Count - 1].mStaticObj.GetModel(), true);
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

                Trees[Trees.Count - 1].SetBoundingPhysicsBasedOnModel(newPoint, 0.5f, Vector3.Zero, true);
                Trees[Trees.Count - 1].mStaticObj.ViewRange = MathHelper.Max(MathHelper.Max(Extents.X, Extents.Z), Extents.Y);
                Trees[Trees.Count - 1].Rotation = Matrix.CreateRotationY(MathHelper.ToRadians((float)GameEngine.RandomValue.Next(0, 360)));
                Trees[Trees.Count - 1].Scale = new Vector3(1, GameEngine.RandomValue.Next(5, 15) * 0.1f, 1);
                Trees[Trees.Count - 1].mStaticObj.CenterOffset = new Vector3(0, -Extents.Y * 0.5f, 0);
                Trees[Trees.Count - 1].Position = newPoint;
                Trees[Trees.Count - 1].mPhyObj.Immovable = true;
                Trees[Trees.Count - 1].mPhyObj.CollisionSkin.callbackFn += new JigLibX.Collision.CollisionCallbackFn(FilterCol);
                Trees[Trees.Count - 1].mStaticObj.DisplayShadow = false;
            }
        }

        public CollidableTreeCluster(Vector3 Point, float RangeX, float RangeZ, IEContentManager ContentManager, GameScreen Parent)
        {
            gShadow = GameEngine.Services.GetService<Shadow>();
            CenterPt = Point;
            if (RangeX > 10)
                TotalTree += (int)(RangeX / 5.0f);
            if (RangeZ > 10)
                TotalTree += (int)(RangeZ / 5.0f);
            if (TotalTree < 6)
                TotalTree = GameEngine.RandomValue.Next(2, 5);
            else
                TotalTree = GameEngine.RandomValue.Next((int)RangeX / 8 + (int)RangeZ / 8, TotalTree);

            float RandomX, RandomZ;

            for (int i = 0; i < TotalTree; i++)
            {
                Trees.Add(new PhysicsStaticObject(Parent));
                //Trees[Trees.Count - 1].Parent.Components.Remove(Trees[Trees.Count - 1]);    // Do manuelly
                Trees[Trees.Count - 1].Setup(VariableAsset.Trees[
                            GameEngine.RandomValue.Next(0, VariableAsset.Trees.Count)],
                            "Content/Shader Fx/CelFx", ContentManager);
                Trees[Trees.Count - 1].ManuelUpdate = true;
                Vector3 Extents = GraphicUtility.LargestHalfExtent(Trees[Trees.Count - 1].mStaticObj.GetModel(), true);

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

                Trees[Trees.Count - 1].SetBoundingPhysicsBasedOnModel(newPoint, 0.5f, Vector3.Zero, true);
                Trees[Trees.Count - 1].mStaticObj.ViewRange = MathHelper.Max(MathHelper.Max(Extents.X, Extents.Z), Extents.Y);
                Trees[Trees.Count - 1].Rotation = Matrix.CreateRotationY(MathHelper.ToRadians((float)GameEngine.RandomValue.Next(0, 360)));
                Trees[Trees.Count - 1].Scale = new Vector3(1, GameEngine.RandomValue.Next(5, 15) * 0.1f, 1);
                Trees[Trees.Count - 1].mStaticObj.CenterOffset = new Vector3(0, -Extents.Y * 0.5f ,0);
                Trees[Trees.Count - 1].Position = newPoint;
                Trees[Trees.Count - 1].mPhyObj.Immovable = true;
                Trees[Trees.Count - 1].mPhyObj.CollisionSkin.callbackFn += new JigLibX.Collision.CollisionCallbackFn(FilterCol);
                Trees[Trees.Count - 1].mStaticObj.DisplayShadow = false;
            }
        }
        public bool FilterCol(JigLibX.Collision.CollisionSkin skin0, JigLibX.Collision.CollisionSkin skin1)
        {
            switch((ObjectFilter)skin1.GetMaterialID(0))
            {
                case ObjectFilter.PLAYER:
                case ObjectFilter.CHARACTER:
                    return true;
                default:
                    return false;
            }
        }
        public void Disable()
        {
            for (int i = 0; i < Trees.Count; i++)
            {
                Trees[i].DisableComponent();
                Trees[i].mPhyObj.Body.DisableBody();
                Trees[i].mStaticObj.Shader.Dispose();
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
                Trees[i].Update();
        }

        public void Draw()
        {
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW) return;
            for (int i = 0; i < Trees.Count; i++)
            {
                GameEngine.GraphicDevice.RenderState.DepthBias = -0.0000001f * i;
                if (Trees[i].Visible)
                    gShadow.Draw(Trees[i].mStaticObj.Position + Trees[i].mStaticObj.CenterOffset, Vector3.Zero, Vector3.One * Trees[i].mStaticObj.ViewRange * 2.0f, Trees[i].Rotation);
            }
            /*for (int i = 0; i < Trees.Count; i++)
                if (Trees[i].Visible)
                    Trees[i].Draw();*/
            GameEngine.GraphicDevice.RenderState.DepthBias = 0;
        }
    }
}
