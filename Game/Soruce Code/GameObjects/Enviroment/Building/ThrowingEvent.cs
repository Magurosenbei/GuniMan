using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using XmlContentExtension;
using Engine;

namespace Game
{
    public class DangerZone
    {
        bool bInZone;
        CollisionSkin Skin;
        Body Body;

        public DangerZone()
        {
            Body = new Body();
            Skin = new CollisionSkin(Body);
            Body.CollisionSkin = Skin;

            Skin.RemoveAllPrimitives();
            // To be scripted define as HDBcolbox
            Skin.AddPrimitive(PhysicsObject.CreateBoxFromCenter(new Vector3(18, 0, 1), new Vector3(13.0f, 100.0f, 20.0f), Matrix.Identity),
                          (int)ObjectFilter.BUILDINGS, new MaterialProperties(0, 0, 0));
            Skin.callbackFn += new CollisionCallbackFn(CollisionSkin_callbackFn);
            Body.EnableBody();
            Body.Immovable = true;
        }
        public bool InDangerZone() { return bInZone; }
        public void MoveTo(Vector3 Position, Vector3 PitchYawRoll)
        {
            Body.MoveTo(Position, Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z));
        }
        public void Update() { bInZone = false; }
        public void DisableComponent()
        {
            Body.DisableBody();
        }
        bool CollisionSkin_callbackFn(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.GetMaterialID(0) == (int)ObjectFilter.PLAYER)
                bInZone = true;
            return false;   // Don't Collide no matter what
        }
    }
    public class LifeFadeCount
    {
        public float a = 0;
        public float b = 0;
        public Vector3 originalPos;
        float Life, limit = 0;
        public bool activated;
        public bool BackHome;
        public bool OpeningDoor;
        public bool PauseTick;
        public double startTime, copyTime, pauseTime, addedTime = 0.0f;
        public Vector3 ShadowScale = Vector3.One;
        public bool Dejected;

        public LifeFadeCount(int Level)
        {
            limit += 10 + Level;      // 10 second limit
            Life = limit;
            activated = false;
            BackHome = true;
            Dejected = false;
        }
        public void setLimit(float Limit)
        {
            limit += Limit;
        }
        public bool getActivated()
        {
            return activated;
        }
        public void setStartTime(double gameTime)
        {
            startTime = gameTime;
            copyTime = GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
            activated = true;
            BackHome = false;
        }
        public void ResetTime()
        {
            a = 0;
        }
        public void OverrideLife(float gameTime)
        {
            if (!Dejected)
            {
                limit = Life = 5;
                startTime = gameTime;
                Dejected = true;
            }
        }
        public void setLife(float value)
        {
            if (activated)
                Life = limit - value;
        }
        public bool isDead()
        {
            if (Life <= 0)
            {
                BackHome = true;
                return true;
            }
            else return false;
        }
        public bool GoingBack()
        {
            if (Life <= 5)
            {
                OpeningDoor = false;
                return true;
            }
            else return false;
        }
        public float GetLife()
        {
            return Life;
        }
    }
    public class RandomStuffThrown
    {
        List<PhysicsStaticObject> RandomPlants;
        List<LifeFadeCount> LifeCounter;
        Shadow g_Shadow;
        public bool Visible, HitTarget;
        public Vector3 mPitchYawRoll;
        int level = 0;

        public RandomStuffThrown(BuildingDef Def, IEContentManager ContentManager, GameScreen Parent, float height, int Level)
        {
            HitTarget = false;
            level = Level;
            RandomPlants = new List<PhysicsStaticObject>();
            LifeCounter = new List<LifeFadeCount>();
            g_Shadow = GameEngine.Services.GetService<Shadow>();

            RandomPlants.Capacity = LifeCounter.Capacity = Def.HouseSlot.Count;
            for (int i = 0; i < Def.HouseSlot.Count; i++)
            {
                int index = GameEngine.RandomValue.Next(0, 2);
                // Each house   
                RandomPlants.Add(new PhysicsStaticObject(Parent));
                LifeCounter.Add(new LifeFadeCount(level));

                string VarModel = VariableAsset.FlowerPots[GameEngine.RandomValue.Next(0, VariableAsset.FlowerPots.Count)];
                RandomPlants[i].Setup(VarModel, "Content/Shader Fx/CelFx", ContentManager);

                Vector3 Extents = GraphicUtility.LargestHalfExtent(RandomPlants[i].mStaticObj.GetModel(), true);
                LifeCounter[i].ShadowScale = Extents * 2.0f;
                RandomPlants[i].SetBoundingPhysicsBasedOnModel(Def.HouseSlot[i].Position,
                          1.0f, new Vector3(0, 0, 0), true, (int)ObjectFilter.DECOS);

                RandomPlants[i].mStaticObj.CenterOffset = new Vector3(0, -Extents.Y, 0);
                RandomPlants[i].mStaticObj.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                //RandomPlants[i].mStaticObj.OutlineScale = new Vector3(1.08f, 1.005f, 1.08f);
                RandomPlants[i].mStaticObj.Cullable = false;
                RandomPlants[i].mStaticObj.DisplayShadow = false;
                RandomPlants[i].mPhyObj.Body.MoveTo(Def.HouseSlot[i].Position + new Vector3(9, height + 4 + Extents.Y, 4), Matrix.Identity);
                RandomPlants[i].mPhyObj.SetMass(Extents.LengthSquared() * 2);
                RandomPlants[i].mPhyObj.Immovable = false;
                RandomPlants[i].mStaticObj.CustomFade = true;
                RandomPlants[i].ManuelUpdate = true;

                RandomPlants[i].mPhyObj.Mass = 100.0f;
                RandomPlants[i].mPhyObj.CollisionSkin.callbackFn += new CollisionCallbackFn(CheckImpact);
                int appear = GameEngine.RandomValue.Next(0, 500);
                if (appear > 150)
                {
                    RandomPlants[i].mStaticObj.Cullable = true;
                    RandomPlants[i].Visible = true;
                }
                else
                    RandomPlants[i].Visible = false;
                RandomPlants[i].ChangePhysicsViaCull = false;
                RandomPlants[i].mPhyObj.Body.DisableBody();
            }
        }

        public void MoveTo(Vector3 Position, Vector3 PitchYawRoll)
        {
            for (int i = 0; i < RandomPlants.Count; i++)
            {
                MoveWorldCoords(RandomPlants[i], Position, PitchYawRoll);
                LifeCounter[i].originalPos = RandomPlants[i].Position; // store it
            }
        }
        private void MoveWorldCoords(PhysicsStaticObject Object, Vector3 Position, Vector3 PitchYawRoll)
        {
            //Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z)
            Object.mPhyObj.Body.MoveTo(Position + (Matrix.CreateTranslation((Object.mPhyObj.Body.Position)) * Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z)).Translation, Matrix.Identity);
            Object.Position = Object.mPhyObj.Body.Position;
            mPitchYawRoll = PitchYawRoll;
        }
        public void SetBodies(bool Enable)
        {

        }
        public void Update()
        {
            // Update position etc.. Spawn
            if (HitTarget)
            {
                Guniman Player = GameEngine.Services.GetService<Guniman>();
                bool AddStar = true;
                for (int i = 0; i < Player.HoldItem.Count; i++)
                    if (Player.HoldItem[i].Item is StarHead)
                        AddStar = false;
                if (AddStar)
                {
                    GameEngine.Services.GetService<AchievementOverlord>().Headshot();
                    Player.HoldItem.Add(new Equipment(new StarHead(4, 1.5f, GameEngine.Services.GetService<IEContentManager>())));
                    Player.HoldItem[Player.HoldItem.Count - 1].Item.CenterOffset = new Vector3(0, 1.0f, 0);
                    Player.HoldItem[Player.HoldItem.Count - 1].Item.Rotation = Matrix.CreateRotationZ(-1.202f);
                    Player.mAnimatedObj.FindBonePosition("character_gman_set01_v02_01:character_proto2:head", out Player.HoldItem[Player.HoldItem.Count - 1].ABS_ParentID, out Player.HoldItem[Player.HoldItem.Count - 1].Skin_ParentID);
                    Player.HoldItem[Player.HoldItem.Count - 1].ABS_Parent = Player.mAnimatedObj.Transformation[Player.HoldItem[Player.HoldItem.Count - 1].ABS_ParentID];
                    Player.HoldItem[Player.HoldItem.Count - 1].UseBone = true;
                    PlayerState.Currently = PlayerState.State.STUNT;
                    GameEngine.Services.GetService<RadialBlur>().Visible = true;
                    GameEngine.Services.GetService<RadialBlur>().EndSequence = false;
                    GameEngine.Services.GetService<HealthHud>().ChangeLifeByPercentage(-0.2f);
                }
                HitTarget = false;
            }
            int count = GameEngine.RandomValue.Next(0, 1000);
            if (count == 55 || GameEngine.Services.GetService<KeyboardDevice>().Key_Pressed(Keys.Enter))
            {
                if (RandomPlants.Count > 0)
                {
                    int ThrowIndex = GameEngine.RandomValue.Next(0, RandomPlants.Count);
                    if (!RandomPlants[ThrowIndex].mPhyObj.Body.IsBodyEnabled && RandomPlants[ThrowIndex].mStaticObj.Cullable)
                    {
                        int randVelocX = GameEngine.RandomValue.Next(5, 8);
                        int randVelocZ = GameEngine.RandomValue.Next(-4, 4);

                        Vector3 RotatedVel = (Matrix.CreateTranslation(new Vector3(randVelocX, -50, randVelocZ)) * Matrix.CreateFromYawPitchRoll(mPitchYawRoll.Y, mPitchYawRoll.X, mPitchYawRoll.Z)).Translation;

                        LifeCounter[ThrowIndex].setStartTime(GameEngine.GameTime.TotalGameTime.TotalSeconds);
                        RandomPlants[ThrowIndex].mPhyObj.Body.EnableBody();
                        RandomPlants[ThrowIndex].mPhyObj.Velocity = RotatedVel;
                    }
                }
            }
        }
        public bool CheckImpact(CollisionSkin Skin0, CollisionSkin Skin1)
        {
            if (Skin1.GetMaterialID(0) == (int)ObjectFilter.PLAYER)
            {
                if (Skin0.Owner.Position.Y > Skin1.Owner.Position.Y)
                    HitTarget = true;
                else HitTarget = false;
            }
            return true;
        }
        public void UpdateFade()
        {
            for (int i = 0; i < RandomPlants.Count; i++)       // Each plant update, to be configured to certain fuzzy logic later
            {
                RandomPlants[i].Update();
                if (RandomPlants[i].mStaticObj.DistanceFromCamera > 100)
                    RandomPlants[i].Visible = false;
                if (RandomPlants[i].mPhyObj.Body.IsBodyEnabled && LifeCounter[i].GetLife() < 5)
                    RandomPlants[i].mStaticObj.Color.W = LifeCounter[i].GetLife() * 0.2f;
                if (LifeCounter[i].isDead()) // Check Death
                {
                    RandomPlants[i].mPhyObj.Body.MoveTo(LifeCounter[i].originalPos, Matrix.Identity);
                    RandomPlants[i].Position = RandomPlants[i].mPhyObj.Body.Position;
                    RandomPlants[i].mPhyObj.Body.DisableBody();
                    RandomPlants[i].mStaticObj.Color.W = 1.0f;
                }
                LifeCounter[i].setLife((float)(GameEngine.GameTime.TotalGameTime.TotalSeconds - LifeCounter[i].startTime));
            }
        }
        public void Draw()
        {
            if (SceneControl.RenderMode == SceneControl.Rendering.SHADOW) return;
            for (int i = 0; i < RandomPlants.Count; i++)
            {
                if (LifeCounter[i].isDead() || !RandomPlants[i].mPhyObj.Body.IsBodyEnabled) continue;
                g_Shadow.Draw(new Vector3(RandomPlants[i].Position.X, 0, RandomPlants[i].Position.Z), Vector3.Zero, LifeCounter[i].ShadowScale, Matrix.Identity);
            }
        }
        public void DisableComponent()
        {
            for (int i = 0; i < RandomPlants.Count; i++)
            {
                RandomPlants[i].DisableComponent();
                RandomPlants[i].mPhyObj.Body.DisableBody();
            }
            RandomPlants.Clear();
            LifeCounter.Clear();
        }
    }
}
