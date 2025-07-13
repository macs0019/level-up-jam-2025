using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpriteGroup
{
    public List<Sprite> sprites;
}

[CreateAssetMenu(fileName = "CharacterSpriteSO", menuName = "ScriptableObjects/CharacterSpriteSO", order = 1)]
public class CharacterSpriteSO : ScriptableObject
{
    [Header("Body Sprites")]
    public List<SpriteGroup> torsoSprites = new List<SpriteGroup>();
    public List<SpriteGroup> headSprites = new List<SpriteGroup>();
    public List<SpriteGroup> faceSprites = new List<SpriteGroup>();

    [Header("Foods")]
    public List<FoodPOJO> foods = new List<FoodPOJO>();

    [Header("Color Variants")]
    public List<Color> colorVariants = new List<Color>();

    [Header("Food Sprites")]
    private List<Sprite> foodSprites = new List<Sprite>();


    // Cada vez que Unity entra en Play Mode y recarga dominios,
    // OnEnable se dispara, así que:
    void OnValidate()
    {
        ResetFoodNames();
    }

    public void ResetFoodNames()
    {
        foods.ForEach(f => f.Name = "");
    }

    /// <summary>
    /// Devuelve un sprite de torso aleatorio.
    /// </summary>
    public SpriteGroup GetRandomTorso()
    {
        if (torsoSprites == null || torsoSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de torso asignados.");
            return null;
        }
        int index = UnityEngine.Random.Range(0, torsoSprites.Count);
        return torsoSprites[index];
    }

    /// <summary>
    /// Devuelve un sprite de cabeza aleatorio.
    /// </summary>
    public SpriteGroup GetRandomHead()
    {
        if (headSprites == null || headSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de cabeza asignados.");
            return null;
        }
        int index = UnityEngine.Random.Range(0, headSprites.Count);
        return headSprites[index];
    }

    /// <summary>
    /// Devuelve un sprite de cara aleatorio.
    /// </summary>
    public SpriteGroup GetRandomFace()
    {
        if (faceSprites == null || faceSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de cara asignados.");
            return null;
        }
        int index = UnityEngine.Random.Range(0, faceSprites.Count);
        return faceSprites[index];
    }

    /// <summary>
    /// Devuelve un objeto FoodPOJO aleatorio.
    /// </summary>
    public FoodPOJO GetRandomFood()
    {
        if (foods == null || foods.Count == 0)
        {
            Debug.LogWarning("No hay objetos de comida asignados.");
            return null;
        }
        int index = UnityEngine.Random.Range(0, foods.Count);
        return foods[index];
    }

    /// <summary>
    /// Actualiza el nombre de un objeto FoodPOJO dado su ID.
    /// </summary>
    public void SetFoodName(int id, string newName)
    {
        if (foods == null || foods.Count == 0)
        {
            Debug.LogWarning("No hay objetos de comida asignados.");
            return;
        }

        FoodPOJO food = foods.Find(f => f.Id == id);
        if (food != null)
        {
            food.Name = newName;
        }
        else
        {
            Debug.LogWarning($"No se encontró ningún objeto FoodPOJO con el ID {id}.");
        }
    }

    /// <summary>
    /// Devuelve un color aleatorio de la lista de variantes.
    /// </summary>
    public Color GetRandomColor()
    {
        if (colorVariants == null || colorVariants.Count == 0)
        {
            Debug.LogWarning("No hay colores asignados en colorVariants.");
            return Color.white;
        }
        int index = UnityEngine.Random.Range(0, colorVariants.Count);
        return colorVariants[index];
    }

    /// <summary>
    /// Devuelve una lista de objetos FoodPOJO cuyo nombre no es vacío ni nulo.
    /// </summary>
    public List<FoodPOJO> GetNamedFoods()
    {
        if (foods == null || foods.Count == 0)
        {
            Debug.LogWarning("No hay objetos de comida asignados.");
            return new List<FoodPOJO>();
        }

        return foods.FindAll(food => !string.IsNullOrEmpty(food.Name));
    }

    /// <summary>
    /// Devuelve una lista de objetos FoodPOJO cuyo nombre es vacío o nulo.
    /// Si el número solicitado excede la cantidad disponible, devuelve todas las unamed foods.
    /// </summary>
    /// <param name="count">Número de unamed foods a devolver.</param>
    /// <returns>Lista de objetos FoodPOJO unamed.</returns>
    public List<FoodPOJO> GetUnamedFoods(int count)
    {
        if (foods == null || foods.Count == 0)
        {
            Debug.LogWarning("No hay objetos de comida asignados.");
            return new List<FoodPOJO>();
        }

        List<FoodPOJO> unamedFoods = foods.FindAll(food => string.IsNullOrEmpty(food.Name));

        if (unamedFoods.Count <= count)
        {
            return unamedFoods;
        }

        return unamedFoods.GetRange(0, count);
    }
}
