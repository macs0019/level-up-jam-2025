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
    public float interactionDistance = 3.0f; // Distancia máxima para interactuar
    public LayerMask interactionLayerMask; // Capa para filtrar objetos interactuables

    protected bool canInteract = false;

    protected void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned. Please assign it in the Inspector.");
            return;
        }

        // 1) Comprobamos distancia
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > interactionDistance)
        {
            if (canInteract)
            {
                canInteract = false;
                HideInteractionPrompt();
            }
            return;
        }

        // 2) Raycast desde el jugador hacia adelante
        Vector3 rayOrigin = playerTransform.position + playerTransform.up * 0.5f; // Ajusta la altura del rayo para que salga desde la cabeza del jugador
        Vector3 rayDir = playerTransform.forward;

        RaycastHit hit;
        bool didHit = Physics.Raycast(rayOrigin, rayDir, out hit, interactionDistance, interactionLayerMask);

        // (Opcional) Dibuja el rayo en escena para debug
        Debug.DrawRay(rayOrigin, rayDir * interactionDistance, didHit ? Color.green : Color.red);

        // 3) Solo si el rayo choca con este mismo objeto
        if (didHit && hit.transform == this.transform)
        {
            if (!canInteract)
            {
                canInteract = true;
                ShowInteractionPrompt();
                Debug.Log("Player in range and looking at: " + interactionMessage);
            }

            // Input System: tecla E
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                OnInteract();
            }
        }
        else if (canInteract)
        {
            canInteract = false;
            HideInteractionPrompt();
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
