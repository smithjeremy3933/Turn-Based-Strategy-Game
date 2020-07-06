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
    // Unit Identification
    public float unitID;
    public UnitType unitType = UnitType.player;
    public string name;

    // Unit Stats
    public float movementRange = 5f;
    public float health = 100;

    public float baseAttackDamage = 10f;
    public float equippedATK;

    public float baseHIT = 90f;
    public float equippedHIT;

    public float baseCRIT = 5f;
    public float equippedCRIT;

    public float maxActionPoints = 7f;
    public float actionPoints = 7f;
    public float calcAPC;

    public Item equippedWeapon;

    public List<Unit> surroundingEnemies;
    public List<Item> unitInventory = new List<Item>();

    public bool hasMoved = false;
    public bool isWaiting = false;
    public bool isSelected = false;
    public bool isSurrEnemies = false;
    public bool isAttacking = false;
    public bool isPathfinding = false;

    // Positional Data
    public GameObject gameObject;
    public Vector3 position;
    public Node currentNode;
    public int xIndex = -1;
    public int yIndex = -1;

    public Unit( int xIndex, int yIndex)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
    }

    public Unit(int xIndex, int yIndex, Node node, UnitType unitType, string name)
        : this(xIndex, yIndex)
    {
        this.currentNode = node;
        this.unitType = unitType;
        this.name = name;
        this.unitID = Random.Range(100f, 10000f);
    }

    public void ProcessTurn()
    {
        ResetActionPoints(this.actionPoints);
        hasMoved = false;
        isWaiting = false;
        isAttacking = false;
        isPathfinding = false;
    }

    public void ResetActionPoints()
    {
        ResetActionPoints(maxActionPoints);
    }

    private void ResetActionPoints(float AP)
    {
        actionPoints = AP;
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

    public void GetWeaponStats(Unit playerUnit, Item currentItem)
    {
        if (playerUnit != null || currentItem != null)
        {
            equippedWeapon = currentItem;
            equippedATK = playerUnit.baseAttackDamage + currentItem.stats["ATK"];
            equippedHIT = playerUnit.baseHIT + currentItem.stats["HIT"];
            equippedCRIT = playerUnit.baseCRIT + currentItem.stats["CRIT"];
            calcAPC = playerUnit.actionPoints - currentItem.stats["APC"];
        }
    }

    public void GetEquippedStats(Unit playerUnit)
    {
        if (playerUnit != null || equippedWeapon != null)
        {
            equippedATK = playerUnit.baseAttackDamage + equippedWeapon.stats["ATK"];
            equippedHIT = playerUnit.baseHIT + equippedWeapon.stats["HIT"];
            equippedCRIT = playerUnit.baseCRIT + equippedWeapon.stats["CRIT"];
            calcAPC = playerUnit.actionPoints - equippedWeapon.stats["APC"];
        }
    }

    public void ResetSurroundingEnemies(Unit unit)
    {
        surroundingEnemies = null;
        unit.isSurrEnemies = false;
    }
}