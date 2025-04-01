using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DeadCondition", menuName = "ConditionSO/Death")]
public class DeathConditionSO : ConditionSO
{
    public override bool CheckCondition(EnemyIA ec)
    {
        return ec.HP < 1;
    }
}
