using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;

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
    public bool isDancing = false;
    public bool canRotate = true;
    public bool objectPicked = false;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        playerInputs = new Inputs();
        boxCollider = GetComponent<BoxCollider>();
        playerInputs.Player.SetCallbacks(this);
        character = GetComponent<MainCharacter>();
        animator = GetComponent<Animator>();
        Healthbar.maxValue = HP;
        Healthbar.value = HP;

        if (GameSaveManager.HasSavedData())
        {
            GameSaveManager.LoadGame(out Vector3 pos, out Quaternion rot, out bool hasObj, out bool enemyKilled);
            transform.position = pos;
            transform.rotation = rot;
            objectPicked = hasObj;

            if (objectPicked)
            {
                FindObjectOfType<Sword>().gameObject.SetActive(false);
                FindObjectOfType<SwordBack>().gameObject.SetActive(true);
            }
            Debug.Log(enemyKilled);
            if (enemyKilled)
            {
                List<EnemyIA> a = FindObjectsOfType<EnemyIA>().ToList();
                foreach (EnemyIA enemy in a)
                {
                    Destroy(enemy.gameObject);
                }
            }
        }

    }

    void FixedUpdate()
    {
        if (isDancing) return;

        if (canMove)
        {
            rb.MovePosition(rb.position + speed * Time.deltaTime * ipMove.normalized);
        }

        if (canRotate && !isAttacking)
        {
            if (ipMove != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(ipMove);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
            }
        }

        Vector3 rayOrigin = transform.position + Vector3.up * 10f;
        float rayDistance = 10f;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
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
        if (isDancing || !canMove) return;

        if (context.performed)
        {
            ipMove = context.ReadValue<Vector3>();
            animator.SetBool("IsWalking", true);
            isWalking = true;

            if (speed == 10)
            {
                animator.SetBool("isRunning", true);
            }
        }
        else if (context.canceled)
        {
            ipMove = Vector3.zero;
            animator.SetBool("IsWalking", false);
            isWalking = false;

            if (!attack && speed == 10)
            {
                animator.SetBool("isRunning", false);
                speed = 5;
            }
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
        if (context.performed && !isAttacking && objectPicked == true)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            canMove = false;
            canRotate = false;

            if (target != null)
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
        canRotate = true;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isWalking && !crouched)
            {
                speed = 10;
                animator.SetBool("isRunning", true);
            }
        }
        else if (context.canceled)
        {
            if (!isWalking || crouched)
            {
                speed = 5;
                animator.SetBool("isRunning", false);
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded == true)
        {
            animator.SetBool("IsJumping", true);
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
            if (!crouched)
            {
                speed = 3;
                crouched = true;
                animator.SetBool("IsCrouching", true);
                animator.SetBool("StandingUp", false);
                animator.SetBool("StandUp", false);
                animator.SetBool("Crouched", true);
                isWalking = false;

                if (ipMove != Vector3.zero)
                {
                    animator.SetBool("CrouchedWalking", true);
                }

                StartCoroutine(WaitForAction(0.1f));
            }
            else
            {
                speed = 5;
                crouched = false;
                animator.SetBool("StandingUp", true);
                animator.SetBool("IsCrouching", false);
                animator.SetBool("Crouched", false);
                animator.SetBool("StandUp", true);
                animator.SetBool("CrouchedWalking", false);

                if (ipMove != Vector3.zero)
                {
                    animator.SetBool("IsWalking", true);
                    isWalking = true;

                    if (speed == 10)
                    {
                        animator.SetBool("StandingUp", false);
                        animator.SetBool("IsWalking", false);
                        animator.SetBool("isRunning", true);
                    }
                }
            }
        }
    }

    public void OnDance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isDancing = true;
            animator.SetBool("IsDancing", true);
            canMove = false;
            canRotate = false;
            StartCoroutine(WaitAndEnableMovement(10f));
        }
        else if (context.canceled)
        {
            isDancing = false;
            animator.SetBool("IsDancing", false);
        }
    }

    private IEnumerator WaitAndEnableMovement(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canMove = true;
        canRotate = true;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(2);
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
            animator.SetBool("IsAimingIdle", false);
            animator.SetBool("IsAimingWalking", true);
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