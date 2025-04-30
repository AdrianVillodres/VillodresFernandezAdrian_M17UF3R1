using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private SwordBack swordBack;
    private bool hasSword = false;

    void Start()
    {
        if (!hasSword && swordBack != null)
        {
            swordBack.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasSword && swordBack != null)
        {
            swordBack.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasSword = true;
            if (swordBack != null)
            {
                swordBack.gameObject.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
