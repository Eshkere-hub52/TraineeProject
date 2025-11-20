using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
   
    public CharacterController controller;
    public float speed = 12f;
    public float sprintSpeed = 25f;
    public float gravity = -9.81f ;
    public float jumpHeight = 5f;

    [Header("Перевірка Землі")]
    public Transform groundCheck;
    public float groundDistance = 1f;
    public LayerMask groundMask;

    
    [Header("Налаштування Витривалості")]
    public float maxStamina = 100f;
    public float staminaDrain = 25f;
    public float staminaRegen = 15f;
    public float staminaRegenThreshold = 30f;
    public Image staminaBar;
    public CanvasGroup staminaCanvasGroup;
    public float uiFadeSpeed = 5f;

    
    [Header("Хитання Камери")]
    [SerializeField] private Camera _playerCamera;
    public float bobFrequency = 2.0f;
    public float bobAmplitude = 0.05f;
    public float sprintBobMultiplier = 1.5f;
    private Vector3 _cameraOriginalLocalPosition;
    private float _walkTime;

    
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting = false;
    private float currentStamina;
    private bool isExhausted = false;

    void Start()
    {
        
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        
        if (_playerCamera == null)
        {
            _playerCamera = GetComponentInChildren<Camera>();
            if (_playerCamera == null)
            {
                Debug.LogError("Камера не знайдена в дочірніх об'єктах!");
            }
        }

        if (_playerCamera != null)
        {
            _cameraOriginalLocalPosition = _playerCamera.transform.localPosition;
        }

        currentStamina = maxStamina;
        if (staminaCanvasGroup != null)
        {
            staminaCanvasGroup.alpha = 0;
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        HandleStamina(z);
        HandleMovement(x, z);
        UpdateStaminaUI();

       
        HandleHeadBob(x, z);
    }

    private void OnDrawGizmos()
    {
        // Це спрацює лише у вікні Scene, коли гра запущена
        if (groundCheck != null)
        {
            // Візуалізація: Зелений, якщо на землі, Червоний, якщо ні
            Gizmos.color = isGrounded ? Color.green : Color.red;

            // Малюємо прозору сферу, яка показує зону перевірки
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
    private void HandleMovement(float x, float z)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


       
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        
        float currentSpeed = speed;
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }

     
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

      
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

       
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

  
    private void HandleStamina(float z)
    {
       
        if (isExhausted)
        {
            if (currentStamina >= staminaRegenThreshold)
            {
                isExhausted = false;
            }
        }


        bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) && isGrounded && z > 0 && !isExhausted;

        if (isTryingToSprint)
        {
            if (currentStamina > 0)
            {
                isSprinting = true;
              
                currentStamina -= staminaDrain * Time.deltaTime;

                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    isExhausted = true;
                }
            }
            else
            {
                isSprinting = false;
                isExhausted = true; 
            }
        }
        else
        {
            isSprinting = false;
        }

        if (!isSprinting && currentStamina < maxStamina && isGrounded)
        {
            currentStamina += staminaRegen * Time.deltaTime;
        }

      
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

 
    void UpdateStaminaUI()
    {
        if (staminaBar == null || staminaCanvasGroup == null)
            return;

      
        staminaBar.fillAmount = currentStamina / maxStamina;

       
        float targetAlpha = (isSprinting || currentStamina < maxStamina || isExhausted) ? 1f : 0f;
        staminaCanvasGroup.alpha = Mathf.MoveTowards(staminaCanvasGroup.alpha, targetAlpha, uiFadeSpeed * Time.deltaTime);
    }


    private void HandleHeadBob(float x, float z)
    {
        if (_playerCamera == null) return;

    
        Vector3 flatVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);

    
        if (flatVelocity.magnitude > 0.1f && isGrounded)
        {
            _walkTime += Time.deltaTime;


            float currentFrequency = bobFrequency;
            float currentAmplitude = bobAmplitude;

            if (isSprinting)
            {
                currentFrequency *= sprintBobMultiplier;
                currentAmplitude *= sprintBobMultiplier;
            }


            float verticalBob = Mathf.Sin(_walkTime * currentFrequency) * currentAmplitude;


            float horizontalBob = Mathf.Cos(_walkTime * (currentFrequency * 0.5f)) * currentAmplitude * 0.5f;


            _playerCamera.transform.localPosition = new Vector3(
                _cameraOriginalLocalPosition.x + horizontalBob,
                _cameraOriginalLocalPosition.y + verticalBob,
                _cameraOriginalLocalPosition.z
            );
        }
        else
        {
            _walkTime = 0f;

          
            _playerCamera.transform.localPosition = Vector3.Lerp(
                _playerCamera.transform.localPosition,
                _cameraOriginalLocalPosition,
                Time.deltaTime * (bobFrequency * 2) 
            );
        }
    }
}