using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using Aviss;

public class NamerManager : MonoBehaviour
{
    [Header("Configuración")]
    public List<TextMeshProUGUI> texts;
    public int maxVowels = 3;
    public int maxLetterRepetitions = 2;

    [Header("Scriptable Object")]
    public CharacterSpriteSO characterSpriteSO;

    [Header("Imágenes")]
    public Image[] foodRenderers;

    [Header("Configuración de Foods")]
    public int numberOfUnamedFoods = 3;

    [Header("Input para nombre de comida")]
    public TMP_InputField foodNameInput; // Input único para ingresar el nombre de la comida

    [Header("Imagen de comida")]
    public Transform foodBalloonTransform;
    public SpriteRenderer foodRenderer; // Imagen única para mostrar el ícono de la comida

    public GameObject startUI;

    public PlayerController playerController;

    // Estado interno
    private Dictionary<char, int> letterCounts = new Dictionary<char, int>();
    private char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
    private char[] consonants = { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] assignedLetters;
    private int currentInputIndex = 0;
    private List<FoodPOJO> unamedFoods;
    private List<Sprite> foodIcons;// Lista para almacenar los íconos de las comidas sin nombre

    private void Start()
    {
        StartNamingAction();
    }

    public void StartNamingAction()
    {
        foodIcons = new List<Sprite>();
        unamedFoods = new List<FoodPOJO>();
        currentInputIndex = 0;
        letterCounts.Clear();

        playerController.LookAtBossAnimation();
        foodBalloonTransform.gameObject.SetActive(false);

        Debug.Log("NamerManager activado. Preparando UI y letras...");

        if (startUI != null)
            startUI.SetActive(true);

        // Prepara las letras
        assignedLetters = new char[texts.Count];
        AssignRandomLetters();

        // Obtén y almacena los íconos de las comidas
        if (characterSpriteSO != null && foodRenderer != null)
        {
            unamedFoods = characterSpriteSO.GetUnamedFoods(numberOfUnamedFoods);
            if (unamedFoods.Count < numberOfUnamedFoods)
            {
                Debug.LogError("No hay suficientes FoodPOJO sin nombre en el ScriptableObject.");
            }
            else
            {
                foreach (var food in unamedFoods)
                {
                    Debug.Log($"Agregando ícono de comida: {food.Name}");
                    foodIcons.Add(food.Icon);
                }

                foodRenderer.sprite = foodIcons[0]; // Mostrar el primer ícono

                foodBalloonTransform.DOKill(true);
                foodBalloonTransform.gameObject.SetActive(true);
                foodBalloonTransform.localScale = Vector3.zero;
                foodBalloonTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }
        else
        {
            Debug.LogError("El ScriptableObject no está asignado o la imagen no está configurada.");
        }

        // Suscribe el input y fócalo
        if (foodNameInput != null)
        {
            SubscribeToInput(foodNameInput);
            StartCoroutine(FocusInputImmediately());
        }
    }

    private void SubscribeToInput(TMP_InputField input)
    {
        input.onSubmit.RemoveAllListeners(); // Asegurarse de que no haya listeners duplicados
        input.onValueChanged.RemoveAllListeners();

        input.onSubmit.AddListener(HandleSubmit);
        input.onValueChanged.AddListener(ValidateInput);
    }

    private void ValidateInput(string inputText)
    {
        Dictionary<char, int> inputLetterCounts = new Dictionary<char, int>();
        string validText = "";

        // Resetear colores de las letras
        foreach (var text in texts)
        {
            text.color = Color.black;
        }

        foreach (char c in inputText)
        {
            char upperC = char.ToUpper(c);
            if (System.Array.Exists(assignedLetters, letter => letter == upperC))
            {
                if (!inputLetterCounts.ContainsKey(upperC))
                    inputLetterCounts[upperC] = 0;

                if (inputLetterCounts[upperC] < letterCounts[upperC])
                {
                    validText += c;
                    inputLetterCounts[upperC] += 1;

                    // Cambiar el color de la letra en texts a gris
                    int count = 0;
                    for (int i = 0; i < texts.Count; i++)
                    {
                        if (texts[i].text.Equals(upperC.ToString(), System.StringComparison.OrdinalIgnoreCase) && count < inputLetterCounts[upperC])
                        {
                            texts[i].color = Color.gray;
                            count++;
                        }
                    }
                }

                playerController.StartWritingAnimation();
            }
        }

        foodNameInput.text = validText;
    }

    public void HandleSubmit(string inputText)
    {
        Debug.Log($"Input recibido: {inputText}");

        // Guarda el nombre en la lista
        if (currentInputIndex < unamedFoods.Count)
        {
            unamedFoods[currentInputIndex].Name = inputText;
            foodNameInput.text = string.Empty;
        }

        currentInputIndex++;

        if (currentInputIndex < foodIcons.Count)
        {
            // Cambia la imagen y limpia el input
            foodRenderer.transform.DOKill(true);
            foodRenderer.transform.DOPunchScale(Vector3.one / 3f, 0.3f);

            foodRenderer.sprite = foodIcons[currentInputIndex];
            foodNameInput.text = string.Empty;
            AssignRandomLetters(); // Randomizar letras nuevamente
            StartCoroutine(FocusInputDelayed());
        }
        else
        {
            OnAllInputsFilled();
        }
    }

    private IEnumerator FocusInputDelayed()
    {
        yield return new WaitForEndOfFrame();
        foodNameInput.Select();
        foodNameInput.ActivateInputField();
    }

    private IEnumerator FocusInputImmediately()
    {
        yield return new WaitForSecondsRealtime(0.05f);

        if (foodNameInput != null)
        {
            EventSystem.current.SetSelectedGameObject(foodNameInput.gameObject);
            foodNameInput.ActivateInputField();
        }
    }

    private void OnAllInputsFilled()
    {
        playerController.EndNamingAnimation(() =>
        {
            GameManager.Instance.IsNamingFood = false;

        });

        // Tutoriales de mierda 
        if (TutorialController.Instance.gameObject.activeSelf)
        {
            TutorialController.Instance.Continue();
        }

        if (startUI != null)
            startUI.SetActive(false);

        // Limpiar el input y la imagen
        foodNameInput.text = string.Empty;

        // Ocultamos el globo
        foodBalloonTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            foodRenderer.sprite = null;
            foodBalloonTransform.gameObject.SetActive(false);
        }).SetUpdate(true);
    }

    private void AssignRandomLetters()
    {
        letterCounts.Clear();
        int vowelCount = 0;

        for (int i = 0; i < texts.Count; i++)
        {
            char randomLetter;
            do
            {
                randomLetter = GetRandomLetter(ref vowelCount);
            }
            while (letterCounts.ContainsKey(randomLetter) && letterCounts[randomLetter] >= maxLetterRepetitions);

            texts[i].text = randomLetter.ToString();
            assignedLetters[i] = randomLetter;

            if (!letterCounts.ContainsKey(randomLetter))
                letterCounts[randomLetter] = 0;
            letterCounts[randomLetter]++;
        }
    }

    private char GetRandomLetter(ref int vowelCount)
    {
        bool useVowel = vowelCount < maxVowels && Random.value < 0.5f;
        if (useVowel)
        {
            vowelCount++;
            return vowels[Random.Range(0, vowels.Length)];
        }
        else
        {
            return consonants[Random.Range(0, consonants.Length)];
        }
    }

    private void Update()
    {
        // Forzar el foco en el campo de texto si se pierde
        if (EventSystem.current.currentSelectedGameObject != foodNameInput.gameObject && GameManager.Instance.IsNamingFood)
        {
            EventSystem.current.SetSelectedGameObject(foodNameInput.gameObject);
            foodNameInput.ActivateInputField();
        }
    }
}
