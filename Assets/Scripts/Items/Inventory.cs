using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] public ItemDatabase itemDatabase;

    public void SpawnItemToUnit(Unit unit, int ID)
    {
        Item item = itemDatabase.GetItem(ID);
        unit.unitInventory.Add(item);
        SetEquippedStats(unit);
    }

    void SetEquippedStats(Unit unit)
    {
        unit.equippedATK = unit.baseAttackDamage + unit.unitInventory[0].stats["ATK"];
    }
}
