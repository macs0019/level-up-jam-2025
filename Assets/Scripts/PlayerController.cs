using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5.0f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100.0f;
    public Transform cameraTransform; // Assign in Inspector

    private float rotationX = 0.0f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("No Rigidbody found on player object.");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue context)
    {
        moveInput = context.Get<Vector2>();
    }

    public void OnLook(InputValue context)
    {
        lookInput = context.Get<Vector2>();
    }

    void Update()
    {
        // Horizontal rotation (rotate player body)
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation (rotate camera up/down)
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
}
