using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Node StartNode { get => startNode; set => startNode = value; }
    public event EventHandler<OnEnemyTurnEndedEventArgs> OnEnemyTurnEnded;
    public class OnEnemyTurnEndedEventArgs : EventArgs
    {
        public Unit enemy;
    }

    TurnManager turnManager;
    Pathfinder pathfinder;
    UnitDatabase unitDatabase;
    Graph graph;
    Node startNode;
    Queue<Unit> currentEnemies;
    bool isEnemyTurn = false;
    float moveDelay = 0.2f;

    private void Start()
    {
        TurnManager.OnTurnEnded += TurnManager_OnTurnEnded;
        unitDatabase = FindObjectOfType<UnitDatabase>();
        pathfinder = FindObjectOfType<Pathfinder>();
    }

    private void TurnManager_OnTurnEnded(object sender, System.EventArgs e)
    {
        StartCoroutine(InitEnemyTurn());
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
                        Unit closestPlayer = enemyMovement.FindClosestPlayer(enemy, unitDatabase, pathfinder);

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
        foreach (Unit enemy in unitDatabase.EnemyUnits)
        {
            enemy.ResetActionPoints();
        }
        Unit enemyUnit = unitDatabase.EnemyUnits[0];
        OnEnemyTurnEnded?.Invoke(this, new OnEnemyTurnEndedEventArgs { enemy = enemyUnit });
        selectionIndicator.ShowSelectionIndicator();
        currentEnemies = null;
    }
}