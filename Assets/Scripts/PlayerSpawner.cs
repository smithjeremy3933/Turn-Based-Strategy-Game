using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Dictionary<Node, Unit> unitNodeMap;
    public Graph graph;
    public PlayerUnitView playerUnitView;

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
}
