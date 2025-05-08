using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Engine;
using XmlContentExtension;

namespace Game
{
    public class SDHHandle : Component
    {
        List <PhysicsStaticObject> Houses = new List<PhysicsStaticObject>();
        KeyboardDevice KB;
        GamepadDevice GPD;

        public SDHHandle(List<XmlContentExtension.Point> Area, IEContentManager ContentManager, GameScreen Parent)
            : base(Parent)
        {
            KB = GameEngine.Services.GetService<KeyboardDevice>();
            GPD = GameEngine.Services.GetService<GamepadDevice>();
            List <StaticObject> objlist = new List<StaticObject>();
            List<Model> ColBox = new List<Model>();
            ColBox.Capacity = objlist.Capacity = VariableAsset.SDHPaths.Count;
            for(int i = 0; i < VariableAsset.SDHPaths.Count; i++)
            {
                objlist.Add(new StaticObject(ContentManager.Load<Model>(VariableAsset.SDHPaths[i]), Vector3.Zero));
                objlist[i].LoadShader("Content/Shader Fx/CelFx", ContentManager);
                ColBox.Add(ContentManager.Load<Model>(VariableAsset.SDHPaths[i] + "_box"));
            }
            for (int i = 0; i < Area.Count; i++)
            {
                Houses.Add(new PhysicsStaticObject(Parent));
                int slelct = GameEngine.RandomValue.Next(0, objlist.Count - 1);
                Houses[i].mStaticObj = new StaticObject(objlist[slelct].GetModel(), Vector3.Zero);
                Houses[i].mStaticObj.Shader = objlist[slelct].Shader.Clone(GameEngine.GraphicDevice);
                Houses[i].mStaticObj.camera = objlist[slelct].camera;
                //Houses[i].SetBoundingPhysicsBasedOnModel(Area[i].Position, 1.0f, Area[i].PitchYawRoll, true);
                Houses[i].mStaticObj.Scale = Vector3.One;
                Vector3 HalfExtents = GraphicUtility.LargestHalfExtent(Houses[i].mStaticObj.GetModel(), true);
                Area[i].Position.Y += HalfExtents.Y;
                Houses[i].mPhyObj = new BoxShape(GraphicUtility.LargestHalfExtent(ColBox[slelct], true), Area[i].Position, Area[i].PitchYawRoll, base.Parent);
                Houses[i].mStaticObj.ViewRange = MathHelper.Max(HalfExtents.Z, MathHelper.Max(HalfExtents.X, HalfExtents.Y));
                Houses[i].mStaticObj.CenterOffset = new Vector3(0, -HalfExtents.Y, 0);
                Houses[i].mPhyObj.Immovable = true;
            }
            objlist.Clear();
        }
        public override void DisableComponent()
        {
            for (int i = 0; i < Houses.Count; i++)
            {
                Houses[i].mPhyObj.Body.DisableBody();
                Houses[i].DisableComponent();
            }
            base.DisableComponent();
        }
      
        public override void Update()
        {
            Guniman Player = GameEngine.Services.GetService<Guniman>();
            Interactive Interact = GameEngine.Services.GetService<Interactive>();
            for (int i = 0; i < Houses.Count; i++)
            {
                if (Houses[i].Visible && PlayerState.Currently == PlayerState.State.NORMAL)
                {
                    if (!Houses[i].mPhyObj.BoundingBox.Intersects(Player.mPhyObj.BoundingBox)) continue;
                    Vector2 RelDir = new Vector2(   Player.mPhyObj.Position.X - Houses[i].mPhyObj.Position.X,
                                                    Player.mPhyObj.Position.Z - Houses[i].mPhyObj.Position.Z);
                    if ((Math.Abs((float)Math.Atan2(-RelDir.Y, -RelDir.X) - (float)Math.Atan2(Player.GetDirection().Z, Player.GetDirection().X))) > 1.0f) 
                        continue;
                    Interact.Visible = true;
                    if (KB.Key_Pressed(Microsoft.Xna.Framework.Input.Keys.Z)
                        || GPD.Button_Pressed(Microsoft.Xna.Framework.Input.Buttons.A))
                    {
                        if (GameEngine.Services.GetService<PlayerStats>().GetMoney() > 99999)
                        {
                            GameEngine.Services.GetService<PlayerStats>().MoneyChange(-100000);
                            Vector3 Pos = Houses[i].mPhyObj.Position;
                            Pos.Y = 0;
                            GameEngine.Services.GetService<SoundManager>().ChangeSong("win");
                            GameEngine.Services.GetService<TransitionScreen>().SetScene(Pos, Houses[i].mPhyObj.Rotation);
                            GameEngine.Services.GetService<TransitionScreen>().StartFading(0, 5);
                            //GameEngine.Services.GetService<ObtainHouseCut>().Initialize_Scene(Pos, new Vector3(0, 0, 25), Houses[i].mPhyObj.Rotation);
                            break;
                        }
                        else
                            GameEngine.Services.GetService<PlayerEvent>().PlayerBoLui();
                            
                    }
                }
            }
        }
    }
}
