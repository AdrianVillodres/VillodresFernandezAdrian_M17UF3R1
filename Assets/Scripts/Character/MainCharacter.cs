using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainCharacter : MonoBehaviour, Inputs.IPlayerActions, IHurteable
{
    private Inputs playerInputs;
    public int HP = 10;
    public Vector3 ipMove;
    private Rigidbody rb;
    private MainCharacter character;
    public Slider Healthbar;
    public int speed;
    private bool attack;
    private GameObject target;
    Animator animator;


    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rayDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private BoxCollider boxCollider;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = new Inputs();
        boxCollider = GetComponent<BoxCollider>();
        playerInputs.Player.SetCallbacks(this);
        character = GetComponent<MainCharacter>();
        animator = GetComponent<Animator>();
        Healthbar.maxValue = HP;
        Healthbar.value = HP;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + speed * Time.deltaTime * ipMove.normalized);

        Vector3 rayOrigin = transform.position + Vector3.down * (boxCollider.size.y / 2f - 0.05f);
        float rayDistance = 0.1f;

        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red);

        bool isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayDistance, groundLayer);
        animator.SetBool("IsJumping", !isGrounded);
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
        if (context.performed)
        {
            ipMove = context.ReadValue<Vector3>();
            animator.SetBool("isRunning", true);
        }
        else if (context.canceled)
        {
            ipMove = Vector3.zero;
            animator.SetBool("isRunning", false);
        }
    }

    public void OnDealDamage(InputAction.CallbackContext context)
    {
        if (context.performed && attack)
        {
            if (target != null)
            {
                IHurteable hurteable = target.GetComponent<IHurteable>();
                if (hurteable != null)
                {
                    hurteable.Hurt(1);
                }
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnJump activado por: " + context.control);
            Vector3 rayOrigin = transform.position + Vector3.down * (boxCollider.size.y / 2f - 0.05f);
            float rayLength = 0.1f;

            Debug.DrawRay(rayOrigin, Vector3.down * rayLength, Color.green, 1f);

            if (Physics.Raycast(rayOrigin, Vector3.down, rayLength, groundLayer))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                animator.SetBool("IsJumping", true);
                Debug.Log("Saltando");
            }
            else
            {
                Debug.Log("No grounded");
            }
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
}
