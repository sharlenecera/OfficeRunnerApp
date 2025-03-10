using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Make sure these two components are attached to the GameObject
// once this PlayerController script is attached
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float initialPlayerSpeed = 4f;
    [SerializeField]
    private float maximumPlayerSpeed = 30f;
    [SerializeField]
    private float playerSpeedIncreaseRate = .1f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float initialGravityValue = -9.81f;
    // need to set ^ as player stays in air for longer the faster they move,
    // so this needs to increase in proportion to player speed
    [SerializeField]
    private LayerMask groundLayer;
    // LayerMasks let you specify what layers you want to use

    private float playerSpeed;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;

    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private CharacterController controller;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
    }

    private void OnEnable() // called after Awake, when this script is enabled
    {
        // Subscribe to the events (start listening for them)
        turnAction.performed += PlayerTurn;
        slideAction.performed += PlayerSlide;
        jumpAction.performed += PlayerJump;
    }

    private void OnDisable() // called when this script is disabled
    {
        // Unsubscribe from the events (so stop listening for them)
        turnAction.performed -= PlayerTurn;
        slideAction.performed -= PlayerSlide;
        jumpAction.performed -= PlayerJump;
    }

    private void Start()
    {
        playerSpeed = initialPlayerSpeed;
        gravity = initialGravityValue;
    }

    private void PlayerTurn(InputAction.CallbackContext context)
    {

    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {

    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void Update()
    {
        controller.Move(transform.forward * playerSpeed * Time.deltaTime);

        if (IsGrounded() && playerVelocity.y < 0)
        {
            // if grounded and we have gravity, it might create glitching effect
            // this also acts as a way to reset gravity on every frame
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // making our own function as the character controller one is unreliable
    private bool IsGrounded(float length = .2f) // length of raycast that we want to cast downwards
    {   // raycast is a ray cast in a specific direction
        // Make 2 raycasts in case one hits the mini gap between tiles

        Vector3 raycastOriginFirst = transform.position; // starts at centre of player
        raycastOriginFirst.y -= controller.height / 2f; // move it to the bottom of the player
        raycastOriginFirst.y += .1f; // add an offset so that it does not go through the ground

        Vector3 raycastOriginSecond = raycastOriginFirst;
        // Make the raycasts the same distance away from origin
        raycastOriginFirst -= transform.forward * .2f;
        raycastOriginSecond += transform.forward * .2f;

        Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green,  2f); // 2 seconds
        Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red,  2f); // 2 seconds

        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || 
            Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
        {
            return true;
        }
        return false;
    }
}
