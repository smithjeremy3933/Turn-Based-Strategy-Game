﻿using System;
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
    float movementCost = 1f;
    Vector3 mouseOverPosition;
    List<Node> currentPath;


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
            if (hasHit && isSelected == false && hit.rigidbody != null && mouseOverPosition == hit.transform.position)
            {
                ProcessUnitMoveSelection(hit);
            }
            else if (hasHit && isSelected == true && startNode != null && startPos != hit.transform.position && isGoalSelected == false)
            {
                // goalnode hit
                Node hitGoalNode = m_graph.GetNodeAt((int)hit.transform.position.x, (int)hit.transform.position.z);
                float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitGoalNode);
                if (distanceBetweenNodes > currentUnit.movementRange)
                {
                    Debug.Log("Cannot select goal node outside of the current unit movement Range " + distanceBetweenNodes);
                    return;
                }
                if (m_playerSpawner.unitNodeMap.ContainsKey(hitGoalNode))
                {
                    Debug.Log("Node already occupied");
                    return;
                }
                if (currentUnit.actionPoints < distanceBetweenNodes)
                {
                    Debug.Log("Not enough action points!!");
                    return;
                }
                ProcessMoveToValidGoal(hit);
            }
            else
            {
                isSelected = false;
            }
        }
    }

    private void ProcessUnitMoveSelection(RaycastHit hit)
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

    private void ProcessMoveToValidGoal(RaycastHit hit)
    {
        isGoalSelected = true;
        goalPos = hit.transform.position;
        goalNode = m_graph.GetNodeAt((int)goalPos.x, (int)goalPos.z);
        if (!m_playerSpawner.unitNodeMap.ContainsKey(goalNode))
        {
            m_playerSpawner.unitNodeMap[goalNode] = currentUnit;
            m_playerSpawner.unitNodeMap.Remove(startNode);
        }
        Debug.Log(startNode.position);
        Debug.Log(goalNode.position);
        CalculatePath(startNode, goalNode, currentUnit);
        if (currentPath != null)
        {
            StartCoroutine(FollowPath(currentPath, currentUnit));
        }
        if (currentPath == null)
        {
            Debug.Log("null path");
        }
        currentUnit.position = goalPos;
        currentUnit.currentNode = goalNode;
    }

    private void OnMouseOver()
    {
        mouseOverPosition = gameObject.transform.position;
    }

    private void HighlightUnitMovementRange(Unit unit)
    {
        Debug.Log("Highlight unit's movement range");
    }

    private void CalculatePath(Node start, Node goal, Unit unit)
    {
        m_pathfinder.Init(m_graph, m_graphView, start, goal);
        currentPath = m_pathfinder.SearchRoutine(unit, m_graph);
        if (currentPath.Count <= 1)
        {
            Debug.Log("There should never be less then two node in the path");
        }
    }

    IEnumerator FollowPath(List<Node> path, Unit unit)
    {
        Debug.Log(path.Count);
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(0.5f);
            if (node != startNode)
            {
                unit.actionPoints -= node.movementCost;
            }

            transform.position = node.position;
            unit.position = node.position;
            unit.currentNode = node;   
        }
        isGoalSelected = false;
        isSelected = false;
        startNode = null;
        goalNode = null;
        currentPath = null;
    }
}
