using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

using Engine;

using XmlContentExtension;

namespace Game
{
    public enum ActState { NONE, COME, THINK, RETIRE };
    // These people will arrive at spawn point, then think (show random emoicon), Go away
    public class CutsceneActor : Guniman
    {
        protected Fuzz PriceThinking = new Fuzz();
        protected Fuzz CrowdPressure = new Fuzz();

        public Vector3 RetirePoint;
        public Vector3 SpawnPoint;     // Where it come from
        public Vector3 GoalPoint;      // Where to go to
        public Vector3 SellerPos;
        public ActState NowDoing = ActState.COME;   // default new does this

        public float ThinkDuration = 10.0f;
        TimeSpan TimePassed = new TimeSpan();
        TimeSpan LastShowEmo = new TimeSpan();
        public List<CutsceneActor> Comrades;
        public CutsceneActor() : base() { }
        public CutsceneActor(GameScreen Parent) : base(Parent) { }

        public void SetActor(Vector3 Spawn, Vector3 Goal, Vector3 SellerPosition, int Popularity, int Overprice, int NumberPeople, AnimatedObject obj)
        {
            ManuelUpdate = true;
            CrowdPressure.GenerateRandom();
            PriceThinking.GenerateRandom();
            PriceThinking.CurrentValue = Overprice;
            float High, Low;
            High = PriceThinking.HowHigh();
            Low = PriceThinking.HowLow();
            bool ThinkLonger = (High > Low) ? true : false;

            ThinkDuration = 0.0f;
            if (High == 0.0f && Low == 0.0f)
                ThinkDuration += 10;
            else
            {
                if (ThinkLonger && High != 0.0f)
                    ThinkDuration += High * 15;
                else if(Low != 0.0f)
                    ThinkDuration += (1.0f - Low) * 7;
            }
            CrowdPressure.CurrentValue = Popularity;
            High = CrowdPressure.HowHigh();
            Low = CrowdPressure.HowLow();

            ThinkLonger = (High + NumberPeople * 0.05f > Low) ? true : false;
            if (High == 0.0f && Low == 0.0f)
                ThinkDuration += 10;
            else
            {
                if (ThinkLonger && High != 0.0f)
                    ThinkDuration += High * 20;
                else if (Low != 0.0f)
                    ThinkDuration += (1.0f - Low) * 10;
            }
            int Min = 0;
            Min = (int)(MathHelper.Max(High, Low) * 10);
            
            if (ThinkDuration > 25) ThinkDuration = 25;
            if (Min > ThinkDuration) ThinkDuration = Min + 2;
            ThinkDuration = (float)GameEngine.RandomValue.Next(Min, (int)ThinkDuration);

            GoalPoint = Goal;
            SpawnPoint = Spawn;
            RetirePoint = Spawn;
            SellerPos = SellerPosition;
            LastShowEmo = new TimeSpan();
            TimePassed = new TimeSpan();
            mAnimatedObj = new AnimatedObject(obj.GetAnimatedModel(), Spawn);
            mAnimatedObj.Shader = obj.Shader.Clone(GameEngine.GraphicDevice);
            mAnimatedObj.Camera = GameEngine.Services.GetService<Camera>();
            mAnimatedObj.Transformation = obj.Transformation;
            //........
            // Make Customer Run to Player lol
            int Chance = GameEngine.RandomValue.Next(1, 99);
            int Run = GameEngine.RandomValue.Next(1, 10);
            if (Chance < Popularity && Run > 5)
            {
                MovingSpd = GameEngine.RandomValue.Next(800, 1000);
                mAnimatedObj.Animator.PlayClip(mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);
                mAnimatedObj.Animator.Speed = 1.5f;
            }
            else
            {
                MovingSpd = GameEngine.RandomValue.Next(300, 400);
                mAnimatedObj.Animator.PlayClip(mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);
            }
            NowDoing = ActState.COME;
        }
        public override void DisableComponent()
        {
            EmoIconSystem.Subscribe.Remove(Emotion);
            base.DisableComponent();
        }
        public override void Update()
        {
            if (mAnimatedObj == null || mPhyObj == null) return;
            if (mPhyObj.Position.Y > -mAnimatedObj.CenterOffset.Y)
                mPhyObj.Position = new Vector3(mPhyObj.Position.X, -mAnimatedObj.CenterOffset.Y, mPhyObj.Position.Z);
            Emotion.Position = mAnimatedObj.Position - mAnimatedObj.CenterOffset * 2.0f + LookerCam.Rotation.Left * 2.4f;
            if (mAnimatedObj.Cullable)
            {
                float blend = 1.0f;
                Visible = mAnimatedObj.Camera.InView(mAnimatedObj.Position, mAnimatedObj.ViewRange, out blend, out mAnimatedObj.DistanceFromCamera);
            }
            mAnimatedObj.Update();
            mAnimatedObj.Position = mPhyObj.Position;
            mAnimatedObj.Rotation = mPhyObj.Rotation;
            switch (NowDoing)
            {
                case ActState.COME:
                    {
                        Vector2 RelDir = new Vector2(GoalPoint.X - mPhyObj.Position.X,
                                                        GoalPoint.Z - mPhyObj.Position.Z);
                        if (RelDir.LengthSquared() < 2.0f)  // reached
                        {
                            NowDoing = ActState.THINK;
                            Emotion.Timeout = new TimeSpan();
                            Emotion.Duration = new TimeSpan(0, 0, 0, 1);
                            Emotion.Visisble = true;
                            mPhyObj.Velocity = Vector3.Zero;
                            mPhyObj.Immovable = true;
                            break;
                        }
                        ProcceedToPoint(RelDir);
                    }
                    break;

                case ActState.THINK:
                    {
                        TimePassed += GameEngine.GameTime.ElapsedGameTime;
                        mAnimatedObj.Animator.Speed = 1.0f;
                        if (mAnimatedObj.Animator.AnimationClip.Name != "Talk")
                            mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Talk"], new TimeSpan(0, 0, 0, 0, 20));
                        if (TimePassed > TimeSpan.FromSeconds(ThinkDuration))
                        {
                            NowDoing = ActState.RETIRE; // Finish thinking in the crowd and go back
                            MovingSpd = GameEngine.RandomValue.Next(300, 350);
                            // Display random emo when leaving
                            LastShowEmo = new TimeSpan();
                            Emotion.Emotion = GameEngine.RandomValue.Next(0, VariableAsset.EmoIcon.Count);
                            Emotion.Visisble = true;
                            Emotion.Timeout = new TimeSpan();
                            Emotion.Duration = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(2, 4), 0);
                            mPhyObj.Immovable = false;
                            break;
                        }
                        // Do this while thinking more than 1 second then change emo
                        FacePoint(SellerPos);
                        if (!Emotion.Visisble && LastShowEmo.Seconds > 1)  // Change Emoicons when dissappeared
                        {
                            LastShowEmo = new TimeSpan();
                            Emotion.Emotion = GameEngine.RandomValue.Next(0, VariableAsset.EmoIcon.Count);
                            Emotion.Visisble = true;
                            Emotion.Timeout = new TimeSpan(0, 0, 0, 0, 0);
                            Emotion.Duration = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(2, 4), 0);
                        }
                        else if (!Emotion.Visisble)
                            LastShowEmo += GameEngine.GameTime.ElapsedGameTime;
                    }
                    break;

