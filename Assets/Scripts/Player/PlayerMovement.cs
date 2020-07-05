using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event EventHandler<OnUnitMovedEventArgs> OnUnitMoved;
    public class OnUnitMovedEventArgs : EventArgs
    {
        public Unit currentUnit;
        public Node startNode;
        public Node endNode;
    }

    PlayerManager m_playerManager;
    List<Node> currentPath;
    Graph m_graph;
    float moveDelay = 0.2f;

    private void Start()
    {
        m_playerManager = FindObjectOfType<PlayerManager>();
    }

    public void Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder)
    {
        unit.isPathfinding = false;
        currentPath = pathfinder.GetPath(hitNode, unit);
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
        Node startNode = path[0];
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(moveDelay);
            if (node != startNode)
            {
                Graph graph = FindObjectOfType<Graph>();
                m_graph = graph;
                float distanceBetweenNodes = m_graph.GetNodeDistance(node.previous, node);
                unit.actionPoints -= distanceBetweenNodes;
            }

            if (isEnemySelected)
            {
                UpdateUnitPosData(unit, unitView, node);
            }

            if (!isEnemySelected)
            {
                UpdateUnitPosData(unit, unitView, node);
            }
        }
        Unit currentUnit = unit;
        Node currentNode = unit.currentNode;
        unit.hasMoved = true;
        if (unit.actionPoints < 1)
        {
            unit.isWaiting = true;
        }
        OnUnitMoved?.Invoke(this, new OnUnitMovedEventArgs { startNode = startNode, currentUnit = currentUnit, endNode = currentNode });
        m_playerManager.DeselectUnit(unit);
        Cursor.visible = true;
    }

    public static void UpdateUnitPosData(Unit unit, GameObject unitView, Node node)
    {
        unitView.transform.position = node.position;
        unit.position = node.position;
        unit.currentNode = node;
        unit.xIndex = node.xIndex;
        unit.yIndex = node.yIndex;
    }
}