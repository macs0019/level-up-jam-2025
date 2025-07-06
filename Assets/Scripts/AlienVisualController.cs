using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlienVisualController : MonoBehaviour
{
    [SerializeField] private CharacterSpriteSO characterSprite;

    [SerializeField] private SpriteRenderer torsoRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer faceRenderer;

    private Sprite assignedTorso;
    private Sprite assignedHead;
    private Sprite assignedFace;

    private void Awake()
    {
        // Asignamos sprites aleatorios para cada uno de los aliens
        assignedTorso = characterSprite.GetRandomTorso();
        assignedHead = characterSprite.GetRandomHead();
        assignedFace = characterSprite.GetRandomFace();

        torsoRenderer.sprite = assignedTorso;
        headRenderer.sprite = assignedHead;
        faceRenderer.sprite = assignedFace;
    }
}
