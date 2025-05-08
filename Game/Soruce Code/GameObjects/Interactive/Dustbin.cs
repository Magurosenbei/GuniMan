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
    public class DustbinGenerator : Component
    {
        List<Dustbin> Bins = new List<Dustbin>();

        Guniman Player;
        KeyboardDevice KB;
        GamepadDevice GPD;
        PlayerEvent EventCaller;
        Interactive Displaycall;

        public DustbinGenerator() : base() { }
        public DustbinGenerator(GameScreen Parent) : base(Parent) { }
        public void Load(List<HDBLevelColBox> Def, IEContentManager ContentManager)
        {
            for (int i = 0; i < Def.Count; i++)
            {
                // Able to random dustbins
                Bins.Add(new Dustbin(Parent));
                Bins[Bins.Count - 1].Load(ContentManager);
                Bins[Bins.Count - 1].mStaticObj.Color.X = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
                Bins[Bins.Count - 1].mStaticObj.Color.Y = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
                Bins[Bins.Count - 1].mStaticObj.Color.Z = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;

                Bins[Bins.Count - 1].mPhyObj.Body.MoveTo(Bins[Bins.Count - 1].mPhyObj.Body.Position + Def[i].Position, Matrix.CreateRotationY(MathHelper.ToRadians(GameEngine.RandomValue.Next(0, 360))));
            }
            Player = GameEngine.Services.GetService<Guniman>();
            KB = GameEngine.Services.GetService<KeyboardDevice>();
            GPD = GameEngine.Services.GetService<GamepadDevice>();
            Displaycall = GameEngine.Services.GetService<Interactive>();
            EventCaller = GameEngine.Services.GetService<PlayerEvent>();
        }
        public override void DisableComponent()
        {
            for (int i = 0; i < Bins.Count; i++)
                Bins[i].mPhyObj.DisableComponent();
            Bins.Clear();
            base.DisableComponent();
        }
        public override void Update()
        {
            for (int i = 0; i < Bins.Count; i++)
            {
                if (!Bins[i].Visible)
                {
                    Bins[i].mPhyObj.Body.DisableBody();
                    continue;
                }
                else
                    Bins[i].mPhyObj.Body.EnableBody();
                if (!Bins[i].InRange(Player.mPhyObj.BoundingBox)) continue;
                Vector2 RelDir = new Vector2(Player.mPhyObj.Position.X - Bins[i].mPhyObj.Position.X,
                            Player.mPhyObj.Position.Z - Bins[i].mPhyObj.Position.Z);
                if ((Math.Abs((float)Math.Atan2(-RelDir.Y, -RelDir.X) - (float)Math.Atan2(Player.GetDirection().Z, Player.GetDirection().X))) > 1.0f)
                    continue;
                Displaycall.Visible = true;
                if (PlayerState.Currently == PlayerState.State.NORMAL &&
                        (KB.Key_Pressed(Microsoft.Xna.Framework.Input.Keys.Z) || GPD.Button_Pressed(Microsoft.Xna.Framework.Input.Buttons.A)))
                {
                    if (!Bins[i].Taken)
                    {
                        Bins[i].Taken = true;
                        // Call Pick Sequence
                        EventCaller.PickUpDustBin();   // Pick
                    }
                    else
                    {
                        EventCaller.Empty();   // No more item 
                    }
                }  
            }
            base.Update();
        }
    }
    public class Dustbin : PhysicsStaticObject
    {
        public bool Taken = false;
        protected TimeSpan RespawnDuration = new TimeSpan();
        protected TimeSpan TakenElapsed = new TimeSpan();
        public Dustbin(GameScreen Parent) : base(Parent) { }
        public Dustbin() : base() { }

        public void Load(IEContentManager ContentManager)
        {
            Setup(VariableAsset.DustBin[GameEngine.RandomValue.Next(0, VariableAsset.DustBin.Count)], "Content/Shader Fx/CelFx", ContentManager);
            Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(mStaticObj.GetModel(), true);
            SetBoundingPhysicsBasedOnModel(new Vector3(0, 0, 0), 1.0f, Vector3.Zero, true, (int)ObjectFilter.INTERACTIVE);
            mPhyObj.Body.MoveTo(new Vector3(0, HalfExtents.Y, 0), Matrix.Identity);
            mStaticObj.CenterOffset = new Vector3(0, -HalfExtents.Y, 0);

            RespawnDuration = new TimeSpan(0, 0, GameEngine.RandomValue.Next(1, 5), 0, 0);

            mPhyObj.Immovable = true;
            mPhyObj.Body.EnableBody();
            base.InitializeComponent(Parent);
        }
        public bool Filter(CollisionSkin skin0, CollisionSkin skin1)
        {
            if (skin1.GetMaterialID(0) == (int)ObjectFilter.CHARACTER || skin1.GetMaterialID(0) == (int)ObjectFilter.PLAYER)
                return true;
            else return false;
        }
        public bool InRange(BoundingBox PlayerBox)
        {
            if(mPhyObj.BoundingBox.Intersects(PlayerBox))
                return true;   // Not in place at all
            return false;
        }
        public override void Update()
        {
            if (Taken)
            {
                TakenElapsed += GameEngine.GameTime.ElapsedGameTime;
                if (TakenElapsed > RespawnDuration)
                {
                    TakenElapsed = new TimeSpan();
                    RespawnDuration = new TimeSpan(0, 0, GameEngine.RandomValue.Next(1, 5), 0, 0);
                    Taken = false;
                }
            }
            base.Update();
            if (!Visible && mPhyObj.Body.IsBodyEnabled)
                mPhyObj.Body.DisableBody();
            else if (Visible && !mPhyObj.Body.IsBodyEnabled)
                mPhyObj.Body.EnableBody();
        }
    }
}
