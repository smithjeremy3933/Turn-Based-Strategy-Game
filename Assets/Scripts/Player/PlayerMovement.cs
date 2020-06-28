using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerManager m_playerManager;
    PlayerSpawner m_playerSpawner;
    UIController uiController;
    List<Node> currentPath;
    Graph m_graph;
    float moveDelay = 0.2f;

    private void Start()
    {
        m_playerManager = FindObjectOfType<PlayerManager>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        uiController = FindObjectOfType<UIController>();
    }

    public void Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder)
    {
        unit.isPathfinding = false;
        currentPath = pathfinder.GetPath(hitNode, unit);
        m_playerManager.CurrentPath = currentPath;
        if (currentPath != null && hitNode != null)
        {
            StartCoroutine(FollowPath(currentPath, unit, unitView, false));
        }
        if (currentPath == null)
        {
            Debug.Log("null path");
        }
    }

    public IEnumerator FollowPath(List<Node> path, Unit unit, GameObject unitView, bool isEnemySelected)
    {
        Cursor.visible = false;
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(moveDelay);
            if (node != m_playerManager.startNode)
            {
                Graph graph = FindObjectOfType<Graph>();
                m_graph = graph;
                float distanceBetweenNodes = m_graph.GetNodeDistance(node.previous, node);
                unit.actionPoints -= distanceBetweenNodes;
            }

            if (isEnemySelected)
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }

            if (!isEnemySelected)
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }
        }
        m_playerSpawner.UpdateDicts(unit, m_playerManager.startNode, unit.currentNode);
        unit.hasMoved = true;
        if (unit.actionPoints < 1)
        {
            unit.isWaiting = true;
        }
        m_playerManager.DeselectUnit(unit);
        Cursor.visible = true;
    }

    public static void UpdateUnitPosData(UIController uiController, Unit unit, GameObject unitView, Node node)
    {
        uiController.UpdateUnitSelectText(unit);
        unitView.transform.position = node.position;
        unit.position = node.position;
        unit.currentNode = node;
        unit.xIndex = node.xIndex;
        unit.yIndex = node.yIndex;
    }
}