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
    public class NewHouse
    {
        StaticObject Door;
        Vector3 DoorStep;
        float mBearing;
        float mDoorIncrea = 0.0f;
        float mAddonAngle = 1.571f;
        float mIncrement = 0.05f;
        public bool OpenDoor = false;
        protected bool InverseFormula = false;

        public NewHouse(string ShaderPath, Vector3 Position, Vector3 DoorPivotOffSet, Vector3 TurnInfo, IEContentManager ContentManager)
        {
            Door = new StaticObject(ContentManager.Load<Model>(VariableAsset.Doors[GameEngine.RandomValue.Next(0, VariableAsset.Doors.Count)]), DoorPivotOffSet);
            Door.LoadShader(ShaderPath, ContentManager);
            Door.Color.X = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
            Door.Color.Y = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
            Door.Color.Z = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;

            Door.Position = Position + DoorPivotOffSet;
            DoorStep = Position;
            Door.DisplayShadow = true;
            mAddonAngle = TurnInfo.Y;
            mIncrement = TurnInfo.X;
            InverseFormula = (TurnInfo.Z > 0)? true : false;
        }
        public NewHouse(StaticObject mdlDoor, Vector3 Position, Vector3 DoorPivotOffSet, Vector3 TurnInfo)
        {
            Door = new StaticObject(mdlDoor.GetModel(), DoorPivotOffSet);
            Door.Color.X = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
            Door.Color.Y = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
            Door.Color.Z = (float)GameEngine.RandomValue.Next(50, 100) * 0.01f;
            Door.Shader = mdlDoor.Shader.Clone(GameEngine.GraphicDevice);
            Door.camera = mdlDoor.camera;
            Door.Position = Position + DoorPivotOffSet;
            DoorStep = Position;
            Door.DisplayShadow = true;
            mAddonAngle = TurnInfo.Y;
            mIncrement = TurnInfo.X;
            InverseFormula = (TurnInfo.Z > 0) ? true : false;
        }
        public void Set3DStruct(Vector3 Position, Vector3 PitchYawRoll)
        {
            Matrix Rotation = Matrix.CreateFromYawPitchRoll(PitchYawRoll.Y, PitchYawRoll.X, PitchYawRoll.Z);
            mBearing = PitchYawRoll.Y;
            mDoorIncrea = mBearing + 1.571f;
            Door.Position = (Matrix.CreateTranslation(Door.Position) * Rotation).Translation + Position;
            Door.Rotation = Rotation;
            DoorStep = (Matrix.CreateTranslation(DoorStep) * Rotation).Translation + Position;
        }
        public void Disable()
        {
            Door.Shader.Dispose();
        }
        public Vector3 GetDoorStep() { return DoorStep; }
        public void Update()
        {
            if (OpenDoor)
            {
                if (!InverseFormula && mDoorIncrea < mBearing + mAddonAngle)
                {
                    mDoorIncrea += mIncrement;
                    Door.Rotation = Matrix.CreateFromYawPitchRoll(mDoorIncrea, 0, 0);
                }
                else if(InverseFormula && mDoorIncrea > mBearing + mAddonAngle)
                {
                    mDoorIncrea += mIncrement;
                    Door.Rotation = Matrix.CreateFromYawPitchRoll(mDoorIncrea, 0, 0);
                }
            }
            else
            {
                if (!InverseFormula && mDoorIncrea > mBearing)
                {
                    mDoorIncrea -= mIncrement;
                    Door.Rotation = Matrix.CreateFromYawPitchRoll(mDoorIncrea, 0, 0);
                }
                else if (InverseFormula && mDoorIncrea < mBearing)
                {
                    mDoorIncrea -= mIncrement;
                    Door.Rotation = Matrix.CreateFromYawPitchRoll(mDoorIncrea, 0, 0);
                }
            }
        }

        public void Draw()
        {
            Door.Draw("Normal");
        }
    }
}
