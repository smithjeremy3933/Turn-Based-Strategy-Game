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
