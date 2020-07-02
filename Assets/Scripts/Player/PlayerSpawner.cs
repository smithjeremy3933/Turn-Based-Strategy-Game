using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]public UnitDatabase unitDatabase;
    [SerializeField] public Inventory inventory;

    public Graph graph;
    public PlayerUnitView playerUnitView;
    public EnemyUnitView enemyUnitView;

    public void SpawnPlayer(Graph graph, GameObject player, int xIndex, int yIndex, string name)
    {
        ItemDatabase itemDatabase = FindObjectOfType<ItemDatabase>();
        itemDatabase.BuildDatabase();
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.player, name);
        newUnit.currentNode = node;
        newUnit.position = node.position;
        SetUnitWeapons(newUnit);
        unitDatabase.PlayerUnits.Add(newUnit);
        unitDatabase.AllUnits.Add(newUnit);
        GameObject instance = Instantiate(player, node.position, Quaternion.identity, this.transform);
        newUnit.gameObject = instance;
        playerUnitView.Init(newUnit);
        unitDatabase.NodeUnitViewMap[node] = newUnit.gameObject;
        unitDatabase.UnitGOMap[newUnit] = instance;

        if (unitDatabase.UnitNodeMap == null)
        {
            unitDatabase.UnitNodeMap = new Dictionary<Node, Unit>();
            unitDatabase.UnitNodeMap[node] = newUnit;
        }
        else
        {
            unitDatabase.UnitNodeMap[node] = newUnit;
        }
    }

    private void SetUnitWeapons(Unit newUnit)
    {
        if (newUnit != null)
        {
            inventory.SpawnItemToUnit(newUnit, 0);
            inventory.SpawnItemToUnit(newUnit, 1);
            newUnit.equippedWeapon = newUnit.unitInventory[0];
        }
    }

    public void SpawnEnemy(Graph graph, GameObject enemy, int xIndex, int yIndex, string name)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.enemy, name);
        newUnit.currentNode = node;
        newUnit.position = node.position;
        SetUnitWeapons(newUnit);
        unitDatabase.AllUnits.Add(newUnit);

        GameObject instance = Instantiate(enemy, node.position, Quaternion.identity, this.transform);      
        enemyUnitView.Init(newUnit);
        newUnit.gameObject = instance;
        unitDatabase.NodeUnitViewMap[node] = newUnit.gameObject;
        unitDatabase.UnitGOMap[newUnit] = instance;

        if (unitDatabase.UnitNodeMap == null)
        {
            unitDatabase.UnitNodeMap = new Dictionary<Node, Unit>();
            unitDatabase.UnitNodeMap[node] = newUnit;
        }
        else
        {
            unitDatabase.UnitNodeMap[node] = newUnit;
        }
    }
}
