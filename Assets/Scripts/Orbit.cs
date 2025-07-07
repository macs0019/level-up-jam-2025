using UnityEngine;

public class Orbit : MonoBehaviour
{
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float angularSpeed = 30f;

    [Tooltip("Eje de rotación (por defecto Y)")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Rota este objeto alrededor del origen (0,0,0)
        transform.RotateAround(Vector3.zero, rotationAxis, angularSpeed * Time.deltaTime);
    }
}
