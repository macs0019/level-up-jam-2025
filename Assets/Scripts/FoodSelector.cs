using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Asegúrate de tener la referencia a DOTween

public class FoodSelector : InteractableBase
{
    public List<Food> foods; // Lista de alimentos asignada desde el Inspector
    public SpriteRenderer targetRenderer; // SpriteRenderer donde se mostrará el sprite del alimento seleccionado
    public GameObject hiddenObject; // Objeto que se volverá visible
    public float activeTime = 5.0f; // Tiempo que el objeto estará activo
    public int sortingOffset = 0;

    public List<SpriteRenderer> renderers; // Lista de SpriteRenderers para ordenar
    private PlayerController playerController; // Referencia al PlayerController

    private bool isFoodActive = false; // Indica si la comida está activa

    public Sprite foodCallSprite; // Sprite que se muestra inicialmente
    private Sprite selectedFoodSprite; // Sprite de la comida seleccionada

    private int showFoodAndObjectCalls = 0; // Contador de llamadas a ShowFoodAndObject

    private Tween deactivateTween; // Referencia al Tween de desactivación
    private float remainingTime; // Tiempo restante para la desactivación


    private bool orderTaken = false; // Indica si el objeto ha sido grabado

    public bool OrderTaken
    {
        get => orderTaken;
        set
        {
            orderTaken = value;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Buscar al jugador por el tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();

            if (playerController == null)
            {
                Debug.LogError("PlayerController no encontrado en el objeto con tag 'Player'.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'Player'.");
        }
    }

    public bool IsFoodActive => isFoodActive; // Propiedad para acceder al estado de la comida activa

    public int ShowFoodAndObjectCalls => showFoodAndObjectCalls; // Propiedad para acceder al contador

    public void ShowFoodAndObject()
    {
        showFoodAndObjectCalls++; // Incrementar el contador cada vez que se llama

        if (foods != null && foods.Count > 0 && targetRenderer != null)
        {
            // Seleccionar un alimento aleatorio
            Food selectedFood = foods[Random.Range(0, foods.Count)];

            // Guardar el sprite de la comida seleccionada
            selectedFoodSprite = selectedFood.foodSprite;

            // Mostrar el sprite de "foodCall" inicialmente
            targetRenderer.sprite = foodCallSprite;

            // Hacer visible el objeto oculto
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
                isFoodActive = true; // Activar el indicador de comida activa
                DeactivateAfterTime(); // Iniciar la coroutine de desactivación
            }
            else
            {
                Debug.LogError("El objeto oculto no está asignado en el Inspector.");
            }
        }
        else
        {
            Debug.LogError("Asegúrate de asignar la lista de alimentos, el SpriteRenderer y el objeto oculto en el Inspector.");
        }
    }

    private void DeactivateAfterTime()
    {
        if (hiddenObject != null)
        {
            // Detener cualquier Tween existente antes de crear uno nuevo
            if (deactivateTween != null && deactivateTween.IsActive())
            {
                deactivateTween.Kill(false); // Detener el Tween sin completar la acción
            }

            remainingTime = activeTime; // Inicializar el tiempo restante
            deactivateTween = DOVirtual.DelayedCall(remainingTime, () =>
            {
                hiddenObject.SetActive(false);
                isFoodActive = false;

                if (!orderTaken)
                {
                    GameManager.Instance.HandleFoodSelectorLeavingWithDelay(this);
                }
            });
        }
    }

    public void PauseDeactivationTween()
    {
        if (deactivateTween != null && deactivateTween.IsPlaying())
        {
            remainingTime = deactivateTween.Elapsed(false); // Guardar el tiempo restante
            deactivateTween.Pause(); // Pausar el Tween sin eliminarlo
        }
    }

    public void ResumeDeactivationTween()
    {
        if (hiddenObject != null && isFoodActive && deactivateTween != null)
        {
            deactivateTween.Play();
        }
    }

    public override void OnInteract()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetLastInteractedFoodSelector() != null && GameManager.Instance.GetLastInteractedFoodSelector() != this)
        {
            Debug.Log("No puedes interactuar con este FoodSelector porque no es el último interactuado.");
            return;
        }

        if (isFoodActive) // Verificar si la comida está activa
        {
            if (playerController != null)
            {
                Debug.Log("Interactuando con " + gameObject.name);

                // Llamar al GameManager para agregar el comando
                if (GameManager.Instance != null && foods != null && targetRenderer != null)
                {
                    Food selectedFood = foods.Find(f => f.foodSprite == targetRenderer.sprite);
                    if (selectedFood != null)
                    {
                        GameManager.Instance.AddCommand(selectedFood.foodName, this);
                    }
                    else
                    {
                        Debug.LogError("No se encontró el alimento correspondiente al sprite actual.");
                    }
                }
                else
                {
                    Debug.LogError("GameManager.Instance o datos necesarios no están disponibles.");
                }

            }
            else
            {
                Debug.LogError("PlayerController no está asignado.");
            }
        }
        else
        {
            Debug.Log("No hay comida activa para interactuar.");
        }
    }

    protected override void ShowInteractionPrompt()
    {
        if (!isFoodActive || targetRenderer == null || selectedFoodSprite == null)
        {
            return; // No mostrar el texto de interacción si la comida no está activa
        }

        // Cambiar el sprite al de la comida seleccionada cuando el jugador esté en rango
        targetRenderer.sprite = selectedFoodSprite;

        base.ShowInteractionPrompt();
    }

    void LateUpdate()
    {
        if (renderers != null && renderers.Count > 0 && playerController != null)
        {
            HandleRendererSortingOrder();
        }
    }

    private void HandleRendererSortingOrder()
    {
        float playerDistance = Vector3.Distance(transform.position, playerController.transform.position);

        for (int i = 0; i < renderers.Count; i++)
        {
            var renderer = renderers[i];
            if (renderer != null)
            {
                // Orden basado en la distancia al jugador: más cerca, mayor z-index
                float sortingValue = 1 / (playerDistance + 0.1f); // Evitar división por cero
                renderer.sortingOrder = 1000 + (int)(sortingValue * 1000) + sortingOffset + i; // Añadir offset incremental
            }
        }
    }
}
