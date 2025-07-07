using UnityEngine;

public class BossController : InteractableBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public override void OnInteract()
    {
        if (GameManager.Instance.GetLastInteractedFoodSelector() == null)
        {
            return; // No hacer nada si no hay un FoodSelector interactuado
        }

        Debug.Log("Interacción con el jefe iniciada.");
        GameManager.Instance.InteractWithBossController();
        GameManager.Instance.HandleError(); // Llamar a la función que comprueba si hay que restar vidas
        GameManager.Instance.ClearEnteredWords(); // Limpiar las palabras ingresadas

        // Lógica adicional para la interacción con el jefe
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
