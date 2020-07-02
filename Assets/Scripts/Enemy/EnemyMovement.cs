using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    UnitDatabase unitDatabase;
    UIController uiController;
    List<Node> currentPath;
    Graph graph;
    EnemyManager enemyManager;
    bool isUnitMoving;
    float moveDelay = 0.2f;

    private void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        unitDatabase = FindObjectOfType<UnitDatabase>();
        uiController = FindObjectOfType<UIController>();
        graph = FindObjectOfType<Graph>();
    }

    public IEnumerator Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder)
    {
        Node startNode = enemyManager.StartNode;
        List<Node> currentPath = pathfinder.GetPath(hitNode, unit);
        if (currentPath != null && hitNode != null)
        {
            yield return StartCoroutine(FollowPath(currentPath, unit, unitView));
        }
        if (currentPath == null)
        {
            Debug.Log("null path");
        }
    }

    public IEnumerator FollowPath(List<Node> path, Unit unit, GameObject unitView)
    {
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(moveDelay);
            if (node != enemyManager.StartNode)
            {
                Graph graph = FindObjectOfType<Graph>();
                this.graph = graph;
                //float distanceBetweenNodes = this.graph.GetNodeDistance(node.previous, node);
                //unit.actionPoints -= distanceBetweenNodes;
            }

            if (!unitDatabase.UnitNodeMap.ContainsKey(node))
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }
        }

        unitDatabase.UpdateDicts(unit, enemyManager.StartNode, unit.currentNode);
        unit.hasMoved = true;
        enemyManager.StartNode = null;

        if (unit.actionPoints < 1)
        {
            unit.isWaiting = true;
        }
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

    public Unit FindClosestPlayer(Unit enemy, UnitDatabase unitDatabase, Pathfinder pathfinder)
    {
        if (unitDatabase != null && pathfinder != null)
        {
            List<Node> shortestPath = new List<Node>();
            Unit closestUnit = null;
            foreach (Unit unit in unitDatabase.PlayerUnits)
            {
                List<Node> potentialPath = pathfinder.GetPath(unit.currentNode, enemy);
                Debug.Log(potentialPath.Count);
                if (potentialPath.Count < shortestPath.Count || shortestPath.Count == 0)
                {
                    closestUnit = unit;
                    Debug.Log(closestUnit);
                    shortestPath = potentialPath;
                }
                Debug.Log(closestUnit.name);
            }
            return closestUnit;
        }
        else
        {
            return null;
        }
    }
}
