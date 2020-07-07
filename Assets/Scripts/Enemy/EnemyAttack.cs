using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public IEnumerator AttackPlayer(Unit enemy)
    {
        if (!CanAttack(enemy))
        {
            Debug.Log("Enemy is waiting because they do not have enough AP to attack");
            yield break;
        }

        Debug.Log("enemy attacked unit");
        if (enemy != null && enemy.surroundingEnemies != null)
        {
            Unit playerLowestHP = FindPlayerLowestHP(enemy);
            ProcessAttack(enemy, playerLowestHP);
        }
        yield return new WaitForSeconds(3f);
    }

    private void ProcessAttack(Unit enemy, Unit playerLowestHP)
    {
        GetBestWeapon(enemy);
        playerLowestHP.health -= enemy.equippedATK;
        Debug.Log(playerLowestHP.health);
        playerLowestHP.gameObject.GetComponent<PlayerUnitView>().UnitDeath(playerLowestHP);
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

    bool CanAttack(Unit unit)
    {
        Item potentialWeapon = null;
        foreach (Item weapon in unit.unitInventory)
        {
            if (unit.actionPoints >= weapon.stats["APC"])
            {
                potentialWeapon = weapon;
            }
        }

        if (potentialWeapon == null)
            return false;
        else
            return true;
    }

    void GetBestWeapon(Unit unit)
    {
        Item bestWeapon = unit.equippedWeapon;
        foreach (Item weapon in unit.unitInventory)
        {
            if (unit.actionPoints >= weapon.stats["APC"] && bestWeapon.stats["ATK"] < weapon.stats["ATK"])
            {
                bestWeapon = weapon;
                Debug.Log(bestWeapon.title);
            }
        }
        unit.GetWeaponStats(unit, bestWeapon);
    }
}
