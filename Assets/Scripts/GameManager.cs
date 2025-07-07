using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // Importar el nuevo Input System
using DG.Tweening; // Importar DOTween

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    public TMP_InputField commandInputField; // InputField para ingresar el comando
    public TMP_Text enteredWordsText; // Texto para mostrar las palabras ingresadas

    private PlayerController playerController; // Referencia al PlayerController
    private FoodSelector lastInteractedFoodSelector; // Referencia al último FoodSelector con el que se interactuó

    private bool error = false; // Indica si el usuario se ha equivocado al introducir el texto

    public GameObject[] lives; // Array de objetos que representan las vidas del jugador
    private int currentLives; // Contador de vidas actuales

    [System.Serializable]
    public class Table
    {
        public Transform[] path; // Camino hacia la mesa, donde la última posición es la mesa
        public bool isOccupied = false; // Estado de la mesa
    }

    public List<Table> tables; // Lista de mesas con sus caminos
    [SerializeField]
    public List<Transform[]> paths; // Lista de caminos (cada camino es un array de puntos)

    public GameObject munchoPrefab; // Prefab del muncho

    public int numberOfTablesToOccupy = 4; // Número de mesas a ocupar asignado desde el Inspector

    private int occupiedTablesCount = 0; // Contador de mesas ocupadas
    private float spawnTimer = 0f; // Temporizador para el spawn de munchos

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLives = lives.Length; // Inicializar el contador con el número de vidas del array

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
        spawnTimer += Time.deltaTime;

        // Spawnear munchos cada 10 segundos hasta que se ocupen el número de mesas especificado
        if (spawnTimer >= 10f && occupiedTablesCount < numberOfTablesToOccupy)
        {
            Debug.Log("Spawneando un nuevo muncho...");
            spawnTimer = 0f;
            SendMunchoToTable();
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
                foodSelector.PauseDeactivationTween();
            }


            // Almacenar el último FoodSelector interactuado
            SetLastInteractedFoodSelector(foodSelector);

            commandInputField.onEndEdit.RemoveAllListeners(); // Limpiar listeners previos
            commandInputField.onEndEdit.AddListener((input) =>
            {
                if (!string.IsNullOrEmpty(input)) // Detectar cuando se finaliza la edición
                {
                    foodSelector.OrderTaken = true; // Marcar que el pedido ha sido tomado
                    if (input.Equals(foodName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log("Correct");
                        error = false;
                    }
                    else
                    {
                        Debug.Log("Incorrect");
                        error = true;
                    }

                    // Agregar la palabra ingresada al texto
                    if (enteredWordsText != null)
                    {
                        enteredWordsText.text += input + "\n";
                    }
                }

                commandInputField.text = string.Empty; // Limpiar el texto
                commandInputField.gameObject.SetActive(false); // Desactivar el InputField

                // Reactivar el movimiento del jugador
                if (playerController != null)
                {
                    playerController.enabled = true;
                    foodSelector.ResumeDeactivationTween();
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
                        foodSelector.ResumeDeactivationTween();
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

    public bool HasError()
    {
        return error;
    }

    public void SubtractLife()
    {
        for (int i = lives.Length - 1; i >= 0; i--)
        {
            if (lives[i].activeSelf)
            {
                lives[i].SetActive(false); // Deshabilitar la vida más a la derecha
                currentLives--; // Restar una vida del contador
                break;
            }
        }
    }

    public bool HandleError()
    {
        bool hasError = error;

        if (error)
        {
            SubtractLife(); // Restar una vida si hay un error
        }

        error = false; // Resetear el estado de error
        lastInteractedFoodSelector = null; // Eliminar el último FoodSelector interactuado

        return hasError;
    }

    public void ClearEnteredWords()
    {
        if (enteredWordsText != null)
        {
            enteredWordsText.text = string.Empty; // Limpiar el texto ingresado
        }
    }

    public TMP_Text interactionUIText; // Texto de la UI de interacción asignado desde el Inspector
    public Transform playerTransform; // Transform del jugador asignado desde el Inspector

    public void SendMunchoToTable()
    {
        // Filtrar mesas disponibles
        List<Table> availableTables = tables.FindAll(table => !table.isOccupied);
        if (availableTables.Count == 0)
        {
            Debug.LogError("No hay mesas disponibles.");
            return;
        }

        // Seleccionar una mesa aleatoria de las disponibles
        Table targetTable = availableTables[UnityEngine.Random.Range(0, availableTables.Count)];

        // Marcar la mesa como ocupada inmediatamente
        targetTable.isOccupied = true;
        occupiedTablesCount++; // Incrementar el contador de mesas ocupadas

        // Usar el camino asociado a la mesa seleccionada
        Transform[] path = targetTable.path;

        // Instanciar el muncho
        GameObject muncho = Instantiate(munchoPrefab, path[0].position, Quaternion.identity);

        // Configurar propiedades adicionales del muncho
        FoodSelector foodSelector = muncho.GetComponent<FoodSelector>();
        if (foodSelector != null)
        {

            // Asignar propiedades desde el Inspector
            foodSelector.interactionUIText = interactionUIText as TextMeshProUGUI;
            foodSelector.playerTransform = playerTransform;
        }
        else
        {
            Debug.LogError("El muncho no tiene un componente FoodSelector.");
        }

        // Asignar la mesa al MunchoMovement
        MunchoMovement munchoMovement = muncho.GetComponent<MunchoMovement>();
        if (munchoMovement == null)
        {
            munchoMovement = muncho.AddComponent<MunchoMovement>();
        }
        munchoMovement.SetTableAndPath(targetTable);

    }

    public float probabilityToLeave = 1f; // Probabilidad de que el último FoodSelector se vaya

    public void InteractWithBossController()
    {
        Debug.Log("Interacting with BossController...");
        Debug.Log("Último FoodSelector interactuado: " + (lastInteractedFoodSelector != null ? lastInteractedFoodSelector.name : "Ninguno"));
        FoodSelector foodSelectorToLeave = lastInteractedFoodSelector;
        if (foodSelectorToLeave != null)
        {
            MunchoMovement munchoMovement = foodSelectorToLeave.GetComponent<MunchoMovement>();
            if (munchoMovement != null && munchoMovement.ShouldLeave())
            {
                Debug.Log("El último FoodSelector se irá inmediatamente.");

                // Manejar la salida usando DOTween
                HandleFoodSelectorLeavingWithDelay(foodSelectorToLeave);
            }
        }
    }

    public void HandleFoodSelectorLeavingWithDelay(FoodSelector foodSelector)
    {
        foodSelector.enabled = false;

        MunchoMovement munchoMovement = foodSelector.GetComponent<MunchoMovement>();

        if (munchoMovement != null)
        {
            munchoMovement.MoveAlongPathReverse(() =>
            {
                Table targetTable = munchoMovement.CurrentTable;

                if (targetTable != null)
                {
                    targetTable.isOccupied = false;
                    occupiedTablesCount--;
                }

                Destroy(foodSelector.gameObject);

            });
        }
    }
}
