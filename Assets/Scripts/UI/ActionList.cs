using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionList : MonoBehaviour
{
    [SerializeField] public GameObject actionList;
    [SerializeField] GameObject attackButton;

    private void Start()
    {
        actionList.SetActive(false);
        PlayerManager.OnUnitSelected += PlayerManager_OnUnitSelected;
        TurnManager.OnTurnEnded += TurnManager_OnTurnEnded;
    }

    private void TurnManager_OnTurnEnded(object sender, System.EventArgs e)
    {
        HandleTurn();
    }

    private void PlayerManager_OnUnitSelected(object sender, PlayerManager.OnUnitSelectedEventArgs e)
    {
        if (e.currentUnit.hasMoved && e.currentUnit.unitType == UnitType.player)
        {
            HandleMovedUnit(e.currentUnit);
        }
        else
        {
            actionList.SetActive(false);
        }
    }

    public void HandleTurn()
    {
        actionList.SetActive(false);
    }

    public void HandleMovedUnit(Unit unit)
    {
        if (unit.isWaiting)
        {
            actionList.SetActive(false);
            return;
        }
        else if (unit.hasMoved)
        {
            actionList.SetActive(true);
            HandleAttackButton(unit);
        }
    }

    public void HandleAttackButton(Unit unit)
    {
        if (unit.isSurrEnemies)
        {
            attackButton.SetActive(true);
            Debug.Log("This unit has enemies within attack range");
        }
        else
        {
            attackButton.SetActive(false);
        }
    }
}
