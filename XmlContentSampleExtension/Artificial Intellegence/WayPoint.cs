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

using WayPoint = System.String;
using Integer = System.String;

namespace XmlContentExtension
{
    public class Integer
    {
        public int Value;
        public Integer() { }
        public class IntegerContentReader : ContentTypeReader<Integer>
        {
            protected override Integer Read(ContentReader input, Integer num)
            {
                if (num == null) num = new Integer();
                num.Value = input.ReadInt32();
                return num;
            }
        }
    }
    [ContentTypeWriter]
    public class IntegerTypeWritter : ContentTypeWriter<Integer>
    {
        protected override void Write(ContentWriter output, Integer value)
        {
            output.Write(value.Value);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(Integer.IntegerContentReader).AssemblyQualifiedName;
        }
    }

    public class WayPoint
    {
        public Vector3      Position;
        public float        RadiusRange;
        public int          Id;
        public List <Integer>   LinkIndex;

        public WayPoint() { }
        public class WayPointContentReader : ContentTypeReader<WayPoint>
        {
            protected override WayPoint Read(ContentReader input, WayPoint pt)
            {
                if (pt == null) pt = new WayPoint();
                pt.Position = input.ReadVector3();
                pt.RadiusRange = (float)input.ReadDouble();
                pt.RadiusRange *= pt.RadiusRange;
                pt.Id = input.ReadInt32();
                pt.LinkIndex = input.ReadObject<List<Integer>>();
                return pt;
            }
        }
    }
    [ContentTypeWriter]
    public class WayPointTypeWritter : ContentTypeWriter<WayPoint>
    {
        protected override void Write(ContentWriter output, WayPoint value)
        {
            output.Write(value.Position);
            output.Write((double)value.RadiusRange);
            output.Write(value.Id);
            output.WriteObject<List<Integer>>(value.LinkIndex);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(WayPoint.WayPointContentReader).AssemblyQualifiedName;
        }
    }
}
