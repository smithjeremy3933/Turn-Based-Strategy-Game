using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public void HandleAttack()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager.currentUnit != null)
        {
            playerManager.IsSelectingEnemy = true;
            SetUnitToAttack(playerManager.currentUnit);
            Debug.Log("Unit is in attack mode.");
        }
    }

    private void SetUnitToAttack(Unit unit)
    {
        unit.isAttacking = true;
    }
}
