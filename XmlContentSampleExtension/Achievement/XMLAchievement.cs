using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class AchievementTabDef
    {
        public string Picture;        // The texture that will be drawn to represent the icon
        public string Background;         // The texture that will be drawn to represent the background

        public string Title;              // The desciption of the achievement
        public string Description;       // The desciption of the achievement
       

        public string NormalText;
        public string BoldText;
        public int Counter;           // The counter of the achievement
       
        public AchievementTabDef() { }


        public class AchievementTabDefContentReader : ContentTypeReader<AchievementTabDef>
        {
            protected override AchievementTabDef Read(ContentReader input, AchievementTabDef TabStuff)
            {
                AchievementTabDef TabThingy = new AchievementTabDef();
                TabThingy.Picture = input.ReadString();
                TabThingy.Background = input.ReadString();
                TabThingy.Title = input.ReadString();
                TabThingy.Description = input.ReadString();
                TabThingy.NormalText = input.ReadString();
                TabThingy.BoldText = input.ReadString();
                TabThingy.Counter = input.ReadInt32();
             

                return TabThingy;
            }
        }

    }
}