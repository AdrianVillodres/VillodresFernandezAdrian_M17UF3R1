using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OnRange", menuName = "ConditionSO/Range")]
public class RangeCondition : ConditionSO
{
    public override bool CheckCondition(EnemyIA ec)
    {
        return ec.GetComponent<EnemyVision>().PlayerInSight;
    }

}
