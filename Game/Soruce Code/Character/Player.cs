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

using Engine;

namespace Game
{
    public static class PlayerState
    {
        public enum State { NONE, NORMAL, TALK, SELL, PICK, GAMEMENU, INVENTORY, LIFT, SAVE, STUNT, HORN, ACT };
        public static State Currently = State.NORMAL;
        public static string[] AnimKey = { "Idle", "Walk", "Run" };
        public static string[] EquipKey = { "_B", "_B", "_T", "_T" };
        public enum Player_Eq { BAG = 0, BAGHORN = 1, TRUCK = 2, TRUCKHORN = 3 }
        public static Player_Eq Equipedwith = Player_Eq.TRUCKHORN;
    }
    public class Guniman : PhysicsAnimatedObject
    {
        public float GoalHeight = -1.0f;
        protected KeyboardDevice Kbdevice;
        protected GamepadDevice GPdevice;
        protected Camera LookerCam;

        protected Vector3 Front = new Vector3(0, 0, 1);
        protected float MovingSpd = 300.0f;
        protected float TurnSpd = 0.1f;
        protected float Bearing = 0.0f;
        protected TimeSpan CrossfadeTime = new TimeSpan(0, 0, 0, 0, 20);
        protected TimeSpan FootStep = new TimeSpan();

        Shadow gShadow;
        public Vector3 ShadowScale = Vector3.One;
        public List<Equipment> HoldItem = new List<Equipment>();
        protected bool displayEquip = true;
        ////////////////
        /// Sound
        ////////////////
        AudioComponent RefSource;
        Cue SoundThingy = null;
        protected EmoIconSub Emotion;

        public Guniman() : base() { }
        public Guniman(GameScreen Parent) : base(Parent) { }

