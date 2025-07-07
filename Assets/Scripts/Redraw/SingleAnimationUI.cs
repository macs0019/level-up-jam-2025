using UnityEngine;
using UnityEngine.UI;

namespace Redraw
{
    [RequireComponent(typeof(Image))]
    public class SingleAnimationUI : SingleAnimation
    {
        private Image image;

        protected override void Awake()
        {
            image = GetComponent<Image>();
            base.Awake();
        }

        protected override void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}