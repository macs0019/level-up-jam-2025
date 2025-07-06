using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    public FoodSelector[] foodSelectors; // Array de FoodSelectors asignado desde el Inspector
    public float interval = 5.0f; // Intervalo de tiempo en segundos asignado desde el Inspector

    private float timer;
    private int lastSelectorIndex = -1; // Índice del último FoodSelector seleccionado

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
