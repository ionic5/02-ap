using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class SoundPlayer : MonoBehaviour, ISoundPlayer
    {
        [SerializeField] AudioSource bgmAudioSource;
        [SerializeField] AudioSource[] sfxAudioSources;

        private void Awake()
        {
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }

        public void SetBGMVolume(float volume)
        {
            bgmAudioSource.volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            foreach (var sfx in sfxAudioSources)
            {
                sfx.volume = volume;
            }
        }
    }
}