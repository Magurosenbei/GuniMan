using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*
            Unset = 0,
            UserDefined = 1,
            NotBouncySmooth = 2,
            NotBouncyNormal = 3,
            NotBouncyRough = 4,
            NormalSmooth = 5,
            NormalNormal = 6,
            NormalRough = 7,
            BouncySmooth = 8,
            BouncyNormal = 9,
            BouncyRough = 10,
            NumMaterialTypes = 11,*/

namespace Game
{
    public enum ObjectFilter { LIFT = 0, BUILDINGS = 1, CHARACTER = 3, PLAYER = 7, STATICOBJ = 4, INTERACTIVE = 2, SAVEPOINT = 5, OUT = 8 , DECOS = 6}
    public static class VariableAsset
    {
        public static List  <string> Doors = new List<string>();
        public static List  <string> BuildingNames = new List<string>();
        public static List  <string> SDHPaths = new List<string>();
        public static List  <string> Trees = new List<string>();
        public static List  <string> SavePoint = new List<string>();
        public static List  <string> LampFile = new List<string>();
        public static List  <string> DustBin = new List<string>();

        public static List  <string> FlowerPots = new List<string>();

        public static List  <string> PlayerMDL = new List<string>();
        public static List  <string> Deco = new List<string>();
        public static List  <string> EmoIcon = new List<string>(); 

        public static List  <string> Pickables = new List<string>();
        
        public static void ReadAssets(string XMLFILE)
        {
            Doors.Add("Content/Door And Windows/Door");
            //Doors.Add("Content/Door And Windows/Door2");
            BuildingNames.Add("Content/Buildings/HDB/Scripts/HDB1/HDBDef");
            BuildingNames.Add("Content/Buildings/HDB/Scripts/HDB2/HDBDef");
            BuildingNames.Add("Content/Buildings/HDB/Scripts/HDB3/HDBDef");

            SDHPaths.Add("Content/Buildings/SDH/sdh");
            Pickables.Add("Content/Models/Pickable/pickables");

            Trees.Add("Content/Models/Enviroment/tree01");
            LampFile.Add("Content/LightPost/StreetLamp/StreetLampDef");
            DustBin.Add("Content/Dustbins/bin");

            PlayerMDL.Add("Content/Models/Humaniod/Test/hahatest");

            FlowerPots.Add("Content/Trash/FlowerPot");
            FlowerPots.Add("Content/Trash/FlowerPot2");

            Deco.Add("Content/Models/Enviroment/testfence");
            Deco.Add("Content/Models/Enviroment/barricade02");
            Deco.Add("Content/Models/Enviroment/lamppost02");
            Deco.Add("Content/LightPost/StreetLamp/StreetLamp");
            Deco.Add("Content/Models/Enviroment/firehydrant");
            Deco.Add("Content/Models/Enviroment/bench");
            Deco.Add("Content/Models/Enviroment/swing");
            Deco.Add("Content/Buildings/SDH/sdh");

            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_ehhh");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_exclaim");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_fullmoon");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_fuming");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_hmmpt");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_joyous");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_money");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_moody");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_skull");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_slouchy");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_sun");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_trashbin");
            EmoIcon.Add("Content/Textures/EmoIcon/hud_icons_zzz");
        }
    }
}
