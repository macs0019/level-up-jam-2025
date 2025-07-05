using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Obtener la dirección hacia la cámara
        Vector3 lookDirection = Camera.main.transform.position - transform.position;
        lookDirection.y = 0f;  // No tener en cuenta la altura

        if (lookDirection == Vector3.zero) return;

        // Calcular la rotación deseada hacia la cámara en el eje Y
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        // Aplicar la rotación solo en el eje Y
        transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

    }
}
