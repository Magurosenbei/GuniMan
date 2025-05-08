using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class CommandDef
    {
        public string Command;        // The Command
        public int ID;                // ID

        public CommandDef() { }


        public class CommandDefContentReader : ContentTypeReader<CommandDef>
        {
            protected override CommandDef Read(ContentReader input, CommandDef CommandStuff)
            {
                CommandDef CommandThingy = new CommandDef();
                CommandThingy.Command = input.ReadString();
                CommandThingy.ID = input.ReadInt32();
                return CommandThingy;
            }
        }

    }
}