using System.Collections.Generic;
using UnityEngine;

namespace Zubble
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        public List<AudioClip> JumpSounds = new();
        public List<AudioClip> HurtSounds = new();
        public List<AudioClip> SoapSounds = new();

        public AudioSource MusicSource;
        
        public void ToggleMusic(bool on)
        {
            MusicSource.enabled = on;
            if (on)
            {
                MusicSource.Play();
            }
            else
            {
                MusicSource.Stop();
            }
        }
    }
}
