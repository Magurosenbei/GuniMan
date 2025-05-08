using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;

using Engine;
using XmlContentExtension;

namespace Game
{
    public class GenericBehavior
    {
        int JumpCounter;
        float LazeTimer, SpazTimer, IdleSwitchTimer;
        protected bool AnimateJumping;
        State currentState;
        public enum State
        {
            IDLE = 1,
            FOLLOW = 2,
            CHASE = 3,
            SLEEP = 4,
            PISSED = 5,
            RETURN = 6
        }
        public GenericBehavior()
        {
            IdleSwitchTimer = LazeTimer = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds;
            SpazTimer = (float)GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
            currentState = State.FOLLOW;
            AnimateJumping = false;
            JumpCounter = 0;
        }
        public State GetState()
        {
            return currentState;
        }
        public void AmmendState(State SetState, float Time)
        {
            if (SetState == State.CHASE)
                LazeTimer = Time;

            currentState = SetState;
        }
        public bool CatEnabled(bool Talk)
        {
            if (!AnimateJumping && Talk)
            {
                SpazTimer = (float)GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
                return AnimateJumping = true;
            }
            return false;
        }
        public void UpdateBehavior(PhysicsObject mPhyObj, float gameTime)
        {
            if (currentState == State.CHASE)
            {
                PhysicsObject DummyObj = mPhyObj;
                if (JumpCounter >= 2)
                    AnimateJumping = false;
                if ((float)GameEngine.GameTime.TotalGameTime.TotalMilliseconds - SpazTimer > 500 && AnimateJumping)
                {
                    DummyObj.Velocity += new Vector3(0, 10, 0);
                    JumpCounter += 1;
                    SpazTimer = (float)GameEngine.GameTime.TotalGameTime.TotalMilliseconds;
                }
                if (!AnimateJumping)
                {
                    DummyObj.Velocity -= new Vector3(0, JumpCounter * 10, 0);
                    JumpCounter = 0;
                }


                mPhyObj = DummyObj;     // Set !
                if (gameTime - LazeTimer > 60)  // 30 sec duration && condition of Chase Behavior
                {
                    currentState = State.FOLLOW;
                    LazeTimer = gameTime;
                }
            }
            else if (currentState == State.FOLLOW)
            {
                PhysicsObject DummyObj = mPhyObj;
                DummyObj.Velocity -= new Vector3(0, 1, 0);
                DummyObj.Body.ApplyGravity = true;
                mPhyObj = DummyObj;

                int Idle = GameEngine.RandomValue.Next(0, 5000);
                if (Idle == 0)
                {
                    currentState = State.IDLE;
                    IdleSwitchTimer = (float)GameEngine.GameTime.TotalGameTime.TotalSeconds;
                }
            }
            else if (currentState == State.IDLE)
            {
                int FOLLOW = GameEngine.RandomValue.Next(0, 250);
                if (FOLLOW == 0 && GameEngine.GameTime.TotalGameTime.TotalSeconds - IdleSwitchTimer > 5)
                    currentState = State.FOLLOW;
            }
        }
    }
    public class Character : Guniman
    {
        protected WayPoint Goal;
        protected PathSystem RefernceSystem;
        protected GenericBehavior Behavior = new GenericBehavior();
        public bool Male = true;


        private bool Recognition = false;

        public bool mbTalk;
        public bool mReallyTalk = false;

        public bool isAnimal = false;
        public bool Rejected = false;

        protected float AvAngle = 0.0f;
        protected bool HitSomething = false;

        public Character() : base() { }
        public Character(GameScreen Parent) : base(Parent) { }
        public Character(GameScreen Parent, bool isAnimal_F) : base(Parent) { isAnimal = isAnimal_F; }

        public void SetSpd(float spd) { MovingSpd = spd; }
        public void AssignGoal(WayPoint WayPoint, PathSystem Path)
        {
            Goal = WayPoint;
            //Goal = RefernceSystem.GetPoint(WayPoint.LinkIndex[0].Value);
            RefernceSystem = Path;
        }

        public bool getRecognition() { return Recognition; }
        public void setAIRemb() { Recognition = true; }

