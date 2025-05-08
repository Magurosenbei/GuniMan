using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace Game
{
    public class Lift
    {
        CollisionSkin   Skin;
        Body            Body;
        int TotalLevel;
        float VDHeight, LvlHeight;
        bool bInLift = false;
        Vector3 mPitchYawRoll;

        bool UsePhysics;

        public Lift(float VoidDeckHeight, float LevelHeight, int levels, bool physics)
        {
            UsePhysics = physics;
            VDHeight = VoidDeckHeight;
            LvlHeight = LevelHeight;
            TotalLevel = levels;
            
                Body = new Body();
                Skin = new CollisionSkin(Body);
                Body.CollisionSkin = Skin;
            if (UsePhysics)
            {
                float TotalHeight = VoidDeckHeight + levels * LevelHeight;
                Skin.RemoveAllPrimitives();
                Skin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(new Vector3(0, TotalHeight * 0.5f, 0), new Vector3(0.5f, TotalHeight * 0.5f, 0.5f), Matrix.Identity),
                                            (int)ObjectFilter.LIFT, new MaterialProperties(0, 0, 0));
                Skin.callbackFn += new CollisionCallbackFn(CollisionSkin_callbackFn);
                Body.Immovable = true;
                Body.EnableBody();
            }
        }
        public void SetBodyEnable(bool Enable)
        {
            if (Enable && !Body.IsBodyEnabled)
                Body.EnableBody();
            else if (Body.IsBodyEnabled)
                Body.DisableBody();
        }
        public void ForceDisable()
        {
            Body.DisableBody();
        }
        public Vector3 GetPosition()
        {
            return Body.Position;
        }
        public Vector3 GetPitchYawRoll()
        {
            return mPitchYawRoll;
        }
        public bool InLift() { return bInLift; }
        public void MoveTo(Vector3 Position, Vector3 PitchYawRoll)
        {
            mPitchYawRoll = PitchYawRoll;
            Body.MoveTo(Position, Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z));
        }
        public void Update()
        {
            if (!Body.IsBodyEnabled)
                bInLift = false;
        }
        public int AtLevel(float CurrentHeight)
        {
            if(!bInLift) return -1;
            if (CurrentHeight < VDHeight) return 0;
            else
                return (int)((CurrentHeight - VDHeight) / LvlHeight) + 1;
        }
        public float GetLevelHeight(int Targetlevel)
        {
            if (Targetlevel == 0)
                return 0;
            if (Targetlevel > TotalLevel)
                Targetlevel = TotalLevel;
            return VDHeight + (Targetlevel - 1) * LvlHeight;
        }
        bool CollisionSkin_callbackFn(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.GetMaterialID(0) == (int)ObjectFilter.PLAYER)
                bInLift = true;
            else
                bInLift = false;
            return false;   // Don't Collide no matter what
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
