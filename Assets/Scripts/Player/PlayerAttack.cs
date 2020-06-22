using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public void MeleeAttackEnemy()
    {
        Debug.Log("Die Scum");
    }

    public void Attack(PlayerSpawner playerSpawner, Node enemyNode, Unit playerUnit)
    {
        Unit enemyUnit = playerSpawner.UnitNodeMap[enemyNode];
        Debug.Log("(B4Hit)Enemy Health: " + enemyUnit.health);
        enemyUnit.health -= playerUnit.baseAttackDamage;
        Debug.Log("(After)Enemy Health: " + enemyUnit.health);
    }
}