        public void EnableTalk()
        {
            mPhyObj.CollisionSkin.callbackFn += new CollisionCallbackFn(TalkChecker);
        }
        public void EnableAvoidObstalce()
        {
            mPhyObj.CollisionSkin.callbackFn += new CollisionCallbackFn(Avoid);
        }
        bool Avoid(CollisionSkin skin0, CollisionSkin skin1)
        {
            if ((skin1.GetPrimitiveLocal(0) is JigLibX.Geometry.Plane) == false && skin1.Owner != null)
            {
                Vector2 RelDir = new Vector2(mPhyObj.Position.X - skin1.Owner.Position.X,
                                                mPhyObj.Position.Z - skin1.Owner.Position.Z);
                AvAngle += Math.Abs((float)Math.Atan2(-RelDir.Y, -RelDir.X) - (float)Math.Atan2(GetDirection().Z, GetDirection().X));
                HitSomething = true;
            }
            if (skin1.GetMaterialID(0) == (int)ObjectFilter.OUT)
                return false;
            return true;
        }
        private void getRandomEmoticon()
        {
            if (!Emotion.Visisble)
            {
                Emotion.Visisble = true;
                Emotion.Timeout = new TimeSpan();
                Emotion.Duration = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(3, 5), 0);
            }
        }
        private void SetEmotionBasedOnBehavior(GenericBehavior Behavior)
        {
            // 0 = Ehhh , 1 = Exclaim , 2 = FullMoon, 3 = Fuming, 4 = Hmpt
            // 5 = Joyous, 6 = Money, 7 = Moody, 8 = Skull, 9 = Sloucy
            // 10 = Sun, 11 = Thrashbin, 12 = Zzz
            int LastEmo = Emotion.Emotion;
            if (Behavior.GetState() != GenericBehavior.State.SLEEP && Behavior.GetState() != GenericBehavior.State.PISSED)
            {
                if (Behavior.GetState() == GenericBehavior.State.CHASE) { Emotion.Emotion = 1; }
                if (Behavior.GetState() == GenericBehavior.State.FOLLOW) { Emotion.Emotion = 5; }
                if (Behavior.GetState() == GenericBehavior.State.IDLE) { Emotion.Emotion = 9; }
            }
            if (Behavior.GetState() == GenericBehavior.State.SLEEP) { Emotion.Emotion = 12; }
            if (Behavior.GetState() == GenericBehavior.State.PISSED) { Emotion.Emotion = 3; }
            if (Behavior.GetState() == GenericBehavior.State.RETURN) { Emotion.Emotion = 4; }
            if (LastEmo != Emotion.Emotion && World3D.PlayTime.Seconds > 5)
            {
                Emotion.Visisble = true;
                Emotion.Timeout = new TimeSpan();
                Emotion.Duration = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(3, 5), 0);
            }
        }
        public void SetBehaviorBasedOnPosition(GenericBehavior.State State) { Behavior.AmmendState(State, (float)GameEngine.GameTime.TotalGameTime.TotalSeconds); }
        //public void SetAngerBasedOnNegotiation() { Behavior.AmmendState(GenericBehavior.State.PISSED, (float)GameEngine.GameTime.TotalGameTime.TotalSeconds); }
        public bool TalkChecker(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (!mbTalk && skin1.GetMaterialID(0) == (int)ObjectFilter.PLAYER && PlayerState.Currently != PlayerState.State.TALK)
            {
                if (Kbdevice.Key_Pressed(Keys.Z) || GPdevice.Button_Pressed(Buttons.A))
                    mbTalk = true;
            }
            else if (!GameEngine.Services.GetService<DialogueEngine>().Visible)
                mbTalk = false;
            return true;
        }
        public void GetCheckTalk(Character PasserBy)
        {
            Guniman Player = GameEngine.Services.GetService<Guniman>();
            DialogueEngine Dialog = GameEngine.Services.GetService<DialogueEngine>();

            if (!Dialog.Visible && PlayerState.Currently == PlayerState.State.TALK) // finish talking liao la
            {
                PasserBy.mbTalk = false;
                PlayerState.Currently = PlayerState.State.NORMAL;
            }
            if (PasserBy.mPhyObj.BoundingBox.Intersects(Player.mPhyObj.BoundingBox) && PlayerState.Currently == PlayerState.State.NORMAL)
            {
                PasserBy.mbTalk = true;

                if (!mReallyTalk || Rejected || PasserBy.Behavior.GetState() == GenericBehavior.State.SLEEP) return;
                GameEngine.Services.GetService<Interactive>().Visible = true;
                if (Kbdevice.Key_Pressed(Microsoft.Xna.Framework.Input.Keys.Z) || GPdevice.Button_Pressed(Microsoft.Xna.Framework.Input.Buttons.A))
                {
                    if (Male)
                    {
                        GameEngine.Services.GetService<DialogueEngine>().StartConversation("Portrait_Guniman", "Portrait_Guy", Male, 0);
                    }
                    else
                    {
                        GameEngine.Services.GetService<DialogueEngine>().StartConversation("Portrait_Guniman", "Portrait_Girl", Male, 0);
                    }
                    Emotion.Emotion = 5;
                    Emotion.Visisble = true;
                    Emotion.Timeout = new TimeSpan();
                    Emotion.Duration = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(3, 5), 0);
                    Rejected = true;
                }
                PlayerState.Currently = PlayerState.State.TALK;
            }
        }
        public override void Update()
        {
            if (PlayerState.Currently == PlayerState.State.SELL) return;
            Emotion.Position = mAnimatedObj.Position - mAnimatedObj.CenterOffset * 2.0f + LookerCam.Rotation.Left * 2.4f;

            SetEmotionBasedOnBehavior(Behavior);    // Set Behavior emoticon
            if (mAnimatedObj == null || mPhyObj == null) return;
            if (mAnimatedObj.Cullable)
            {
                float blend = 1.0f;
                Visible = mAnimatedObj.Camera.InView(mAnimatedObj.Position, mAnimatedObj.ViewRange, out blend, out mAnimatedObj.DistanceFromCamera);
            }
            if (mAnimatedObj.DistanceFromCamera < 200 && Visible)
                mAnimatedObj.Update();
            else
            {
                mAnimatedObj.CalculatedOffset = (Matrix.CreateTranslation(mAnimatedObj.CenterOffset) * mAnimatedObj.Rotation).Translation;
                mAnimatedObj.World = MathsUtility.CreateWorldMatrix(mAnimatedObj.Position + mAnimatedObj.CalculatedOffset, mAnimatedObj.Rotation, mAnimatedObj.Scale);
            }
            mAnimatedObj.Position = mPhyObj.Position;
            if (!isAnimal)
                mAnimatedObj.Rotation = mPhyObj.Rotation;
            else
                mAnimatedObj.Rotation = mPhyObj.Rotation * Matrix.CreateFromYawPitchRoll(-1.571f, 0, 0);

            if (mbTalk)
            {
                Guniman Player = GameEngine.Services.GetService<Guniman>();
                Vector2 RelDir = new Vector2(Player.mPhyObj.Position.X - mPhyObj.Position.X,
                                                Player.mPhyObj.Position.Z - mPhyObj.Position.Z);
                Vector3 Dir = Player.GetDirection();
                float newBear = Math.Abs((float)Math.Atan2(-RelDir.Y, -RelDir.X) - (float)Math.Atan2(Dir.Z, Dir.X));
                if (newBear < 1.0f)
                {
                    mReallyTalk = true;
                }
                else
                {
                    mReallyTalk = false;
                    return;
                }
                if (PlayerState.Currently != PlayerState.State.TALK)
                {
                    FollowWayPoint();
                    return;
                }
                if (mAnimatedObj.Animator.AnimationClip.Name != "Talk" && !isAnimal)
                    mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Talk"], new TimeSpan(0, 0, 0, 0, 20));
                else if(mAnimatedObj.Animator.AnimationClip.Name != "Idle" && isAnimal)
                    mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 20));
                mPhyObj.Body.Velocity = Vector3.Zero;

                newBear = ((float)Math.Atan2(RelDir.Y, RelDir.X) - (float)Math.Atan2(1, 0));
                float dAngle = Bearing - newBear;
                if (dAngle < 0 && -dAngle < TurnSpd || dAngle > 0 && dAngle < TurnSpd)
                    Bearing = newBear;
                else if (dAngle < 0)
                    Bearing += TurnSpd;
                else if (dAngle > 0)
                    Bearing -= TurnSpd;
                mPhyObj.Body.Orientation = Matrix.CreateRotationY(-Bearing);

                if (isAnimal)   // if it is an animal it will follow u. wahaha.
                    Behavior.AmmendState(GenericBehavior.State.CHASE, (float)GameEngine.GameTime.TotalGameTime.TotalSeconds);
                else getRandomEmoticon();
            }
            else if (Behavior.GetState() == GenericBehavior.State.CHASE)
            {
                Vector3 PlayerPos = GameEngine.Services.GetService<Guniman>().mPhyObj.Position;
                //Goal.Position = GameEngine.Services.GetService<Guniman>().mPhyObj.Position;
                Vector2 FolDir = new Vector2(PlayerPos.X - mPhyObj.Position.X,
                                        PlayerPos.Z - mPhyObj.Position.Z);
                if (FolDir.LengthSquared() > Goal.RadiusRange)
                {
                    float newBearing = ((float)Math.Atan2(FolDir.Y, FolDir.X) - (float)Math.Atan2(1, 0));
                    float dirAngle = Bearing - newBearing;
                    if (dirAngle < 0 && -dirAngle < TurnSpd || dirAngle > 0 && dirAngle < TurnSpd)
                        Bearing = newBearing;
                    else if (dirAngle < 0)
                        Bearing += TurnSpd;
                    else if (dirAngle > 0)
                        Bearing -= TurnSpd;
                    Vector3 Direction = (Matrix.CreateTranslation(Front) * Matrix.CreateRotationY(-Bearing)).Translation;
                    mPhyObj.Body.Orientation = Matrix.CreateRotationY(-Bearing);
                    mPhyObj.Body.Velocity = Direction * MovingSpd * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds + new Vector3(0, mPhyObj.Body.Velocity.Y, 0);
                    if (!mPhyObj.Body.IsBodyEnabled)
                        mPhyObj.Body.UpdatePosition((float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds);

                }
            }
            else if (Behavior.GetState() == GenericBehavior.State.FOLLOW || Behavior.GetState() == GenericBehavior.State.PISSED || Behavior.GetState() == GenericBehavior.State.RETURN)
                FollowWayPoint();
            else if (Behavior.GetState() == GenericBehavior.State.IDLE)
            {
                if (mAnimatedObj.Animator.AnimationClip.Name != "Idle")
                {
                    mAnimatedObj.Animator.PlaybackMode = PlaybackMode.Forward;
                    mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 30));
                    mAnimatedObj.Animator.Speed = 1.0f;
                }
            }
            if (Kbdevice.Key_Pressed(Keys.Z) || GPdevice.Button_Pressed(Buttons.A))
            {
                if (Behavior.CatEnabled(mReallyTalk) && isAnimal)
                {
                    setAIRemb();
                    if (mAnimatedObj.Animator.AnimationClip.Name != "jump")
                    {
                        mAnimatedObj.Animator.PlaybackMode = PlaybackMode.Forward;
                        mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["jump"], new TimeSpan(0, 0, 0, 0, 30));
                        mAnimatedObj.Animator.Speed = 1.0f;
                    }
                }
            }
            Behavior.UpdateBehavior(mPhyObj, (float)GameEngine.GameTime.TotalGameTime.TotalSeconds);
        }
        void FollowWayPoint()
        {
            if (Goal == null) return;
            Vector2 RelDir = new Vector2(Goal.Position.X - mPhyObj.Position.X,
                                            Goal.Position.Z - mPhyObj.Position.Z);
            if (RelDir.LengthSquared() < Goal.RadiusRange)
                Goal = RefernceSystem.GetPoint(Goal.LinkIndex[GameEngine.RandomValue.Next(0, Goal.LinkIndex.Count)].Value);
            else
            {
                float newBear = ((float)Math.Atan2(RelDir.Y, RelDir.X) - (float)Math.Atan2(1, 0)) + AvAngle;
                float dAngle = Bearing - newBear;
                /*if (HitSomething)
                {
                    HitSomething = false;
                    if (AvAngle < 0)
                        Bearing += TurnSpd;
                    else if (AvAngle > 0)
                        Bearing -= TurnSpd;
                    AvAngle = 0.0f;
                }*/
                AvAngle = 0.0f;
                if (Math.Abs(dAngle) < TurnSpd)
                    Bearing = newBear;
                else if (dAngle < 0)
                    Bearing += TurnSpd;
                else if (dAngle > 0)
                    Bearing -= TurnSpd;
                Vector3 Direction = (Matrix.CreateTranslation(Front) * Matrix.CreateRotationY(-Bearing)).Translation;
                mPhyObj.Body.Orientation = Matrix.CreateRotationY(-Bearing);
                mPhyObj.Body.Velocity = Direction * MovingSpd * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                if (!mPhyObj.Body.IsBodyEnabled)
                    mPhyObj.Body.UpdatePosition((float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds);
            }
            if (Visible)
            {
                if (mPhyObj.Body.Velocity.LengthSquared() > 0.1)
                {
                    if (!isAnimal)
                    {
                        if (mAnimatedObj.Animator.AnimationClip.Name != "Walk")
                            mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"], new TimeSpan(0, 0, 0, 0, 20));
                    }
                    else
                    {
                        if (mAnimatedObj.Animator.AnimationClip.Name != "Walk")
                            mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"], new TimeSpan(0, 0, 0, 0, 20));
                    }
                }
                else if (mAnimatedObj.Animator.AnimationClip.Name != "Idle")
                {
                    mAnimatedObj.Animator.PlaybackMode = PlaybackMode.Forward;
                    mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 30));
                    mAnimatedObj.Animator.Speed = 1.0f;
                }
            }
        }
        public override void Draw()
        {
            if (PlayerState.Currently == PlayerState.State.SELL || PlayerState.Currently == PlayerState.State.ACT) return;
            base.Draw();
        }
    }
}
