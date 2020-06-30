using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectButton : MonoBehaviour
{
    WeaponSelectButton[] weaponButtons;

    private void Start()
    {
        weaponButtons = FindObjectsOfType<WeaponSelectButton>();
        Debug.Log(weaponButtons.Length);
    }

    public void SelectWeapon(Unit unit)
    {

    }
}
