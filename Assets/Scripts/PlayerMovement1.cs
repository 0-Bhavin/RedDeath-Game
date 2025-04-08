using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For stamina UI

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement1 : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject flashlight;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 4f;
    public float gravity = 12f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    // Torch system
    public float torchMaxDuration = 15f; 
    private float torchRemainingTime;
    private bool isFlashlightOn = false;
    private bool torchDepleted = false;
    public Text torchMessage;

    // Stamina system
    public float maxStamina = 10f; // Player can run for 10 seconds
    private float currentStamina;
    public float staminaRegenRate = 2f; // 2 stamina per second
    public Slider staminaBar; // UI bar for stamina (Optional)
    private bool isRunning = false;
    private bool canRun = true; // Controls if player can run
    private float staminaRegenDelay = 1.5f; // Wait 1.5 seconds before stamina starts regenerating
    private float timeSinceStoppedRunning = 0f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        flashlight.SetActive(false);
        torchRemainingTime = torchMaxDuration;
        currentStamina = maxStamina;

        if (torchMessage != null)
            torchMessage.gameObject.SetActive(false);

        if (staminaBar != null)
            staminaBar.maxValue = maxStamina;
    }

    void Update()
    {
        HandleMovement();
        HandleTorch();
        HandleStamina();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        isRunning = Input.GetKey(KeyCode.LeftShift) && canRun && canMove && currentStamina > 0;
        
        if (isRunning)
        {
            timeSinceStoppedRunning = 0f; // Reset stamina recovery delay
        }

        float curSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleTorch()
    {
        if (torchDepleted) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.SetActive(isFlashlightOn);
        }

        if (isFlashlightOn)
        {
            torchRemainingTime -= Time.deltaTime;
            if (torchRemainingTime <= 0)
            {
                torchRemainingTime = 0;
                isFlashlightOn = false;
                flashlight.SetActive(false);
                torchDepleted = true;

                if (torchMessage != null)
                {
                    torchMessage.gameObject.SetActive(true);
                    torchMessage.text = "Torch has run out!";
                }
            }
        }
    }

    void HandleStamina()
    {
        if (isRunning)
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canRun = false; // Player must wait before running again
            }
        }
        else
        {
            timeSinceStoppedRunning += Time.deltaTime;
            if (timeSinceStoppedRunning >= staminaRegenDelay) // Wait before regenerating
            {
                if (currentStamina < maxStamina)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    if (currentStamina >= maxStamina)
                    {
                        currentStamina = maxStamina;
                        canRun = true; // Player can run again
                    }
                }
            }
        }

        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }
    }
}
