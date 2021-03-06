﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitButton : MonoBehaviour
{

    public void HandleWait()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();

        if (playerManager.currentUnit != null)
        {
            Unit selectedUnit = playerManager.currentUnit;
            SetUnitToWait(selectedUnit);
            playerManager.DeselectUnit(selectedUnit);
            Debug.Log("Unit is waiting.");
        }
    }

    private void SetUnitToWait(Unit unit)
    {
        ActionList actionList = FindObjectOfType<ActionList>();
        actionList.actionList.SetActive(false);
        unit.isWaiting = true;
        unit.hasMoved = true;
    }
}
