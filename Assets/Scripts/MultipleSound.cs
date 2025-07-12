using UnityEngine;
using System.Collections.Generic;

namespace Aviss
{
    [System.Serializable]
    public class MultipleSound : Sound
    {
        [SerializeField] private List<AudioClip> clips = new List<AudioClip>();

        public override void Initialize(AudioSource _source)
        {
            source = _source;
            int index = Random.Range(0, clips.Count);

            source.clip = clips[index];
            source.volume = Volume;
            source.pitch = Pitch;
            source.loop = Loop;
        }

        public override void Play()
        {
            int index = Random.Range(0, clips.Count);

            source.clip = clips[index];
            source.Play();
        }

        public override void Stop()
        {
            source.Stop();
        }
        public override void Pause()
        {
            source.Pause();
        }

        public override void UnPause()
        {
            int index = Random.Range(0, clips.Count);

            source.clip = clips[index];
            source.Play();
        }
    }
}

