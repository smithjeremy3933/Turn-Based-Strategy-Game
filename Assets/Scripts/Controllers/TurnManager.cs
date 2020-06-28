using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn {
    playerTurn, enemyTurn
}

public class TurnManager : MonoBehaviour
{
    UnitDatabase m_unitDatabase;
    PlayerManager m_playerManager;
    ActionList m_actionList;
    int m_turnNumber = 1;
    List<Unit> m_playerUnits;

    public void EndTurn()
    {
        m_unitDatabase = FindObjectOfType<UnitDatabase>();
        m_playerManager = FindObjectOfType<PlayerManager>();
        m_actionList = FindObjectOfType<ActionList>();
        List<Unit> playerUnits = m_unitDatabase.GetComponent<UnitDatabase>().PlayerUnits;
        m_playerUnits = playerUnits;
        ResetStats(playerUnits);
        if (m_playerManager.currentUnit != null)
        {
            UIController uIController = FindObjectOfType<UIController>();
            uIController.UpdateUnitSelectText(m_playerManager.currentUnit);
            m_playerManager.DeselectUnit(m_playerManager.currentUnit);
        }
        m_turnNumber++;
        m_actionList.HandleTurn();
    }

    public void ResetStats(List<Unit> units)
    {       
        foreach (Unit unit in units)
        {        
            unit.ProcessTurn();
        }
    }
}
