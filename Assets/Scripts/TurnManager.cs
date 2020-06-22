using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn {
    playerTurn, enemyTurn
}

public class TurnManager : MonoBehaviour
{
    PlayerSpawner m_playerSpawner;
    int m_turnNumber = 1;
    List<Unit> m_playerUnits;

    public void EndTurn()
    {
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        List<Unit> playerUnits = m_playerSpawner.GetComponent<PlayerSpawner>().PlayerUnits;
        m_playerUnits = playerUnits;
        m_turnNumber++;
        ResetStats(playerUnits);
    }

    public void ResetStats(List<Unit> units)
    {       
        foreach (Unit unit in units)
        {
            unit.ResetActionPoints();
        }
    }
}
