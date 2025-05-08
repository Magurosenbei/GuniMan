using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine;

namespace Game
{
    public class House
    {
        StaticObject Door;
        Vector3 DoorStep;
        float   mBearing;
        float   mDoorIncrea = 0.0f;
        public bool  OpenDoor = false;
        
        public House(string Model, string Shader, Vector3 BuildingPos, Vector3 Position, Vector3 DoorPivotOffset, Matrix Rotation, float Bearing)
        {
            mBearing = Bearing;
            mDoorIncrea = Bearing - 1.571f;
            Door = new StaticObject(GameEngine.Content.Load<Model>(Model), Position + DoorPivotOffset);
            Door.LoadShader(Shader);
            Door.Position = (Matrix.CreateTranslation(Door.Position) * Rotation).Translation + BuildingPos;
            Door.Rotation = Rotation;
            DoorStep = (Matrix.CreateTranslation(Position) * Rotation).Translation + BuildingPos;
        }
        public Vector3 GetDoorStep() { return DoorStep; }
        public void Update()
        {
            if (OpenDoor)
            {
                if (mDoorIncrea > mBearing - 1.571f)
                {
                    mDoorIncrea -= 0.05f;
                    Door.Rotation = Matrix.CreateFromYawPitchRoll(mDoorIncrea, 0, 0);
                }  
            }
            else
            {
                if (mDoorIncrea < mBearing)
                {
                    mDoorIncrea += 0.05f;
                    Door.Rotation = Matrix.CreateFromYawPitchRoll(mDoorIncrea, 0, 0);
                }
            }     
            Door.Update();
        }

        public void Draw()
        {
            Door.Draw("Normal");
        }
    }
}
