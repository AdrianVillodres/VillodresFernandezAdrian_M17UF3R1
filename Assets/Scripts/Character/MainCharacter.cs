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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = new Inputs();
        playerInputs.Player.SetCallbacks(this);
        character = GetComponent<MainCharacter>();
        Healthbar.maxValue = HP;
        Healthbar.value = HP;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + speed * Time.deltaTime * ipMove.normalized);
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
        }
        else if (context.canceled)
        {
            ipMove = Vector3.zero;
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
