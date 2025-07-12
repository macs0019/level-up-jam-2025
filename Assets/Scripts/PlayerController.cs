using System;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Aviss;

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
    public SpriteRenderer blackScreenOverlay;

    [Header("Arm Bob & Shake Settings")]
    public float armBobAmplitude = 0.1f;
    public float armBobDuration = 0.5f;
    public float armShakeDuration = 0.3f;
    public Vector3 armShakeStrength = new Vector3(0.1f, 0.1f, 0f);
    public int armShakeVibrato = 10;
    public float armShakeElasticity = 0.5f;

    private float rotationX, rotationY = 0f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;

    private bool isMoving = false;
    private Vector3 leftArmStartPos;
    private Vector3 rightArmStartPos;
    private Tween leftBobTween;
    private Tween rightBobTween;

    private bool stopped = true;
    private float stepCooldown = 0f; // Temporizador para controlar el intervalo entre pasos
    private const float stepInterval = 0.5f; // Intervalo de tiempo entre pasos

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
        if (GameManager.Instance.IsNamingFood) return;

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Vertical
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Horizontal
        rotationY += mouseX;

        // Aplica solo a la cámara y las armas
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        if (armsCamTransform != null)
            // armsCamTransform.localRotation = Quaternion.Euler(-rotationX, rotationY, 0f);

            // Gestionar bob vs shake
            if (moveInput.sqrMagnitude > 0.01f)
                StartArmBob();
            else
                StopArmBobAndShake();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsNamingFood) return;

        // Actualizar el temporizador
        stepCooldown += Time.fixedDeltaTime;

        // Obtén los ejes de la cámara en horizontal
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Movimiento relativo a la cámara
        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;
        bool isMunchoMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMunchoMoving && stepCooldown >= stepInterval)
        {
            AudioController.Instance.Play("Muncho Walk");
            stepCooldown = 0f; // Reiniciar el temporizador
        }
        else if (!isMunchoMoving)
        {
            AudioController.Instance.Stop("Muncho Walk");
        }

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

        // Anular fuerzas externas (si realmente quieres 'locked' físico)
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public Vector3 GetForwardVector()
    {
        return cameraTransform.forward;
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
        leftArm.DOKill(true);
        rightArm.DOKill(true);
        blackScreenOverlay.DOKill(true);

        leftArm.DOLocalMoveY(-0.3f, 0.2f);
        rightArm.DOLocalMoveX(-0.5f, 0.2f);
        blackScreenOverlay.DOFade(0.8f, 0.2f).SetAutoKill();

        // Look at the target balloon
        Vector3 targetPos = cameraTransform.position
                          + cameraTransform.forward * 3f;

        Quaternion targetRot = Quaternion.LookRotation(cameraTransform.forward);

        // Lanza los tweens
        foodSelector.ShowFoodAnimation(targetPos, targetRot);
    }

    public void EndInteractAnimation(Action onComplete = null)
    {
        leftArm.DOKill(true);
        rightArm.DOKill(true);
        blackScreenOverlay.DOKill(true);

        blackScreenOverlay.DOFade(0f, 0.2f).SetAutoKill();
        leftArm.DOLocalMoveY(0f, 0.2f);
        rightArm.DOLocalMoveX(1.8f, 0.2f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void StartWritingAnimation()
    {
        // Mata cualquier tween activo en rightArm
        rightArm.DOKill(complete: true);


        AudioController.Instance.UnPause("Writting");


        rightArm.DOPunchPosition(armShakeStrength / 2, armShakeDuration / 2, armShakeVibrato / 2, armShakeElasticity).SetAutoKill().OnComplete(() =>
        {
            AudioController.Instance.Pause("Writting");
        });
    }

    public void LookAtBossAnimation(Action onComplete = null)
    {
        leftArm.DOKill(true);
        rightArm.DOKill(true);
        cameraTransform.DOKill(true);

        Sequence restartSeq = DOTween.Sequence();

        restartSeq
            .Join(this.transform.DOMove(new Vector3(0.4f, -1.3f, 0.3f), 1f))
            .Join(cameraTransform.DORotateQuaternion(new Quaternion(-0.0203461256f, -0.968669772f, -0.0847476646f, 0.232557163f), 1f))
            .Join(leftArm.DOLocalMove(new Vector3(1, -0.3f, 2.4f), 0.4f))
            .Join(rightArm.DOLocalMove(new Vector3(2.4f, -0.1f, 2.4f), 0.4f))
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }

    public void EndNamingAnimation(Action onComplete = null)
    {
        leftArm.DOKill(true);
        rightArm.DOKill(true);
        cameraTransform.DOKill(true);

        leftArm.DOLocalMove(leftArmStartPos, 0.2f);
        rightArm.DOLocalMove(rightArmStartPos, 0.2f);

        cameraTransform.DORotateQuaternion(Quaternion.identity, 0.5f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void StartShakeAnimation()
    {
        transform.DOKill(true);

        transform.DOShakePosition(
            duration: 0.8f,
            strength: new Vector3(1.5f, 1.5f, 0f),
            vibrato: 15,
            randomness: 90f,
            snapping: false,
            fadeOut: true
        );
    }
}
