using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public void Attack(UnitDatabase unitDatabase, Node enemyNode, Unit unit)
    {
        Unit enemyUnit = unitDatabase.UnitNodeMap[enemyNode];
        Debug.Log("(B4Hit)Enemy Health: " + enemyUnit.health);
        enemyUnit.health -= unit.baseAttackDamage;
        Debug.Log("(After)Enemy Health: " + enemyUnit.health);
        unit.isWaiting = true;
    }
}
