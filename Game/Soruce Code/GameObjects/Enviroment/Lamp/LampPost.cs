using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XmlContentExtension;
using Engine;

namespace Game
{
    public class LampPost : Component
    {
        StaticObject    mStaticObj;
        Vector3         LightBulbPosition = Vector3.Zero;

        public LampPost(string LampPath, Vector3 Position) : base() { }
        public LampPost(string LampPath, Vector3 Position, GameScreen Parent) : base(Parent) { }

        void Load(string LampScript)
        {
            XmlContentExtension.Point DB = GameEngine.Content.Load<XmlContentExtension.Point>(LampScript);
            DB.Position = LightBulbPosition;
            mStaticObj = new StaticObject(GameEngine.Content.Load<Model>(VariableAsset.LampFile[0].Substring(0, VariableAsset.LampFile[0].Length)), Vector3.Zero);
        }

        public override void Update()
        {
            mStaticObj.Update();
            base.Update();
        }

        public override void Draw()
        {
            mStaticObj.Draw("Normal");
        }
    }
}
