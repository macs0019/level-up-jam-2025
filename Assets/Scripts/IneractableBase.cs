using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string interactionMessage = "Press E to interact";
    public KeyCode interactionKey = KeyCode.E;
    public TextMeshProUGUI interactionUIText; // TextMeshProUGUI element to display the interaction message

    public Transform playerTransform; // Transform del jugador asignado desde el Inspector
    public float interactionDistance = 5.0f; // Distancia máxima para interactuar

    protected bool playerInRange = false;

    private void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= interactionDistance)
            {
                if (!playerInRange)
                {
                    playerInRange = true;
                    ShowInteractionPrompt();
                    Debug.Log("Player in range: " + interactionMessage);
                }

                // Check for interaction key press using Input System
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) // Replace 'eKey' with the specific key
                {
                    OnInteract();
                }
            }
            else if (playerInRange)
            {
                playerInRange = false;
                HideInteractionPrompt();
            }
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned. Please assign it in the Inspector.");
        }
    }

    public abstract void OnInteract();

    protected virtual void ShowInteractionPrompt()
    {
        if (interactionUIText != null)
        {
            interactionUIText.text = interactionMessage;
            interactionUIText.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(interactionMessage);
        }
    }

    protected virtual void HideInteractionPrompt()
    {
        if (interactionUIText != null)
        {
            interactionUIText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Out of range");
        }
    }
}
