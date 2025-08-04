using System.Collections;
using System.Collections.Generic;
using Aviss;
using DG.Tweening;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    [Header("UI Elements")]
    public TMP_InputField commandInputField; // InputField para ingresar el comando
    public TMP_Text enteredWordsText; // Texto para mostrar las palabras ingresadas

    public TMP_Text interactionUIText; // Texto de la UI de interacción asignado desde el Inspector

    private PlayerController playerController; // Referencia al PlayerController
    private FoodSelector lastInteractedFoodSelector; // Referencia al último FoodSelector con el que se interactuó

    public FoodSelector LastInteractedFoodSelector
    {
        get { return lastInteractedFoodSelector; }
        set { lastInteractedFoodSelector = value; }
    }

    private bool error = false; // Indica si el usuario se ha equivocado al introducir el texto

    [Header("Game Properties")]
    public GameObject[] lives; // Array de objetos que representan las vidas del jugador
    private int currentLives; // Contador de vidas actuales

    public GameObject endScreen;

    public GameObject youWinTitle;

    public GameObject youLoseTitle;

    public float probabilityToLeave = 1f; // Probabilidad de que el último FoodSelector se vaya

    [System.Serializable]
    public class Table
    {
        public Transform[] path; // Camino hacia la mesa, donde la última posición es la mesa
        public bool isOccupied = false; // Estado de la mesa
    }

    public List<Table> tables; // Lista de mesas con sus caminos
    [SerializeField] public List<Transform[]> paths; // Lista de caminos (cada camino es un array de puntos)

    [Header("Spawning Munchos Properties")]

    public GameObject munchoPrefab; // Prefab del muncho
    private int occupiedTablesCount = 0; // Contador de mesas ocupadas
    private float spawnTimer = 3f; // Temporizador para el spawn de munchos

    public NamerManager namerManager; // Referencia al NamerManager

    private int exitedMunchosCount = 0; // Contador de munchos que han salido

    private bool isNamingFood = true;

    public bool IsNamingFood { get => isNamingFood; set => isNamingFood = value; }
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    [Header("Level Configuration")]
    public LevelSO levelSO; // Referencia al ScriptableObject LevelSO

    private int currentLevel = 0; // Nivel actual, comienza en 0
    private bool isPaused = false;

    public Button playButton;

    public Button exitButton;

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

        playButton.onClick.AddListener(() =>
        {
            SceneController.Instance.LoadScene(1);
        });

        exitButton.onClick.AddListener(() =>
          {
              SceneController.Instance.LoadScene(0);
          });
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNamingFood && !isPaused)
        {
            spawnTimer += Time.deltaTime;

            // Spawnear munchos cada 10 segundos hasta que se ocupen el número de mesas especificado
            if (spawnTimer >= levelSO.Levels[currentLevel].MunchoEntryDelay && occupiedTablesCount < levelSO.Levels[currentLevel].NumberOfOccupiedTables)
            {
                spawnTimer = 0f;
                SendMunchoToTable();
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

            foodSelector.HideInteractionPrompt(); // Ocultar el mensaje de interacción 
            // Desactivar el movimiento del jugador
            if (playerController != null)
            {
                playerController.enabled = false;
                playerController.StartInteractAnimation(foodSelector);

                foodSelector.PauseDeactivationTimer();
            }

            // Para cada letra escrita / borrada, hacemos animación del player
            commandInputField.onValueChanged.AddListener((input) =>
            {
                playerController.StartWritingAnimation();
            });

            commandInputField.onEndEdit.RemoveAllListeners();
            commandInputField.onEndEdit.AddListener((input) =>
            {

                if (!string.IsNullOrEmpty(input)) // Detectar cuando se finaliza la edición
                {

                    // Almacenar el último FoodSelector interactuado
                    LastInteractedFoodSelector = foodSelector;
                    foodSelector.OrderTaken = true; // Marcar que el pedido ha sido tomado

                    input = input.Trim();

                    error = !input.Equals(foodName, System.StringComparison.OrdinalIgnoreCase); // Comparar el texto ingresado con el nombre de la comida

                    // Agregar la palabra ingresada al texto
                    if (enteredWordsText != null)
                    {
                        enteredWordsText.text += input + "\n";
                    }

                    // TUTORIALES DE MIERDA
                    if (TutorialController.Instance.gameObject.activeSelf)
                    {
                        Debug.Log("GameManager Continue");
                        TutorialController.Instance.NextText();
                    }

                    // Reactivar el movimiento del jugador
                    Resume(foodSelector, resetLastInteracted: false);
                }
                else
                {
                    foodSelector.ShowInteractionPrompt(); // Ocultar el mensaje de interacción
                                                          // Reactivar el movimiento del jugador
                    Resume(foodSelector);
                }

                commandInputField.text = string.Empty; // Limpiar el texto
                commandInputField.gameObject.SetActive(false); // Desactivar el InputField

            });
        }
        else
        {
            Debug.LogError("Command Input Field no está asignado en el Inspector.");
        }
    }

    private void Resume(FoodSelector foodSelector, bool resetLastInteracted = true)
    {

        if (playerController != null)
        {
            if (resetLastInteracted)
            {
                LastInteractedFoodSelector = null; // Resetear el último FoodSelector interactuado
            }

            if (resetLastInteracted == false)
            {
                foodSelector.DeactivateSpeechBalloon();
            }
            else
            {
                foodSelector.HideFoodAnimation(() =>
                {
                    foodSelector.ResumeDeactivationTimer();
                }, true);

            }
            playerController.EndInteractAnimation(() =>
            {
                playerController.enabled = true;
            });
        }
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
                lives[i].GetComponent<RectTransform>().DOShakeAnchorPos(0.5f).OnComplete(() =>
                {
                    lives[i].SetActive(false); // Deshabilitar la vida más a la derecha
                });
                currentLives--; // Restar una vida del contador

                if (currentLives == 0)
                {
                    endScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.8f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        isPaused = true;
                        Cursor.lockState = CursorLockMode.None;
                    });

                    youLoseTitle.SetActive(true); // Mostrar título de derrota
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    isNamingFood = true; // Desactivar el modo de nombrar comida
                    AudioController.Instance.Play("GameOver");
                }
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
        LastInteractedFoodSelector = null; // Eliminar el último FoodSelector interactuado

        return hasError;
    }

    public void ClearEnteredWords()
    {
        if (enteredWordsText != null)
        {
            enteredWordsText.text = string.Empty; // Limpiar el texto ingresado
        }
    }

    public void SendMunchoToTable()
    {
        // Comprobar si ya se ha alcanzado el límite de munchos
        if (exitedMunchosCount >= levelSO.Levels[currentLevel].MaxMunchos)
        {
            Debug.Log("Límite de munchos alcanzado para este nivel.");
            return;
        }

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
        Debug.Log($"Spawning muncho at {path[0].position} on table with path length {path.Length}");
        GameObject muncho = Instantiate(munchoPrefab, path[0].position, Quaternion.identity);

        // Incrementar el contador de munchos que han salido
        exitedMunchosCount++;

        // Asignar el tiempo de espera al componente FoodSelector
        FoodSelector munchoFoodSelector = muncho.GetComponent<FoodSelector>();
        if (munchoFoodSelector != null)
        {
            munchoFoodSelector.activeTime = levelSO.Levels[currentLevel].waitingTime;
        }
        AudioController.Instance.Play("Door");

        // Configurar propiedades adicionales del muncho
        FoodSelector foodSelector = muncho.GetComponent<FoodSelector>();
        if (foodSelector != null)
        {
            // Asignar propiedades desde el Inspector
            foodSelector.interactionUIText = interactionUIText as TextMeshProUGUI;
            foodSelector.playerController = playerController;
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

    public void InteractWithBossController()
    {
        FoodSelector foodSelectorToLeave = LastInteractedFoodSelector;

        lastInteractedFoodSelector = null; // Limpiar la referencia al último FoodSelector interactuado

        if (foodSelectorToLeave != null)
        {
            MunchoMovement munchoMovement = foodSelectorToLeave.GetComponent<MunchoMovement>();
            if (munchoMovement != null && munchoMovement.ShouldLeave())
            {
                // Manejar la salida usando DOTween
                HandleFoodSelectorLeavingWithDelay(foodSelectorToLeave, false);
            }
        }
    }

    public void HandleFoodSelectorLeavingWithDelay(FoodSelector foodSelector, bool subtractLife = true)
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

                // Si se debe restar una vida, hacerlo
                if (subtractLife)
                {
                    SubtractLife();
                }

                Destroy(foodSelector.gameObject);

                // Comprobar si todos los munchos han salido
                if (exitedMunchosCount >= levelSO.Levels[currentLevel].MaxMunchos && occupiedTablesCount == 0)
                {
                    StartCoroutine(WaitAndStartNextLevel());
                    WaitAndStartNextLevel();
                }
            });
        }
    }

    private IEnumerator WaitAndStartNextLevel()
    {
        AudioController.Instance.Play("Next Level");
        yield return new WaitForSeconds(2f);

        isNamingFood = true;
        StartNextLevel();
    }

    public void StartNextLevel()
    {
        occupiedTablesCount = 0; // Reiniciar el contador de mesas ocupadas
        exitedMunchosCount = 0; // Reiniciar el contador de munchos que han salido
        // Reiniciar el temporizador de spawn
        spawnTimer = 3f;

        // Comenzar el nivel de forma normal
        foreach (var table in tables)
        {
            table.isOccupied = false; // Liberar todas las mesas
        }

        // Reactivar el NamerManager para el nuevo nivel
        if (namerManager != null)
        {
            currentLevel++;

            if (currentLevel >= levelSO.Levels.Count)
            {
                isPaused = true; // Pausar el juego si se ha alcanzado el último nivel
                Cursor.lockState = CursorLockMode.None;

                endScreen.GetComponent<RectTransform>().DOAnchorPosY(0, 0.8f).SetEase(Ease.OutBack);
                AudioController.Instance.Play("Victory");
                youWinTitle.SetActive(true); // Mostrar título de victoria
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                namerManager.StartNamingAction();
            }
        }
    }

    public int getCurrentLevel()
    {
        return currentLevel;
    }
}
