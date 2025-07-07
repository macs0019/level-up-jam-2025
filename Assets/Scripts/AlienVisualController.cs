using NUnit.Framework;
using Redraw;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlienVisualController : MonoBehaviour
{
    [SerializeField] private CharacterSpriteSO characterSprite;

    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject face;
    [SerializeField] private GameObject speechBalloon; // Globo de diálogo asignado desde el Inspector
    [SerializeField] private GameObject foodSign; // Señal de comida asignada desde el Inspector

    private SpriteRenderer torsoRenderer;
    private SpriteRenderer headRenderer;
    private SpriteRenderer faceRenderer;

    private SingleAnimationSprite torsoAnim;
    private SingleAnimationSprite headAnim;
    private SingleAnimationSprite faceAnim;


    private Color assignedColor;

    private void Awake()
    {
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

        // Asignamos sprites aleatorios para cada uno de los aliens
        torsoRenderer = torso.GetComponent<SpriteRenderer>();
        headRenderer = head.GetComponent<SpriteRenderer>();
        faceRenderer = face.GetComponent<SpriteRenderer>();

        torsoAnim = torso.GetComponent<SingleAnimationSprite>();
        headAnim = head.GetComponent<SingleAnimationSprite>();
        faceAnim = face.GetComponent<SingleAnimationSprite>();

        SpriteGroup torsoSprites = characterSprite.GetRandomTorso();
        SpriteGroup headSprites = characterSprite.GetRandomHead();
        SpriteGroup faceSprites = characterSprite.GetRandomFace();

        assignedColor = characterSprite.GetRandomColor();

        torsoRenderer.sprite = torsoSprites.sprites[0];
        headRenderer.sprite = headSprites.sprites[0];
        faceRenderer.sprite = faceSprites.sprites[0];

        torsoAnim.Sprites = torsoSprites.sprites;
        headAnim.Sprites = headSprites.sprites;
        faceAnim.Sprites = faceSprites.sprites;

        torsoRenderer.color = assignedColor;
        headRenderer.color = assignedColor;
    }

    [SerializeField] private PlayerController playerController; // Referencia al PlayerController
    [SerializeField] private int sortingOffset = 0;

    void LateUpdate()
    {
        if (playerController != null)
        {
            float playerDistance = Vector3.Distance(transform.position, playerController.transform.position);

            // Ordenar torso, head y face
            if (torsoRenderer != null)
            {
                torsoRenderer.sortingOrder = 1000 + (int)(1 / (playerDistance + 0.1f) * 1000) + sortingOffset;
            }
            if (headRenderer != null)
            {
                headRenderer.sortingOrder = torsoRenderer.sortingOrder + 1;
            }
            if (faceRenderer != null)
            {
                faceRenderer.sortingOrder = headRenderer.sortingOrder + 1;
            }

            // Ordenar speechBalloon y foodSign
            if (speechBalloon != null)
            {
                var speechRenderer = speechBalloon.GetComponent<SpriteRenderer>();
                if (speechRenderer != null)
                {
                    speechRenderer.sortingOrder = faceRenderer.sortingOrder + 1;
                }
            }
            if (foodSign != null)
            {
                var foodSignRenderer = foodSign.GetComponent<SpriteRenderer>();
                if (foodSignRenderer != null)
                {
                    foodSignRenderer.sortingOrder = faceRenderer.sortingOrder + 2;
                }
            }
        }
    }
}
