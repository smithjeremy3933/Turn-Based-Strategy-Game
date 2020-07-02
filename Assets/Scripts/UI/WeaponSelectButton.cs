using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectButton : MonoBehaviour
{
    Item currentItem;

    [SerializeField] public Item CurrentItem { get => currentItem; set => currentItem = value; }

    public void HandleWeaponSwitch()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager.currentUnit != null)
        {
            SwitchWeapon(playerManager.currentUnit);
        }
    }

    public void SwitchWeapon(Unit playerUnit)
    {
        EngageUI engageUI = FindObjectOfType<EngageUI>();
        playerUnit.GetWeaponStats(playerUnit, currentItem);
        UpdateUI(playerUnit, engageUI);
    }

    private static void UpdateUI(Unit playerUnit, EngageUI engageUI)
    {
        engageUI.playerUnitATK.text = "ATK: " + playerUnit.equippedATK.ToString();
        engageUI.playerUnitHIT.text = "HIT: " + playerUnit.equippedHIT.ToString();
        engageUI.playerUnitCRIT.text = "CRIT: " + playerUnit.equippedCRIT.ToString();
        engageUI.playerUnitAP.text = "AP: " + playerUnit.calcAPC.ToString();
    }
}
