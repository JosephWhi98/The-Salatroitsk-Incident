using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(CharacterController))]

public class PlayerController : Singleton<PlayerController>
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public float stamina = 1f; 


    [HideInInspector]
    public bool canMove = true;
    public bool isRunning;

    public Vector2 lookInput;
    public Vector2 moveInput;

    public bool jump;

    public AudioSource breathing;

    public NavMeshObstacle obsticle;  

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rotationX = playerCamera.transform.localRotation.x;
    }

    void LateUpdate()
    {
        breathing.volume = 1 - stamina;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * moveInput.y : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * moveInput.x : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (jump && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (isRunning && moveDirection.magnitude > 0)
        {
            stamina -= Time.deltaTime / 2f;

            if (stamina <= 0)
                isRunning = false; 
        }
        else
        {
            stamina += Time.deltaTime / 2;
        }

        stamina = Mathf.Clamp01(stamina);

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && !GameManager.Instance.GameOver)
        {
            rotationX += -lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }
    }


    public void ResetCamera()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}