using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float inputThreshold = 0.1f;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private bool hasMoved = false;
    private float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    private CharacterController characterController;

    // Start is called before the first frame update
    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Set skinWidth to a small value to prevent aggressive collisions
        characterController.skinWidth = 0.01f;

        // Set initial camera rotation
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.eulerAngles.x;
    }

    private void Update()
    {
        // Check if a dialogue is playing, game is paused
        if (DialogueManager.GetInstance().dialogueIsPlaying || GameMenuManager.GetInstance().GameIsPaused)
            return;
            
        // Handle mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        cameraTransform.eulerAngles = new Vector3(pitch, yaw, 0f);
        
    }

    private void FixedUpdate()
    {
        // Check if a dialogue is playing, game is paused
        if (DialogueManager.GetInstance().dialogueIsPlaying || GameMenuManager.GetInstance().GameIsPaused)
            return;

        // Handle movement input
        Vector3 moveDirection = Vector3.zero;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontalInput) > inputThreshold)
        {
            moveDirection += cameraTransform.right * horizontalInput;
        }

        if (Mathf.Abs(verticalInput) > inputThreshold)
        {
            moveDirection += cameraTransform.forward * verticalInput;
        }

        moveDirection.y = 0f;
        moveDirection.Normalize();

        // Move the character
        if (moveDirection != Vector3.zero)
        {
            if (!hasMoved)
            {
                hasMoved = true;
            }

            Vector3 currentVelocity = characterController.velocity;

            if (currentVelocity.magnitude > 0)
            {
                Vector3 currentDirection = currentVelocity.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                if (Vector3.Dot(currentDirection, moveDirection) < 0)
                {
                    targetRotation = Quaternion.LookRotation(-moveDirection);
                }

                float rotationSpeed = Mathf.Clamp01(currentVelocity.magnitude / 5f);
                characterController.transform.rotation = Quaternion.Lerp(characterController.transform.rotation, targetRotation, rotationSpeed);
            }

            characterController.Move(moveDirection * speed * Time.deltaTime);
        }
        else
        {
            hasMoved = false;
        }
        // Calculate the gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Move the character based on the vertical velocity
        Vector3 motion = new Vector3(0, verticalVelocity, 0);
        characterController.Move(motion * Time.deltaTime);
        
    }
}