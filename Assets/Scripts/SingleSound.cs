using UnityEngine;

namespace Aviss
{
    [System.Serializable]
    public class SingleSound : Sound
    {
        [SerializeField] private AudioClip clip;

        public override void Initialize(AudioSource _source)
        {
            source = _source;

            source.clip = clip;
            source.volume = Volume;
            source.pitch = Pitch;
            source.loop = Loop;
        }

        public override void Play()
        {
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
            if (!source.isPlaying)
                source.Play();
            else
                source.UnPause();

        }
    }
}

