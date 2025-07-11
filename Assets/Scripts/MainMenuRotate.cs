using UnityEngine;

public class MainMenuRotate : MonoBehaviour
{
    [Tooltip("Grados por segundo")]
    public float velocity = 10f;

    void Update()
    {
        // Rota en el eje Y de forma lenta y continua
        transform.Rotate(0f, velocity * Time.deltaTime, 0f);
    }
}
