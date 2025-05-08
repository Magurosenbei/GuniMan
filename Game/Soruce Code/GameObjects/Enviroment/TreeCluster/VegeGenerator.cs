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
    public class VegetationGenerator : Component
    {
        List<TreeCluster> Clusters = new List<TreeCluster>();
        List<CollidableTreeCluster> ColliCluster = new List<CollidableTreeCluster>();

        public VegetationGenerator(List<HDBLevelColBox> Areas, IEContentManager ContentManger) : base() { Generate(Areas, ContentManger); this.Parent = GameEngine.DefaultScreen; }
        public VegetationGenerator(List<HDBLevelColBox> Areas, IEContentManager ContentManger, GameScreen Parent) : base(Parent) { Generate(Areas, ContentManger); this.Parent = Parent; }

        public void Generate(List<HDBLevelColBox> Areas, IEContentManager ContentManger)
        {
            Shadow shadow = GameEngine.Services.GetService<Shadow>();
            List <StaticObject> Asset = new List<StaticObject>();
            for(int i = 0; i < VariableAsset.Trees.Count; i++)
            {
                Model mdl = ContentManger.Load<Model>(VariableAsset.Trees[i]);
                Asset.Add(new StaticObject(mdl, Vector3.Zero));
                Asset[i].LoadShader("Content/Shader Fx/CelFx", ContentManger);
                Asset[i].camera = GameEngine.Services.GetService<Camera>();
            }
            Clusters.Capacity = Areas.Count;
            Parallel.For(0, Areas.Count, delegate(int i)
            {
                //Clusters.Add(new TreeCluster(Areas[i].Position, Areas[i].HalfExtents.X, Areas[i].HalfExtents.Z, ContentManger));
                Clusters.Add(new TreeCluster(Areas[i].Position, Areas[i].HalfExtents.X, Areas[i].HalfExtents.Z, Asset, shadow));
                Clusters[Clusters.Count - 1].RotateWholeSale(Matrix.CreateFromYawPitchRoll(Areas[i].PitchYawRoll.Y, Areas[i].PitchYawRoll.X, Areas[i].PitchYawRoll.Z));
            });
            Asset.Clear();
        }
        public void GenerateColliCluster(List<HDBLevelColBox> Areas, IEContentManager ContentManger, GameScreen Parent)
        {
            Shadow shadow = GameEngine.Services.GetService<Shadow>();
            List<StaticObject> Asset = new List<StaticObject>();
            for (int i = 0; i < VariableAsset.Trees.Count; i++)
            {
                Model mdl = ContentManger.Load<Model>(VariableAsset.Trees[i]);
                Asset.Add(new StaticObject(mdl, Vector3.Zero));
                Asset[i].LoadShader("Content/Shader Fx/CelFx", ContentManger);
                Asset[i].camera = GameEngine.Services.GetService<Camera>();
            }
            ColliCluster.Capacity = Areas.Count;
            Parallel.For(0, Areas.Count, delegate(int i)
            {
                //ColliCluster.Add(new CollidableTreeCluster(Areas[i].Position, Areas[i].HalfExtents.X, Areas[i].HalfExtents.Z, ContentManger, Parent));
                ColliCluster.Add(new CollidableTreeCluster(Areas[i].Position, Areas[i].HalfExtents.X, Areas[i].HalfExtents.Z, ContentManger, Parent, Asset, shadow));
                ColliCluster[ColliCluster.Count - 1].RotateWholeSale(Matrix.CreateFromYawPitchRoll(Areas[i].PitchYawRoll.Y, Areas[i].PitchYawRoll.X, Areas[i].PitchYawRoll.Z));
            });
            Asset.Clear();
        }
        public override void Update()
        {
            int total = (int)MathHelper.Max(Clusters.Count, ColliCluster.Count);
            Parallel.For(0, total, delegate(int i)
            {
                if (i < Clusters.Count)
                    Clusters[i].Update();
                if (i < ColliCluster.Count)
                    ColliCluster[i].Update();
            });
            /*for (int i = 0; i < total; i++)
            {
                if (i < Clusters.Count)
                    Clusters[i].Update();
                if (i < ColliCluster.Count)
                    ColliCluster[i].Update();
            }*/
            base.Update();
        }
        public override void Draw()
        {
            int total = (int)MathHelper.Max(Clusters.Count, ColliCluster.Count);

            for (int i = 0; i < total; i++)
            {
                if (i < Clusters.Count)
                    Clusters[i].Draw();
                if (i < ColliCluster.Count)
                    ColliCluster[i].Draw();
            }
            base.Draw();
        }
        public override void DisableComponent()
        {
            Clusters.Clear();
            for (int i = 0; i < ColliCluster.Count; i++)
                ColliCluster[i].Disable();
            ColliCluster.Clear();
            base.DisableComponent();
        }
    }
}
