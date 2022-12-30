using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    //Creates a field to arrange player speed and sprint
    [Header("Player Movement")]
    public float playerSpeed = 1.9f;
    public float playerSprint = 3f;


    [Header("Player Health Things")]
    private float playerHealth = 120f;
    private float presentHealth;

    //Creates a field to input the MainCamera
    [Header("Player Script Cameras")]
    public Transform playerCamera;

    //Creates a field to input the Character Controller
    [Header("Player Animator and Gravity")]
    public CharacterController cC;
    public float gravity = -9.81f;
    public Animator animator;

    [Header("Player Jumping and Velocity")]
    public float jumpRange = 1f;
    Vector3 velocity;
    public float turnCalmTime = 0.1f;
    float turnCalmVelocity;
    public Transform surfaceCheck;
    bool onSurface;
    public float surfaceDistance = 0.2f;
    public LayerMask surfaceMask;
    public bool isJumping = false;
    public IEnumerator jumpCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        presentHealth = playerHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isJumping)
        {
            onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);
        }
        if(onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //print(onSurface);

        //gravity
        velocity.y += gravity * Time.deltaTime;
        cC.Move(velocity * Time.deltaTime);

        Jump();

        playerMove();

        Sprint();
    }

    //Lets the player move by using arrow keys or WASD
    void playerMove()
    {
        float horizontal_axis = Input.GetAxisRaw("Horizontal");
        float vertical_axis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

        if (direction.magnitude >= 0.1f)
        {
            //Setting the same parameters as the animation controller
            if(onSurface && !isJumping)
            {
                animator.SetBool("Walk", true);
            }
            
            animator.SetBool("Running", false);
            animator.SetBool("Idle", false);
            animator.SetBool("AimWalk", false);
            animator.SetBool("IdleAim", false);


            //This basically rotates the player model when moving to the sides and makes the rotation smoother.
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //This lets the player turn towards mouse location.
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cC.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Running", false);
            animator.SetBool("AimWalk", false);
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && onSurface)
        {

            isJumping = true;
            onSurface= false;
            animator.SetBool("Jumping", true);
            animator.SetBool("Walk", false);
            
            jumpCoroutine = WaitForJump();
            StartCoroutine(jumpCoroutine);

            velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
        }
    }


    IEnumerator WaitForJump()
    {
        yield return new WaitForSeconds(1);
        isJumping = false;
        animator.SetBool("Jumping", false);
    }


    //Lets the player move by using arrow keys or WASD
    void Sprint()
    {
        if(Input.GetButton("Sprint") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) && onSurface)
        {
            float horizontal_axis = Input.GetAxisRaw("Horizontal");
            float vertical_axis = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal_axis, 0f, vertical_axis).normalized;

            if (direction.magnitude >= 0.1f)
            {

                animator.SetBool("Running", true);
                animator.SetBool("Idle", false);
                animator.SetBool("Walk", false);
                animator.SetBool("IdleAim", false);


                //This basically rotates the player model when moving to the sides and makes the rotation smoother.
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnCalmVelocity, turnCalmTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //This lets the player turn towards mouse location.
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                cC.Move(moveDirection.normalized * playerSprint * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Idle", false);
                animator.SetBool("Walk", false);
            }
        }
    }

    public void playerHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        if(presentHealth <= 0) 
        {
            PlayerDie();
        }
    }

    private void PlayerDie()
    {
        Cursor.lockState = CursorLockMode.None;
        Object.Destroy(gameObject, 1.0f);
    }
}
