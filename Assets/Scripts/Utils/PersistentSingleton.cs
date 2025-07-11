using UnityEngine;

namespace Aviss
{
    public class PersistentSingleton<T> : MonoBehaviour where T : PersistentSingleton<T>
    {
        private static T instance;

        [Header("Singleton attributes")]
        [SerializeField] private bool isPersistent;
        public static T Instance { get { return instance; } }

        protected virtual void Awake()
        {
            if (isPersistent)
            {
                if (!instance)
                {
                    instance = this as T;
                }
                else
                {
                    Destroy(gameObject);
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                instance = this as T;
            }
        }
    }
}
