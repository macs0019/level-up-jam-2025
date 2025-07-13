using UnityEngine;
using DG.Tweening;
using static UnityEngine.UI.Image;
using Aviss;

public class BossController : InteractableBase
{
    public Transform bossTransform;

    public override void OnInteract()
    {
        if (GameManager.Instance.LastInteractedFoodSelector == null)
        {
            return; // No mostrar el mensaje si no hay un FoodSelector interactuado
        }

        // TUTORIALES DE MIERDA
        if (TutorialController.Instance.gameObject.activeSelf)
        {
            Debug.Log("BossController Continue");
            TutorialController.Instance.NextText();

            DOVirtual.DelayedCall(2f, () =>
            {
                TutorialController.Instance.NextText();
            });
        }

        GameManager.Instance.InteractWithBossController();
        bool error = GameManager.Instance.HandleError(); // Llamar a la función que comprueba si hay que restar vidas
        GameManager.Instance.ClearEnteredWords(); // Limpiar las palabras ingresadas

        // Lógica adicional para la interacción con el jefe
        if (error)
        {
            AudioController.Instance.Play("Boss angry");
            bossTransform.DOShakePosition(
                duration: 0.8f,
                strength: new Vector3(1.5f, 1.5f, 0f),
                vibrato: 15,
                randomness: 90f,
                snapping: false,
                fadeOut: true
            );
        }
        else
        {
            AudioController.Instance.Play("Boss happy");
            bossTransform.DOScale(1.3f, 0.3f)
              .SetLoops(4, LoopType.Yoyo)
              .SetEase(Ease.InOutSine);
        }
    }

    public override void ShowInteractionPrompt()
    {
        if (GameManager.Instance.LastInteractedFoodSelector == null)
        {
            return; // No mostrar el mensaje si no hay un FoodSelector interactuado
        }

        base.ShowInteractionPrompt(); // Mostrar el mensaje si se cumple la condición
    }
}
