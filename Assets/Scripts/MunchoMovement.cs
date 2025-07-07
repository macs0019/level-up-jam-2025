using System.Collections;
using UnityEngine;
using DG.Tweening; // Importar DOTween

public class MunchoMovement : MonoBehaviour
{
    public Transform[] path;
    public float speed = 1.0f;
    public float probabilityToLeave = 1f; // Probabilidad de que el muncho se vaya

    public float minWaitTime = 3f; // Tiempo mínimo de espera en cada punto del camino
    public float maxWaitTime = 5f; // Tiempo máximo de espera en

    private FoodSelector foodSelector; // Referencia al FoodSelector para interactuar con la comida

    private void Awake()
    {
        foodSelector = this.GetComponent<FoodSelector>();
    }

    public void SetTableAndPath(GameManager.Table table)
    {
        if (table == null || table.path == null || table.path.Length == 0)
        {
            Debug.LogError("Table or path is not assigned or empty.");
            return;
        }

        path = table.path;

        MoveAlongPath(() =>
        {
            // Candidato para elegir comida
            foodSelector.ShowFoodAndObject();

        });


    }

    public void MoveAlongPath(System.Action onComplete)
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Path is not assigned or empty.");
            onComplete?.Invoke();
            return;
        }

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < path.Length; i++)
        {
            float distance = i == 0 ? Vector3.Distance(transform.position, path[i].position) : Vector3.Distance(path[i - 1].position, path[i].position);
            float duration = distance / speed; // Calcular la duración basada en la velocidad
            sequence.Append(transform.DOMove(path[i].position, duration).SetEase(Ease.Linear));
        }
        sequence.AppendInterval(Random.Range(minWaitTime, maxWaitTime));

        sequence.OnComplete(() => onComplete?.Invoke());
    }

    public void MoveAlongPathReverse(System.Action onComplete)
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Path is not assigned or empty.");
            onComplete?.Invoke();
            return;
        }

        Sequence sequence = DOTween.Sequence();
        for (int i = path.Length - 1; i >= 0; i--)
        {
            float distance = i == path.Length - 1 ? Vector3.Distance(transform.position, path[i].position) : Vector3.Distance(path[i + 1].position, path[i].position);
            float duration = distance / speed; // Calcular la duración basada en la velocidad
            sequence.Append(transform.DOMove(path[i].position, duration).SetEase(Ease.Linear));
        }

        sequence.OnComplete(() => onComplete?.Invoke());
    }

    public bool ShouldLeave()
    {
        return UnityEngine.Random.value < probabilityToLeave;
    }
}
