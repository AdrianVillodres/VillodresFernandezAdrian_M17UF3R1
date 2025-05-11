using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private MainCharacter character;
    private Animator animator;
    void Start()
    {
        character = FindAnyObjectByType<MainCharacter>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && character.objectPicked == true)
        {
            animator.SetBool("canOpen", true);
        }
    }
}
