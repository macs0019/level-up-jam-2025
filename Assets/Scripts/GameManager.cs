using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // Importar el nuevo Input System

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    public FoodSelector[] foodSelectors; // Array de FoodSelectors asignado desde el Inspector
    public float interval = 5.0f; // Intervalo de tiempo en segundos asignado desde el Inspector

    private float timer;
    private int lastSelectorIndex = -1; // Índice del último FoodSelector seleccionado

    public TMP_InputField commandInputField; // InputField para ingresar el comando
    public TMP_Text enteredWordsText; // Texto para mostrar las palabras ingresadas

    private PlayerController playerController; // Referencia al PlayerController
    private FoodSelector lastInteractedFoodSelector; // Referencia al último FoodSelector con el que se interactuó

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

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;

            Debug.Log("foods: " + foodSelectors.Length);

            if (foodSelectors != null && foodSelectors.Length > 0)
            {
                if (foodSelectors.Length == 1)
                {
                    Debug.Log("Solo hay un FoodSelector disponible. Mostrándolo una vez...");
                    foodSelectors[0].ShowFoodAndObject();
                    enabled = false; // Desactivar el GameManager para evitar más actualizaciones
                    return;
                }

                int randomIndex;
                do
                {
                    randomIndex = UnityEngine.Random.Range(0, foodSelectors.Length);
                } while (randomIndex == lastSelectorIndex);

                Debug.Log("Seleccionando un nuevo FoodSelector... " + foodSelectors[randomIndex].name);
                var selector = foodSelectors[randomIndex];
                if (selector != null)
                {
                    selector.ShowFoodAndObject(); // Mostrar la comida y el objeto
                }

                lastSelectorIndex = randomIndex; // Actualizar el último índice seleccionado
            }
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruir duplicados
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persistir entre escenas
    }

    public void AddCommand(string foodName, FoodSelector foodSelector)
    {
        if (commandInputField != null)
        {
            commandInputField.gameObject.SetActive(true); // Activar el InputField
            commandInputField.ActivateInputField(); // Darle focus para escribir

            // Desactivar el movimiento del jugador
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            commandInputField.onEndEdit.RemoveAllListeners(); // Limpiar listeners previos
            commandInputField.onEndEdit.AddListener((input) =>
            {
                if (!string.IsNullOrEmpty(input)) // Detectar cuando se finaliza la edición
                {
                    if (input.Equals(foodName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log("Correct");
                    }
                    else
                    {
                        Debug.Log("Incorrect");
                    }

                    // Agregar la palabra ingresada al texto
                    if (enteredWordsText != null)
                    {
                        enteredWordsText.text += input + "\n";
                    }

                    // Almacenar el último FoodSelector interactuado
                    SetLastInteractedFoodSelector(foodSelector);
                }

                commandInputField.text = string.Empty; // Limpiar el texto
                commandInputField.gameObject.SetActive(false); // Desactivar el InputField

                // Reactivar el movimiento del jugador
                if (playerController != null)
                {
                    playerController.enabled = true;
                }
            });

            // Detectar si se presiona Escape para cerrar el InputField sin guardar
            commandInputField.onSubmit.RemoveAllListeners();
            commandInputField.onSubmit.AddListener((input) =>
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame) // Usar el nuevo Input System
                {
                    commandInputField.text = string.Empty; // Limpiar el texto
                    commandInputField.gameObject.SetActive(false); // Desactivar el InputField

                    // Reactivar el movimiento del jugador
                    if (playerController != null)
                    {
                        playerController.enabled = true;
                    }
                }
            });
        }
        else
        {
            Debug.LogError("Command Input Field no está asignado en el Inspector.");
        }
    }

    public void SetLastInteractedFoodSelector(FoodSelector foodSelector)
    {
        lastInteractedFoodSelector = foodSelector;
        Debug.Log("Último FoodSelector interactuado: " + foodSelector.name);
    }

    public FoodSelector GetLastInteractedFoodSelector()
    {
        return lastInteractedFoodSelector;
    }
}
