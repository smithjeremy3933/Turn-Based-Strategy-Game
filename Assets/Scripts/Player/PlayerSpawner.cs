﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]public UnitDatabase unitDatabase;

    public Graph graph;
    public PlayerUnitView playerUnitView;
    public EnemyUnitView enemyUnitView;

    public void SpawnPlayer(Graph graph, GameObject player, int xIndex, int yIndex)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.player);
        newUnit.currentNode = node;
        newUnit.position = node.position;
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

    public void SpawnEnemy(Graph graph, GameObject enemy, int xIndex, int yIndex)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.enemy);
        newUnit.currentNode = node;
        newUnit.position = node.position;
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
