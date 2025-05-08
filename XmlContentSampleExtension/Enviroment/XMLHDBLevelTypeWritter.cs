using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using HDBLevelColBox = System.String;
using HDBLevelDef = System.String;
using BuildingDef = System.String;
using BuildingAsset = System.String;

namespace XmlContentExtension
{
    [ContentTypeWriter]
    public class HDBLevelColBoxTypeWritter : ContentTypeWriter<HDBLevelColBox>
    {
        protected override void Write(ContentWriter output, HDBLevelColBox value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.Position);
            output.Write(value.HalfExtents);
            output.Write(value.PitchYawRoll);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            //return "MyNamespace.MyContentReader, MyGameAssembly";
            return typeof(HDBLevelColBox.HDBLevelColBoxContentReader).AssemblyQualifiedName;
        }
    }

    [ContentTypeWriter]
    public class BuildingDefTypeWritter : ContentTypeWriter<BuildingDef>
    {
        protected override void Write(ContentWriter output, BuildingDef value)
        {
            output.Write(value.ModelPath);
            output.Write(value.ShaderPath);
            output.Write(value.DoorPath);
            output.Write(value.Height);
            output.Write(value.Scale);
            output.Write(value.OutlineScale);
            output.Write(value.ModelOffset);
            output.WriteObject<List<HDBLevelColBox>>(value.ColStore);
            output.WriteObject<HDBLevelColBox>(value.LiftSlot);
            output.WriteObject<List<HDBLevelColBox>>(value.HouseSlot);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BuildingDef.BuildingDefContentReader).AssemblyQualifiedName;
        }
    }

    [ContentTypeWriter]
    public class BuildingAssetTypeWritter : ContentTypeWriter<BuildingAsset>
    {
        protected override void Write(ContentWriter output, BuildingAsset value)
        {
            output.Write(value.VoidDeckDef);
            output.Write(value.LevelDef);
            output.Write(value.RoofDef);
            output.Write(value.LiftDef);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(BuildingAsset.BuildingAssetContentReader).AssemblyQualifiedName;
        }
    }
}
