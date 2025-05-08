using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Engine;

namespace Game
{
    public class AudioComponent : Component
    {
        AudioEngine audioEngine;

        public WaveBank    waveBank;
        public SoundBank   soundBank;
        
        public AudioCategory MusicCategory;
        public AudioCategory SFXCategory;

        //Cue engineSound = null;

        public AudioComponent() : base() { }
        public AudioComponent(GameScreen Parent) : base(Parent){ }

        protected override void InitializeComponent(GameScreen Parent)
        {
            audioEngine = new AudioEngine("Content/Audio/GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");

            MusicCategory = audioEngine.GetCategory("Music");
            SFXCategory = audioEngine.GetCategory("SFX");

            base.InitializeComponent(Parent);
        }
        public void MusicVolume(float volume)
        {
            if (volume <= 0)
            {
                volume = 0;
            }
            MusicCategory.SetVolume(volume);
        }
        public void SFXVolume(float volume)
        {
            if (volume <= 0)
            {
                volume = 0;
            }
            SFXCategory.SetVolume(volume);
        }
        public override void Update()
        {
            audioEngine.Update();
            base.Update();
        }

        public override void Draw()
        {
            return;
        }
    }
}
