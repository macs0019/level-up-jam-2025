using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Aviss
{
    public class AudioController : PersistentSingleton<AudioController>
    {
        [Header("Sound List")]
        [SerializeField] private List<SingleSound> singleSounds;
        [SerializeField] private List<MultipleSound> multipleSounds;

        private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();

        private new void Awake()
        {
            base.Awake();

            foreach (Sound sound in singleSounds)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                sound.Initialize(source);

                sounds.Add(sound.Name, sound);
            }

            foreach (Sound sound in multipleSounds)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                sound.Initialize(source);

                sounds.Add(sound.Name, sound);
            }
        }

        public void Play(string name, float pitch = 1.0f)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.Source.pitch = pitch;
                sound.Play();
            }
        }

        public void Stop(string name)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.Stop();
            }
        }

        
        public void Pause(string name)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.Pause();
            }
        }

        public void UnPause(string name)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.UnPause();
            }
        }

        public void FadeIn(string name, float duration)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                float volume = sound.Source.volume;
                sound.Source.volume = 0.0f;
                sound.Play();

                sound.Source.DOFade(volume, duration).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    sound.Source.volume = volume;
                });
            }
        }

        public void FadeOut(string name, float duration)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                sound.Source.DOFade(0.0f, duration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    sound.Source.volume = 0.0f;
                });
            }
        }

        public void FadeOutIn(string name, float outValue, float duration)
        {
            if (sounds.TryGetValue(name, out Sound sound))
            {
                float volume = sound.Source.volume;

                sound.Source.DOFade(outValue, duration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    sound.Source.DOFade(volume, duration).SetEase(Ease.InSine);
                });
            }
        }

        public void FadeOutAll(float duration)
        {
            foreach (Sound sound in sounds.Values)
            {
                if (sound.Source.isPlaying)
                {
                    float volume = sound.Source.volume;

                    sound.Source.DOFade(0.0f, duration).OnComplete(() =>
                    {
                        sound.Stop();
                        sound.Source.volume = volume;
                    });
                }
            }
        }
    }
}