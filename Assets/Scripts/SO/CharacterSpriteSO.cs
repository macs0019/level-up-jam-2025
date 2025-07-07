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

    [Header("Food Sprites")]
    public List<Sprite> foodSprites = new List<Sprite>();

    [Header("Color Variants")]
    public List<Color> colorVariants = new List<Color>();

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
    /// Devuelve un sprite de comida aleatorio.
    /// </summary>
    public Sprite GetRandomFood()
    {
        if (foodSprites == null || foodSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de comida asignados.");
            return null;
        }
        int index = UnityEngine.Random.Range(0, foodSprites.Count);
        return foodSprites[index];
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
}
