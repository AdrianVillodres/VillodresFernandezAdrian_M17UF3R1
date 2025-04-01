using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "IdleState", menuName = "StatesSO/Idle")]
public class IdleState : StateSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
    }

    public override void OnStateExit(EnemyIA ec)
    {
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        Debug.Log("Here chillin");
    }
}