        public float GetBearing() { return Bearing; }
        public void SetBearing(float Bear) { Bearing = Bear; }
        public Vector3 GetDirection() { return (Matrix.CreateTranslation(Front) * Matrix.CreateRotationY(Bearing)).Translation; }
        protected override void InitializeComponent(GameScreen Parent)
        {
            HoldItem.Capacity = 3;
            Kbdevice = GameEngine.Services.GetService<KeyboardDevice>();
            GPdevice = GameEngine.Services.GetService<GamepadDevice>();
            LookerCam = GameEngine.Services.GetService<Camera>();
            gShadow = GameEngine.Services.GetService<Shadow>();
            RefSource = GameEngine.Services.GetService<AudioComponent>();
            Emotion = new EmoIconSub();
            Emotion.Emotion = 0;

            EmoIconSystem.Subscribe.Add(Emotion);
            //HitEffect = new StarHead(4, 2.0f, GameEngine.Services.GetService<IEContentManager>());
            base.InitializeComponent(Parent);
            DrawOrder = 10;
        }
        private void Footsteps(float spd)
        {
            FootStep += GameEngine.GameTime.ElapsedGameTime;
            // cannot put them together due to iro iro reasons
            if (SoundThingy == null)
            {
                SoundThingy = RefSource.soundBank.GetCue("dirt" + GameEngine.RandomValue.Next(1, 4).ToString());
                SoundThingy.Play();
            }
            else if(spd > 0.1f)
            {
                if (spd < 1.2 * MovingSpd * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds)
                {
                    if (FootStep > TimeSpan.FromMilliseconds(400))
                    {
                        SoundThingy = RefSource.soundBank.GetCue("dirt" + GameEngine.RandomValue.Next(1, 4).ToString());
                        SoundThingy.Play();
                        FootStep = new TimeSpan();
                    }
                }
                else
                {
                    if (FootStep > TimeSpan.FromMilliseconds(150))
                    {
                        SoundThingy = RefSource.soundBank.GetCue("dirt" + GameEngine.RandomValue.Next(1, 4).ToString());
                        SoundThingy.Play();
                        FootStep = new TimeSpan();
                    }
                }
            }
            //if(FootStep.Milliseconds < TimeSpan.FromMilliseconds(
        }
        public void SetBoundingPhysicsBasedOnModel(Vector3 Position, float Scale, Vector3 Rotation, bool FilterFunction, int ID, bool Aboveground)
        {
            mAnimatedObj.Scale = Vector3.One * Scale;
            Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(mAnimatedObj.GetAnimatedModel().Model, Aboveground) * Scale;
            Vector3 PhyExtents = HalfExtents;
            HalfExtents.X *= 0.75f;
            HalfExtents.Z *= 0.75f;
            Position.Y += HalfExtents.Y;

            PhyExtents.X *= 0.5f;
            PhyExtents.Z *= 0.5f;
            base.SetCollision(new BoxShape(PhyExtents, Position, Vector3.Zero, ID, base.Parent));
            mAnimatedObj.CenterOffset = new Vector3(0, -HalfExtents.Y, 0);

            ShadowScale.X = HalfExtents.X / ShadowScale.X * 1.2f;
            ShadowScale.Y = HalfExtents.Y / ShadowScale.Y;
            ShadowScale.Z = HalfExtents.Z / ShadowScale.Z * 1.2f;
            mPhyObj.SetMass(10.0f);
            if (FilterFunction)
                mPhyObj.CollisionSkin.callbackFn += new CollisionCallbackFn(CollisionSkin_callbackFn);
        }
        public bool CollisionSkin_callbackFn(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.GetMaterialID(0) == (int)ObjectFilter.SAVEPOINT)
                GameEngine.Services.GetService<Interactive>().Visible = true;
            return true;
        }
        public void ChangeEquipment(IEContentManager ContentManager)
        {
            HoldItem.Clear();
            switch (PlayerState.Equipedwith)
            {
                case PlayerState.Player_Eq.BAG:
                    {
                        AnimatedObject Bag = new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Equip/TBAG"), Vector3.Zero);
                        Bag.LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                        Bag.Animator.StartClip(Bag.GetAnimatedModel().AnimationClips["Idle"]);
                        HoldItem.Add(new Equipment(Bag));
                        HoldItem[HoldItem.Count - 1].Item.Scale = mAnimatedObj.Scale;
                    }
                    break;
                case PlayerState.Player_Eq.BAGHORN:
                    {
                        AnimatedObject Bag = new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Equip/TBAG"), Vector3.Zero);
                        Bag.LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                        Bag.Animator.StartClip(Bag.GetAnimatedModel().AnimationClips["Idle"]);
                        HoldItem.Add(new Equipment(Bag));
                        HoldItem[HoldItem.Count - 1].Item.Scale = mAnimatedObj.Scale;
                        AnimatedObject Horn = new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Equip/HORN_tbag"), Vector3.Zero);
                        Horn.LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                        Horn.Animator.StartClip(Bag.GetAnimatedModel().AnimationClips["Idle"]);
                        HoldItem.Add(new Equipment(Horn));
                        HoldItem[HoldItem.Count - 1].Item.Scale = mAnimatedObj.Scale;
                    }
                    break;
                case PlayerState.Player_Eq.TRUCK:
                    {
                        AnimatedObject Truck = new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Equip/HTRUCK"), Vector3.Zero);
                        Truck.LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                        Truck.Animator.StartClip(Truck.GetAnimatedModel().AnimationClips["Idle"]);
                        HoldItem.Add(new Equipment(Truck));
                        HoldItem[HoldItem.Count - 1].Item.Scale = mAnimatedObj.Scale;
                    }
                    break;
                case PlayerState.Player_Eq.TRUCKHORN:
                    {
                        AnimatedObject Truck = new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Equip/HTRUCK"), Vector3.Zero);
                        Truck.LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                        Truck.Animator.StartClip(Truck.GetAnimatedModel().AnimationClips["Idle"]);
                        HoldItem.Add(new Equipment(Truck));
                        HoldItem[HoldItem.Count - 1].Item.Scale = mAnimatedObj.Scale;
                        AnimatedObject Horn = new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Equip/HORN_htruck"), Vector3.Zero);
                        Horn.LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                        Horn.Animator.StartClip(Horn.GetAnimatedModel().AnimationClips["Idle"]);
                        HoldItem.Add(new Equipment(Horn));
                        HoldItem[HoldItem.Count - 1].Item.Scale = mAnimatedObj.Scale;
                    }
                    break;
            }
        }
        public void FunAddItemTo(string Where)
        {
            string syntax = "";
            switch (Where.ToLower())
            {
                case "head":
                    syntax = "character_gman_set01_v02_01:character_proto2:head";
                    break;
                case "arm":
                    syntax = "character_gman_set01_v02_01:character_proto2:r_palm";
                    break;
                case "hand":
                    syntax = "character_gman_set01_v02_01:character_proto2:l_elbow";
                    break;
                case "leg":
                    syntax = "character_gman_set01_v02_01:character_proto2:l_ankle";
                    break;
            }
            HoldItem.Add(new Equipment(new StaticObject(GameEngine.Services.GetService<IEContentManager>().Load<Model>("Content/Models/Pickable/pickables"), Vector3.Zero)));
            mAnimatedObj.FindBonePosition("character_gman_set01_v02_01:character_proto2:l_ankle", out HoldItem[HoldItem.Count - 1].ABS_ParentID, out HoldItem[HoldItem.Count - 1].Skin_ParentID);
            HoldItem[HoldItem.Count - 1].ABS_Parent = mAnimatedObj.Transformation[HoldItem[HoldItem.Count - 1].ABS_ParentID];
            HoldItem[HoldItem.Count - 1].UseBone = true;
            HoldItem[HoldItem.Count - 1].Item.Scale = Vector3.One * 0.5f;
            HoldItem[HoldItem.Count - 1].Item.UpdateWorldFromDraw = false;
        }
        public void FunClearItem()
        {
            ChangeEquipment(GameEngine.Services.GetService<IEContentManager>());
        }
        public override void Update()
        {
            if (Kbdevice.Key_Pressed(Keys.NumPad0))
                FunAddItemTo("head");
            if (Kbdevice.Key_Pressed(Keys.NumPad1))
                FunAddItemTo("arm");
            if (Kbdevice.Key_Pressed(Keys.NumPad2))
                FunAddItemTo("leg");
            if (Kbdevice.Key_Pressed(Keys.NumPad3))
                FunAddItemTo("hand");
            if (Kbdevice.Key_Pressed(Keys.NumPad4))
                FunClearItem();
            Emotion.Position = mAnimatedObj.Position - mAnimatedObj.CenterOffset * 2.5f + LookerCam.Rotation.Left * 2.5f;
            if(PlayerState.Currently != PlayerState.State.ACT)
                LookerCam.View = mAnimatedObj.Position - mAnimatedObj.CenterOffset * 1.5f;

            //forever on unless certain conditions are met   eg. talking
            bool lastState = GameEngine.Services.GetService<HealthHud>().Visible;
            displayEquip = true;

            GameEngine.Services.GetService<HealthHud>().Visible = true;
            if (PlayerState.Currently == PlayerState.State.STUNT || PlayerState.Currently == PlayerState.State.HORN)
            {
                if(mAnimatedObj.Animator.AnimationClip.Name != "Idle")
                    mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Idle"], CrossfadeTime);
                mAnimatedObj.Animator.Speed = 0.5f;
                mPhyObj.Velocity = Vector3.Zero;
            }
            if (PlayerState.Currently == PlayerState.State.ACT)
                GameEngine.Services.GetService<HealthHud>().Visible = false;
            else if (PlayerState.Currently == PlayerState.State.SELL)
            {
                mPhyObj.Immovable = true;
                GameEngine.Services.GetService<HealthHud>().Visible = false;
                displayEquip = false;
                if (!Emotion.Visisble)
                {
                    Emotion.Emotion = GameEngine.RandomValue.Next(0, VariableAsset.EmoIcon.Count);
                    Emotion.Visisble = true;
                    Emotion.Timeout = new TimeSpan();
                    Emotion.Duration = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(2, 4), 0);
                }
            }
            else
                mPhyObj.Immovable = false;
            if (mAnimatedObj.Animator.AnimationClip.Name != "Pick")
                mAnimatedObj.Animator.LoopEnabled = true;
            if (PlayerState.Currently == PlayerState.State.PICK)
            {
                mPhyObj.Body.Velocity = Vector3.Zero;
                if (mAnimatedObj.Animator.AnimationClip.Name != "Pick" && lastState)
                {
                    displayEquip = false;
                    mAnimatedObj.Animator.PlayClip(mAnimatedObj.GetAnimatedModel().AnimationClips["Pick"]);
                    mAnimatedObj.Animator.Speed = 1.0f;
                    mAnimatedObj.Animator.LoopEnabled = false;
                }
                else if (mAnimatedObj.Animator.HasFinished && mAnimatedObj.Animator.AnimationClip.Name != "Idle" + PlayerState.EquipKey[(int)PlayerState.Equipedwith])
                {
                    mAnimatedObj.Animator.PlayClip(mAnimatedObj.GetAnimatedModel().AnimationClips["Idle" + PlayerState.EquipKey[(int)PlayerState.Equipedwith]]);
                    mAnimatedObj.Animator.LoopEnabled = true;
                    mAnimatedObj.Animator.Speed = 1.0f;
                }
                else if (mAnimatedObj.Animator.AnimationClip.Name != "Idle" + PlayerState.EquipKey[(int)PlayerState.Equipedwith])
                    displayEquip = false;
                GameEngine.Services.GetService<HealthHud>().Visible = false;
            }

