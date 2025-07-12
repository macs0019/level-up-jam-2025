using UnityEngine;

namespace Aviss
{
    [System.Serializable]
    public abstract class Sound
    {
        [SerializeField] private string name;
        [SerializeField] private bool loop;
        [SerializeField][Range(0.0f, 1.0f)] private float volume = 0.5f;
        [SerializeField][Range(0.1f, 3f)] private float pitch = 1f;

        protected AudioSource source;

        public string Name { get => name; set => name = value; }
        protected float Volume { get => volume; set => volume = value; }
        protected bool Loop { get => loop; set => loop = value; }
        protected float Pitch { get => pitch; set => pitch = value; }
        public AudioSource Source { get => source; set => source = value; }

        public abstract void Initialize(AudioSource _source);
        public abstract void Play();
        public abstract void Stop();

        public abstract void Pause();
        public abstract void UnPause();
    }
}
