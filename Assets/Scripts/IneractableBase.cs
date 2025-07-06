using UnityEngine;
using TMPro;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string interactionMessage = "Press E to interact";
    public KeyCode interactionKey = KeyCode.E;
    public TextMeshProUGUI interactionUIText; // TextMeshProUGUI element to display the interaction message

    protected bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowInteractionPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractionPrompt();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            OnInteract();
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
