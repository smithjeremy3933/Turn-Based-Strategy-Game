using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Node StartNode { get => startNode; set => startNode = value; }

    TurnManager turnManager;
    Pathfinder pathfinder;
    UnitDatabase unitDatabase;
    Graph graph;
    UIController uiController;
    Node startNode;
    Queue<Unit> currentEnemies;
    bool isEnemyTurn = false;
    float moveDelay = 0.2f;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        unitDatabase = FindObjectOfType<UnitDatabase>();
        uiController = FindObjectOfType<UIController>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    public IEnumerator InitEnemyTurn()
    {
        SelectionIndicator selectionIndicator = FindObjectOfType<SelectionIndicator>();
        StartEnemyTurn(selectionIndicator);

        while (isEnemyTurn)
        {
            yield return StartCoroutine(ProcessEnemy(currentEnemies, pathfinder));
        }

        EndEnemyTurn(selectionIndicator);
    }

    IEnumerator ProcessEnemy(Queue<Unit> enemies, Pathfinder pathfinder)
    {
        if (enemies != null && pathfinder != null)
        {
            while (enemies.Count > 0)
            {
                Unit enemy = enemies.Dequeue();
                GameObject enemyView = enemy.gameObject;
                startNode = enemy.currentNode;
                if (enemy != null && enemyView != null)
                {
                    EnemyMovement enemyMovement = enemyView.GetComponent<EnemyMovement>();
                    if (enemyMovement != null)
                    {
                        enemyMovement.SensePlayerUnits(enemy, unitDatabase);

                        if (enemy.isSurrEnemies)
                        {
                            Debug.Log("Attack the player unit.");
                            EnemyAttack enemyAttack = enemyView.GetComponent<EnemyAttack>();
                            if (enemyAttack != null)
                            {
                                yield return StartCoroutine(enemyAttack.AttackPlayer(enemy));
                            }
                        }
                        else
                        {
                            Unit closestPlayer = enemyMovement.FindClosestPlayer(enemy, unitDatabase, pathfinder);

                            if (closestPlayer != null)
                            {
                                yield return StartCoroutine(enemyMovement.Move(closestPlayer.currentNode, enemy, enemyView, pathfinder));
                            }
                        }
                    }
                }
            }
        }
        isEnemyTurn = false;
    }

    private void StartEnemyTurn(SelectionIndicator selectionIndicator)
    {
        selectionIndicator.HideSelectionIndicator();
        Cursor.visible = false;
        Debug.Log("Start of Enemy Turn.");
        isEnemyTurn = true;
        currentEnemies = new Queue<Unit>();
        currentEnemies = unitDatabase.GetEnemeiesForTurn();
    }

    private void EndEnemyTurn(SelectionIndicator selectionIndicator)
    {
        Debug.Log("End of Enemy Turn.");
        Cursor.visible = true;
        selectionIndicator.ShowSelectionIndicator();
        currentEnemies = null;
        turnManager.currentTurn = Turn.playerTurn;
    }
}