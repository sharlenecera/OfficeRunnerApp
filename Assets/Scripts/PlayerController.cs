using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace RunnerGame.Player {
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
    [SerializeField]
    private LayerMask turnLayer;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private LayerMask doorLayer;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip slideAnimationClip;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float scoreMultiplier = 294f;
    [SerializeField]
    public bool isGameOver = false;
    private bool reachedDoor = false;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;

    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;

    private CharacterController controller;

    private int slidingAnimationId;

    private bool sliding = false;
    private float score = 0;

    [SerializeField]
    private UnityEvent<Vector3> turnEvent;
    [SerializeField]
    private UnityEvent<int> gameOverEvent;
    [SerializeField]
    private UnityEvent<int> scoreUpdateEvent;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();

        slidingAnimationId = Animator.StringToHash("Sliding");

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
        Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
        if(!turnPosition.HasValue)
        {
            Debug.Log("Wrong turn");
            GameOver();
            return;
        }
        // Turn the player 90 degrees left or right depending on the input (-1 or 1) along the y-axis
        Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) *
            movementDirection;
        turnEvent.Invoke(targetDirection); // send targetDirection to SpawnManager
        Turn(context.ReadValue<float>(), turnPosition.Value);
    }

    // Return value is either Vector3 or null as it has a '?' at the end
    private Vector3? CheckTurn(float turnValue)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
        if(hitColliders.Length != 0)
        {
            Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
            TileType type = tile.type;
            if ((type == TileType.LEFT && turnValue == -1) ||
                (type == TileType.RIGHT && turnValue == 1) ||
                (type == TileType.SIDEWAYS)){
                return tile.pivot.position;
            }
        }
        return null;
    }

    private void Turn(float turnValue, Vector3 turnPosition)
    {
        // Keep the player at the same height
        Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
        // Disable controller to change position
        controller.enabled = false;
        transform.position = tempPlayerPosition;
        controller.enabled = true;

        // Rotate the player
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
        transform.rotation = targetRotation;
        movementDirection = transform.forward.normalized; // gives a vector of length 1 since we only want direction
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {
        if(!sliding && IsGrounded())
        {
            StartCoroutine(Slide());
        }
    }
    
    // Coroutine is a function that can pause execution until a certain condition is met
    private IEnumerator Slide()
    {
        sliding = true;

        // Shrink the collider (so it does not hit the obstacle when it slides)
        Vector3 originalControllerCenter = controller.center;
        Vector3 newControllerCenter = originalControllerCenter;
        controller.height /= 2;
        newControllerCenter.y -= controller.height / 2;
        controller.center = newControllerCenter;

        // Play the sliding animation
        animator.Play(slidingAnimationId);
        yield return new WaitForSeconds(slideAnimationClip.length / animator.speed);

        // Set the character controller collider back to normal after sliding
        controller.height *= 2;
        controller.center = originalControllerCenter;

        sliding = false;
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
        if(isGameOver) // shoots raycast with length 20
        {
            return;
        }
        else if(!IsGrounded(20f))
        {
            Debug.Log("Fall off tile");
            GameOver();
            return;
        }

        // Score functionality

        score += scoreMultiplier * Time.deltaTime;
        scoreUpdateEvent.Invoke((int)score);

        // make sure the CharacterController is enabled before calling Move to prevent error
        if (controller.enabled)
        {
            // delta time is time between frames
            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (IsGrounded() && playerVelocity.y < 0)
            {
                // if grounded and we have gravity, it might create glitching effect
                // this also acts as a way to reset gravity on every frame
                playerVelocity.y = 0f;
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Player Acceleration Functionality
            if(playerSpeed < maximumPlayerSpeed)
            {
                playerSpeed += Time.deltaTime * playerSpeedIncreaseRate;
                gravity = initialGravityValue - playerSpeed;

                if(animator.speed < 1.25f)
                {
                    animator.speed += (1/playerSpeed) * Time.deltaTime;
                }
            }
        }        
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

        // Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green,  2f); // 2 seconds
        // Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red,  2f); // 2 seconds

        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || 
            Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
        {
            return true;
        }
        return false;
    }

    public void GameOver(bool timerFinished=false)
    {
        Time.timeScale = 0; // alternative to below
        // gameObject.SetActive(false);
        int finalScore;
        float elapsedTime = Time.time;
        isGameOver = true;
        Debug.Log("Game Over");

        if (reachedDoor) // won the game
        {    
            Debug.Log("elapsed time: " + elapsedTime);
            Debug.Log("score before calc: " + score);

            // Calculate final score
            finalScore = Mathf.RoundToInt(score / elapsedTime);
            Debug.Log("Final Score: " + finalScore);
            UnlockNewLevel();
        }
        else
        {
            finalScore = 0;
            Debug.Log("Failed so score = 0");
        }
        gameOverEvent.Invoke(finalScore);

        PlayerPrefs.SetInt("GameOver", 1); // Set the game over state
        PlayerPrefs.Save();
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // checking if the object that the player collided with is in the obstacle layer
        if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
        {
            Debug.Log("Hit obstacle");
            GameOver();
        }
        else if (((1 << hit.collider.gameObject.layer) & doorLayer) != 0)
        {
            Debug.Log("Hit door");
            reachedDoor = true;
            GameOver();
        }
    }

    void UnlockNewLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
}