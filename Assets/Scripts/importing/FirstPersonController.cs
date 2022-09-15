using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstPersonController : MonoBehaviour
{
    public bool canMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(SprintKey) && !isCrouching;
    // !isCrouching bunu ben ekledim hata olursa bak
    private bool ShouldJump => Input.GetKeyDown(JumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(CrouchKey) && !DuringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHaedBob = true;
    [SerializeField] private bool WillSlideOnSlopes = true;
    [SerializeField] private bool CanZoom = true;
    [SerializeField] private bool CanInteract = true;
    [SerializeField] private bool useFootSteps = true;
    [SerializeField] private bool useStamina = true;

    [Header("Controls")]
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode CrouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode ZommKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode InteractKey = KeyCode.E;

    [Header("Movement Parameters")]
    [SerializeField] private float WalkSpeed = 3.0f;
    [SerializeField] private float SprintSpeed = 6.0f;
    [SerializeField] private float CrouchSpeed = 1.5f;
    [SerializeField] private float SlopeSpeed = 8f;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float LookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float LookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float UpperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float LowerLookLimit = 80.0f;

    [Header("Healt Parameters")]
    [SerializeField] private float maxHealt = 100f;
    [SerializeField] private float timeBeforeRegenStarts = 3f;
    [SerializeField] private float healtValueIncrement = 1f;
    [SerializeField] private float healtTimeIncrement = 0.1f;
    private float currentHealt;
    private Coroutine regenratingHealty;
    public static Action<float> onTakeDamage;
    public static Action<float> onDamage;
    public static Action<float> onHeal;

    [Header("Stamina Parameters")]
    [SerializeField] private float MaxStamina = 100;
    [SerializeField] private float StaminaUseMulipler = 15;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 5;
    [SerializeField] private float StaminaValueIncrement = 2;
    [SerializeField] private float StaminaTimeIncrement = 0.1f;
    private float currentStamina;
    private Coroutine regenratingStamina;
    public static Action<float> onStaminaChange;
    [SerializeField] private AudioClip[] HardBreathing = default;
    private float BreathingStepTimer = 0;
    private float BreathingSpeed = 0.8f;

    [Header("Jumping Parameters")]
    [SerializeField] private float JumpForce = 8.0f;
    [SerializeField] private float Gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float CroucHeight = 0.5f;
    [SerializeField] private float StandHeight = 2f;
    [SerializeField] private float TimeToCrouch = 0.25f;
    [SerializeField] private Vector3 CrouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 StandingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool DuringCrouchAnimation;


    [Header("HeadBob Parameters")]
    [SerializeField] private float WalkBobSpeed = 14f;
    [SerializeField] private float WalkBobAmount = 0.05f;
    [SerializeField] private float SprintBobSpeed = 18f;
    [SerializeField] private float SprintBobAmount = 0.1f;
    [SerializeField] private float CrouchBobSpeed = 8f;
    [SerializeField] private float CrouchBobAmount = 0.025f;
    private float defultYPos = 0f;
    private float timer;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float ZoomFov = 30f;
    private float defualtFov;
    private Coroutine zoomRoutine;

    [Header("Foot Steps Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float CrouchStepMultipler = 1.5f;
    [SerializeField] private float SprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource audioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] MetalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] NormalClips = default;
    private float footStepTimer = 0;
    private float GetCurrentOffset => isCrouching ? (baseStepSpeed * CrouchStepMultipler) : isSprinting ? (baseStepSpeed * SprintStepMultipler) : baseStepSpeed;




    //SLÝDÝNG PARAMETERS ///editorde degistirilecek bir sey olmadýgýndan boyle yaptýk
    private Vector3 hitPontNormal;//ustunde bulundugumuz yuzey

    private bool isSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopehit, 2f))
            {
                hitPontNormal = slopehit.normal;
                return Vector3.Angle(hitPontNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }

        }
    }

    [Header("Interaction")]
    [SerializeField] private Vector3 interactonRayPoint = default;
    [SerializeField] private float interactonDÝstance = default;
    [SerializeField] private LayerMask interactonLayer = default;
    private Interactable currentInteractable;


    private Camera PlayerCamera;
    private CharacterController characterController;

    private Vector3 moveDirecton;
    private Vector2 currentInput;

    private float RotationX = 0;


    
    public static FirstPersonController instance;//Singleton yapiyoz


    private void OnEnable()
    {
        onTakeDamage += ApplyDamage;
    }

    private void OnDisable()
    {
        onTakeDamage -= ApplyDamage;
    }

    private void Awake()
    {
        instance = this;

        PlayerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        defultYPos = PlayerCamera.transform.localPosition.y;
        defualtFov = PlayerCamera.fieldOfView;
        currentHealt = maxHealt;
        currentStamina = MaxStamina;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
                HandleJump();

            if (canCrouch)
                HandleCrouch();

            if (canUseHaedBob)
                HandleHeadBob();

            if (CanZoom)
                HandleZoom();

            if (useFootSteps)
                HandleFootSteps();

            if (CanInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            if (useStamina)
                HandleStamina();

            ApplyFinalyMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? CrouchSpeed : isSprinting ? SprintSpeed : WalkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? CrouchSpeed : isSprinting ? SprintSpeed : WalkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirecton.y;

        moveDirecton = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);

        moveDirecton.y = moveDirectionY;



    }

    private void HandleMouseLook()
    {
        RotationX -= Input.GetAxis("Mouse Y") * LookSpeedY;
        RotationX = Mathf.Clamp(RotationX, -UpperLookLimit, LowerLookLimit);
        PlayerCamera.transform.localRotation = Quaternion.Euler(RotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeedX, 0);

    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirecton.y = JumpForce;
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirecton.x) > 0.1f || Mathf.Abs(moveDirecton.z) > 0.1f)//bunun yerine magnutide olur mu acaba
        {
            timer += Time.deltaTime * (isCrouching ? CrouchBobSpeed : isSprinting ? SprintBobSpeed : WalkBobSpeed);
            PlayerCamera.transform.localPosition = new Vector3(
                PlayerCamera.transform.localPosition.x,
                defultYPos + Mathf.Sin(timer) * (isCrouching ? CrouchBobAmount : isSprinting ? SprintBobAmount : WalkBobAmount),
                PlayerCamera.transform.localPosition.x);
        }

    }

    private void HandleZoom()
    {
        if (Input.GetKeyDown(ZommKey))
        {
            if (zoomRoutine != null)//null ise calismiyordur
            {
                StopCoroutine(zoomRoutine);//calisiyorsa durduruyoruz
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if (Input.GetKeyUp(ZommKey))//biraktiginda
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(PlayerCamera.ViewportPointToRay(interactonRayPoint), out RaycastHit hit, interactonDÝstance))
        {
            if (hit.collider.gameObject.layer == 10 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);
                if (currentInteractable)
                    currentInteractable.onFocus();


            }
        }
        else if (currentInteractable)
        {
            currentInteractable.onLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(InteractKey) && currentInteractable != null &&
            Physics.Raycast(PlayerCamera.ViewportPointToRay(interactonRayPoint), out RaycastHit hit, interactonDÝstance, interactonLayer))
        {
            currentInteractable.onInteract();
        }
    }

    private void HandleFootSteps()
    {
        if (!characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footStepTimer -= Time.deltaTime;

        if (footStepTimer <= 0)
        {
            if (Physics.Raycast(PlayerCamera.transform.position, Vector3.down, out RaycastHit hit, 3f))
            {
                switch (hit.collider.tag)
                {
                    case "FootSteps/Grass":
                        audioSource.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
                        break;
                    case "FootSteps/Metal":
                        audioSource.PlayOneShot(MetalClips[UnityEngine.Random.Range(0, MetalClips.Length - 1)]);
                        break;
                    case "FootSteps/Wood":
                        audioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length - 1)]);
                        break;

                    default:
                        audioSource.PlayOneShot(NormalClips[UnityEngine.Random.Range(0, NormalClips.Length - 1)]);
                        break;
                }
            }

            footStepTimer = GetCurrentOffset;
        }

    }

    private void ApplyDamage(float dmg)
    {
        currentHealt -= dmg;
        onDamage?.Invoke(currentHealt);

        if (currentHealt <= 0)
            KillPlayer();
        else if (regenratingHealty != null)
            StopCoroutine(regenratingHealty);

        regenratingHealty = StartCoroutine(RegenerateHealth());

    }

    private void KillPlayer()
    {
        currentHealt = 0;

        if (regenratingHealty != null)
            StopCoroutine(RegenerateHealth());

        print("Dead");
    }

    private void HandleStamina()
    {
        if (isSprinting && currentInput != Vector2.zero)
        {
            if (regenratingStamina != null)//yenileme yapiliyorsa duruyo
            {
                StopCoroutine(regenratingStamina);
                regenratingStamina = null;
            }

            currentStamina -= StaminaUseMulipler * Time.deltaTime;

            if (currentStamina < 0)
                currentStamina = 0;

            onStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
                canSprint = false;


        }

        if (!isSprinting && currentStamina < MaxStamina && regenratingStamina == null)
        {
            regenratingStamina = StartCoroutine(RegenerateStamina());
        }



        if (currentStamina <= 10)
        {
            BreathingStepTimer -= Time.deltaTime;

            if (BreathingStepTimer <= 0)
            {
                audioSource.PlayOneShot(HardBreathing[UnityEngine.Random.Range(0, HardBreathing.Length - 1)]);
                BreathingStepTimer = BreathingSpeed;
            }
        }

    }

    private void ApplyFinalyMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirecton.y -= Gravity * Time.deltaTime;//en yakin yuzeye ceker
        }

        if (WillSlideOnSlopes && isSliding)
            moveDirecton += new Vector3(hitPontNormal.x, -hitPontNormal.y, hitPontNormal.z) * SlopeSpeed;

        characterController.Move(moveDirecton * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        //tavani fark edebilmek icin ///yoksa kafamiz icine giriyo

        if (isCrouching && Physics.Raycast(PlayerCamera.transform.position, Vector3.up, 1f))
            yield break;


        DuringCrouchAnimation = true;

        float timeElapsed = 0f;
        float targetHeight = isCrouching ? StandHeight : CroucHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? StandingCenter : CrouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < TimeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / TimeToCrouch);
            //Saniyenin 4de 1 kadar surede
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / TimeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //Yukarda minik aciklar veriyor o yuzden net olsun diye buraya yazdik
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        DuringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFov = isEnter ? ZoomFov : defualtFov;
        float staringFov = PlayerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom)
        {
            PlayerCamera.fieldOfView = Mathf.Lerp(staringFov, targetFov, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        PlayerCamera.fieldOfView = targetFov;
        zoomRoutine = null;

    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(healtTimeIncrement);

        while (currentHealt < maxHealt)
        {
            currentHealt += healtValueIncrement;
            if (currentHealt > maxHealt)
                currentHealt = maxHealt;


            onHeal?.Invoke(currentHealt);
            yield return timeToWait;
        }

        regenratingHealty = null;
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timetoWait = new WaitForSeconds(StaminaTimeIncrement);

        while (currentStamina < MaxStamina)
        {
            if (currentStamina > 0)
                canSprint = true;

            currentStamina += StaminaValueIncrement;

            if (currentStamina > MaxStamina)
                currentStamina = MaxStamina;

            onStaminaChange?.Invoke(currentStamina);

            yield return timetoWait;
        }

        regenratingStamina = null;
    }

}
