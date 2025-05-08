using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Game
{
    public class AudioButton
    {
        private bool Done;                  // Button is pressed or not
        private bool AlmostDone;             // activate effect to be finished
        private float Timing;               // Timing of the Button
        private string Button;              // Name of the button to be pressed
        private string Sound;               // Name of the sound to be played
        private string Grade;               // Grade
        private float Alpha;                // Alpha factor
        private float Size;                 // Size factor
        private Vector2 Position;           // Position 
        private float Dropvalue;            // Drop value    
        private bool Drop;                  // drop?

        public AudioButton(string button, string sound, float timing, float speed, Vector2 Bar)
        {
            Button = button;
            Timing = timing;
            Sound = sound;
            Done = false;
            AlmostDone = false;
            Drop = false;
            Position = new Vector2(Bar.X - Timing * 60.0f * speed, Bar.Y);
            Grade = "-";
            Alpha = 1.0f;
            Size = 1.0f;
            Dropvalue = 1.0f;
        }
        
        public float GetAlpha()
        {
            return Alpha;
        }
        public float GetSize()
        {
            return Size;
        }
        public string GetButton()
        {
            return Button;
        }
        public float GetTiming()
        {
            return Timing;
        }
        public bool GetAlmostDone()
        {
            return AlmostDone;
        }
        public string GetSound()
        {
            return Sound;
        }
        public Vector2 GetPosition()
        {
            return Position;
        }
        public bool GetDone()
        {
            return Done;
        }
        public string GetGrade()
        {
            return Grade;
        }
        public void SetDrop(bool temp)
        {
            Drop = temp;
        }
        public void SetButton(string button)
        {
            Button = button;
        }
        public void SetGrade(string grade)
        {
            Grade = grade;
        }
        public void Finishing()
        {
            AlmostDone = true;
        }
        public void Finish()
        {
            Done = true;
        }
        public void Reset(float speed, Vector2 Bar)
        {
            Grade = "-";
            Done = false;
            Position = new Vector2(Bar.X - Timing * 60.0f * speed, Bar.Y);
            Alpha = 1.0f;
            Size = 1.0f;
            Dropvalue = 1.0f;
            Drop = false;
            AlmostDone = false;
        }
        public void Update(float x, float y)
        {
            Position = new Vector2(Position.X + x, Position.Y + y);
            if (AlmostDone)
            {
                Alpha -= 0.05f;
                Size += 0.2f;
                if (Alpha <= 0.0f)
                {
                    AlmostDone = false;
                    Finish();
                }
            }
            if (Drop)
            {
                if (Size > 0.4f)
                {
                    Size -= 0.025f;
                }
                Dropvalue += Dropvalue * 0.05f;
                Position = new Vector2(Position.X, Position.Y + Dropvalue);
            }
        }

    }
}
