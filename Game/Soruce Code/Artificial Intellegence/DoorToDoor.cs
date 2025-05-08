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
    class DoorToDoor
    {

        List<Character> DoorAnswerer = new List<Character>();
        List<Vector3> originalPos;
        List<int> added;
        List<LifeFadeCount> TimerCount = new List<LifeFadeCount>();

        int level = 0;
        float height;
        IEContentManager mContentManager;
        GameScreen mParent;
        public Vector3 mPosition, mPitchYawRoll;

        DialogueEngine DialogEng;

        private int durationThingy;
        public bool SpawnedAI;

        public DoorToDoor(BuildingDef Def, IEContentManager ContentManager, GameScreen Parent, float Height, int Level)
        {
            durationThingy = 0;
            SpawnedAI = false;
            level = Level;
            height = Height;
            mContentManager = ContentManager;
            mParent = Parent;
            originalPos = new List<Vector3>();
            added = new List<int>();

            for (int a = 0; a < Def.HouseSlot.Count; a++)
            {
                originalPos.Add(new Vector3(Def.HouseSlot[a].Position.X, Def.HouseSlot[a].Position.Y, Def.HouseSlot[a].Position.Z));
                DoorAnswerer.Add(new Character(mParent));
                TimerCount.Add(new LifeFadeCount(Level));

                //string VarModel = VariableAsset.MalesMDL[GameEngine.RandomValue.Next(0, VariableAsset.MalesMDL.Count)];
                //DoorAnswerer[a].Setup(VarModel, "Content/Shader Fx/CelSkinnedModelEffect", mContentManager);
                AnimatedObject obj = null;
                bool male = true;
                switch (GameEngine.RandomValue.Next(0, 2))
                {
                    case 0:
                        male = false;
                        obj = GameEngine.Services.GetService<Humaniod_Buffer>().GetRandomFemaleNPC();
                        break;
                    case 1:
                        male = true;
                        obj = GameEngine.Services.GetService<Humaniod_Buffer>().GetRandomMaleNPC();
                        break;
                    default:
                        male = false;
                        obj = GameEngine.Services.GetService<Humaniod_Buffer>().GetRandomFemaleNPC();
                        break;
                }
                DoorAnswerer[a].Male = male;
                DoorAnswerer[a].mAnimatedObj = new AnimatedObject(obj.GetAnimatedModel(), Vector3.Zero);
                DoorAnswerer[a].mAnimatedObj.Shader = obj.Shader.Clone(GameEngine.GraphicDevice);
                DoorAnswerer[a].mAnimatedObj.Camera = GameEngine.Services.GetService<Camera>();

                DoorAnswerer[a].SetBoundingPhysicsBasedOnModel(Def.HouseSlot[a].Position,
                          0.4f, new Vector3(0, 0, 0), true, (int)ObjectFilter.CHARACTER, true);

                Vector3 Extent = GraphicUtility.LargestHalfExtent(DoorAnswerer[a].mAnimatedObj.GetAnimatedModel().Model, true);
                DoorAnswerer[a].mPhyObj.Body.Position = Def.HouseSlot[a].Position + new Vector3(1, height + Extent.Y - 4.5f, 5);
                //DoorAnswerer[a].mPhyObj.Body.Position = originalPos[RandomValue] + new Vector3(1, height + Extent.Y - 2.5f, 5);
                //DoorAnswerer[a].mPhyObj.Body.MoveTo((mPosition + (Matrix.CreateTranslation(DoorAnswerer[a].mPhyObj.Body.Position) * Matrix.CreateFromYawPitchRoll(mPitchYawRoll.Y, mPitchYawRoll.X, mPitchYawRoll.Z)).Translation), Matrix.Identity);

                DoorAnswerer[a].mPhyObj.Immovable = true;
                DoorAnswerer[a].mAnimatedObj.Cullable = false;
                DoorAnswerer[a].Visible = false;
                DoorAnswerer[a].mPhyObj.Body.ApplyGravity = false;
                DoorAnswerer[a].mAnimatedObj.Animator.StartClip(DoorAnswerer[a].mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);
                DoorAnswerer[a].mAnimatedObj.Animator.LoopEnabled = true;
                DoorAnswerer[a].ShadowScale = Vector3.Zero;
                DoorAnswerer[a].mAnimatedObj.DisplayShadow = false;
                DoorAnswerer[a].mAnimatedObj.Scale = Vector3.One * 0.4f;
                //DoorAnswerer[a].mAnimatedObj.OutlineScale = new Vector3(1.05f, 1.01f, 1.05f);

                DoorAnswerer[a].mPhyObj.Body.DisableBody();
                DoorAnswerer[a].SetBehaviorBasedOnPosition(GenericBehavior.State.SLEEP);
                //DoorAnswerer[a].EnableTalk();
            }
        }
        public void GetWorld(Vector3 Position, Vector3 PitchYawRoll)
        {
            mPosition = Position;
            mPitchYawRoll = PitchYawRoll;
            for (int a = 0; a < DoorAnswerer.Count(); a++)
                DoorAnswerer[a].mPhyObj.Body.MoveTo((mPosition + (Matrix.CreateTranslation(DoorAnswerer[a].mPhyObj.Body.Position) * Matrix.CreateFromYawPitchRoll(mPitchYawRoll.Y, mPitchYawRoll.X, mPitchYawRoll.Z)).Translation), Matrix.Identity);
        }
        public void CreateOnSpot(BuidlingBlock Parts, int durationMod)
        {
            if (!SpawnedAI)
            {
                bool check = false;
                int limit = GameEngine.RandomValue.Next(1, originalPos.Count());        // 1~3 index randomize door

                for (int i = 0; i < limit; i++)
                {
                    int RandomValue = GameEngine.RandomValue.Next(0, limit + 1);

                ReGen:
                    for (int b = 0; b < added.Count; b++)
                    {
                        if (added[b] == RandomValue)  // if added from list before?
                        {
                            check = true;
                            RandomValue = GameEngine.RandomValue.Next(0, limit + 1);
                            goto ReGen;
                        }
                    }
                    if (!check)
                    {
                        added.Add(RandomValue);

                        // Door Open
                        Parts.OpenByIndex(RandomValue);
                        TimerCount[RandomValue].OpeningDoor = true;
                        DoorAnswerer[RandomValue].Visible = true;
                        DoorAnswerer[RandomValue].mAnimatedObj.Cullable = true;
                        DoorAnswerer[RandomValue].mPhyObj.Body.EnableBody();
                        int addOn = durationMod / 30;
                        double randomTime = GameEngine.RandomValue.Next(0+addOn, 10 + addOn * GameEngine.RandomValue.Next(1,7));     // set duration of Door To Door AI here
                        TimerCount[RandomValue].setLimit((float)randomTime);
                        TimerCount[RandomValue].setStartTime(GameEngine.GameTime.TotalGameTime.TotalSeconds);
                        TimerCount[RandomValue].ResetTime();
                        if (DoorAnswerer[RandomValue].mAnimatedObj.Animator.AnimationClip.Name != "Walk")
                            DoorAnswerer[RandomValue].mAnimatedObj.Animator.StartClip(DoorAnswerer[RandomValue].mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);
                        DoorAnswerer[RandomValue].SetBehaviorBasedOnPosition(GenericBehavior.State.PISSED);
                    }
                }
                SpawnedAI = true;
            }
        }
        public void UpdateAI(BuidlingBlock Parts)
        {
            if (DialogEng == null)
                DialogEng = GameEngine.Services.GetService<DialogueEngine>();
            for (int a = 0; a < DoorAnswerer.Count(); a++)
            {
                if (TimerCount[a].getActivated())
                {
                    if (DoorAnswerer[a].Rejected)
                        TimerCount[a].OverrideLife((float)(GameEngine.GameTime.TotalGameTime.TotalSeconds));
                    if (!DialogEng.Visible)
                    {
                        if (TimerCount[a].PauseTick)
                        {
                            TimerCount[a].startTime += TimerCount[a].addedTime;
                            TimerCount[a].PauseTick = false;
                            TimerCount[a].addedTime = 0;
                        }
                        if (!TimerCount[a].GoingBack()) // if u are not talking to him, make him more bored.
                        {
                            float TurnSpd = 0.1f;
                            Vector2 FolDir = new Vector2(DoorAnswerer[a].mPhyObj.Position.X - DoorAnswerer[a].mPhyObj.Position.X,
                                            DoorAnswerer[a].mPhyObj.Position.Z - (DoorAnswerer[a].mPhyObj.Position.Z - 5));
                            float newBearing = ((float)Math.Atan2(FolDir.Y, FolDir.X) - (float)Math.Atan2(1, 0));
                            float dirAngle = DoorAnswerer[a].GetBearing() - newBearing;
                            if (dirAngle < 0 && -dirAngle < TurnSpd || dirAngle > 0 && dirAngle < TurnSpd)
                                DoorAnswerer[a].SetBearing(newBearing);
                            else if (dirAngle < 0)
                                DoorAnswerer[a].SetBearing(DoorAnswerer[a].GetBearing() + TurnSpd);
                            else if (dirAngle > 0)
                                DoorAnswerer[a].SetBearing(DoorAnswerer[a].GetBearing() - TurnSpd);

                            DoorAnswerer[a].mPhyObj.Rotation = Matrix.CreateRotationY(DoorAnswerer[a].GetBearing());

                            if (TimerCount[a].a >= 1.0f)     // Just Walked Out , close the damn door
                            {
                                Parts.CloseByIndex(a);
                                TimerCount[a].OpeningDoor = false;
                            }
                            if (TimerCount[a].a <= 5.0f)
                            {
                                if (GameEngine.GameTime.TotalGameTime.TotalMilliseconds - TimerCount[a].copyTime > 50)
                                {
                                    DoorAnswerer[a].mPhyObj.Position += new Vector3(0, 0, 0.1f);
                                    TimerCount[a].copyTime = GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
                                    TimerCount[a].a += 0.1f;
                                }
                            }
                            else
                            {
                                if (DoorAnswerer[a].mAnimatedObj.Animator.AnimationClip.Name != "Idle")
                                    DoorAnswerer[a].mAnimatedObj.Animator.StartClip(DoorAnswerer[a].mAnimatedObj.GetAnimatedModel().AnimationClips["Idle"]);
                                DoorAnswerer[a].SetBehaviorBasedOnPosition(GenericBehavior.State.IDLE);
                            }
                        }
                        else
                        {
                            if (DoorAnswerer[a].mAnimatedObj.Animator.AnimationClip.Name != "Walk")
                                DoorAnswerer[a].mAnimatedObj.Animator.StartClip(DoorAnswerer[a].mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);
                            float TurnSpd = 0.1f;
                            Vector2 FolDir = new Vector2(DoorAnswerer[a].mPhyObj.Position.X - DoorAnswerer[a].mPhyObj.Position.X,
                                           (DoorAnswerer[a].mPhyObj.Position.Z - 5) - DoorAnswerer[a].mPhyObj.Position.Z);
                            float newBearing = ((float)Math.Atan2(FolDir.Y, FolDir.X) - (float)Math.Atan2(1, 0));
                            float dirAngle = DoorAnswerer[a].GetBearing() - newBearing;
                            if (dirAngle < 0 && -dirAngle < TurnSpd || dirAngle > 0 && dirAngle < TurnSpd)
                                DoorAnswerer[a].SetBearing(newBearing);
                            else if (dirAngle < 0)
                                DoorAnswerer[a].SetBearing(DoorAnswerer[a].GetBearing() + TurnSpd);
                            else if (dirAngle > 0)
                                DoorAnswerer[a].SetBearing(DoorAnswerer[a].GetBearing() - TurnSpd);

                            DoorAnswerer[a].mPhyObj.Rotation = Matrix.CreateRotationY(DoorAnswerer[a].GetBearing());

                            if(!DoorAnswerer[a].Rejected)
                                DoorAnswerer[a].SetBehaviorBasedOnPosition(GenericBehavior.State.RETURN);
                            else DoorAnswerer[a].SetBehaviorBasedOnPosition(GenericBehavior.State.PISSED);
                            if (TimerCount[a].a >= 0.0f && !TimerCount[a].OpeningDoor)
                            {
                                if (GameEngine.GameTime.TotalGameTime.TotalMilliseconds - TimerCount[a].copyTime > 50)
                                {
                                    DoorAnswerer[a].mPhyObj.Position -= new Vector3(0, 0, 0.1f);
                                    TimerCount[a].copyTime = GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
                                    TimerCount[a].a -= 0.1f;
                                }
                            }
                            else
                            {
                                if (!TimerCount[a].OpeningDoor)     // Going Back need to open door
                                {
                                    Parts.OpenByIndex(a);
                                    TimerCount[a].OpeningDoor = true;
                                }
                                if (GameEngine.GameTime.TotalGameTime.TotalMilliseconds - TimerCount[a].copyTime > 50)
                                {
                                    DoorAnswerer[a].mPhyObj.Position -= new Vector3(0, 0, 0.1f);
                                    TimerCount[a].copyTime = GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
                                    TimerCount[a].b += 0.1f; 
                                }
                            }
                        }
                        TimerCount[a].pauseTime = GameEngine.GameTime.TotalGameTime.TotalSeconds;
                    }
                    if (!DialogEng.Visible) // if u are not talking to him, make him more bored.
                        TimerCount[a].setLife((float)(GameEngine.GameTime.TotalGameTime.TotalSeconds - TimerCount[a].startTime));
                    else
                    {
                        TimerCount[a].PauseTick = true;
                        TimerCount[a].addedTime = GameEngine.GameTime.TotalGameTime.TotalSeconds - TimerCount[a].pauseTime;
                    }
                }


                if (TimerCount[a].isDead())
                {
                    DoorAnswerer[a].SetBehaviorBasedOnPosition(GenericBehavior.State.SLEEP);
                    for (float left = TimerCount[a].b; left > 0.0f; left -= 0.1f)
                        DoorAnswerer[a].mPhyObj.Position += new Vector3(0, 0, 0.1f);
                    TimerCount[a].b = 0.0f;
                    DoorAnswerer[a].Visible = false;
                    DoorAnswerer[a].mAnimatedObj.Cullable = false;
                    DoorAnswerer[a].mPhyObj.Body.DisableBody();
                    DoorAnswerer[a].Rejected = false;
                    TimerCount[a].activated = false;
                    Parts.CloseByIndex(a);
                    TimerCount[a].OpeningDoor = false;
                    TimerCount[a].addedTime = 0;

                }
                DoorAnswerer[a].GetCheckTalk(DoorAnswerer[a]);

            }

        }
        public bool getOccupiedBuilding()
        {
            for (int i = 0; i < DoorAnswerer.Count; i++)
            {
                if (DoorAnswerer[i].mPhyObj.Body.IsBodyEnabled)
                    return true;
            }
            return false;
        }
        public void UpdateAfterBorder()
        {
            bool TotalCheck = false;
            for (int i = 0; i < added.Count(); i++)
            {
                if (!TimerCount[added[i]].BackHome) // he have not return back home
                    TotalCheck = true;
                else
                    continue;
            }
            if (!TotalCheck)        // after all check
            {
                added.Clear();
                SpawnedAI = false;
            }
        }
        public void DisableComponent()
        {
            for (int i = 0; i < DoorAnswerer.Count(); i++)
            {
                DoorAnswerer[i].DisableComponent();
                DoorAnswerer[i].mPhyObj.Body.DisableBody();
            }
            added.Clear();
            DoorAnswerer.Clear();
            TimerCount.Clear();
        }

    }
}