            if (PlayerState.Currently == PlayerState.State.TALK)
            {
                displayEquip = false;
                if (mAnimatedObj.Animator.AnimationClip.Name != "Talk")
                {
                    mAnimatedObj.Animator.PlayClip(mAnimatedObj.GetAnimatedModel().AnimationClips["Talk"]);
                    mAnimatedObj.Animator.Speed = 1.0f;
                }
                GameEngine.Services.GetService<HealthHud>().Visible = false;
                mPhyObj.Body.Velocity = Vector3.Zero;
            }
            if (PlayerState.Currently == PlayerState.State.LIFT)
            {
                displayEquip = false;
                LiftSequence();
                base.Update();
                return;
            }
            mPhyObj.Body.Orientation = Matrix.CreateRotationY(Bearing);
            if (PlayerState.Currently == PlayerState.State.NORMAL)
            {
                NewMovement();
                // Test Buttons
                if (Kbdevice.KeyDown(Keys.PageUp))
                    mPhyObj.Body.Velocity += new Vector3(0, 1.1f, 0);
                else if (Kbdevice.KeyDown(Keys.PageDown))
                    mPhyObj.Body.Position -= new Vector3(0, 0.1f, 0);
            }
            if (PlayerState.Currently != PlayerState.State.SELL && PlayerState.Currently != PlayerState.State.ACT)
            {
                float Spd = mPhyObj.Body.Velocity.Length();
                Footsteps(Spd);
                if (Spd > 0.1)
                {
                    if ((Kbdevice.KeyDown(Keys.RightControl) || GPdevice.ButtonDown(Buttons.X)) && Spd > 2.0f * MovingSpd * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds)
                    {
                        if (mAnimatedObj.Animator.AnimationClip.Name != "Run" + PlayerState.EquipKey[(int)PlayerState.Equipedwith])
                            mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Run" + PlayerState.EquipKey[(int)PlayerState.Equipedwith]],
                                            CrossfadeTime);
                        else
                            mAnimatedObj.Animator.Speed = 1.0f;
                    }
                    else if (mAnimatedObj.Animator.AnimationClip.Name != "Walk" + PlayerState.EquipKey[(int)PlayerState.Equipedwith])
                        mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Walk" + PlayerState.EquipKey[(int)PlayerState.Equipedwith]],
                                        CrossfadeTime);
                    else
                        mAnimatedObj.Animator.Speed = Spd * 0.3f;
                }
                else if (mAnimatedObj.Animator.AnimationClip.Name != "Idle" + PlayerState.EquipKey[(int)PlayerState.Equipedwith] && (PlayerState.Currently == PlayerState.State.NORMAL || PlayerState.Currently == PlayerState.State.INVENTORY || PlayerState.Currently == PlayerState.State.LIFT))
                {
                    mAnimatedObj.Animator.PlaybackMode = PlaybackMode.Forward;
                    mAnimatedObj.Animator.CrossFade(mAnimatedObj.GetAnimatedModel().AnimationClips["Idle" + PlayerState.EquipKey[(int)PlayerState.Equipedwith]], CrossfadeTime);
                    mAnimatedObj.Animator.Speed = 1.0f;
                }
            }
            if (displayEquip)
            {
                int AnimIndex = -1;
                // Synchro items
                if (mAnimatedObj.Animator.AnimationClip.Name.Contains(PlayerState.AnimKey[0]))
                    AnimIndex = 0;
                else if (mAnimatedObj.Animator.AnimationClip.Name.Contains(PlayerState.AnimKey[1]))
                    AnimIndex = 1;
                else if (mAnimatedObj.Animator.AnimationClip.Name.Contains(PlayerState.AnimKey[2]))
                    AnimIndex = 2;

                for (int i = 0; i < HoldItem.Count; i++)
                {
                    // If the current animation allow item anims
                    if (HoldItem[i].Item.Remove)
                    {
                        if(HoldItem[i].Item.bLastDraw)
                            LastDraw.LastList.Remove(HoldItem[i].Item);
                        if (HoldItem[i].Item is StarHead)
                        {
                            PlayerState.Currently = PlayerState.State.NORMAL;
                            GameEngine.Services.GetService<RadialBlur>().EndSequence = true;
                        }
                        HoldItem.RemoveAt(i);
                        continue;
                    }
                    if (HoldItem[i].Item is AnimatedObject && AnimIndex > -1)
                    {
                        AnimatedObject Obj = (AnimatedObject)HoldItem[i].Item;
                        if (!mAnimatedObj.Animator.AnimationClip.Name.Contains(Obj.Animator.AnimationClip.Name))
                            Obj.Animator.CrossFade(Obj.GetAnimatedModel().AnimationClips[PlayerState.AnimKey[AnimIndex]], CrossfadeTime);
                        Obj.Animator.Speed = mAnimatedObj.Animator.Speed;
                        Obj.Animator.Time = mAnimatedObj.Animator.Time;
                    }
                    
                    //HoldItem[i].Item.Scale = mAnimatedObj.Scale;
                    //HoldItem[i].UpdateTransform(mAnimatedObj.Animator.SkinnedBoneTransforms[HoldItem[i].Skin_ParentID], mAnimatedObj.World);
                    HoldItem[i].UpdateTransform(mAnimatedObj.Animator.SkinnedBoneTransforms[HoldItem[i].Skin_ParentID],
                                    mAnimatedObj.Position + mAnimatedObj.CalculatedOffset, mAnimatedObj.Rotation, mAnimatedObj.Scale);
                }
            }
            if (Kbdevice.Key_Pressed(Keys.M))
            {
                GameEngine.Services.GetService<PlayerStats>().MoneyChange(10000);
            }
            if (Kbdevice.Key_Pressed(Keys.F))
            {
                GameEngine.Services.GetService<DialogueEngine>().StartConversation("Portrait_Guniman", "Portrait_Guniman", true, 0);
            }
            //if (Kbdevice.Key_Pressed(Keys.H) || GPdevice.Button_Pressed(Buttons.LeftShoulder) && PlayerState.Currently == PlayerState.State.NORMAL)

            if (Kbdevice.Key_Pressed(Keys.S) && Kbdevice.Key_Pressed(Keys.D) && PlayerState.Currently == PlayerState.State.NORMAL ||
                GPdevice.Button_Pressed(Buttons.LeftShoulder) && GPdevice.Button_Pressed(Buttons.RightShoulder) && PlayerState.Currently == PlayerState.State.NORMAL)
            {
                if (GameEngine.Services.GetService<Inventory>().GetAmount("GuniHorn") == 1)
                {
                    //must have horn
                    GameEngine.Services.GetService<AudioGame>().Visible = true;
                    PlayerState.Currently = PlayerState.State.HORN;
                }
            }
            if ((Kbdevice.Key_Pressed(Keys.V) || GPdevice.Button_Pressed(Buttons.Y))&& PlayerState.Currently != PlayerState.State.GAMEMENU)
            {
                GameEngine.Services.GetService<InventoryDisplay>().Visible = true;
                PlayerState.Currently = PlayerState.State.GAMEMENU;
                mPhyObj.Velocity = Vector3.Zero;
            }
            else if ((Kbdevice.Key_Pressed(Keys.N) || GPdevice.Button_Pressed(Buttons.Start)) && PlayerState.Currently != PlayerState.State.GAMEMENU)
            {
                GameEngine.Services.GetService<InGameMainMenu>().Visible = true;
                PlayerState.Currently = PlayerState.State.GAMEMENU;
                mPhyObj.Velocity = Vector3.Zero;
            }
            base.Update();
            //Emitors[0].Update();
            //Emitors[0].Position = (mAnimatedObj.Transformation[13] * mAnimatedObj.Animator.SkinnedBoneTransforms[7] * mAnimatedObj.World).Translation;
        }
        void LiftSequence()
        {
            if (GoalHeight > -1)
            {
                Vector3 FloorStand = mPhyObj.Position + mAnimatedObj.CenterOffset;
                if (FloorStand.Y < GoalHeight - 0.2f)
                    mPhyObj.Position += new Vector3(0, 8.0f, 0) * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                else if (FloorStand.Y > GoalHeight + 0.2f)
                    mPhyObj.Position -= new Vector3(0, 8.0f, 0) * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                else
                {
                    SoundThingy.Stop(AudioStopOptions.Immediate);
                    SoundThingy = RefSource.soundBank.GetCue("bell1");
                    SoundThingy.Play();
                    GPdevice.Vibrate(0, 0, 0);
                    GoalHeight = -1;
                }
                if (SoundThingy.IsStopped)
                {
                    SoundThingy = RefSource.soundBank.GetCue("lift_moving");
                    SoundThingy.Play();
                    GPdevice.Vibrate(0, 0.2f, 0.2f);
                }
            }
        }

        public void NewMovement()
        {
            Vector2 NewDir = Vector2.Zero;
            float Factor = 1.0f;
            NewDir = GPdevice.LeftStickPosition;
            if (GPdevice.ButtonDown(Buttons.X))
                Factor = 3.0f;

            if (NewDir.X == 0 && NewDir.Y == 0)
            {
                if (GPdevice.ButtonDown(Buttons.DPadDown))
                    NewDir.Y = -1;
                else if (GPdevice.ButtonDown(Buttons.DPadUp))
                    NewDir.Y = 1;

                if (GPdevice.ButtonDown(Buttons.DPadLeft))
                    NewDir.X = -1;
                else if (GPdevice.ButtonDown(Buttons.DPadRight))
                    NewDir.X = 1;
            }
            if (NewDir.X == 0 && NewDir.Y == 0)
            {
                if (Kbdevice.KeyDown(Keys.RightControl))
                    Factor = 3.0f;
                if (Kbdevice.KeyDown(Keys.Down))
                    NewDir.Y = -1;
                else if (Kbdevice.KeyDown(Keys.Up))
                    NewDir.Y = 1;

                if (Kbdevice.KeyDown(Keys.Left))
                    NewDir.X = -1;
                else if (Kbdevice.KeyDown(Keys.Right))
                    NewDir.X = 1;
            }
            if (NewDir.X == 0 && NewDir.Y == 0)
            {
                GameEngine.Services.GetService<HealthHud>().ChangeLifeBy(0.001f);
                return;
            }
            else
                GameEngine.Services.GetService<HealthHud>().ChangeLifeBy(-0.002f * Factor);
            
            // Corner Value = -1 -1
            float d = ((Static3rdCam)mAnimatedObj.Camera).OffsetAngle;
            if (d < -(Math.PI) * 0.5f)
                d = (float)Math.PI * 1.5f;
            float newBear = ((float)Math.Atan2(NewDir.Y, NewDir.X) - (float)Math.Atan2(-1, -1)) + d;

            float dAngle = newBear - Bearing;
            if (dAngle < 0 && -dAngle < TurnSpd || dAngle > 0 && dAngle < TurnSpd)
                Bearing = newBear;
            else if (dAngle > 0 && dAngle > Math.PI)
                Bearing -= TurnSpd;
            else if (dAngle < 0 && (-dAngle < Math.PI))
                Bearing -= TurnSpd;
            else if (Bearing != newBear)
                Bearing += TurnSpd;

            if (Bearing > Math.PI * 2.0f)
                Bearing = 0;
            else if (Bearing < 0)
                Bearing = (float)Math.PI * 2.0f;

            Vector3 Direction = (Matrix.CreateTranslation(Front) * Matrix.CreateRotationY(Bearing)).Translation;
            mPhyObj.Body.Velocity = Direction * MovingSpd * Factor * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
        }
        public override void Draw()
        {
            if (ShadowScale.LengthSquared() > 0 && SceneControl.RenderMode != SceneControl.Rendering.SHADOW)
            {
                GameEngine.GraphicDevice.RenderState.DepthBias = -0.00001f;
                gShadow.Draw(new Vector3(mPhyObj.Body.Position.X, 0.01f, mPhyObj.Body.Position.Z), Vector3.Zero, ShadowScale, mPhyObj.Rotation);
                GameEngine.GraphicDevice.RenderState.DepthBias = 0;
            }
            mAnimatedObj.Color.W = 1.0f;
            /*GameEngine.Services.GetService<Billboard>().Position = mPhyObj.Body.Position + new Vector3(0, 10, 0);
            GameEngine.Services.GetService<Billboard>().SetSize(5.0f);
            GameEngine.Services.GetService<Billboard>().Render(LookerCam, null);*/
            base.Draw();
            if (displayEquip || PlayerState.Currently == PlayerState.State.NORMAL || PlayerState.Currently == PlayerState.State.GAMEMENU)
            {
                for (int i = 0; i < HoldItem.Count; i++)
                    if(!HoldItem[i].Item.bLastDraw)
                        HoldItem[i].Draw();
            }
            //HitEffect.Render();
        }
    }
}
