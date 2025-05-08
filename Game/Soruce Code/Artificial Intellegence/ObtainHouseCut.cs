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
    public class ObtainHouseCut : Component
    {
        List <AnimatedObject> Cats = null;
        Guniman Player;

        PlayerState.Player_Eq PrevEq;
        Vector3 OldPosition;
        Vector3 PrevCamera;

        Static3rdCam Camera;

        TimeSpan Elapsed = new TimeSpan();

        float PlayerJumpValue = 0.0f;
        float Spinv = 0.0f;
        Vector3 Playerpos;
        bool Flip = false;

        public ObtainHouseCut(IEContentManager ContentManager, GameScreen Parent) :base(Parent)
        {
            AnimatedObject Cat = GameEngine.Services.GetService<Humaniod_Buffer>().GetCatNPC(0);
            Cats = new List<AnimatedObject>();
            for (int i = 0; i < 4; i++)
            {
                Cats.Add(new AnimatedObject(Cat.GetAnimatedModel(), Vector3.Zero));
                Cats[i].Shader = Cat.Shader.Clone(GameEngine.GraphicDevice);
                Cats[i].Camera = Cat.Camera;
            }
            Cat = null;
            
            Visible = false;
        }

        // Foward  = 0, 0, 25
        public void Initialize_Scene(Vector3 Position, Vector3 ForwardVector, Matrix Rotation)
        {
            Vector3 Point = (Matrix.CreateTranslation(ForwardVector) * Rotation).Translation + Position;
            PlayerJumpValue = 0.0f;
            Flip = false;
            Player = GameEngine.Services.GetService<Guniman>();
            PrevEq = PlayerState.Equipedwith;
            PlayerState.Equipedwith = PlayerState.Player_Eq.BAGHORN;
            PlayerState.Currently = PlayerState.State.ACT;
            Player.ChangeEquipment(GameEngine.Services.GetService<IEContentManager>());
            OldPosition = Player.mPhyObj.Position;
            Playerpos = Player.mPhyObj.Position = Point - Player.mAnimatedObj.CalculatedOffset;
            Player.mPhyObj.Rotation = Rotation;

            Player.mAnimatedObj.Animator.PlayClip(Player.mAnimatedObj.GetAnimatedModel().AnimationClips["Walk_B"]);
            Player.mAnimatedObj.Animator.Speed = 0.7f;
            Player.mAnimatedObj.Animator.LoopEnabled = true;
            Cats[0].Rotation = Cats[1].Rotation = Cats[2].Rotation = Cats[3].Rotation = Rotation * Matrix.CreateRotationY(-1.571f);
            Cats[0].Position = Rotation.Left * 7.0f + Point;
            Cats[1].Position = Rotation.Left * 4.0f + Point;
            Cats[2].Position = Rotation.Right * 4.0f + Point;
            Cats[3].Position = Rotation.Right * 7.0f + Point;


            Cats[0].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;
            Cats[1].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;
            Cats[2].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;
            Cats[3].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;

            Cats[0].Animator.PlayClip(Cats[0].GetAnimatedModel().AnimationClips["jump"]);
            Cats[1].Animator.PlayClip(Cats[1].GetAnimatedModel().AnimationClips["jump"]);
            Cats[2].Animator.PlayClip(Cats[2].GetAnimatedModel().AnimationClips["jump"]);
            Cats[3].Animator.PlayClip(Cats[3].GetAnimatedModel().AnimationClips["jump"]);

            Camera = ((Static3rdCam)GameEngine.Services.GetService<Camera>());
            Camera.OffsetAngle = 0.0f;
            PrevCamera = Camera.Offset;
            Camera.AllowControl = false;
            Camera.Offset = (Matrix.CreateTranslation(ForwardVector) * Rotation).Translation + new Vector3(0, 0.5f, 0);

            Visible = true;
        }
        public void EndScene()
        {
            PlayerState.Equipedwith = PrevEq;
            PlayerState.Currently = PlayerState.State.NORMAL;
            Camera.Offset = PrevCamera;
            Camera.AllowControl = true;
            Player.ManuelUpdate = false;
            Player.mPhyObj.Position = OldPosition;
            Player.SetBearing(0);
            Player.mPhyObj.Rotation = Matrix.Identity;
            Player.mAnimatedObj.Animator.Speed = 1.0f;
            Player.ChangeEquipment(GameEngine.Services.GetService<IEContentManager>());
            Player = null;
            Camera = null;    
            Visible = false;
        }
        public override void DisableComponent()
        {
            Player = null;
            for (int i = 0; i < Cats.Count; i++)
            {
                Cats[i].Shader.Dispose();
                Cats[i].Disable();
            }
            Cats.Clear();
            base.DisableComponent();
        }
        public override void Update()
        {
            if (!Visible) return;
            Elapsed += GameEngine.GameTime.ElapsedGameTime;

            Cats[0].Update();
            Cats[1].Update();
            Cats[2].Update();
            Cats[3].Update();
            if(Cats[0].Animator.HasFinished)
                Cats[0].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;
            if (Cats[1].Animator.HasFinished)
                Cats[1].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;
            if (Cats[2].Animator.HasFinished)
                Cats[2].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;
            if (Cats[3].Animator.HasFinished)
                Cats[3].Animator.Speed = GameEngine.RandomValue.Next(5, 10) * 0.1f;

            if (!Flip)
            {
                if (PlayerJumpValue < 2.0f)
                    PlayerJumpValue += 7.0f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                else
                    Flip = !Flip;
            }
            else
            {
                if (PlayerJumpValue > 0.0f)
                    PlayerJumpValue -= 7.0f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
                else
                    Flip = !Flip;
            }
            if (Spinv > (float)Math.PI * 2.0f)
                Spinv = 0.0f;
            else
                Spinv += 2.0f * (float)GameEngine.GameTime.ElapsedGameTime.TotalSeconds;
            Player.SetBearing(Spinv);
            Player.mAnimatedObj.Rotation = Player.mPhyObj.Rotation = Matrix.CreateRotationY(Spinv);
            Player.mPhyObj.Position = Playerpos + new Vector3(0, PlayerJumpValue, 0);
            Camera.View = Playerpos - Player.mAnimatedObj.CenterOffset * 1.5f;

            if (Elapsed.Seconds > 10)
            {
                GameEngine.Services.GetService<SoundManager>().StopSong();
                GameEngine.Services.GetService<TransitionScreen>().StartFading(5,2);
            }
        }
        public override void Draw()
        {
            if (PlayerState.Currently != PlayerState.State.ACT) return;
            Cats[0].Draw("Normal");
            Cats[1].Draw("Normal");
            Cats[2].Draw("Normal");
            Cats[3].Draw("Normal");
        }
    }
}
