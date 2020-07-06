using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public static event EventHandler<OnEnemyMovedEventArgs> OnEnemyMoved;
    public class OnEnemyMovedEventArgs : EventArgs
    {
        public Unit currentEnemy;
        public Node startNode;
        public Node endNode;
    }

    UnitDatabase unitDatabase;
    UIController uiController;
    List<Node> currentPath;
    Graph graph;
    bool isUnitMoving;
    float moveDelay = 0.2f;

    private void Start()
    {
        unitDatabase = FindObjectOfType<UnitDatabase>();
        uiController = FindObjectOfType<UIController>();
        graph = FindObjectOfType<Graph>();
    }

    public IEnumerator Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder)
    {
        List<Node> currentPath = pathfinder.GetPath(hitNode, unit);
        if (currentPath != null && hitNode != null)
        {
            yield return StartCoroutine(FollowPath(currentPath, unit, unitView));
        }
    }

    public IEnumerator FollowPath(List<Node> path, Unit unit, GameObject unitView)
    {
        Node startNode = path[0];
        foreach (Node node in path)
        {
            if (unit.actionPoints != 0)
            {
                yield return new WaitForSeconds(moveDelay);
                if (!unitDatabase.UnitNodeMap.ContainsKey(node))
                {
                    float distanceBetweenNodes = graph.GetNodeDistance(node.previous, node);
                    unit.actionPoints -= distanceBetweenNodes;
                    UpdateUnitPosData(uiController, unit, unitView, node);
                }
            }
        }

        OnEnemyMoved?.Invoke(this, new OnEnemyMovedEventArgs { currentEnemy = unit, startNode = startNode, endNode = unit.currentNode });
        unit.hasMoved = true;
        SensePlayerUnits(unit, unitDatabase);

        if (unit.surroundingEnemies != null)
        {
            yield return StartCoroutine(gameObject.GetComponent<EnemyAttack>().AttackPlayer(unit));
        }
        else
        {
            unit.isWaiting = true;
            unit.ResetActionPoints();
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

    public void SensePlayerUnits(Unit enemy, UnitDatabase unitDatabase)
    {
        enemy.ResetSurroundingEnemies(enemy);
        enemy.surroundingEnemies = new List<Unit>();
        var hitNodesNieghbors = graph.GetNeighbors(enemy.currentNode.xIndex, enemy.currentNode.yIndex);
        foreach (Node node in hitNodesNieghbors)
        {
            if (unitDatabase.UnitNodeMap.ContainsKey(node))
            {
                if (unitDatabase.UnitNodeMap[node] != null)
                {
                    Unit player = unitDatabase.UnitNodeMap[node];
                    if (player.unitType == UnitType.player)
                    {
                        enemy.surroundingEnemies.Add(player);
                        enemy.isSurrEnemies = true;
                    }
                }
            }
        }
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
                if (potentialPath.Count < shortestPath.Count || shortestPath.Count == 0)
                {
                    closestUnit = unit;
                    shortestPath = potentialPath;
                }
            }
            return closestUnit;
        }
        else
        {
            return null;
        }
    }
}
