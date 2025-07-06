using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlienVisualController : MonoBehaviour
{
    [SerializeField] private List<Sprite> torsoList;
    [SerializeField] private List<Sprite> headList;
    [SerializeField] private List<Sprite> faceList;

    [SerializeField] private SpriteRenderer torsoRenderer;
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer faceRenderer;

    private Sprite assignedTorso;
    private Sprite assignedHead;
    private Sprite assignedFace;

    private void Awake()
    {
        // Asignamos sprites aleatorios para cada uno de los aliens
        assignedTorso = torsoList[Random.Range(0, torsoList.Count)];
        assignedHead = headList[Random.Range(0, headList.Count)];
        assignedFace = faceList[Random.Range(0, faceList.Count)];

        torsoRenderer.sprite = assignedTorso;
        headRenderer.sprite = assignedHead;
        faceRenderer.sprite = assignedFace;
    }
}
