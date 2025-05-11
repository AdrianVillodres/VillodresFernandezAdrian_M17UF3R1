using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackState", menuName = "StatesSO/Attack")]
public class AttackState : StateSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
        ec.animator.SetBool("Attack", true);
    }

    public override void OnStateExit(EnemyIA ec)
    {
        ec.animator.SetBool("Attack", false);
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        ec.animator.SetBool("Attack", true);
        Debug.Log("Te reviento a chancletaso");
    }
}