                case ActState.RETIRE:
                    {
                        Vector2 RelDir = new Vector2(   RetirePoint.X - mPhyObj.Position.X,
                                                        RetirePoint.Z - mPhyObj.Position.Z);
                        if (RelDir.LengthSquared() < 2.0f)
                        {
                            NowDoing = ActState.NONE;   // reached 
                            break;
                        }
                        ProcceedToPoint(RelDir);
                        // Walk Home
                        if (mAnimatedObj.Animator.AnimationClip.Name != "Walk")
                            mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"], new TimeSpan(0, 0, 0, 0, 20));

                    }
                    break;
            }
        }
        protected void ProcceedToPoint(Vector2 RelativeDir)
        {
            float newBear = (float)Math.Atan2(RelativeDir.Y, RelativeDir.X) - (float)Math.Atan2(1, 0);
            float dAngle = Bearing - newBear;
            if (dAngle < 0 && -dAngle < TurnSpd || dAngle > 0 && dAngle < TurnSpd)
                Bearing = newBear;
            else if (dAngle < 0)
                Bearing += TurnSpd;
            else if (dAngle > 0)
                Bearing -= TurnSpd;

            Vector3 Direction = (Matrix.CreateTranslation(Front) * Matrix.CreateRotationY(-Bearing)).Translation;
            mPhyObj.Body.Orientation = Matrix.CreateRotationY(-Bearing);
            mPhyObj.Body.Velocity = Direction * MovingSpd * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;

            float spd = MovingSpd * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
            if (mAnimatedObj.Animator.AnimationClip.Name == "Walk")
                mAnimatedObj.Animator.Speed = spd * 0.3f;
            else if (mAnimatedObj.Animator.AnimationClip.Name == "Run")
                mAnimatedObj.Animator.Speed = 1.0f;
        }
        protected void FacePoint(Vector3 Point)
        {
            float newBear = ((float)Math.Atan2(Point.Z - mPhyObj.Position.Z, Point.X - mPhyObj.Position.X) - (float)Math.Atan2(1, 0));
            float dAngle = Bearing - newBear;
            if (dAngle < 0 && -dAngle < TurnSpd || dAngle > 0 && dAngle < TurnSpd)
                Bearing = newBear;
            else if (dAngle < 0)
                Bearing += TurnSpd;
            else if (dAngle > 0)
                Bearing -= TurnSpd;
            Vector3 Direction = (Matrix.CreateTranslation(Front) * Matrix.CreateRotationY(-Bearing)).Translation;
            mPhyObj.Body.Orientation = Matrix.CreateRotationY(-Bearing);
        }
        public override void Draw()
        {
            base.Draw();
        }
    }
}
