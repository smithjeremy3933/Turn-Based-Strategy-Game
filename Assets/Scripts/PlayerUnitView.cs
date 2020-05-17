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

    private void Start()
    {
        m_graph = FindObjectOfType<Graph>();      
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // SelectPlayerUnit();
        }
    }

    private void SelectPlayerUnit()
    {
        if (isSelected == false)
        {
            startPosition = gameObject.transform.position;
            m_startNode = m_graph.GetNodeAt((int)startPosition.x, (int)startPosition.y);
        }
    }

    public void Init(Unit unit)
    {
        gameObject.name = "Player Unit " + unit.position;
        gameObject.transform.position = unit.position;
        m_startNode = unit.currentNode;
    }
}
