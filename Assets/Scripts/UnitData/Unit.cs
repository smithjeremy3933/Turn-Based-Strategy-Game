using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    player = 0,
    enemy = 1
}
public class Unit
{
    public float unitID;
    public UnitType unitType = UnitType.player;
    public int xIndex = -1;
    public int yIndex = -1;
    public string name = "Steve";
    public float movementRange = 5f;
    public int health = 100;
    public int baseAttackDamage = 10;
    public float actionPoints = 7f;

    public List<Unit> surroundingEnemies;
    public List<Item> unitInventory;

    public bool hasMoved = false;
    public bool isWaiting = false;
    public bool isSelected = false;
    public bool isSurrEnemies = false;
    public bool isAttacking = false;
    public bool isPathfinding = false;

    public GameObject gameObject;
    public Vector3 position;
    public Node currentNode;

    public Unit( int xIndex, int yIndex)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
    }

    public Unit(int xIndex, int yIndex, Node node, UnitType unitType)
        : this(xIndex, yIndex)
    {
        this.currentNode = node;
        this.unitType = unitType;
        this.unitID = Random.Range(100f, 10000f);
    }

    public void Attack(UnitDatabase unitDatabase, Node enemyNode)
    {
        Unit enemyUnit = unitDatabase.UnitNodeMap[enemyNode];
        enemyUnit.health -= baseAttackDamage;
    }

    public void ProcessTurn()
    {
        actionPoints = 7f;
        hasMoved = false;
        isWaiting = false;
        isAttacking = false;
        isPathfinding = false;
    }

    public void GetUnitNeighbors(Node hitNode, Unit unit, Graph graph, UnitDatabase unitDatabase)
    {
        ResetSurroundingEnemies(unit);
        surroundingEnemies = new List<Unit>();
        var hitNodesNieghbors = graph.GetNeighbors(hitNode.xIndex, hitNode.yIndex);
        foreach (Node node in hitNodesNieghbors)
        {
            if (unitDatabase.UnitNodeMap.ContainsKey(node))
            {
                if (unitDatabase.UnitNodeMap[node] != null)
                {
                    Unit enemy = unitDatabase.UnitNodeMap[node];
                    if (enemy.unitType == UnitType.enemy)
                    {
                        unit.surroundingEnemies.Add(enemy);
                        unit.isSurrEnemies = true;
                    }
                }
            }
        }
    }

    public void ResetSurroundingEnemies(Unit unit)
    {
        surroundingEnemies = null;
        unit.isSurrEnemies = false;
    }
}
