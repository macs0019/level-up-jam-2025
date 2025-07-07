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

    private SpriteRenderer torsoRenderer;
    private SpriteRenderer headRenderer;
    private SpriteRenderer faceRenderer;

    private SingleAnimationSprite torsoAnim;
    private SingleAnimationSprite headAnim;
    private SingleAnimationSprite faceAnim;


    private Color assignedColor;

    private void Awake()
    {
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
}
