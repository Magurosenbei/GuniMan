using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using XmlContentExtension;
using Engine;

namespace Game
{
    public class BlockingWalls
    {
        TriangleMeshShape ColMesh;
        public BlockingWalls(string ColWallMDL, IEContentManager ContentManager)
        {
            ColMesh = new TriangleMeshShape();
            ColMesh.SetModel(ColWallMDL, ContentManager);
            ColMesh.Immovable = true;
            ColMesh.CollisionSkin.RemoveAllPrimitives();
            ColMesh.CollisionSkin.AddPrimitive(ColMesh.triangleMesh, (int)ObjectFilter.OUT, new MaterialProperties(1, 1, 1));
        }
        public bool Filter(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.GetMaterialID(0) == (int)ObjectFilter.CHARACTER || skin1.GetMaterialID(0) == (int)ObjectFilter.PLAYER)
                return true;
            return false;
        }
        public void Disable()
        {
            ColMesh.Body.DisableBody();
        }
    }
}
