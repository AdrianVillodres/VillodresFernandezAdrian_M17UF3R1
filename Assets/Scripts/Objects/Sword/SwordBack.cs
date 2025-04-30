using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBack : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
    }

    public void ActivateSwordBack()
    {
        gameObject.SetActive(true);
    }
}
