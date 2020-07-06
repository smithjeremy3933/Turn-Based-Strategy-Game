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

    EnemyManager enemyManager;

    private void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        PlayerMovement.OnUnitMoved += PlayerMovement_OnUnitMoved;
        PlayerManager.OnUnitSelected += PlayerManager_OnUnitSelected;
        enemyManager.OnEnemyTurnEnded += EnemyManager_OnEnemyTurnEnded;
    }

    private void EnemyManager_OnEnemyTurnEnded(object sender, EnemyManager.OnEnemyTurnEndedEventArgs e)
    {
        UpdateUnitSelectText(e.enemy);
    }

    private void PlayerManager_OnUnitSelected(object sender, PlayerManager.OnUnitSelectedEventArgs e)
    {
        UpdateUnitSelectText(e.currentUnit);
    }

    private void PlayerMovement_OnUnitMoved(object sender, PlayerMovement.OnUnitMovedEventArgs e)
    {
        UpdateUnitSelectText(e.currentUnit);
    }

    public void UpdateUnitSelectText(Unit unit)
    {
        unitName.text = "NAME: " + unit.name;
        unitHealth.text = "HP: " + unit.health.ToString();
        unitDamage.text = "DMG: " + unit.baseAttackDamage.ToString();
        unitActionPoints.text = "AP: " + unit.actionPoints.ToString("F2");
    }
}
