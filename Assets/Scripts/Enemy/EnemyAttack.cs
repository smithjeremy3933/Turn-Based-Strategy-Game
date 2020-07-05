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
            Unit playerLowestHP = FindPlayerLowestHP(enemy);
            ProcessAttack(enemy, playerLowestHP);
        }
        yield return new WaitForSeconds(3f);
    }

    private static void ProcessAttack(Unit enemy, Unit playerLowestHP)
    {
        Item bestWeapon = enemy.equippedWeapon;
        playerLowestHP.health -= enemy.equippedATK;
        Debug.Log(playerLowestHP.health);
    }

    private static Unit FindPlayerLowestHP(Unit enemy)
    {
        if (enemy != null)
        {
            Unit playerLowestHP = null;
            float lowestHP = 0;
            foreach (Unit player in enemy.surroundingEnemies)
            {
                if (player.health < lowestHP || lowestHP == 0)
                {
                    playerLowestHP = player;
                    lowestHP = player.health;
                }
            }

            return playerLowestHP;
        }
        else
        {
            return null;
        }
    }
}
