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

    private void Start()
    {
        m_playerManager = FindObjectOfType<PlayerManager>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        uiController = FindObjectOfType<UIController>();
    }

    public void Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder)
    {
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
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(0.5f);
            if (node != m_playerManager.startNode)
            {
                unit.actionPoints -= node.movementCost;
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
        //if (isEnemySelected)
        //{
        //    unit.Attack(m_playerSpawner, goalNode);
        //}
        m_playerSpawner.UpdateDicts(unit, m_playerManager.startNode, unit.currentNode);
        m_playerManager.DeselectUnit(unit);
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