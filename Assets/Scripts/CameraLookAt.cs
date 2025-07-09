using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private float minFieldOfView = 45.0f; // Minimum field of view when far

    [SerializeField]
    private float maxFieldOfView = 90.0f; // Maximum field of view when close

    [SerializeField]
    private float fieldOfViewSensitivity = 10.0f; // Sensitivity for field of view adjustment

    [SerializeField]
    private float minDistance = 5.0f; // Distance at which the field of view is minimum

    [SerializeField]
    private float maxDistance = 1.0f; // Distance at which the field of view is maximum

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null && GetComponent<Camera>() != null)
        {
            // Use a fixed reference position for the camera to avoid zoom effect
            Vector3 referencePosition = transform.parent != null ? transform.parent.position : transform.position;
            Vector3 oppositeDirection = (referencePosition - playerTransform.position).normalized;

            // Update the rotation of the camera to point away from the player
            Quaternion targetRotation = Quaternion.LookRotation(oppositeDirection, Vector3.up);

            // Convert the target rotation to Euler angles
            Vector3 eulerRotation = targetRotation.eulerAngles;

            // Freeze the Y rotation to 0
            eulerRotation.x = 0;

            // Apply the modified rotation back to the transform
            transform.rotation = Quaternion.Euler(eulerRotation);

            // Adjust the field of view based on the distance to the player
            float distanceToPlayer = Vector3.Distance(referencePosition, playerTransform.position);
            float normalizedDistance = Mathf.InverseLerp(maxDistance, minDistance, distanceToPlayer); // Adjust normalization
            float targetFieldOfView = Mathf.Lerp(maxFieldOfView, minFieldOfView, normalizedDistance);

            // Directly set the field of view to ensure it changes
            GetComponent<Camera>().fieldOfView = targetFieldOfView;
        }
    }
}
