using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitView : MonoBehaviour
{
    PlayerManager playerManager;
    Vector3 startPosition;
    Node m_startNode;
    Node m_goalNode;
    public bool isSelected;
    Graph m_graph;
    Pathfinder m_pathfinder;
    GraphView m_graphView;
    NodeView nodeView;

    public void Init(Unit unit)
    {
        gameObject.name = "Player Unit " + unit.name;
        gameObject.transform.position = unit.position;
        m_startNode = unit.currentNode;
        print(gameObject.transform.position);
    }
}
