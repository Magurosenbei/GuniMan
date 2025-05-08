using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;

using XmlContentExtension;
using Engine;

namespace Game
{
    public class PatrolCats
    {
        PathSystem path;
        public PatrolCats(string XMLFile, IEContentManager ContentManager, GameScreen Parent)
        {
            path = new PathSystem(XMLFile, ContentManager, Parent, true);
        }
        public void FillPasserByCats(IEContentManager ContentManager)
        {
            path.FillPassers(ContentManager);
        }
        public WayPoint GetPoint(int index)
        {
            return path.GetPoint(index);
        }
        public void Clear()
        {
            path.Clear();
        }
        public void Update()
        {
            path.Update();
        }
        public int GetCount()
        {
            return path.GetRecognitionCount();
        }
       
    }
}
