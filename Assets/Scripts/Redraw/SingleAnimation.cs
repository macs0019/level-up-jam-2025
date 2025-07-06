using UnityEngine;
using System.Collections.Generic;

namespace Redraw
{
    public abstract class SingleAnimation : MonoBehaviour
    {
        [SerializeField] protected List<Sprite> sprites = new List<Sprite>();
        [SerializeField] protected float redrawInterval = 0.2f;

        protected float timer;
        protected int currentSpriteIndex;

        public List<Sprite> Sprites { get => sprites; set => sprites = value; }
        public float RedrawInterval { get => redrawInterval; set => redrawInterval = value; }

        protected virtual void Awake()
        {
            if (sprites.Count > 0)
            {
                SetSprite(sprites[0]);
            }
            else
            {
                Debug.LogWarning("There're no sprites to animate with!");
            }
        }

        protected void Update()
        {
            if (sprites.Count > 1)
            {
                timer += Time.deltaTime;
                if (timer >= redrawInterval)
                {
                    timer -= redrawInterval;
                    currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Count;
                    SetSprite(sprites[currentSpriteIndex]);
                }
            }
        }

        protected abstract void SetSprite(Sprite sprite);
    }
}