﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn {
    playerTurn, enemyTurn
}

public class TurnManager : MonoBehaviour
{
    public static event EventHandler OnTurnEnded;
    public Turn currentTurn = Turn.playerTurn;
    UnitDatabase m_unitDatabase;
    int m_turnNumber = 1;
    List<Unit> m_playerUnits;

    public void EndTurn()
    {
        OnTurnEnded?.Invoke(this, EventArgs.Empty);
        ProcessUnitStats();
        ProcessTurn();
    }

    private void ProcessTurn()
    {
        m_turnNumber++;
        currentTurn = Turn.enemyTurn;
    }

    private void ProcessUnitStats()
    {
        m_unitDatabase = FindObjectOfType<UnitDatabase>();
        List<Unit> playerUnits = m_unitDatabase.GetComponent<UnitDatabase>().PlayerUnits;
        m_playerUnits = playerUnits;
        ResetStats(playerUnits);
    }

    public void ResetStats(List<Unit> units)
    {       
        foreach (Unit unit in units)
        {        
            unit.ProcessTurn();
        }
    }
}
