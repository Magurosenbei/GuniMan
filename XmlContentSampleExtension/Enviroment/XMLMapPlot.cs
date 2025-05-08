using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using XMLMapPlot = System.String;
using Point = System.String;
using Engine;
namespace XmlContentExtension
{
    public class Point
    {
        public Vector3 Position;
        public Vector3 PitchYawRoll;
        public Point() { }
        public class PointContentReader : ContentTypeReader<Point>
        {
            protected override Point Read(ContentReader input, Point pt)
            {
                if (pt == null)
                    pt = new Point();
                pt.Position = input.ReadVector3();
                pt.PitchYawRoll = input.ReadVector3();
                return pt;
            }
        }
    }
    [ContentTypeWriter]
    public class PointTypeWritter : ContentTypeWriter<Point>
    {
        protected override void Write(ContentWriter output, Point value)
        {
            output.Write(value.Position);
            output.Write(value.PitchYawRoll);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(Point.PointContentReader).AssemblyQualifiedName;
        }
    }
    public class XMLMapPlot
    {
        public string MapModel;
        public Vector3 DisappearPoint;
        public double DisappearRadius;
        public Point SavePoint;

        public string                  LimitingWalls;
        public List <Point>            ActiveHouse;
        public List <Point>            StaticBuilding;
        public List <Point>            FlatHouse;
        public List <HDBLevelColBox>   Trees;
        public List <HDBLevelColBox>   ColTrees;
        public List <HDBLevelColBox>   PickZones;
        public List <HDBLevelColBox>   Dustbin;
        public List <HDBLevelColBox>   SellSpawn;

        public Vector3                 Sell_CenterPoint;
        public Vector3                 Sell_PlayerSitOffSet;
        public double                  Sell_RadiusOfEffect;
        public double                  Sell_RadiusOutBounce;
        public double                  Sell_StartAngle;
        public double                  Sell_EndAngle;
        public double                  Sell_IntervalAngle;

        public XMLMapPlot() { }
        public class XMLMapPlotContentReader : ContentTypeReader<XMLMapPlot>
        {
            protected override XMLMapPlot Read(ContentReader input, XMLMapPlot Map)
            {
                if (Map == null)
                    Map = new XMLMapPlot();
                Map.MapModel = input.ReadString();
                Map.DisappearPoint = input.ReadVector3();
                Map.DisappearRadius = input.ReadDouble();
                Map.SavePoint = input.ReadObject<Point>();

                Map.LimitingWalls = input.ReadString();
                Map.ActiveHouse = input.ReadObject<List<Point>>();
                Map.StaticBuilding = input.ReadObject<List<Point>>();
                Map.FlatHouse = input.ReadObject<List<Point>>();
                Map.Trees = input.ReadObject<List<HDBLevelColBox>>();
                Map.ColTrees = input.ReadObject<List<HDBLevelColBox>>();
                Map.PickZones = input.ReadObject<List<HDBLevelColBox>>();
                Map.Dustbin = input.ReadObject<List<HDBLevelColBox>>();
                // Sell Cut scene
                Map.SellSpawn = input.ReadObject<List<HDBLevelColBox>>();
                Map.Sell_CenterPoint = input.ReadVector3();
                Map.Sell_PlayerSitOffSet = input.ReadVector3();
                Map.Sell_RadiusOfEffect = input.ReadDouble();
                Map.Sell_RadiusOutBounce = input.ReadDouble();
                Map.Sell_StartAngle = input.ReadDouble();
                Map.Sell_EndAngle = input.ReadDouble();
                Map.Sell_IntervalAngle = input.ReadDouble();
                return Map;
            }
        }
    }

    [ContentTypeWriter]
    public class XMLMapPlotTypeWritter : ContentTypeWriter<XMLMapPlot>
    {
        protected override void Write(ContentWriter output, XMLMapPlot value)
        {
            output.Write(value.MapModel);
            output.Write(value.DisappearPoint);
            output.Write(value.DisappearRadius);
            output.WriteObject<Point>(value.SavePoint);
            output.Write(value.LimitingWalls);
            output.WriteObject<List<Point>>(value.ActiveHouse);
            output.WriteObject<List<Point>>(value.StaticBuilding);
            output.WriteObject<List<Point>>(value.FlatHouse);
            output.WriteObject<List<HDBLevelColBox>>(value.Trees);
            output.WriteObject<List<HDBLevelColBox>>(value.ColTrees);
            output.WriteObject<List<HDBLevelColBox>>(value.PickZones);
            output.WriteObject<List<HDBLevelColBox>>(value.Dustbin);
            // Sell Cut scene
            output.WriteObject<List<HDBLevelColBox>>(value.SellSpawn);
            output.Write(value.Sell_CenterPoint);
            output.Write(value.Sell_PlayerSitOffSet);
            output.Write(value.Sell_RadiusOfEffect);
            output.Write(value.Sell_RadiusOutBounce);
            output.Write(value.Sell_StartAngle);
            output.Write(value.Sell_EndAngle);
            output.Write(value.Sell_IntervalAngle);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(XMLMapPlot.XMLMapPlotContentReader).AssemblyQualifiedName;
        }
    }
}
