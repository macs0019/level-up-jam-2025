using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5.0f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100.0f;
    public Transform cameraTransform;
    public Transform armsCamTransform;

    [Header("Visual")]
    public Transform leftArm;
    public Transform rightArm;

    [Header("Arm Bob & Shake Settings")]
    public float armBobAmplitude = 0.1f;
    public float armBobDuration = 0.5f;
    public float armShakeDuration = 0.3f;
    public Vector3 armShakeStrength = new Vector3(0.1f, 0.1f, 0f);
    public int armShakeVibrato = 10;
    public float armShakeElasticity = 0.5f;

    private float rotationX = 0f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;

    private bool isMoving = false;
    private Vector3 leftArmStartPos;
    private Vector3 rightArmStartPos;
    private Tween leftBobTween;
    private Tween rightBobTween;

    // Guarda la rotación original de la cámara
    private Quaternion _originalCamRot;
    private Quaternion _originalArmsCamRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("No Rigidbody found on player object.");

        // Guardar posiciones iniciales
        if (leftArm != null) leftArmStartPos = leftArm.localPosition;
        if (rightArm != null) rightArmStartPos = rightArm.localPosition;

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
        // Rotaci�n horizontal
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // Rotaci�n vertical
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        if (armsCamTransform != null)
            armsCamTransform.localRotation = Quaternion.Euler(-rotationX, 0f, 0f);

        // Gestionar bob vs shake
        if (moveInput.sqrMagnitude > 0.01f)
            StartArmBob();
        else
            StopArmBobAndShake();
    }

    void FixedUpdate()
    {
        // Movimiento del cuerpo
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void StartArmBob()
    {
        if (isMoving) return;

        isMoving = true;

        // Matar tweens anteriores y resetear
        leftArm.DOKill(true);
        leftArm.localPosition = leftArmStartPos;
 
        rightArm.DOKill(true);
        rightArm.localPosition = rightArmStartPos;

        // Iniciar bob en bucle
        leftBobTween = leftArm
            .DOLocalMoveY(leftArmStartPos.y - armBobAmplitude, armBobDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        rightBobTween = rightArm
            .DOLocalMoveY(rightArmStartPos.y - armBobAmplitude, armBobDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopArmBobAndShake()
    {
        if (!isMoving) return;

        isMoving = false;

        // Matar bob tweens
        leftBobTween?.Kill();
        rightBobTween?.Kill();

        leftArm.DOKill(true);
        rightArm.DOKill(true);

        // Volver suavemente a posici�n inicial
        leftArm.DOLocalMove(leftArmStartPos, armShakeDuration * 0.5f).SetEase(Ease.OutQuad);
        rightArm.DOLocalMove(rightArmStartPos, armShakeDuration * 0.5f).SetEase(Ease.OutQuad);

        // Aplicar shake ligero
        leftArm.DOPunchPosition(armShakeStrength, armShakeDuration, armShakeVibrato, armShakeElasticity);
        rightArm.DOPunchPosition(armShakeStrength, armShakeDuration, armShakeVibrato, armShakeElasticity);
    }

    public void StartInteractAnimation(FoodSelector foodSelector)
    {
        // Guardamos la rotación actual
        _originalCamRot = cameraTransform.rotation;
        _originalArmsCamRot = armsCamTransform.rotation;

        leftArm.DOKill(true);
        rightArm.DOKill(true);

        leftArm.DOLocalMoveY(-0.3f, 0.2f);
        rightArm.DOLocalMoveX(-0.5f, 0.2f);

        // Look at the target balloon
        Quaternion targetRot = Quaternion.LookRotation(foodSelector.speechBalloon.transform.position - cameraTransform.position, Vector3.up);
        Vector3 eulerAngles = new Vector3(-targetRot.eulerAngles.x, 0, 0);

        armsCamTransform.DOLocalRotate(eulerAngles, 0.3f);
        cameraTransform.DORotateQuaternion(targetRot, 0.3f);
    }

    public void EndInteractAnimation(Action onComplete = null)
    {
        leftArm.DOKill(true);
        rightArm.DOKill(true);

        leftArm.DOLocalMoveY(0f, 0.2f);
        rightArm.DOLocalMoveX(1.8f, 0.2f);

        armsCamTransform.DORotateQuaternion(_originalArmsCamRot, 0.3f);
        cameraTransform.DORotateQuaternion(_originalCamRot, 0.3f)
        .OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void StartWritingAnimation()
    {
        // Mata cualquier tween activo en rightArm
        rightArm.DOKill(complete: true);

        rightArm.DOPunchPosition(armShakeStrength / 2, armShakeDuration / 2, armShakeVibrato / 2, armShakeElasticity).SetAutoKill();
    }
}
