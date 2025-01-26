using System.Collections.Generic;
using UnityEngine;

namespace Zubble
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        public List<AudioClip> JumpSounds = new();
        public List<AudioClip> HurtSounds = new();
        public List<AudioClip> SoapSounds = new();
        public List<AudioClip> FallingSounds = new();
        public List<AudioClip> DeathSounds = new();

        public AudioSource MusicSource;

        public AudioClip RandomFallSound()
        {
            return FallingSounds[Random.Range(0, FallingSounds.Count)];
        }
        
        public AudioClip RandomDeathSound()
        {
            return DeathSounds[Random.Range(0, DeathSounds.Count)];
        }
        
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
