using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public List<Unit> AllUnits { get => m_allUnits; set => m_allUnits = value; }
    public List<Unit> PlayerUnits { get => m_playerUnits; set => m_playerUnits = value; }
    public List<Unit> EnemyUnits { get => m_enemyUnits; set => m_enemyUnits = value; }
    public List<Unit> KilledPlayerUnits { get => m_killedPlayerUnits; set => m_killedPlayerUnits = value; }
    public Dictionary<Node, Unit> UnitNodeMap { get => unitNodeMap; set => unitNodeMap = value; }
    public Dictionary<Node, GameObject> NodeUnitViewMap { get => nodeUnitViewMap; set => nodeUnitViewMap = value; }
    public Dictionary<Unit, GameObject> UnitGOMap { get => unitGOMap; set => unitGOMap = value; }

    List<Unit> m_allUnits = new List<Unit>();
    List<Unit> m_playerUnits = new List<Unit>();
    List<Unit> m_enemyUnits = new List<Unit>();
    List<Unit> m_killedPlayerUnits = new List<Unit>();
    Dictionary<Node, Unit> unitNodeMap = new Dictionary<Node, Unit>();
    Dictionary<Node, GameObject> nodeUnitViewMap = new Dictionary<Node, GameObject>();
    Dictionary<Unit, GameObject> unitGOMap = new Dictionary<Unit, GameObject>();

    public void UpdateDicts(Unit unit, Node startNode, Node goalNode)
    {
        if (!UnitNodeMap.ContainsKey(goalNode))
        {
            UnitNodeMap[goalNode] = unit;
            NodeUnitViewMap[goalNode] = unit.gameObject;
            UnitNodeMap.Remove(startNode);
            NodeUnitViewMap.Remove(startNode);
        }
    }
}
