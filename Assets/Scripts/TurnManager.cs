using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    PlayerSpawner m_playerSpawner;
    int m_turnNumber = 1;

    private void Update()
    {
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
    }

    public void EndTurn()
    {    
        ResetStats(m_playerSpawner.PlayerUnits);
    }

    public void ResetStats(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            unit.actionPoints = 7;
        }
        m_turnNumber++;
    }
}
