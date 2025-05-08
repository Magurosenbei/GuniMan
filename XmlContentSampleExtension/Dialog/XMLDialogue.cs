using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class DialogueDef
    {
        public string Text;

        public DialogueDef() { }

        public class DialogueDefContentReader : ContentTypeReader<DialogueDef>
        {
            protected override DialogueDef Read(ContentReader input, DialogueDef DialogueStuff)
            {
                DialogueDef DialogueThingy = new DialogueDef();
                DialogueThingy.Text = input.ReadString();
                return DialogueThingy;
            }
        }
    }

}