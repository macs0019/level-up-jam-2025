using UnityEngine;
using DG.Tweening;
using Aviss;

public class MunchoMovement : MonoBehaviour
{
    public Transform[] path;
    public float speed = 1.0f;
    public float probabilityToLeave = 1f; // Probabilidad de que el muncho se vaya

    // Tiempo de espera antes/despues del camino
    public float minWaitTime = 3f;
    public float maxWaitTime = 5f;

    private FoodSelector foodSelector; // Referencia al FoodSelector para interactuar con la comida

    private GameManager.Table currentTable;

    public GameManager.Table CurrentTable => currentTable; // Propiedad para acceder a la mesa actual

    // Propiedades para gestionar las animaciones

    public Transform visualTransform;

    private float walkingHopHeight = 0.5f;
    private float walkingRotAngle = 7.5f;
    private float walkingStepDuration = 0.4f;


    private void Awake()
    {
        foodSelector = GetComponent<FoodSelector>();
    }

    public void SetTableAndPath(GameManager.Table table)
    {
        if (table == null || table.path == null || table.path.Length == 0)
        {
            Debug.LogError("Table or path is not assigned or empty.");
            return;
        }

        currentTable = table;
        path = currentTable.path;

        MoveAlongPath(() =>
        {
            foodSelector.ShowFoodAndObject();
        });
    }

    public void MoveAlongPath(System.Action onComplete)
    {
        StartWalkingAnimation();

        if (path == null || path.Length == 0)
        {
            Debug.LogError("Path is not assigned or empty.");
            onComplete?.Invoke();
            return;
        }

        float yPosition = transform.position.y;

        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 pathPosition = path[i].position;
            pathPosition.y = yPosition;

            float distance;

            if (i == 0)
            {
                distance = Vector3.Distance(transform.position, pathPosition);
            }
            else
            {
                Vector3 lastPathPosition = path[i - 1].position;
                lastPathPosition.y = yPosition;

                distance = Vector3.Distance(lastPathPosition, pathPosition);
            }

            float duration = distance / speed; // Calcular la duración basada en la velocidad

            sequence.Append(transform.DOMove(pathPosition, duration).SetEase(Ease.Linear));
        }

        // Reseteamos la animación de caminar antes de esperar a pedir
        sequence.AppendCallback(() =>
        {
            ResetWalkingAnimation();
        });

        // Saltamos desde el suelo hasta la silla
        sequence.Append(transform.DOLocalJump(path[path.Length - 1].position, walkingHopHeight, 1, walkingStepDuration)
            .SetEase(Ease.InOutQuad));

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

        // Deactivate the speechBalloon
        foodSelector.DeactivateSpeechBalloon();

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(Random.Range(minWaitTime, maxWaitTime));

        // Iniciamos la animación tras ponernos a andar
        sequence.AppendCallback(() =>
        {
            StartWalkingAnimation();
        });

        for (int i = path.Length - 1; i >= 0; i--)
        {
            float distance = i == path.Length - 1 ? Vector3.Distance(transform.position, path[i].position) : Vector3.Distance(path[i + 1].position, path[i].position);
            float duration = distance / speed; // Calcular la duración basada en la velocidad
            sequence.Append(transform.DOMove(path[i].position, duration).SetEase(Ease.Linear));
        }

        // Reseteamos la animación antes de terminar
        sequence.AppendCallback(() =>
        {
            AudioController.Instance.Play("Door");
            ResetWalkingAnimation();
        });

        sequence.OnComplete(() => onComplete?.Invoke());
    }

    public bool ShouldLeave()
    {
        return Random.value < probabilityToLeave;
    }

    private void StartWalkingAnimation()
    {
        // 1) Ponemos el ángulo inicial en -rotAngle
        visualTransform.localEulerAngles = Vector3.forward * -walkingRotAngle;
        // 2) Creamos la rotación oscilante entre -rotAngle y +rotAngle
        visualTransform
            .DOLocalRotate(Vector3.forward * walkingRotAngle, walkingStepDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(UpdateType.Normal, true);

        // 3) Creamos el “hop” completo (sube y baja) sincronizado
        Vector3 startPos = visualTransform.localPosition;

        visualTransform
            .DOLocalJump(startPos, walkingHopHeight, 1, walkingStepDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(UpdateType.Normal, true);
    }

    private void ResetWalkingAnimation()
    {
        DOTween.Kill(visualTransform);

        float resetDuration = 0.2f;

        visualTransform.DOLocalMove(Vector3.zero, resetDuration)
            .SetEase(Ease.InOutQuad);
        visualTransform.DOLocalRotate(Vector3.zero, resetDuration)
            .SetEase(Ease.InOutQuad);
    }
}
