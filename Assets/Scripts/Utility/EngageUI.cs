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

    public void SetEngageUI(Unit playerUnit, Unit enemyUnit)
    {
        playerUnitAP.text = "AP: " + playerUnit.actionPoints.ToString("F2");
        playerUnitName.text = playerUnit.name;
        playerUnitHP.text = "HP: " + playerUnit.health.ToString();
        playerUnitATK.text = "ATK: " + playerUnit.baseAttackDamage.ToString();

        if (playerUnit.unitInventory[0] != null)
        {
            slotOneItemName.text = playerUnit.unitInventory[0].title;
            slotOneATK.text = playerUnit.unitInventory[0].stats["ATK"].ToString();
            slotOneAPC.text = playerUnit.unitInventory[0].stats["APC"].ToString();
        }

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
