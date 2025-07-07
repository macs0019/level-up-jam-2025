using UnityEngine;
using DG.Tweening;

public class BossController : InteractableBase
{
    public Transform bossTransform;

    public override void OnInteract()
    {
        if (GameManager.Instance.GetLastInteractedFoodSelector() == null)
        {
            return; // No hacer nada si no hay un FoodSelector interactuado
        }

        Debug.Log("Interacción con el jefe iniciada.");
        GameManager.Instance.InteractWithBossController();
        bool error = GameManager.Instance.HandleError(); // Llamar a la función que comprueba si hay que restar vidas
        GameManager.Instance.ClearEnteredWords(); // Limpiar las palabras ingresadas

        // Lógica adicional para la interacción con el jefe
        if (error)
        {
            bossTransform.DOShakePosition(
                duration: 0.8f,
                strength: new Vector3(1.5f, 1.5f, 0f),
                vibrato: 15,
                randomness: 90f,
                snapping: false,
                fadeOut: true
            );
        }
    }

    protected override void ShowInteractionPrompt()
    {
        if (GameManager.Instance.GetLastInteractedFoodSelector() == null)
        {
            return; // No mostrar el mensaje si no hay un FoodSelector interactuado
        }

        base.ShowInteractionPrompt(); // Mostrar el mensaje si se cumple la condición
    }
}
