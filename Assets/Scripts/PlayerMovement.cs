using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Node startNode;
    public Node goalNode;
    public Color movementRangeColor = Color.cyan;
    Vector3 startPos;
    Vector3 goalPos;
    public bool isSelected;
    public bool isGoalSelected;
    Graph m_graph;
    Pathfinder m_pathfinder;
    GraphView m_graphView;
    PlayerSpawner m_playerSpawner;
    Ray ray;
    Unit currentUnit;


    private void Start()
    {
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_graphView = FindObjectOfType<GraphView>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
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
                if (m_playerSpawner.unitNodeMap.ContainsKey(startNode))
                {
                    currentUnit = m_playerSpawner.unitNodeMap[startNode];
                }
                HighlightUnitMovementRange(currentUnit);
            }
            else if (hasHit && isSelected == true && startNode != null && startPos != hit.transform.position && isGoalSelected == false)
            {
                // goalnode hit
                isGoalSelected = true;
                goalPos = hit.transform.position;
                goalNode = m_graph.GetNodeAt((int)goalPos.x, (int)goalPos.z);
                currentUnit.position = goalPos;
                currentUnit.currentNode = goalNode;
                if (!m_playerSpawner.unitNodeMap.ContainsKey(goalNode))
                {
                    m_playerSpawner.unitNodeMap[goalNode] = currentUnit;
                    m_playerSpawner.unitNodeMap[startNode] = null;
                }
                CalculatePath(startNode, goalNode);
                var path = m_pathfinder.PathNodes;
                if (path != null)
                {
                    StartCoroutine(FollowPath(path));
                }
            }
        }
    }

    private void HighlightUnitMovementRange(Unit unit)
    {
        Debug.Log("Highlight unit's movement range");
    }

    private void CalculatePath(Node start, Node end)
    {
        m_pathfinder.Init(m_graph, m_graphView, startNode, goalNode);
        m_pathfinder.SearchRoutine();
    }

    IEnumerator FollowPath(List<Node> path)
    {
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(1f);
            transform.position = node.position;
        }
        isGoalSelected = false;
        isSelected = false;
        startNode = null;
        goalNode = null;
    }
}
