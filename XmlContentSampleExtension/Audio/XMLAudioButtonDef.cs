using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class AudioButtonDef
    {
        public string Button;        // The button to be pressed
        public string Sound;         // The sound file
        public float Timing;         // The Timing of note
       
        public AudioButtonDef() { }


        public class AudioButtonDefContentReader : ContentTypeReader<AudioButtonDef>
        {
            protected override AudioButtonDef Read(ContentReader input, AudioButtonDef ButtonStuff)
            {
                AudioButtonDef ButtonThingy = new AudioButtonDef();
                ButtonThingy.Button = input.ReadString();
                ButtonThingy.Sound = input.ReadString();
                ButtonThingy.Timing = input.ReadSingle();
             
                return ButtonThingy;
            }
        }

    }
}