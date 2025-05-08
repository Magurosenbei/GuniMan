using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

using XmlContentExtension;
using Engine;

namespace Game
{

    public class PathSystem
    {
        List<WayPoint> Nodes;
        List<Character> PasserBy;
        Humaniod_Buffer ReferBuff;
        GameScreen Parent;
        public bool isAnimal;

        // Talk Npc all count
        List<bool> peopleTalked = new List<bool>();
        TimeSpan Elapsed = new TimeSpan();


        private int count = 0; // Reg count
        private int RecognitionCats = 0;

        protected AchievementEngine AE;
        protected Guniman Player;
        protected KeyboardDevice KB;
        protected GamepadDevice GPD;
        protected DialogueEngine DialogEng;

        public PathSystem(string XMLFile, IEContentManager ContentManager, GameScreen Parent, bool isAnimal_F)
        {
            this.Parent = Parent;
            ReferBuff = GameEngine.Services.GetService<Humaniod_Buffer>();
            Nodes = ContentManager.Load<List<WayPoint>>(XMLFile);
            AE = GameEngine.Services.GetService<AchievementEngine>();
            isAnimal = isAnimal_F;
        }
        /******************************************/
        public int GetRecognitionCount() { return RecognitionCats; }
        /******************************************/
        public void Reset(IEContentManager ContentManager)
        {
            for (int i = 0; i < PasserBy.Count; i++)
                PasserBy[i].DisableComponent();
            PasserBy.Clear();
            Nodes.Clear();
            FillPassers(ContentManager);
        }
        public void FillPassers(IEContentManager ContentManager)
        {
            PasserBy = new List<Character>();
            int Number = GameEngine.RandomValue.Next(4, (Nodes.Count * 75 / 100));
            PasserBy.Capacity = Number;
            for (int i = 0; i < Number; i++)
            //Parallel.For(0, Number, delegate(int i)
            {
                Character AI = new Character(Parent, isAnimal);
                int index = GameEngine.RandomValue.Next(0, Nodes.Count);
                AI.AssignGoal(Nodes[index], this);
                int male = GameEngine.RandomValue.Next(0, 2);
                //string Model;
                AnimatedObject obj = null;
                if (male > 0)
                {
                    AI.Male = true;
                    if (!AI.isAnimal)
                        obj = ReferBuff.GetRandomMaleNPC();
                    else
                        obj = ReferBuff.GetCatNPC(-1);
                }
                else
                {
                    AI.Male = false;
                    if (!AI.isAnimal)
                        obj = ReferBuff.GetRandomFemaleNPC();
                    else
                        obj = ReferBuff.GetCatNPC(-1);
                }
                //AI.Setup(Model, "Content/Shader Fx/CelSkinnedModelEffect", ContentManager);
                AI.mAnimatedObj = new AnimatedObject(obj.GetAnimatedModel(), Vector3.Zero);
                AI.mAnimatedObj.Shader = obj.Shader.Clone(GameEngine.GraphicDevice);
                AI.mAnimatedObj.Transformation = obj.Transformation;
                AI.mAnimatedObj.Camera = obj.Camera;
                AI.mAnimatedObj.CenterOffset = obj.CenterOffset;
                AI.mAnimatedObj.Scale = obj.Scale;

                if (!AI.isAnimal)
                    AI.mAnimatedObj.Animator.StartClip(AI.mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);
                else
                    AI.mAnimatedObj.Animator.StartClip(AI.mAnimatedObj.GetAnimatedModel().AnimationClips["Walk"]);

                AI.mAnimatedObj.Animator.LoopEnabled = true;
                GameEngine.RandomValue.Next(0, Nodes.Count);

                Vector3 Offset = new Vector3(GameEngine.RandomValue.Next(-10, 10), 0, GameEngine.RandomValue.Next(-10, 10));
                if (AI.isAnimal)
                {
                    AI.mAnimatedObj.Scale = Vector3.One * 1.5f;
                    AI.SetBoundingPhysicsBasedOnModel(Nodes[index].Position + Offset, 2.0f, Vector3.Zero, false, (int)ObjectFilter.CHARACTER, true);
                    AI.mPhyObj.Immovable = true;
                }
                else
                {
                    AI.SetBoundingPhysicsBasedOnModel(Nodes[index].Position + Offset, 0.4f, Vector3.Zero, false, (int)ObjectFilter.CHARACTER, true);
                    AI.mAnimatedObj.Scale = Vector3.One * 0.4f;
                    AI.mPhyObj.Mass = 100.0f;
                }
                AI.EnableAvoidObstalce();
                AI.mPhyObj.Immovable = false;
                PasserBy.Add(AI);
                peopleTalked.Add(false);
            };
        }
        public void Update()
        {
            if (PlayerState.Currently == PlayerState.State.SELL) return;
            if (!isAnimal)
            {
                // Check again if achievement gained
                if (!AE.IsUnlocked(7))
                {
                    Elapsed += GameEngine.GameTime.ElapsedGameTime;

                    bool Achieved = true;
                    for (int i = 0; i < peopleTalked.Count; i++)
                    {
                        if (!peopleTalked[i])
                        {
                            Achieved = false;
                            break;
                        }
                        if (Elapsed.Minutes > 60)
                            peopleTalked[i] = false;
                    }
                    if (Achieved)
                    {
                        //peopleTalked.Clear();
                        GameEngine.Services.GetService<AchievementOverlord>().TalkToEveryPasserBy();
                        Elapsed = new TimeSpan();
                    }
                }
            }
            if (Player == null)
                Player = GameEngine.Services.GetService<Guniman>();
            if (KB == null)
                KB = GameEngine.Services.GetService<KeyboardDevice>();
            if (GPD == null)
                GPD = GameEngine.Services.GetService<GamepadDevice>();
            if (DialogEng == null)
                DialogEng = GameEngine.Services.GetService<DialogueEngine>();
            Parallel.For(0, PasserBy.Count, delegate(int i)
            //for (int i = 0; i < PasserBy.Count; i++)
            {
                // Detect only when press to talk
                count = 0;
                if (PasserBy[i].getRecognition())
                    count++;
                if (count > RecognitionCats)
                    RecognitionCats = count;
                if (!DialogEng.Visible && PasserBy[i].mbTalk)
                {
                    PasserBy[i].mbTalk = false;
                    PlayerState.Currently = PlayerState.State.NORMAL;
                }
                if (PlayerState.Currently != PlayerState.State.TALK)
                {
                    if (PasserBy[i].mPhyObj.BoundingBox.Intersects(Player.mPhyObj.BoundingBox))
                    {
                        PasserBy[i].mbTalk = true;

                        if (!PasserBy[i].mReallyTalk) return;
                        GameEngine.Services.GetService<Interactive>().Visible = true;
                        if (KB.Key_Pressed(Microsoft.Xna.Framework.Input.Keys.Z) || GPD.Button_Pressed(Microsoft.Xna.Framework.Input.Buttons.A))
                        {
                            peopleTalked[i] = true;
                            if (isAnimal)
                            {
                                if (PasserBy[i].Male)
                                {
                                    DialogEng.StartConversation("Portrait_Guniman", "Portrait_WhiteCat", PasserBy[i].Male, -2);
                                }
                                else
                                {
                                    DialogEng.StartConversation("Portrait_Guniman", "Portrait_BlackCat", PasserBy[i].Male, -2);
                                }
                            }

                            else
                            {
                                if (PasserBy[i].Male)
                                {
                                    DialogEng.StartConversation("Portrait_Guniman", "Portrait_Guy", PasserBy[i].Male, -1);
                                }
                                else
                                {
                                    DialogEng.StartConversation("Portrait_Guniman", "Portrait_Girl", PasserBy[i].Male, -1);
                                }
                            }
                            PlayerState.Currently = PlayerState.State.TALK;
                        }
                    }
                    else
                        PasserBy[i].mbTalk = false;
                }
            });
        }
        public WayPoint GetPoint(int index)
        {
            return Nodes[index];
        }
        public void Clear()
        {
            for (int i = 0; i < PasserBy.Count; i++)
                PasserBy[i].DisableComponent();
            PasserBy.Clear();
            Nodes.Clear();
        }
    }
}
