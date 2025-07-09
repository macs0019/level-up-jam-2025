using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WindowCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject window;

    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private int iterations = 7;

    private RenderTexture renderTexture;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        if (window == null)
        {
            Debug.LogError("Window GameObject is not assigned.");
            return;
        }

        if (targetCamera == null)
        {
            Debug.LogError("Target Camera is not assigned.");
            return;
        }

        // Create a RenderTexture to hold the camera's output
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        targetCamera.targetTexture = renderTexture;

        // Assign the RenderTexture to the window's material
        Renderer windowRenderer = window.GetComponent<Renderer>();
        if (windowRenderer != null)
        {
            windowRenderer.material.mainTexture = renderTexture;
        }
        else
        {
            Debug.LogError("Window does not have a Renderer component.");
        }
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= UpdateCamera;

        // Clean up the RenderTexture when the script is disabled
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }

    private void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (!window.activeInHierarchy)
        {
            return;
        }

        targetCamera.targetTexture = renderTexture;
        for (int i = iterations - 1; i >= 0; --i)
        {
            RenderCamera(i, SRC);
        }
    }

    private void RenderCamera(int iterationID, ScriptableRenderContext SRC)
    {
        Transform cameraTransform = targetCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the window.
            Vector3 relativePos = transform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = window.transform.TransformPoint(relativePos);

            // Rotate the camera to look through the window.
            Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = window.transform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-window.transform.forward, window.transform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(targetCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        targetCamera.projectionMatrix = newMatrix;

        // Apply a 180-degree rotation to the camera
        targetCamera.transform.rotation *= Quaternion.Euler(0, 180, 0);

        // Apply a Z offset of 5 to the camera
        targetCamera.transform.position += targetCamera.transform.forward * 2f;

        // Debugging: Check RenderTexture and Camera
        Debug.Log("RenderTexture width: " + renderTexture.width + ", height: " + renderTexture.height);
        Debug.Log("TargetCamera position: " + targetCamera.transform.position + ", rotation: " + targetCamera.transform.rotation);

        // Use URP's ScriptableRenderContext to render the camera.
        SRC.ExecuteCommandBuffer(new CommandBuffer());
    }
}
