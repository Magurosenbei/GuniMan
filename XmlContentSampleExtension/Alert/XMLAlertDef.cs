using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class AlertDef
    {
        public string AlertText;        // The Text

        public AlertDef() { }


        public class AlertDefContentReader : ContentTypeReader<AlertDef>
        {
            protected override AlertDef Read(ContentReader input, AlertDef AlertStuff)
            {
                AlertDef AlertThingy = new AlertDef();
                AlertThingy.AlertText = input.ReadString();
                return AlertThingy;
            }
        }

    }
}