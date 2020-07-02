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
            // Controlled by EngageButton
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

    public void ProcessAttack(Unit unit, Unit enemyUnit)
    {
        if (unit != null && enemyUnit != null && m_engageUI != null)
        {
            m_engageUI.HideEngagePanel();
            if (unit != null)
            {
                ProcessHIT(unit, enemyUnit);
                unit.isWaiting = true;
                unit.isAttacking = false;
                isEngaging = false;
                isEngagedClicked = false;
            }
        }
    }

    private void ProcessHIT(Unit unit, Unit enemyUnit)
    {
        if (UnityEngine.Random.value <= unit.equippedHIT / 100)
        {
            Debug.Log("You HIT the unit." + unit.equippedHIT / 100);
            ProcessDamage(unit, enemyUnit);
        }
        else
        {
            Debug.Log("MISSED!!");
        }
    }

    private void ProcessDamage(Unit unit, Unit enemyUnit)
    {
        if (unit != null && enemyUnit != null)
        {
            EngageUI engageUI = FindObjectOfType<EngageUI>();
            Debug.Log("(B4Hit)Enemy Health: " + enemyUnit.health);
            Debug.Log(unit.equippedWeapon.title);
            enemyUnit.health -= unit.equippedATK;
            enemyUnit.health -= CalcCRIT(unit);
            Debug.Log("(After)Enemy Health: " + enemyUnit.health);
        }
    }

    private static float CalcCRIT(Unit unit)
    {
        if (UnityEngine.Random.value <= unit.equippedCRIT / 100)
        {
            float CRITdamage = unit.equippedATK * UnityEngine.Random.Range(0.25f, 0.5f);
            Debug.Log("You CRIT: " + CRITdamage);
            return CRITdamage;
        } 
        return 0;
    }
}