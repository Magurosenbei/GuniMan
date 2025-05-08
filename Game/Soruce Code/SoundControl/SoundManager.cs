using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace Game
{
    public class SoundManager : Component
    {
        //public Rectangle Rectangle { get; set; }
        /////////////////
        /// Sound
        ///////////////
        
        private Cue MusicThingy = null;

        public SoundManager() : base() 
        {
            MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("NormalSong");
        }
        
        protected override void InitializeComponent(GameScreen Parent)
        {
            Visible = false;
            base.InitializeComponent(Parent);
        }
        public void Reset()
        {           
            MusicThingy.Stop(AudioStopOptions.Immediate);
            MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("NormalSong");
        }
        private void PlayBGM()
        {
            if (MusicThingy == null)
            {
                return;
            }
            else if (!MusicThingy.IsPlaying)
            {
                MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("NormalSong");
                MusicThingy.Play();
            }

        }
        public void StopSong()
        {
            if (MusicThingy != null)
            {
                MusicThingy.Stop(AudioStopOptions.Immediate);
                MusicThingy.Dispose();
                MusicThingy = null;
            }
        }
        public void ChangeSong(string name)
        {
            if (MusicThingy != null)
            {
                MusicThingy.Stop(AudioStopOptions.Immediate);
                MusicThingy.Dispose();
                MusicThingy = null;
            }
            if (MusicThingy == null)
            {
                if (name == "normal")
                {
                    MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("NormalSong");
                    MusicThingy.Play();
                }
                if (name == "win")
                {
                    MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("WinSong");
                    MusicThingy.Play();
                }
                if (name == "sell")
                {
                    MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("SellSong");
                    MusicThingy.Play();
                }
                if (name == "save")
                {
                    MusicThingy = GameEngine.Services.GetService<AudioComponent>().soundBank.GetCue("SaveSong");
                    MusicThingy.Play();
                }
            }
        }
        public override void Update()
        {
            if (!Visible)
            {
                return;
            }

            PlayBGM();
            
            base.Update();
        }
       
        public override void Draw()
        {
 
        }
        protected void DrawButtonOnChoice(string ButtonName)
        {
            //GameEngine.SpriteBatch.Draw(AssetImages[ButtonName], AssetPosition[ButtonName], Color.White);
        }
    }
}
