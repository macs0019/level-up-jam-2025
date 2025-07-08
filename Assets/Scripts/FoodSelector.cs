using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System; // Asegúrate de tener la referencia a DOTween

public class FoodSelector : InteractableBase
{
    public List<Food> foods; // Lista de alimentos asignada desde el Inspector
    public SpriteRenderer targetRenderer; // SpriteRenderer donde se mostrará el sprite del alimento seleccionado
    public GameObject speechBalloon; // Objeto que se volverá visible
    public float activeTime = 5.0f; // Tiempo que el objeto estará activo
    public int sortingOffset = 0;

    public List<SpriteRenderer> renderers; // Lista de SpriteRenderers para ordenar
    private PlayerController playerController; // Referencia al PlayerController

    private bool isFoodActive = false; // Indica si la comida está activa

    public Sprite foodCallSprite; // Sprite que se muestra inicialmente
    private Sprite selectedFoodSprite; // Sprite de la comida seleccionada

    private float remainingTime = 0f;
    private bool timerRunning = false;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private bool orderTaken = false; // Indica si el objeto ha sido grabado

    public bool OrderTaken
    {
        get => orderTaken;
        set
        {
            orderTaken = value;
        }
    }

    public bool IsFoodActive => isFoodActive; // Propiedad para acceder al estado de la comida activa

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

        initialLocalPos = speechBalloon.transform.localPosition;
        initialLocalRot = speechBalloon.transform.localRotation;
    }

    new void Update()
    {
        base.Update(); // Llamar al método Update de la clase base InteractableBase

        // Si está activo el timer, lo descontamos
        if (timerRunning && isFoodActive)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                timerRunning = false;
                isFoodActive = false;

                // Tu desactivación
                DeactivateSpeechBalloon();

                if (!orderTaken)
                    GameManager.Instance.HandleFoodSelectorLeavingWithDelay(this);
            }
        }
    }

    public void ShowFoodAndObject()
    {
        if (foods != null && foods.Count > 0 && targetRenderer != null)
        {
            // Seleccionar un alimento aleatorio
            Food selectedFood = foods[UnityEngine.Random.Range(0, foods.Count)];

            // Guardar el sprite de la comida seleccionada
            selectedFoodSprite = selectedFood.foodSprite;

            // Mostrar el sprite de "foodCall" inicialmente
            targetRenderer.sprite = foodCallSprite;

            // Hacer visible el objeto oculto
            if (speechBalloon != null)
            {
                speechBalloon.SetActive(true);
                speechBalloon.transform.localScale = Vector3.zero;

                speechBalloon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

                isFoodActive = true; // Activar el indicador de comida activa

                StartDeactivationTimer();
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

    private void StartDeactivationTimer()
    {
        remainingTime = activeTime;
        timerRunning = true;

        isFoodActive = true; // Activar el indicador de comida activa
    }

    public void PauseDeactivationTimer()
    {
        if (timerRunning)
        {
            timerRunning = false;
            
            isFoodActive = false; // Desactivar el indicador de comida activa
        }
    }

    public void ResumeDeactivationTimer()
    {
        if (isFoodActive && !timerRunning)
        {
            timerRunning = true;

            isFoodActive = true; // Re-activar el indicador de comida activa
        }
    }

    public void DeactivateSpeechBalloon()
    {
        isFoodActive = false; // Desactivar el indicador de comida activa

        HideFoodAnimation(() =>
        {
            speechBalloon.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                speechBalloon.SetActive(false);
            });
        }); // Ocultar la comida y volver al sprite de foodCall
    }

    public override void OnInteract()
    {
        if (GameManager.Instance != null && GameManager.Instance.LastInteractedFoodSelector != null)
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
                if (GameManager.Instance != null && foods != null && targetRenderer != null && selectedFoodSprite != null)
                {
                    // Buscar el alimento correspondiente al sprite seleccionado
                    Food selectedFood = foods.Find(f => f.foodSprite == selectedFoodSprite);
                    if (selectedFood != null)
                    {
                        targetRenderer.sprite = selectedFoodSprite;
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

    public void ShowFoodAnimation(Vector3 targetPos, Quaternion targetRot)
    {
        if (targetRenderer == null || speechBalloon == null)
        {
            Debug.LogError("targetRenderer, selectedFoodSprite o speechBalloon no están asignados.");
            return;
        }

        //initialLocalPos = speechBalloon.transform.position;
        //initialLocalRot = speechBalloon.transform.rotation;

        speechBalloon.transform.transform.DOKill(true);

        targetRenderer.sprite = selectedFoodSprite; // Asignar el sprite seleccionado al SpriteRenderer

        speechBalloon.transform.DOMove(targetPos, 0.5f);
        speechBalloon.transform.DORotateQuaternion(targetRot, 0.5f);
    }

    public void HideFoodAnimation(Action onComplete = null)
    {
        if (targetRenderer == null || selectedFoodSprite == null)
        {
            Debug.LogError("targetRenderer o selectedFoodSprite no están asignados.");
            return;
        }

        isFoodActive = false; // Desactivar el indicador de comida activa

        speechBalloon.transform.DOKill(true);

        speechBalloon.transform.DOLocalMove(initialLocalPos, 0.5f);
        speechBalloon.transform.DOLocalRotateQuaternion(initialLocalRot, 0.5f)
        .OnComplete(() =>
        {
            targetRenderer.sprite = foodCallSprite;
            isFoodActive = true; // Asegurarse de que la comida no esté activa
            onComplete?.Invoke();
        });
    }

    protected override void ShowInteractionPrompt()
    {
        if (!isFoodActive || targetRenderer == null || selectedFoodSprite == null)
        {
            return; // No mostrar el texto de interacción si la comida no está activa
        }

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
