using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Engine;

namespace Game
{
    // Apply to all components that has bodies, to prevent access violation etc in physics for parallel update
  
    // Inherit this for animated and static object
    public class ObjectType : I3DComponent
    {
        public float DistanceFromCamera = 0.0f;
        public bool bLastDraw = false;
        public bool Remove = false;
        public bool UpdateWorldFromDraw = true;
        public Vector3 CenterOffset = Vector3.Zero;
        public Vector3 CalculatedOffset = Vector3.Zero;
        public Matrix World = Matrix.Identity;
        #region I3DComponent stuff
        public virtual Vector3 Position { get; set; }
        public Vector3 EulerRotation
        {
            get { return MathsUtility.MatrixToVector3(Rotation); }
            set { Rotation = MathsUtility.Vector3ToMatrix(value); }
        }
        public virtual Matrix Rotation { get; set; }
        public virtual Vector3 Scale { get; set; }
        public virtual BoundingBox BoundingBox
        {
            get { return new BoundingBox(Position - (Scale / 2), Position + (Scale / 2)); }
        }
        #endregion
        public ObjectType() { }
        virtual public void Disable() { }
        virtual public void Update(){}
        virtual public void Draw(string Technique){}
    }
}