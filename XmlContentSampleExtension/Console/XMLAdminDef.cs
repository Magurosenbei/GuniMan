using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XmlContentExtension
{
    public class AdminDef
    {
        public string Name;                // The Username
        public string Pass;                // The Password

        public AdminDef() { }


        public class AdminDefContentReader : ContentTypeReader<AdminDef>
        {
            protected override AdminDef Read(ContentReader input, AdminDef AdminStuff)
            {
                AdminDef AdminThingy = new AdminDef();
                AdminThingy.Name = input.ReadString();
                AdminThingy.Pass = input.ReadString();
                return AdminThingy;
            }
        }

    }
}