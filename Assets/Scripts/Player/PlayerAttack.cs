using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    EngageUI m_engageUI;

    public void Attack(UnitDatabase unitDatabase, Node enemyNode, Unit unit)
    {
        EngageUI engageUI = FindObjectOfType<EngageUI>();
        m_engageUI = engageUI;
        Unit enemyUnit = unitDatabase.UnitNodeMap[enemyNode];
        m_engageUI.SetEngageUI(unit, enemyUnit);
        m_engageUI.ShowEngagePanel(unit);
        Debug.Log("(B4Hit)Enemy Health: " + enemyUnit.health);
        enemyUnit.health -= unit.baseAttackDamage;
        Debug.Log("(After)Enemy Health: " + enemyUnit.health);
        unit.isWaiting = true;
    }
}
