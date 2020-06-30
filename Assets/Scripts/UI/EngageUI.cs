using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngageUI : MonoBehaviour
{
    [SerializeField] public GameObject engagePanel;

    [SerializeField] public Text playerUnitAP;

    [SerializeField] public Text playerUnitName;
    [SerializeField] public Text playerUnitHP;
    [SerializeField] public Text playerUnitATK;
    [SerializeField] public Text playerUnitHIT;
    [SerializeField] public Text playerUnitCRIT;

    [SerializeField] public Text enemyUnitName;
    [SerializeField] public Text enemyUnitHP;
    [SerializeField] public Text enemyUnitATK;
    [SerializeField] public Text enemyUnitHIT;
    [SerializeField] public Text enemyUnitCRIT;

    [SerializeField] public Text slotOneItemName;
    [SerializeField] public Text slotOneATK;
    [SerializeField] public Text slotOneAPC;
    [SerializeField] public Text slotOneHIT;
    [SerializeField] public Text slotOneCRIT;

    [SerializeField] public Text slotTwoItemName;
    [SerializeField] public Text slotTwoATK;
    [SerializeField] public Text slotTwoAPC;
    [SerializeField] public Text slotTwoHIT;
    [SerializeField] public Text slotTwoCRIT;

    Item currentSelectedWeapon;

    public void SetEngageUI(Unit playerUnit, Unit enemyUnit)
    {
        SelectWeapon(playerUnit);
        playerUnitAP.text = "AP: " + playerUnit.actionPoints.ToString("F2");
        playerUnitName.text = playerUnit.name;
        playerUnitHP.text = "HP: " + playerUnit.health.ToString();
        playerUnitATK.text = "ATK: " + playerUnit.equippedATK.ToString();
        playerUnitHIT.text = "HIT: " + playerUnit.baseHIT.ToString();
        playerUnitCRIT.text = "CRIT: " + playerUnit.baseCRIT.ToString();

        enemyUnitName.text = enemyUnit.name;
        enemyUnitHP.text = "HP: " + enemyUnit.health.ToString();
        enemyUnitATK.text = "ATK: " + enemyUnit.equippedATK.ToString();
        enemyUnitHIT.text = "HIT: " + enemyUnit.baseHIT.ToString();
        enemyUnitCRIT.text = "CRIT: " + enemyUnit.baseCRIT.ToString();

        if (playerUnit.unitInventory[0] != null)
        {
            slotOneItemName.text = playerUnit.unitInventory[0].title;
            slotOneATK.text = playerUnit.unitInventory[0].stats["ATK"].ToString();
            slotOneAPC.text = playerUnit.unitInventory[0].stats["APC"].ToString();
        }

        if (playerUnit.unitInventory[1] != null)
        {
            slotTwoItemName.text = playerUnit.unitInventory[1].title;
            slotTwoATK.text = playerUnit.unitInventory[1].stats["ATK"].ToString();
            slotTwoAPC.text = playerUnit.unitInventory[1].stats["APC"].ToString();
        }
    }

    public void SelectWeapon(Unit unit)
    {
        currentSelectedWeapon = unit.equippedWeapon;
    }


    public void ShowEngagePanel(Unit unit)
    {
        engagePanel.SetActive(true);
    }

    public void HideEngagePanel()
    {
        engagePanel.SetActive(false);
    }
}
