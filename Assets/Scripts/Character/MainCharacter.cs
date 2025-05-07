using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainCharacter : MonoBehaviour, Inputs.IPlayerActions, IHurteable
{
    private Inputs playerInputs;
    public int HP = 10;
    public Vector3 ipMove;
    public Rigidbody rb;
    private MainCharacter character;
    public Slider Healthbar;
    private int speed = 5;
    public bool attack;
    private bool isGrounded = false;
    private bool isAiming = false;
    private bool crouched = false;
    private GameObject target;
    Animator animator;
    private Sword sword;
    public bool isAttacking = false;
    public bool isWalking = false;
    public float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    private BoxCollider boxCollider;
    private bool canMove = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = new Inputs();
        boxCollider = GetComponent<BoxCollider>();
        playerInputs.Player.SetCallbacks(this);
        sword = GetComponent<Sword>();
        character = GetComponent<MainCharacter>();
        animator = GetComponent<Animator>();
        Healthbar.maxValue = HP;
        Healthbar.value = HP;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(rb.position + speed * Time.deltaTime * ipMove.normalized);
        }

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        float rayDistance = 0.5f;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            isGrounded = true;
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
            isGrounded = false;
        }

        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red);
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        if (context.performed)
        {
            ipMove = context.ReadValue<Vector3>();
            animator.SetBool("IsWalking", true);
            isWalking = true;
        }
        else if (context.canceled)
        {
            ipMove = Vector3.zero;
            animator.SetBool("IsWalking", false);
            isWalking = false;
        }

        if (context.performed && crouched == true)
        {
            animator.SetBool("CrouchedWalking", true);
        }
        else if (context.canceled && crouched == true)
        {
            animator.SetBool("CrouchedWalking", false);
        }

        if (context.performed && isAiming == true)
        {
            animator.SetBool("IsAiming", true);
            animator.SetBool("IsAimingIdle", false);
            animator.SetBool("IsAimingWalking", true);
        }
        else if (context.canceled && isAiming == true)
        {
            animator.SetBool("IsAiming", true);
            animator.SetBool("IsAimingIdle", true);
            animator.SetBool("IsAimingWalking", false);
        }

        if (attack == true)
        {
            ipMove = Vector3.zero;
            animator.SetBool("IsWalking", false);
            attack = false;
        }
    }

    public void OnDealDamage(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            canMove = false;

            if (attack && target != null)
            {
                IHurteable hurteable = target.GetComponent<IHurteable>();
                if (hurteable != null)
                {
                    hurteable.Hurt(1);
                }
            }

            StartCoroutine(ResetAfterAttack());
        }
    }

    private IEnumerator ResetAfterAttack()
    {
        yield return new WaitForSeconds(1.2f);
        isAttacking = false;
        animator.SetBool("IsWalking", false);
        ipMove = Vector3.zero;
        canMove = true;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            speed = 10;
            animator.SetBool("isRunning", true);
        }
        else if (context.canceled)
        {
            speed = 5;
            animator.SetBool("isRunning", false);
            if(!isWalking)
            {
                animator.SetBool("IsWalking", false);
            }
            else
            {
               animator.SetBool("IsWalking", true);
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded == true)
        {
            animator.SetBool("IsJumping", true);
            Debug.Log("Saltando");
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (crouched == false)
            {
                speed = 3;
                animator.SetBool("IsCrouching", true);
                animator.SetBool("StandingUp", false);
                animator.SetBool("StandUp", false);
                crouched = true;
                StartCoroutine(WaitForAction(0.1f));
                animator.SetBool("Crouched", true);
            }
            else
            {
                speed = 5;
                crouched = false;
                animator.SetBool("StandingUp", true);
                animator.SetBool("IsCrouching", false);
                animator.SetBool("Crouched", false);
                animator.SetBool("StandUp", true);
                crouched = false;
            }
        }
    }

    public void OnDance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetBool("IsDancing", true);
        }
        else if (context.canceled)
        {
            animator.SetBool("IsDancing", false);
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        Healthbar.value = HP;
    }

    public void Hurt(int damage)
    {
        TakeDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            attack = true;
            target = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            attack = false;
            target = null;
        }
    }

    public void StartJumpCoroutine()
    {
        StartCoroutine(DelayedJump());
    }

    private IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(0.5f);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private IEnumerator WaitForAction(float wait)
    {
        yield return new WaitForSeconds(wait);
    }

    private IEnumerator EnableMovementAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;
            animator.SetBool("IsAiming", true);
            animator.SetBool("IsAimingIdle", true);
        }
        else if (context.canceled)
        {
            isAiming = false;
            animator.SetBool("IsAimingWalking", false);
            animator.SetBool("IsAiming", false);
            animator.SetBool("IsAimingIdle", false);
            animator.gameObject.GetComponent<CameraManager>().SwitchCamera("ThirdPerson");
        }
    }
}
