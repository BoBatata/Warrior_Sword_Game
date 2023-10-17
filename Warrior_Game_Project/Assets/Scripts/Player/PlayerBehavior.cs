using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
using Collider2D = UnityEngine.Collider2D;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class PlayerBehavior : MonoBehaviour
{
    private const float gravityValue = -9.81f;
    private PlayerControls playerControls;
    public static PlayerBehavior instance;

    #region Private Variables
    #region PlayerComponents
    private Animator animator;
    private Rigidbody2D rigidBody;
    private PlayerSound playerSounds;
    #endregion

    #region Movement variables
    private Vector2 moveDirection;
    private float initialGravityScale;
    private bool isMoving;
    private bool isJumping;
    private bool canJump;
    #endregion

    #region Combat
    private bool canAttack;
    private bool isAttacking;
    private bool cooldownIsOver = true;
    private bool gotHitted;
    private bool canBeHitted = true;

    [Header("Player Status")]
    [SerializeField] private int playerLifes = 3;
    [SerializeField] private int playerScore;
    [SerializeField] private GameObject playerShield;

    [Header("Attack properties")]
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackMask;
    #endregion

    #region Animatior variables
    private int isMovingAnimatorHash;
    private int isJumpingAnimatorHash;
    private int attackAnimatorHash;
    private int isHittedAnimatorHash;
    #endregion
    #endregion

    #region SerializedField Variables
    [SerializeField] private float velocity;

    [Header("Jump Properties")]
    [SerializeField] private float jumpForce;
    [FormerlySerializedAs("jumpFallGravityScale")]
    [SerializeField, Range(1f, 10f)] private float jumpFallGravityMultiplier = 3f;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask groundLayer;

    #endregion

    private void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        #endregion

        GetPlayerComponents();
        SetInputParameters();
        GetAnimatorParametersHash();

        initialGravityScale = rigidBody.gravityScale;
    }

    private void Update()
    {
        MovePlayer();
        AnimatePlayer();
        GravityHandler();
        GetPlayerScore();
    }

    private void MovePlayer()
    {
        if (moveDirection == null) return;

        if (moveDirection.x > 0)
        {
            transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        else if (moveDirection.x < 0)
        {
            transform.rotation = quaternion.identity;
        }
        moveDirection.x = playerControls.Movement.Move.ReadValue<float>();
        rigidBody.velocity = new Vector2(moveDirection.x * velocity, rigidBody.velocity.y);
        isMoving = moveDirection.x != 0;
    }

    private void HandleJump(InputAction.CallbackContext inputContext)
    {
        if (IsGrounded())
        {
            rigidBody.velocity += Vector2.up * jumpForce;
            isJumping = true;
            canAttack = false;
            playerSounds.PlayJumpSound();
        }
    }

    private void HandleAttack(InputAction.CallbackContext inputContext)
    {
        isAttacking = inputContext.ReadValueAsButton();
        if (isAttacking == true && canAttack == true)
        {
            animator.SetTrigger(attackAnimatorHash);
        }
    }

    private void AttackHandler()
    {
        Collider2D[] hittedEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, attackMask);
        foreach (Collider2D hittedEnemie in hittedEnemies)
        {
            if (hittedEnemie.TryGetComponent(out EnemyCrabBehavior enemyCrab))
            {
                enemyCrab.PlayDeathSound();
                playerScore += 10;
            }
            if (hittedEnemie.TryGetComponent(out EnemyOctuBehavior enemyOctu))
            {
                enemyOctu.PlayDeathSound();
                playerScore += 10;
            }
        }
    }

    private void AnimatePlayer()
    {
        if (isMoving && animator.GetBool(isMovingAnimatorHash) == false)
        {
            animator.SetBool(isMovingAnimatorHash, true);
        }
        else if (isMoving == false && animator.GetBool(isMovingAnimatorHash) == true)
        {
            animator.SetBool(isMovingAnimatorHash, false);
        }

        if (!IsGrounded() && animator.GetBool(isJumpingAnimatorHash) == false)
        {
            animator.SetBool(isJumpingAnimatorHash, true);
        }
        else if (animator.GetBool(isJumpingAnimatorHash) == true && IsGrounded())
        {
            animator.SetBool(isJumpingAnimatorHash, false);
        }

        if (isAttacking == true && canAttack == true)
        {
            animator.SetTrigger(attackAnimatorHash);
        }
        else if (isAttacking == false)
        {
            animator.ResetTrigger(attackAnimatorHash);
        }

    }

    private void GetPlayerComponents()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerSounds = GetComponent<PlayerSound>();
    }

    private void SetInputParameters()
    {
        playerControls = new PlayerControls();

        playerControls.Movement.Jump.started += HandleJump;
        playerControls.Movement.Jump.canceled += HandleJump;

        playerControls.Combat.SimpleAttack.started += HandleAttack;
        playerControls.Combat.SimpleAttack.canceled += HandleAttack;
    }



    public int GetPlayerHealth()
    {
        return playerLifes;
    }

    public void GetPlayerScore()
    {
        UIManager.instance.CurrentScore(playerScore);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && canBeHitted == true && cooldownIsOver == true)
        {
            canAttack = false;
            cooldownIsOver = false;
            StartCoroutine(DamageCooldown());
        }
        else if (playerLifes <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void DamageTaken()
    {
        gotHitted = true;
        cooldownIsOver = false;
        if (gotHitted == true && canBeHitted == true)
        {
            animator.SetTrigger(isHittedAnimatorHash);
            playerSounds.PlayerHurtSound();
            playerLifes--;
            gotHitted = false;
        }
        else if (gotHitted == false)
        {
            animator.ResetTrigger(isHittedAnimatorHash);
        }

    }

    private IEnumerator DamageCooldown()
    {
        DamageTaken();
        canBeHitted = false;
        playerShield.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        playerShield.gameObject.SetActive(false);
        canBeHitted = true;
        gotHitted = false;
        cooldownIsOver = true;
    }
    private void GetAnimatorParametersHash()
    {
        isMovingAnimatorHash = Animator.StringToHash("isMoving");
        isJumpingAnimatorHash = Animator.StringToHash("isJumping");
        attackAnimatorHash = Animator.StringToHash("attack");
        isHittedAnimatorHash = Animator.StringToHash("getHitted");

    }
    public Vector2 GetPlayerPosition()
    {
        return transform.position;
    }

    private void GravityHandler()
    {   
        if (IsGrounded())
        {
            isJumping = false;
            canAttack = true;
            rigidBody.gravityScale = initialGravityScale;
        }
        else if (isJumping && rigidBody.velocity.y < 0f)
        {
            isJumping = true;
            rigidBody.gravityScale = initialGravityScale * jumpFallGravityMultiplier;
        }
    }

    #region OnEnable/Disable Functions   
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Movement.Jump.started -= HandleJump;
        playerControls.Movement.Jump.canceled -= HandleJump;

        playerControls.Combat.SimpleAttack.started -= HandleAttack;
        playerControls.Combat.SimpleAttack.canceled -= HandleAttack;
        playerControls.Disable();
    }
    #endregion

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheckPos.position, new Vector2(1f, 0.2f), groundLayer);
        return isGrounded;
    }
    private void OnDrawGizmos()
    {
        if (hitPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPos.position, new Vector3(1f, 0.2f));
    }
}
