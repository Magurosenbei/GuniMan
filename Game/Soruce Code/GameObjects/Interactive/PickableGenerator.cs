using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Engine;
using XmlContentExtension;


namespace Game
{
    public class PickableGenerator : Component
    {
        int PickableLimit = 3;
        List<HDBLevelColBox> Plots;
        List<PickableCluster> Clusters = new List<PickableCluster>();
        public PickableGenerator(List<HDBLevelColBox> Areas, IEContentManager ContentManger) : base() { Generate(Areas, ContentManger); this.Parent = GameEngine.DefaultScreen; }
        public PickableGenerator(List<HDBLevelColBox> Areas, IEContentManager ContentManger, GameScreen Parent) : base(Parent) { Generate(Areas, ContentManger); this.Parent = Parent; }

        public void Generate(List<HDBLevelColBox> Areas, IEContentManager ContentManger)
        {
            Plots = Areas;
            PickableLimit *= Areas.Count;
            Visible = false;
            for (int i = 0; i < Areas.Count; i++)
            {
                Clusters.Add(new PickableCluster());
                Clusters[Clusters.Count - 1].Sector = i;
                PickableLimit -= Clusters[Clusters.Count - 1].Generate(Areas[i].Position, Areas[i].HalfExtents, Areas[i].PitchYawRoll, ContentManger, PickableLimit);
            }
        }
        public void Reset(IEContentManager ContentManager)
        {
            for (int i = 0; i < Clusters.Count; i++)
                Clusters[i].Disable();
            Clusters.Clear();
            PickableLimit *= Plots.Count;
            Clusters.Capacity = Plots.Count;
            for (int i = 0; i < Plots.Count; i++)
            {
                Clusters.Add(new PickableCluster());
                Clusters[Clusters.Count - 1].Sector = i;
                PickableLimit -= Clusters[Clusters.Count - 1].Generate(Plots[i].Position, Plots[i].HalfExtents, Plots[i].PitchYawRoll, ContentManager, PickableLimit);
            }
        }
        public override void Update()
        {
            for (int i = 0; i < Clusters.Count; i++)
                Clusters[i].Update();
            base.Update();
        }
        public override void Draw()
        {
            if (PlayerState.Currently == PlayerState.State.SELL || PlayerState.Currently == PlayerState.State.ACT) return;
            for (int i = 0; i < Clusters.Count; i++)
                Clusters[i].Draw();
            base.Draw();
        }
        public override void DisableComponent()
        {
            for (int i = 0; i < Clusters.Count; i++)
                Clusters[i].Disable();
            Clusters.Clear();
            base.DisableComponent();
        }
    }

    public class PickableCluster
    {
        List<Pickable> FloorItems = new List<Pickable>();
        Camera camera;
        KeyboardDevice KB;
        GamepadDevice GPD;
        Interactive Displaycall;
        Guniman Player;

        public int Sector = 0;

        PlayerEvent EventCaller;

        Vector3 Position;
        Vector3 HalfExtent;
        Vector3 PYW;

