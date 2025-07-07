using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSelector : InteractableBase
{
    public List<Food> foods; // Lista de alimentos asignada desde el Inspector
    public SpriteRenderer targetRenderer; // SpriteRenderer donde se mostrará el sprite del alimento seleccionado
    public GameObject hiddenObject; // Objeto que se volverá visible
    public float activeTime = 5.0f; // Tiempo que el objeto estará activo
    public int sortingOffset = 0;
    private SpriteRenderer sr;

    public List<SpriteRenderer> renderers; // Lista de SpriteRenderers para ordenar
    private PlayerController playerController; // Referencia al PlayerController

    private bool isFoodActive = false; // Indica si la comida está activa

    public Sprite foodCallSprite; // Sprite que se muestra inicialmente
    private Sprite selectedFoodSprite; // Sprite de la comida seleccionada

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

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

    public void ShowFoodAndObject()
    {
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
                StartCoroutine(DeactivateAfterTime()); // Desactivar después de un tiempo
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

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(activeTime);
        if (hiddenObject != null)
        {
            hiddenObject.SetActive(false);
            isFoodActive = false;
        }
    }

    // Implementación del método de la interfaz InteractableBase
    public void Interact()
    {
        //ShowFoodAndObject();
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

                // Meter animación mirar al muncho

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
}
