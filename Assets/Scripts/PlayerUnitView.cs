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

    private void Start()
    {
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_graphView = FindObjectOfType<GraphView>();
    }


    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectPlayerUnit();
        }

    }

    private void SelectPlayerUnit()
    {
        if (isSelected == false)
        {
            isSelected = true;
            startPosition = gameObject.transform.position;
            m_startNode = m_graph.GetNodeAt((int)startPosition.x, (int)startPosition.y);
        }
    }

    private void CalculatePath(Node endNode)
    {
        m_goalNode = m_graph.GetNodeAt(4, 4);
        m_pathfinder.Init(m_graph, m_graphView, m_startNode, m_goalNode);
        m_pathfinder.SearchRoutine();
        Debug.Log("My path is " + m_pathfinder.PathNodes);
    }

    public void Init(Unit unit)
    {
        gameObject.name = "Player Unit " + unit.position;
        gameObject.transform.position = unit.position;
        m_startNode = unit.currentNode;
    }
}
