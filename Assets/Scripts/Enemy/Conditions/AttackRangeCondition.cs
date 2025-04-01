using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackRangeCondition", menuName = "ConditionSO/OnAttackRange")]
public class AttackRangeCondition : ConditionSO
{
    public override bool CheckCondition(EnemyIA ec)
    {
        return ec.attack;
    }
}
