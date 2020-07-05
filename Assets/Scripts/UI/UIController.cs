using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] public Text unitName;
    [SerializeField] public Text unitHealth;
    [SerializeField] public Text unitDamage;
    [SerializeField] public Text unitActionPoints;

    private void Start()
    {
        PlayerMovement.OnUnitMoved += PlayerMovement_OnUnitMoved;
        PlayerManager.OnUnitSelected += PlayerManager_OnUnitSelected;
    }

    private void PlayerManager_OnUnitSelected(object sender, PlayerManager.OnUnitSelectedEventArgs e)
    {
        UpdateUnitSelectText(e.currentUnit);
    }

    private void PlayerMovement_OnUnitMoved(object sender, PlayerMovement.OnUnitMovedEventArgs e)
    {
        Unit unitMoved = e.currentUnit;
        UpdateUnitSelectText(unitMoved);
    }

    public void UpdateUnitSelectText(Unit unit)
    {
        unitName.text = "NAME: " + unit.name;
        unitHealth.text = "HP: " + unit.health.ToString();
        unitDamage.text = "DMG: " + unit.baseAttackDamage.ToString();
        unitActionPoints.text = "AP: " + unit.actionPoints.ToString("F2");
    }
}
