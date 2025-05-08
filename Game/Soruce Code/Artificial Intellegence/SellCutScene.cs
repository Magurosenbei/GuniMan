using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Engine;
using XmlContentExtension;
using System.Diagnostics;

namespace Game
{
    // No physics Body will be used in this, total graphics
    public class SellCutScene : Component
    {
        int NumberOfHumaniods = 3;

        Vector3 CenterPoint = new Vector3(0, 0, 11);                // Center of Effect
        Vector3 PlayerSitPointOffSet = new Vector3(0, 0, -1.5f);    // PlayerSit from Center Effect

        float RadiusOfEffect = 30.0f;       // Max
        float RadiusOut = 10.0f;            // Min
        // Radians
        float StartAngle = 2.09439f;
        float EndAngle = -2.09439f;
        float IntervalAngle = 0.6982f;

        Cue BGM = null;

        TimeSpan Duration = new TimeSpan(0, 0, 0, 30, 0);
        TimeSpan Elapsed = new TimeSpan();

        List<HDBLevelColBox> AreaSpawnZone = new List<HDBLevelColBox>();
        List<List<Vector3>> StandPoint = new List<List<Vector3>>();
        List<CutsceneActor> Customer = new List<CutsceneActor>();
        Humaniod_Buffer Human_Storage;

        public SellCutScene(List<HDBLevelColBox> Area, Vector3 Sell_CenterPoint, Vector3 Sell_PlayerSitOffSet,
                            double Sell_RadiusOfEffect, double Sell_RadiusOutBounce,
                            double Sell_StartAngle, double Sell_EndAngle, double Sell_IntervalAngle, GameScreen Parent)
            : base(Parent)
        {
            AreaSpawnZone = Area;
            CenterPoint = Sell_CenterPoint;
            PlayerSitPointOffSet = Sell_PlayerSitOffSet;
            RadiusOfEffect = (float)Sell_RadiusOfEffect;
            RadiusOut = (float)Sell_RadiusOutBounce;
            StartAngle = (float)Sell_StartAngle;
            EndAngle = (float)Sell_EndAngle;
            IntervalAngle = (float)Sell_IntervalAngle;
            Human_Storage = GameEngine.Services.GetService<Humaniod_Buffer>();
            Visible = false;
            BGM = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("MenuTrack");
        }

        public void Disable() // call this when deleting world
        {
            AreaSpawnZone.Clear();
            StandPoint.Clear();
        }

        public void Initialize_Scene(int Popularity, int OverPricePercentage) // Once per sell, generates characters for scene
        {
            GameEngine.Services.GetService<OldTv>().Visible = true;
            ((Static3rdCam)GameEngine.Services.GetService<Camera>()).OffsetAngle = 0.0f;
            ((Static3rdCam)GameEngine.Services.GetService<Camera>()).AllowControl = false;
            Guniman Player = GameEngine.Services.GetService<Guniman>();
            Player.mPhyObj.Position = PlayerSitPointOffSet + CenterPoint - Player.mAnimatedObj.CenterOffset;
            float Bear = ((float)Math.Atan2(PlayerSitPointOffSet.Z, PlayerSitPointOffSet.X) - (float)Math.Atan2(-1, 0));
            Player.SetBearing(Bear);
            Player.mAnimatedObj.Animator.Speed = 1.0f;
            Player.mAnimatedObj.Animator.PlayClip(Player.mAnimatedObj.GetAnimatedModel().AnimationClips["Talk"]);
            PlayerState.Currently = PlayerState.State.SELL;
            Customer.Clear();
            Elapsed = new TimeSpan();
            List<int> numbers = new List<int>();
            for (int j = 0; j < StandPoint.Count; j++)
                for (int i = 0; i < StandPoint[j].Count; i++)
                    numbers.Add(i);
            int min = 2;
            int max = Popularity * numbers.Count / 100;
            if (Popularity > 50)
                min = Popularity * 50 / 100 * numbers.Count / 100;
            if (max < min) max += 3;
            NumberOfHumaniods = GameEngine.RandomValue.Next(min, max);
            // Fill up Random AI
            int seg = 0; int par = 0; int index = 0;
            for (int i = 0; i < NumberOfHumaniods; i++)
            {
                if (par >= StandPoint[seg].Count)
                {
                    par = 0;
                    seg++;
                }
                Customer.Add(new CutsceneActor(Parent));
                int Zone = GameEngine.RandomValue.Next(0, AreaSpawnZone.Count);
                if(Zone >= AreaSpawnZone.Count) Zone = AreaSpawnZone.Count;

                Customer[i].Comrades = Customer;
                Customer[i].SetActor(SpawnInArea(AreaSpawnZone[Zone]),
                    StandPoint[seg][numbers[index++]] + CenterPoint, CenterPoint + PlayerSitPointOffSet, Popularity, OverPricePercentage, NumberOfHumaniods, Human_Storage.GetRandomNPC());
                Customer[i].SetBoundingPhysicsBasedOnModel(Customer[i].mAnimatedObj.Position, 0.4f, Vector3.Zero, false, (int)ObjectFilter.CHARACTER, true);
                Customer[i].mPhyObj.Mass = 300.0f;
                Customer[i].mPhyObj.CollisionSkin.callbackFn += new JigLibX.Collision.CollisionCallbackFn(Filter);
                int NearestPlace = 0;
                float LastD = 10000000.0f;
                for (int j = 0; j < AreaSpawnZone.Count; j++)
                {
                    float d = (Customer[i].GoalPoint - AreaSpawnZone[j].Position).LengthSquared();
                    if (d < LastD)
                    {
                        NearestPlace = j;
                        LastD = d;
                    }
                }
                Customer[i].RetirePoint = SpawnInArea(AreaSpawnZone[NearestPlace]);

                par++;
            }
            Visible = true;
            GameEngine.Services.GetService<Camera>().Offset = new Vector3(40, 15, 40);
            //GameEngine.Services.GetService<Physics>().UpdatePhysics = false;
            //PlayerState.Currently = PlayerState.State.s
        }
        bool Filter(JigLibX.Collision.CollisionSkin skin0, JigLibX.Collision.CollisionSkin skin1)
        {
            switch((ObjectFilter)skin1.GetMaterialID(0))
            {
                case ObjectFilter.BUILDINGS:
                    return true;
            }
            return false;
        }

