using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class HDBLevelColBox
    {
        public Vector3 Position;
        public Vector3 HalfExtents;
        public Vector3 PitchYawRoll;

        public HDBLevelColBox() { }

        public class HDBLevelColBoxContentReader : ContentTypeReader<HDBLevelColBox>
        {
            protected override HDBLevelColBox Read(ContentReader input, HDBLevelColBox HDBLvl)
            {
                HDBLevelColBox Box = new HDBLevelColBox();
                Box.Position = input.ReadVector3();
                Box.HalfExtents = input.ReadVector3();
                Box.PitchYawRoll = input.ReadVector3();
                return Box;
            }
        }
    }

    public class BuildingDef
    {
        public string ModelPath;
        public string ShaderPath;
        public string DoorPath;

        public double   Height;
        public Vector3  Scale;
        public Vector3  OutlineScale;
        public Vector3  ModelOffset;

        public List <HDBLevelColBox> ColStore;   // Stores Collision Primitives
        public HDBLevelColBox LiftSlot;
        public List <HDBLevelColBox> HouseSlot;
        public BuildingDef() { }

        public class BuildingDefContentReader : ContentTypeReader<BuildingDef>
        {
            protected override BuildingDef Read(ContentReader input, BuildingDef Building)
            {
                if (Building == null)
                    Building = new BuildingDef();
                Building.ModelPath = input.ReadString();
                Building.ShaderPath = input.ReadString();
                Building.DoorPath = input.ReadString();
                Building.Height = input.ReadDouble();
                Building.Scale = input.ReadVector3();
                Building.OutlineScale = input.ReadVector3();
                Building.ModelOffset = input.ReadVector3();
                Building.ColStore = input.ReadObject<List<HDBLevelColBox>>();
                Building.LiftSlot = input.ReadObject<HDBLevelColBox>();
                Building.HouseSlot = input.ReadObject<List<HDBLevelColBox>>();
                return Building;
            }
        }
    }
    /* Contains Path Def for Parts */
    public class BuildingAsset
    {
        public string VoidDeckDef;
        public string LevelDef;
        public string RoofDef;
        public string LiftDef;
        public class BuildingAssetContentReader : ContentTypeReader<BuildingAsset>
        {
            protected override BuildingAsset Read(ContentReader input, BuildingAsset existingInstance)
            {
                if(existingInstance == null)
                    existingInstance = new BuildingAsset();
                existingInstance.VoidDeckDef = input.ReadString();
                existingInstance.LevelDef = input.ReadString();
                existingInstance.RoofDef = input.ReadString();
                existingInstance.LiftDef = input.ReadString();
                return existingInstance;
            }
        }
    }
}
