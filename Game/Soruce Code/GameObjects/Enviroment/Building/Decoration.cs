using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine;
using XmlContentExtension;

namespace Game
{
    public class FixedDecoration : Decoration
    {
        public FixedDecoration(IEContentManager m, GameScreen parent) : base(m, parent, false)
        {
        }
    }
    public class Decoration : Component
    { 
        List<PhysicsStaticObject> cubes;
        List<HDBLevelColBox> PlantPlotting;
        List<int> added;
        public bool randomZone;
        int limit;

        public Decoration(IEContentManager m, GameScreen parent) : base(parent)
        {
            randomZone = true;
            Load(m);
        }
        public Decoration(IEContentManager m, GameScreen parent, bool RandomZone) : base(parent)
        {
            randomZone = RandomZone;
            Load(m);
        }
        public Decoration(IEContentManager m)
            : base()
        {
            Load(m);
        }
        //public void Load(List<HDBLevelColBox> Def, IEContentManager ContentManager)
        public void Load(IEContentManager contentMan)
        {
            //PlantPlotting = new List<HDBLevelColBox>();
            cubes = new List<PhysicsStaticObject>();
            added = new List<int>();
            if (randomZone)
            {
                PlantPlotting = contentMan.Load<List<HDBLevelColBox>>("Content/Scripts/PlantPlotScript");
                if (PlantPlotting.Count() > 0)
                    limit = GameEngine.RandomValue.Next(1, PlantPlotting.Count());
                else limit = 0;
            }
            else 
            {
                PlantPlotting = contentMan.Load<List<HDBLevelColBox>>("Content/Scripts/FixedDecoration");
                limit = PlantPlotting.Count();
            }

            int RandomValue;
            added.Capacity = cubes.Capacity = limit;
            for (int a = 0; a < limit; a++)
            {
                if (randomZone)
                {
                    RandomValue = GameEngine.RandomValue.Next(0, PlantPlotting.Count());
                ReGen:
                    for (int i = 0; i < added.Count; i++)
                    {
                        if (added[i] == RandomValue)  // if added from list before?
                        {
                            RandomValue = GameEngine.RandomValue.Next(0, PlantPlotting.Count());
                            goto ReGen;
                        }
                    }
                }
                else
                    RandomValue = a;

                added.Add(RandomValue);
                cubes.Add(new PhysicsStaticObject(Parent));

                string Model;
                // PitchYawRoll.Z == ModelIndex due to troublesome constrains, and it is not really put to much use =p 
                int index = (int)(PlantPlotting[RandomValue].PitchYawRoll.Z);
                if (index < VariableAsset.Deco.Count())
                    Model = VariableAsset.Deco[index];
                else Model = VariableAsset.Deco[VariableAsset.Deco.Count - 1];
                //string Model = VariableAsset.Deco[GameEngine.RandomValue.Next(3, VariableAsset.Deco.Count)];
                cubes[a].Setup(Model, "Content/Shader Fx/CelFx", contentMan);

                Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(cubes[a].mStaticObj.GetModel(), true);

                cubes[a].mStaticObj.Scale = new Vector3(1, 1, 1);
                //cubes[a].mStaticObj.OutlineScale = new Vector3(1.03f, 1.005f, 1.03f);
                cubes[a].mStaticObj.Cullable = true;
                
                int randX = GameEngine.RandomValue.Next(-(int)PlantPlotting[RandomValue].HalfExtents.X, (int)PlantPlotting[RandomValue].HalfExtents.X);
                //int randY = GameEngine.RandomValue.Next(-(int)PlantPlotting[RandomValue].HalfExtents.Y, (int)PlantPlotting[RandomValue].HalfExtents.Y);
                int randZ = GameEngine.RandomValue.Next(-(int)PlantPlotting[RandomValue].HalfExtents.Z, (int)PlantPlotting[RandomValue].HalfExtents.Z);
                Vector3 HalfExtentPosition = new Vector3(randX, 0, randZ);

                PlantPlotting[RandomValue].Position += HalfExtentPosition;

               
                cubes[a].SetBoundingPhysicsBasedOnModel(PlantPlotting[RandomValue].Position,
                             1.0f, new Vector3(PlantPlotting[RandomValue].PitchYawRoll.X, PlantPlotting[RandomValue].PitchYawRoll.Y, 0), true, (int)ObjectFilter.DECOS);

                if (Model == "Content/LightPost/StreetLamp/StreetLamp")
                {
                    cubes[a].mPhyObj.CollisionSkin.RemoveAllPrimitives();
                    cubes[a].mPhyObj.CollisionSkin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(PlantPlotting[RandomValue].Position, new Vector3(1.0f, 10.0f, 1.0f), Matrix.Identity), (int)ObjectFilter.DECOS,new JigLibX.Collision.MaterialProperties());
                }
                cubes[a].mStaticObj.CenterOffset = new Vector3(0, -HalfExtents.Y, 0);
                cubes[a].mStaticObj.ViewRange = MathHelper.Max(HalfExtents.Y, MathHelper.Max(HalfExtents.X, HalfExtents.Z));
                //cubes[a].mPhyObj.Body.MoveTo(PlantPlotting[RandomValue].Position, Matrix.CreateFromYawPitchRoll(PlantPlotting[RandomValue].PitchYawRoll.Y, PlantPlotting[RandomValue].PitchYawRoll.X, 0));
                cubes[a].mPhyObj.SetMass(HalfExtents.LengthSquared());
                cubes[a].Visible = true;
                cubes[a].mPhyObj.Immovable = true;
                cubes[a].mPhyObj.Body.EnableBody();
                
            }
            PlantPlotting.Clear();
        }
        public override void Draw()
        {
            base.Draw();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void DisableComponent()
        {
            for (int a = 0; a < cubes.Count(); a++)
            {
                cubes[a].DisableComponent();
                cubes[a].mPhyObj.Body.DisableBody();
            }
            cubes.Clear();
            //PlantPlotting.Clear();
            base.DisableComponent();
        }
    }
}
