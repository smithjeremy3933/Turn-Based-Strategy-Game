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
        float calcATK = playerUnit.baseAttackDamage + currentItem.stats["ATK"];
        float calcHIT = playerUnit.baseHIT + currentItem.stats["HIT"];
        float calcCRIT = playerUnit.baseCRIT + currentItem.stats["CRIT"];
        float calcAPC = playerUnit.actionPoints - currentItem.stats["APC"];
        engageUI.playerUnitATK.text = "ATK: " + calcATK.ToString();
        engageUI.playerUnitHIT.text = "HIT: " + calcHIT.ToString();
        engageUI.playerUnitCRIT.text = "CRIT: " + calcCRIT.ToString();
        engageUI.playerUnitAP.text = "AP: " + calcAPC.ToString();
        playerUnit.equippedWeapon = currentItem;
    }
}
