using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

    //Declaring Constants
    private string WALK = "Player_Walk", RUN = "Player_Run", CROUCH = "Player_Crouch", JUMP = "Player_Jump", IDLE = "Player_Idle", MELEE = "Player_Melee_Attack";
    private string GROUND_TAG = "Ground";

    //Declaring Variables
    [SerializeField]
    private float moveForce, jumpForce = 10f;
    private bool isGrounded, isJumpPressed, isRunning, isCrouch, isAttackPressed;
    internal bool isAttacking = false;
    internal bool hasKey = false;
    private float horizontal;
    public int health = 100, lives = 2;
    private string currentState;
    private float attackDelay;


    //Declaring components
    private Rigidbody2D myBody;
    private Transform myTransform;
    private SpriteRenderer sr;
    internal Animator anim;
    public TMPController tMPController;
    public LevelController levelController;


    // Start, Awake, Update etc.
    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
    }

    void Start()
    {
        //code
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
            moveForce = 8f;
            jumpForce = 10f;
        }
        else
        {
            isRunning = false;
            moveForce = 4f;
            jumpForce = 8f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !UIController.isGamePaused)
        {
            isAttackPressed = true;
        }

        if (Input.GetKey(KeyCode.C))
        {
            isCrouch = true;
        }
        else
        {
            isCrouch = false;
        }

        if (!isCrouch && !isAttacking)
        {
            PlayerMovementKeyboard(horizontal);
        }

    }

    private void FixedUpdate()
    {
        PlayerWalk();
        PlayerRun();
        PlayerJump();
        PlayerCrouch();
        PlayerMeleeAttack();
    }

    private void LateUpdate()
    {
        CheckWhereToFace();
    }

    // Collision check
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            isGrounded = true;
        }
    }

    // Player Movement & Animations
    void PlayerMovementKeyboard(float horizontal)
    {
        transform.position += new Vector3(horizontal, 0f, 0f) * Time.deltaTime * moveForce;
    }

    void PlayerWalk()
    {
        if (isGrounded && !isCrouch && !isAttacking)
        {
            if (horizontal == 0)
            {
                ChangeAnimationState(IDLE);
            }
            else if (moveForce == 4f)
            {
                ChangeAnimationState(WALK);
            }
        }
    }

    void PlayerRun()
    {
        if (isGrounded && !isCrouch && !isAttacking)
        {
            if (horizontal == 0)
            {
                ChangeAnimationState(IDLE);
            }
            else if (horizontal != 0 && isRunning && moveForce == 8f)
            {
                ChangeAnimationState(RUN);
            }
        }
    }

    void PlayerJump()
    {
        if (isJumpPressed && isGrounded)
        {
            myBody.velocity = new Vector2(myBody.velocity.x, jumpForce);
            isJumpPressed = false;
            isGrounded = false;
            ChangeAnimationState(JUMP);
        }
    }

    void PlayerCrouch()
    {
        if (isCrouch && isGrounded)
        {
            ChangeAnimationState(CROUCH);
        }
    }

    void PlayerMeleeAttack()
    {
        if (isAttackPressed)
        {
            isAttackPressed = false;
            if (!isAttacking)
            {
                isAttacking = true;
                //attack code
                ChangeAnimationState(MELEE);
                attackDelay = anim.GetCurrentAnimatorStateInfo(0).length;
                Invoke("AttackComplete", attackDelay);
            }
        }
    }

    void AttackComplete()
    {
        isAttacking = false;
    }

    internal void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;
        anim.Play(newState);
        currentState = newState;
    }

    void CheckWhereToFace()
    {
        if (horizontal > 0)
            sr.flipX = false;
        else if (horizontal < 0)
            sr.flipX = true;
    }

    // Interactable functions
    public void PickUpKey()
    {
        hasKey = true;
        StartCoroutine(tMPController.KeyReceive());
    }
}
