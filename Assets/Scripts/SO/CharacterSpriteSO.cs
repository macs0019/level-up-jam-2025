using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpriteSO", menuName = "ScriptableObjects/CharacterSpriteSO", order = 1)]
public class CharacterSpriteSO : ScriptableObject
{
    [Header("Body Sprites")]
    public List<Sprite> torsoSprites = new List<Sprite>();

    public List<Sprite> headSprites = new List<Sprite>();

    public List<Sprite> faceSprites = new List<Sprite>();

    [Header("Food Sprites")]
    public List<Sprite> foodSprites = new List<Sprite>();

    /// <summary>
    /// Devuelve un sprite de torso aleatorio.
    /// </summary>
    public Sprite GetRandomTorso()
    {
        if (torsoSprites == null || torsoSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de torso asignados.");
            return null;
        }
        int index = Random.Range(0, torsoSprites.Count);
        return torsoSprites[index];
    }

    /// <summary>
    /// Devuelve un sprite de cabeza aleatorio.
    /// </summary>
    public Sprite GetRandomHead()
    {
        if (headSprites == null || headSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de cabeza asignados.");
            return null;
        }
        int index = Random.Range(0, headSprites.Count);
        return headSprites[index];
    }

    /// <summary>
    /// Devuelve un sprite de cara aleatorio.
    /// </summary>
    public Sprite GetRandomFace()
    {
        if (faceSprites == null || faceSprites.Count == 0)
        {
            Debug.LogWarning("No hay sprites de cara asignados.");
            return null;
        }
        int index = Random.Range(0, faceSprites.Count);
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
        int index = Random.Range(0, foodSprites.Count);
        return foodSprites[index];
    }
}
