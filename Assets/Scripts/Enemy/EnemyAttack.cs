using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public IEnumerator AttackPlayer(Unit enemy)
    {
        Debug.Log("enemy attacked unit");
        if (enemy != null && enemy.surroundingEnemies != null)
        {
            Unit playerUnitToAttacK = enemy.surroundingEnemies[0];
            playerUnitToAttacK.health -= enemy.baseAttackDamage;
        }
        yield return new WaitForSeconds(3f);
    }
}
