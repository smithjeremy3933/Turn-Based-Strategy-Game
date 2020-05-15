using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerUnitView[,] playerUnitViews;
    public Graph graph;

    
    public void SpawnPlayer(Graph graph, GameObject player, int xIndex, int yIndex)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, UnitType.player);
        GameObject instance = Instantiate(player, node.position, Quaternion.identity, this.transform);
        PlayerUnitView playerUnitView = instance.GetComponent<PlayerUnitView>();

        if (playerUnitViews == null)
        {
            playerUnitViews = new PlayerUnitView[graph.Width, graph.Height];
            playerUnitViews[xIndex, yIndex] = playerUnitView;
        }
        else
        {
            playerUnitViews[xIndex, yIndex] = playerUnitView;
        }

    }
}