        public PickableCluster()
        {
            camera = GameEngine.Services.GetService<Camera>();
            KB = GameEngine.Services.GetService<KeyboardDevice>();
            GPD = GameEngine.Services.GetService<GamepadDevice>();
            Displaycall = GameEngine.Services.GetService<Interactive>();
            Player = GameEngine.Services.GetService<Guniman>();
            EventCaller = GameEngine.Services.GetService<PlayerEvent>();
        }
        public void RespawnItem(Pickable Target)
        {
            float RandomX, RandomZ;
            Regen:
                RandomX = (float)GameEngine.RandomValue.Next(-(int)(HalfExtent.X), (int)(HalfExtent.X));
                RandomZ = (float)GameEngine.RandomValue.Next(-(int)(HalfExtent.Z), (int)(HalfExtent.Z));

                if (RandomZ < -HalfExtent.Z || RandomZ > HalfExtent.Z)
                    goto Regen;
                if (RandomX < -HalfExtent.X || RandomX > HalfExtent.X)
                    goto Regen;
                Vector3 newPoint = new Vector3(RandomX, 2, RandomZ) + Position;

                for (int j = 0; j < FloorItems.Count; j++)
                    if ((newPoint - FloorItems[j].GetPosition()).LengthSquared() < 25.0f)
                        goto Regen;

                //Target.SetPosition(newPoint);
                Target.Reset();
                Target.RespawnElapsed = new TimeSpan();
                Target.RespawnWait = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(30, 45));
                Target.ModelAlpha = 0.0f;
                Target.PickedUp = false;
                Target.Respawn = false;
                Target.Remove = false;
                Target.Visible = true;
                Target.Id = GameEngine.Services.GetService<PlayerEvent>().RandomStuffIDFloor();
                Target.Amt = GameEngine.RandomValue.Next(1, 4);
                Matrix newTransform = Matrix.CreateTranslation(newPoint - Position) *
                                        Matrix.CreateFromYawPitchRoll(PYW.Y, PYW.X, PYW.Z);
                Target.SetPosition(newTransform.Translation + Position);
        }
        public int Generate(Vector3 Point, Vector3 HalfExtents, Vector3 PitchYawRoll, IEContentManager ContentManager, int Left)
        {
            Position = Point;
            HalfExtent = HalfExtents;
            PYW = PitchYawRoll;
            if (Left < 0) return 0;
            int ZoneOccupy = GameEngine.RandomValue.Next(2, 5);
            if (Left - ZoneOccupy <= 0) ZoneOccupy = Left;
            float RandomX, RandomZ;
            for (int i = 0; i < ZoneOccupy; i++)
            {
            Regen:
                RandomX = (float)GameEngine.RandomValue.Next(-(int)(HalfExtents.X), (int)(HalfExtents.X));
                RandomZ = (float)GameEngine.RandomValue.Next(-(int)(HalfExtents.Z), (int)(HalfExtents.Z));

                if (RandomZ < -HalfExtents.Z || RandomZ > HalfExtents.Z)
                    goto Regen;
                if (RandomX < -HalfExtents.X || RandomX > HalfExtents.X)
                    goto Regen;
                Vector3 newPoint = new Vector3(RandomX, 2, RandomZ) + Point;

                for (int j = 0; j < FloorItems.Count; j++)
                    if ((newPoint - FloorItems[j].GetPosition()).LengthSquared() < 25.0f)
                        goto Regen;
                Pickable Item = new Pickable();
                Item.Setup(newPoint, ContentManager);
                Item.SetPosition(newPoint);
                Item.RespawnWait = new TimeSpan(0, 0, 0, GameEngine.RandomValue.Next(30, 45));
                Item.Id = GameEngine.Services.GetService<PlayerEvent>().RandomStuffIDFloor();
                Item.Amt = GameEngine.RandomValue.Next(1, 4);
                FloorItems.Add(Item);
            }
            for (int i = 0; i < FloorItems.Count; i++)
            {
                Matrix newTransform = Matrix.CreateTranslation(FloorItems[i].GetPosition() - Point) *
                                        Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z);
                FloorItems[i].SetPosition(newTransform.Translation + Point);
            }
            return FloorItems.Count;
        }
        public void Disable()
        {
            for (int i = 0; i < FloorItems.Count; i++)
                FloorItems[i].Disable();
            FloorItems.Clear();
        }
        public void Update()
        {
            int sls = -1, seg = -1;
            EventCaller.GetPickCall(Sector, out sls, out seg);
            if (seg == Sector && sls > -1)
                FloorItems[sls].PickedUp = true;

            for(int i = 0; i < FloorItems.Count; i++)
            {
                float blend = 1;
                //bool DrawShadow = true;
                if (!FloorItems[i].Remove)
                {
                    FloorItems[i].Visible = camera.InView(FloorItems[i].GetPosition(), FloorItems[i].ViewRange, out blend, out FloorItems[i].DistanceFromCamera);
                    if (FloorItems[i].DistanceFromCamera > 150) FloorItems[i].Visible = false;
                }
                if (FloorItems[i].Visible)
                {
                    FloorItems[i].Update();
                    if (!FloorItems[i].PickedUp && FloorItems[i].InRange(Player.mPhyObj.Position, Player.mAnimatedObj.ViewRange))
                    {
                        Vector2 RelDir = new Vector2(Player.mPhyObj.Position.X - FloorItems[i].GetPosition().X,
                                                    Player.mPhyObj.Position.Z - FloorItems[i].GetPosition().Z);
                        if ((Math.Abs((float)Math.Atan2(-RelDir.Y, -RelDir.X) - (float)Math.Atan2(Player.GetDirection().Z, Player.GetDirection().X))) > 1.0f)
                            continue;
                        Displaycall.Visible = true;
                        if (PlayerState.Currently == PlayerState.State.NORMAL &&
                            (KB.Key_Pressed(Microsoft.Xna.Framework.Input.Keys.Z) || GPD.Button_Pressed(Microsoft.Xna.Framework.Input.Buttons.A)))
                        {
                            //FloorItems[i].PickedUp = true;
                            // Call Pick Sequence
                            EventCaller.PickUpFloor(i, Sector, FloorItems[i].Id, FloorItems[i].Amt);
                        }
                    }
                }         
                if (FloorItems[i].Respawn)
                    RespawnItem(FloorItems[i]);
            }
        }
        public void Draw()
        {
            for (int i = 0; i < FloorItems.Count; i++)
                if (FloorItems[i].Visible)
                    FloorItems[i].Draw();
        }
    }
}
