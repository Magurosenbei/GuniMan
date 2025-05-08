using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;
using XNAnimation.Pipeline;

using Engine;


namespace Game
{
    public class Humaniod_Buffer
    {
        List<AnimatedObject> PlayerMDL = new List<AnimatedObject>();
        List<AnimatedObject> NPC_MaleMDL = new List<AnimatedObject>();
        List<AnimatedObject> NPC_FemaleMDL = new List<AnimatedObject>();
        List<AnimatedObject> NPC_CATS = new List<AnimatedObject>();
        public void LoadPlayerMDLBuffer(IEContentManager ContentManager)
        {
            PlayerMDL.Add(new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Humaniod/Player/GMAN"), Vector3.Zero));

            for (int i = 0; i < PlayerMDL.Count; i++)
            {
                PlayerMDL[i].LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                PlayerMDL[i].CopyBodeTransform();
                PlayerMDL[i].Scale = Vector3.One * 0.4f;

                Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(PlayerMDL[i].GetAnimatedModel().Model, true) * 0.4f;
                PlayerMDL[i].CenterOffset = new Vector3(0, -HalfExtents.Y, 0);
            }
        }
        public void LoadNPCMDLBuffer(IEContentManager ContentManager)
        {
            NPC_MaleMDL.Add(new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Humaniod/NPC/npc02ani"), Vector3.Zero));
            //NPC_MaleMDL.Add(new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Humaniod/NPC/npc01ani"), Vector3.Zero));

            for (int i = 0; i < NPC_MaleMDL.Count; i++)
            {
                NPC_MaleMDL[i].LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                NPC_MaleMDL[i].CopyBodeTransform();
                NPC_MaleMDL[i].Scale = Vector3.One * 0.4f;
                NPC_MaleMDL[i].Animator.StartClip(NPC_MaleMDL[i].GetAnimatedModel().AnimationClips["Idle"]);
                Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(NPC_MaleMDL[i].GetAnimatedModel().Model, true) * 0.4f;
                NPC_MaleMDL[i].CenterOffset = new Vector3(0, -HalfExtents.Y * 0.5f, 0);
            }

            NPC_FemaleMDL.Add(new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Humaniod/NPC/npc01ani"), Vector3.Zero));

            for (int i = 0; i < NPC_FemaleMDL.Count; i++)
            {
                NPC_FemaleMDL[i].LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                NPC_FemaleMDL[i].CopyBodeTransform();
                NPC_FemaleMDL[i].Scale = Vector3.One * 0.4f;
                NPC_FemaleMDL[i].Animator.StartClip(NPC_FemaleMDL[i].GetAnimatedModel().AnimationClips["Idle"]);
                Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(NPC_FemaleMDL[i].GetAnimatedModel().Model, true) * 0.4f;
                NPC_FemaleMDL[i].CenterOffset = new Vector3(0, -HalfExtents.Y * 0.5f, 0);
            }

            NPC_CATS.Add(new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Humaniod/Cats/catTyp01"), Vector3.Zero));
            NPC_CATS.Add(new AnimatedObject(ContentManager.Load<SkinnedModel>("Content/Models/Humaniod/Cats/catTyp02"), Vector3.Zero));
            for (int i = 0; i < NPC_CATS.Count; i++)
            {
                NPC_CATS[i].LoadShader("Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                NPC_CATS[i].CopyBodeTransform();
                NPC_CATS[i].Scale = Vector3.One * 0.4f;
                NPC_CATS[i].Animator.StartClip(NPC_CATS[i].GetAnimatedModel().AnimationClips["Idle"]);
                Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(NPC_CATS[i].GetAnimatedModel().Model, true) * 0.4f;
                NPC_CATS[i].CenterOffset = new Vector3(0, -HalfExtents.Y * 0.5f, 0);
            }
        }
        public void Disable()
        {
            try
            {
                for (int i = 0; i < PlayerMDL.Count; i++)
                    PlayerMDL[i].Shader.Dispose();
                for (int i = 0; i < NPC_MaleMDL.Count; i++)
                    NPC_MaleMDL[i].Shader.Dispose();
                for (int i = 0; i < NPC_FemaleMDL.Count; i++)
                    NPC_FemaleMDL[i].Shader.Dispose();
                for (int i = 0; i < NPC_CATS.Count; i++)
                    NPC_CATS[i].Shader.Dispose();
                PlayerMDL.Clear();
                NPC_MaleMDL.Clear();
                NPC_FemaleMDL.Clear();
                NPC_CATS.Clear();
            }
            catch (Exception e)
            {
                Debug.Write("\n Probably MDL's Shader is already disposed" + e.Message);
            }
        }
        public AnimatedObject GetPlayerMDL(int Eqs)
        {
            return PlayerMDL[Eqs];
        }
        public AnimatedObject GetCatNPC(int index)
        {
            if (index < 0)
                return NPC_CATS[GameEngine.RandomValue.Next(0, NPC_CATS.Count)];
            return NPC_CATS[index];
        }
        public AnimatedObject GetRandomMaleNPC()
        {
            return NPC_MaleMDL[GameEngine.RandomValue.Next(0, NPC_MaleMDL.Count)];
        }
        public AnimatedObject GetRandomFemaleNPC()
        {
            // Temp
            return NPC_FemaleMDL[GameEngine.RandomValue.Next(0, NPC_FemaleMDL.Count)];
        }
        public AnimatedObject GetRandomNPC()
        {
            switch (GameEngine.RandomValue.Next(0, 2))
            {
                case 0:
                    return NPC_MaleMDL[GameEngine.RandomValue.Next(0, NPC_MaleMDL.Count)];
                case 1:
                    return NPC_FemaleMDL[GameEngine.RandomValue.Next(0, NPC_FemaleMDL.Count)];
                default:
                    return NPC_MaleMDL[GameEngine.RandomValue.Next(0, NPC_MaleMDL.Count)];
            }
        }
    }
}