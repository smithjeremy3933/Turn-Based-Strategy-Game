using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool IsEngagedClicked { get => isEngagedClicked; set => isEngagedClicked = value; }

    EngageUI m_engageUI;
    bool isEngaging = false;
    bool isEngagedClicked = false;

    public IEnumerator Attack(Unit player, Unit enemy)
    {
        ProcessEngagement(player, enemy);

        while (isEngaging)
        {
            isEngaging = !isEngagedClicked;
            yield return null;
        }

        ProcessAttack(player, enemy);
    }

    void ProcessEngagement(Unit player, Unit enemy)
    {
        Debug.Log("Start Engagement");
        EngageUI engageUI = FindObjectOfType<EngageUI>();
        m_engageUI = engageUI;
        m_engageUI.SetEngageUI(player, enemy);
        m_engageUI.ShowEngagePanel(player);
        isEngaging = true;
    }

    private void ProcessAttack(Unit unit, Unit enemyUnit)
    {
        m_engageUI.HideEngagePanel();
        ProcessDamage(unit, enemyUnit);
        unit.isWaiting = true;
        unit.isAttacking = false;
        isEngaging = false;
        isEngagedClicked = false;
    }

    private static void ProcessDamage(Unit unit, Unit enemyUnit)
    {
        Debug.Log("(B4Hit)Enemy Health: " + enemyUnit.health);
        enemyUnit.health -= unit.equippedATK;
        enemyUnit.health -= CalcCRIT(unit);
        Debug.Log("(After)Enemy Health: " + enemyUnit.health);
    }

    private static float CalcCRIT(Unit unit)
    {
        if (UnityEngine.Random.value <= unit.baseCRIT / 100)
        {
            float CRITdamage = unit.equippedATK * UnityEngine.Random.Range(0.25f, 0.5f);
            return CRITdamage;
        } 
        return 0;
    }
}
