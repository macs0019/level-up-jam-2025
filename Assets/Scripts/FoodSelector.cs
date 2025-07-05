using System.Collections.Generic;
using UnityEngine;

public class FoodSelector : MonoBehaviour
{
    public List<Food> foods; // Lista de alimentos asignada desde el Inspector
    public SpriteRenderer targetRenderer; // SpriteRenderer donde se mostrará el sprite del alimento seleccionado
    public GameObject hiddenObject; // Objeto que se volverá visible

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (foods != null && foods.Count > 0 && targetRenderer != null)
        {
            // Seleccionar un alimento aleatorio
            Food selectedFood = foods[Random.Range(0, foods.Count)];

            // Mostrar el sprite del alimento seleccionado
            targetRenderer.sprite = selectedFood.foodSprite;

            // Hacer visible el objeto oculto
            if (hiddenObject != null)
            {
                hiddenObject.SetActive(true);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
