using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;


namespace Game
{
    // Structure of Save Data, just add on and use
    [Serializable]
    public struct SaveData
    {
        public int Equipment;
        public int Money;
        public bool Achievement1;
        public bool Achievement2;
        public bool Achievement3;
        public bool Achievement4;
        public bool Achievement5;
        public bool Achievement6;
        public bool Achievement7;
        public bool Achievement8;
        public bool Achievement9;
    }
    /* Usage */
    /*      
        SaveFile = new Game.SaveGameStore();
        SaveFile.GotSaveFile(); // See if there is a file
        SaveFile.Load();    // Load file, auto create a save file if don't have
        SaveFile.Save();    // Save File
    */
    public class SaveGameStore
    {
        public SaveData Data;                   // All data contained here so just access
        public string FilePath = "Save.xml";    // save to where and what name
        public string FilePathTemp = "SaveTemp.xml";    // save to where and what name
        
        public void Save()
        {
            FileStream stream = File.Open(FilePath, FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.Serialize(stream, Data);
            stream.Close();
        }
        public void SaveTemp()
        {
            FileStream stream = File.Open(FilePathTemp, FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.Serialize(stream, Data);
            stream.Close();
        }
        public void Reset()
        {
            Data = new SaveData();
        }
        public bool GotSaveFile()
        {
            return !File.Exists(FilePath);
        }
        public void Load()
        {
            if (!File.Exists(FilePath))
                Save();
            FileStream stream = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            Data = (SaveData)serializer.Deserialize(stream);
            stream.Close();
        }
        public void LoadNew()
        {
            if (!File.Exists(FilePathTemp))
                SaveTemp();
            FileStream stream = File.Open(FilePathTemp, FileMode.OpenOrCreate, FileAccess.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            Data = (SaveData)serializer.Deserialize(stream);
            stream.Close();
        }
    }
}

