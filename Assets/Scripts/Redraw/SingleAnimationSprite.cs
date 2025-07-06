using UnityEngine;

namespace Redraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SingleAnimationSprite : SingleAnimation
    {
        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            base.Awake();
        }

        protected override void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
