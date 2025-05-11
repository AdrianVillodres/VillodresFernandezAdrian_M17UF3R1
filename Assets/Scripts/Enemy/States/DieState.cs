using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DieState", menuName = "StatesSO/Die")]
public class DieState : StateSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
        ec.animator.SetBool("Die", true);
    }

    public override void OnStateExit(EnemyIA ec)
    {
        ec.animator.SetBool("Die", false);
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        Destroy(ec.gameObject);
        Debug.Log("Abandon� este mundo de miseria y desesperaci�n");
    }
}
