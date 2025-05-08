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

using Engine;

namespace Game
{
    public class Equipment
    {
        public int      Skin_ParentID;  // Use controller's Transform
        public int      ABS_ParentID;  // Use Transformation absolute
        public Matrix   ABS_Parent;
        public string   ParentName;
        public bool     UseBone = false;

        public ObjectType Item;

        public Equipment(ObjectType EqItem)
        {
            Item = EqItem;
        }
        public void UpdateTransform(Matrix SkinTransform, Matrix ParentWorld)
        {
            Item.Update();
            Vector3 Position, Scale;
            Quaternion Quater;
            ParentWorld.Decompose(out Scale, out Quater, out Position);
            if (!UseBone)
            {
                Item.World = MathsUtility.CreateWorldMatrix(Item.Position + Item.CalculatedOffset, Item.Rotation, Item.Scale) *
                                Matrix.CreateFromQuaternion(Quater) * Matrix.CreateTranslation(Position); // ParentWorld
            }
            else
            {
                // Create a matrix that contains the world bone of the parent
                Matrix BoneWorld = ABS_Parent * SkinTransform;
                // Scale only the translation of the world, to prevent stack scaling eg. 0.4f* 0.4f
                BoneWorld.Translation *= Scale;
                // Get the real world point of bone reference mainbody's position;
                Item.World = MathsUtility.CreateWorldMatrix(Item.Position + Item.CalculatedOffset, Item.Rotation, Item.Scale) *
                        BoneWorld * Matrix.CreateFromQuaternion(Quater) * Matrix.CreateTranslation(Position); //Bone World
            }
        }
  
        public void UpdateTransform(Matrix SkinTransform, Vector3 Position, Matrix Rotation, Vector3 Scale)
        {
            Item.Update();
            if (!UseBone)
            {
                Item.World = MathsUtility.CreateWorldMatrix(Item.Position + Item.CalculatedOffset, Item.Rotation, Item.Scale) *
                            Rotation * Matrix.CreateTranslation(Position); // ParentWorld
            }
            else
            {
                // Create a matrix that contains the world bone of the parent
                Matrix BoneWorld = ABS_Parent * SkinTransform;
                // Scale only the translation of the world, to prevent stack scaling eg. 0.4f* 0.4f
                BoneWorld.Translation *= Scale;
                // Get the real world point of bone reference mainbody's position;
                Item.World = MathsUtility.CreateWorldMatrix(Item.Position + Item.CalculatedOffset, Item.Rotation, Item.Scale) *
                        BoneWorld * Rotation * Matrix.CreateTranslation(Position);
            }
        }
        public void Draw()
        {
            Item.Draw("Normal");
        }
    }
}
