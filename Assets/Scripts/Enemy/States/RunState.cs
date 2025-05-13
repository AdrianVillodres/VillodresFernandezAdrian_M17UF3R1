using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunState", menuName = "StatesSO/Run")]
public class RunState : StateSO
{
    public override void OnStateEnter(EnemyIA ec)
    {
        ec.isFleeing = true;
    }

    public override void OnStateExit(EnemyIA ec)
    {
        ec.GetComponent<ChaseBehaviour>().StopChasing();
        ec.isFleeing = false;
    }

    public override void OnStateUpdate(EnemyIA ec)
    {
        Debug.Log("CoSorro");
        ec.GetComponent<ChaseBehaviour>().Run(ec.target.transform, ec.transform);
    }
}
