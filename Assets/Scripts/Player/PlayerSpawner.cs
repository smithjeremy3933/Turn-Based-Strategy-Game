using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Dictionary<Node, Unit> unitNodeMap;
    public Graph graph;
    public PlayerUnitView playerUnitView;
    public EnemyUnitView enemyUnitView;

    public void SpawnPlayer(Graph graph, GameObject player, int xIndex, int yIndex)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, UnitType.player);
        newUnit.currentNode = node;
        newUnit.position = node.position;
        GameObject instance = Instantiate(player, node.position, Quaternion.identity, this.transform);
        playerUnitView.Init(newUnit);

        if (unitNodeMap == null)
        {
            unitNodeMap = new Dictionary<Node, Unit>();
            unitNodeMap[node] = newUnit;
        }
        else
        {
            unitNodeMap[node] = newUnit;
        }
    }

    public void SpawnEnemy(Graph graph, GameObject enemy, int xIndex, int yIndex)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, UnitType.enemy);
        newUnit.currentNode = node;
        newUnit.position = node.position;
        GameObject instance = Instantiate(enemy, node.position, Quaternion.identity, this.transform);
        enemyUnitView.Init(newUnit);

        if (unitNodeMap == null)
        {
            unitNodeMap = new Dictionary<Node, Unit>();
            unitNodeMap[node] = newUnit;
        }
        else
        {
            unitNodeMap[node] = newUnit;
        }
    }
}
