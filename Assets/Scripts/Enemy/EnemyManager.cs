using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    TurnManager turnManager;
    Pathfinder pathfinder;
    UnitDatabase unitDatabase;
    Graph graph;
    UIController uiController;
    Node startNode;
    Queue<Unit> currentEnemies;
    bool isEnemyTurn = false;
    bool hasUnitFinished = true;
    float moveDelay = 0.2f;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        unitDatabase = FindObjectOfType<UnitDatabase>();
        uiController = FindObjectOfType<UIController>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    public IEnumerator RunEnemyTurn()
    {
        yield return StartCoroutine(InitEnemyTurn());
    }

    public IEnumerator InitEnemyTurn()
    {
        SelectionIndicator selectionIndicator = FindObjectOfType<SelectionIndicator>();
        selectionIndicator.HideSelectionIndicator();
        Cursor.visible = false;
        Debug.Log("Start of Enemy Turn.");
        isEnemyTurn = true;
        currentEnemies = new Queue<Unit>();
        currentEnemies = unitDatabase.GetEnemeiesForTurn();
        ProcessEnemy(currentEnemies, pathfinder);

        while (isEnemyTurn)
        {
            yield return null;
        }

        Debug.Log("End of Enemy Turn.");
        Cursor.visible = true;
        selectionIndicator.ShowSelectionIndicator();
        currentEnemies = null;
        turnManager.currentTurn = Turn.playerTurn;
    }

    void ProcessEnemy(Queue<Unit> enemies, Pathfinder pathfinder)
    {
        if (enemies != null)
        {
            while (enemies.Count > 0)
            {
                Unit enemy = enemies.Dequeue();
                GameObject enemyView = enemy.gameObject;
                Move(unitDatabase.PlayerUnits[0].currentNode, enemy, enemyView, pathfinder);
            }
        }
        isEnemyTurn = false;
    }

    public void Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder)
    {
        startNode = unit.currentNode;
        List<Node> currentPath = pathfinder.GetPath(hitNode, unit);
        if (currentPath != null && hitNode != null)
        {
            StartCoroutine(FollowPath(currentPath, unit, unitView));
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
            if (node != startNode)
            {
                Graph graph = FindObjectOfType<Graph>();
                this.graph = graph;
                //float distanceBetweenNodes = this.graph.GetNodeDistance(node.previous, node);
                //unit.actionPoints -= distanceBetweenNodes;
            }
            
            if (node != path[path.Count -1])
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }
        }
        unitDatabase.UpdateDicts(unit, startNode, unit.currentNode);
        unit.hasMoved = true;
        startNode = null;
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
}