        public void EndScene()
        {
            for (int i = 0; i < Customer.Count; i++)
            {
                Customer[i].mPhyObj.Body.DisableBody();
                Customer[i].DisableComponent();
            }
            GameEngine.Services.GetService<Camera>().Offset = new Vector3(20, 7, 20);   // Reset to default
            ((Static3rdCam)GameEngine.Services.GetService<Camera>()).OffsetAngle = 0.0f;
            ((Static3rdCam)GameEngine.Services.GetService<Camera>()).AllowControl = true;
            PlayerState.Currently = PlayerState.State.NORMAL;
            Visible = false;
            GameEngine.Services.GetService<OldTv>().Visible = false;
            //GameEngine.Services.GetService<Physics>().UpdatePhysics = true;
        }
        public Vector3 SpawnInArea(HDBLevelColBox Area)
        {
            float RandomX, RandomZ;
        Regen:
            RandomX = (float)GameEngine.RandomValue.Next(-(int)(Area.HalfExtents.X), (int)(Area.HalfExtents.X));
            RandomZ = (float)GameEngine.RandomValue.Next(-(int)(Area.HalfExtents.Z), (int)(Area.HalfExtents.Z));
            if (RandomZ < -Area.HalfExtents.Z || RandomZ > Area.HalfExtents.Z)
                goto Regen;
            if (RandomX < -Area.HalfExtents.X || RandomX > Area.HalfExtents.X)
                goto Regen;
            Vector3 newPoint = new Vector3(RandomX, 0, RandomZ) + Area.Position;
            return (Matrix.CreateTranslation(newPoint) *
                        Matrix.CreateFromYawPitchRoll(Area.PitchYawRoll.Y, Area.PitchYawRoll.X, Area.PitchYawRoll.Z)).Translation;
        }


        public void GenerateGoalPointDir()
        {
            int mod = 0;
            float d = 4.0f;
            for (float Depth = RadiusOut; Depth < RadiusOfEffect; Depth += d)
            {
                StandPoint.Add(new List<Vector3>());
                float I = 0.0f;
                if (mod % 4 == 0)
                    I = IntervalAngle * 0.5f;
                else if (mod % 2 == 0)
                    I = IntervalAngle * 0.75f;
                else
                    I = IntervalAngle * 0.25f;
                for (float Angle = EndAngle + I; Angle < StartAngle - I; Angle += IntervalAngle)
                {
                    Vector3 Point = (Matrix.CreateTranslation(new Vector3(0, 0, 1)) * Matrix.CreateRotationY(Angle)).Translation;
                    StandPoint[mod].Add(Point * Depth);
                }
                d *= 0.9f;
                mod++;
            }
        }

        public override void Update()
        {
            if (!Visible) return;
            
            bool End = true;
            Parallel.For(0, Customer.Count, delegate(int i)
            //for(int i = 0; i < Customer.Count; i++)
            {
                if (Customer[i].NowDoing != ActState.RETIRE && Customer[i].NowDoing != ActState.NONE)
                    End = false;
                Customer[i].Update();
            });
            //if (Elapsed > Duration)
            if (End)
            {
                Elapsed += GameEngine.GameTime.ElapsedGameTime;
                if(Elapsed > TimeSpan.FromSeconds(2))
                    GameEngine.Services.GetService<TransitionScreen>().StartFading(6, -1);
            }
            /*if (End && Elapsed > TimeSpan.FromSeconds(3))
                GameEngine.Services.GetService<TransitionScreen>().StartFading(-1, -1);
            if (End && Elapsed > TimeSpan.FromMilliseconds(3100))
                EndScene();*/
        }

        public override void Draw()
        {
        }
        public List<int> Shuffle(List<int> quelot)
        {
            for (int i = 0; i < quelot.Count; i++)
            {
                int r = i + GameEngine.RandomValue.Next(0, quelot.Count - 1);
                int tmp = quelot[i];
                quelot[i] = quelot[r];
                quelot[r] = tmp;
            }
            return quelot;
        }
    }
}
