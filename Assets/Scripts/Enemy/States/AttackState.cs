using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackState", menuName = "StatesSO/Attack")]
public class AttackState : StateSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
    }

    public override void OnStateExit(EnemyIA ec)
    {
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        Debug.Log("Te reviento a chancletaso");
    }
}
