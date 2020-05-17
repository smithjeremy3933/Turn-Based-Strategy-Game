using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Node startNode;
    public Node goalNode;
    Vector3 startPos;
    Vector3 goalPos;
    PlayerSpawner playerSpawner;
    public bool isSelected;
    Graph m_graph;
    Pathfinder m_pathfinder;
    GraphView m_graphView;
    Ray ray;

    private void Start()
    {
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_graphView = FindObjectOfType<GraphView>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit && isSelected == false && hit.rigidbody != null)
            {
                isSelected = true;
                startPos = hit.transform.position;
                startNode = m_graph.GetNodeAt((int)startPos.x, (int)startPos.z);
            }
            else if (hasHit && isSelected == true && startNode != null && startPos != hit.transform.position)
            {
                // goalnode hit
                goalPos = hit.transform.position;
                goalNode = m_graph.GetNodeAt((int)goalPos.x, (int)goalPos.z);
                CalculatePath(startNode, goalNode);
            }
        }


    }

    private void CalculatePath(Node start, Node end)
    {
        m_pathfinder.Init(m_graph, m_graphView, startNode, goalNode);
        m_pathfinder.SearchRoutine();
        Debug.Log("My path is " + m_pathfinder.PathNodes);
    }
}
