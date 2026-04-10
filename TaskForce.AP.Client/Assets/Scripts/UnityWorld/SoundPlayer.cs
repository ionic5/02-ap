using TaskForce.AP.Client.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace TaskForce.AP.Client.UnityWorld
{
    public class SoundPlayer : MonoBehaviour, ISoundPlayer
    {
        [SerializeField] AudioSource bgmAudioSource;
        [SerializeField] private AudioMixer audioMixer;

        private void Awake()
        {
            bgmAudioSource.loop = true;
        }
        
        public void Play()
        {
            bgmAudioSource.Play();
        }

        public void SetBGMVolume(float volume)
        {
            bgmAudioSource.volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            if (audioMixer == null)
                return;
            
            float v = Mathf.Clamp(volume, 0.0001f, 1f);
            audioMixer.SetFloat("SFX", Mathf.Log10(v) * 20);
        }
    }
}