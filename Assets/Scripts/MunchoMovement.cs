using System.Collections;
using UnityEngine;
using DG.Tweening; // Importar DOTween

public class MunchoMovement : MonoBehaviour
{
    public Transform[] path;
    public float speed = 1.0f;

    public void SetTableAndPath(GameManager.Table table)
    {
        if (table == null || table.path == null || table.path.Length == 0)
        {
            Debug.LogError("Table or path is not assigned or empty.");
            return;
        }

        path = table.path;
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
        foreach (Transform point in path)
        {
            sequence.Append(transform.DOMove(point.position, speed).SetEase(Ease.Linear));
        }

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
            sequence.Append(transform.DOMove(path[i].position, speed).SetEase(Ease.Linear));
        }

        sequence.OnComplete(() => onComplete?.Invoke());
    }
}
