using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public MapData mapData;
    public Graph graph;
    public GraphView graphView;
    public Pathfinder pathfinder;
    public PlayerSpawner playerSpawner;
    [SerializeField] GameObject playerUnitViewPrefab;
    public int startX = 0;
    public int startY = 0;
    public int goalX = 1;
    public int goalY = 1;

    public void Awake()
    {

        if (mapData != null && graph != null)
        {
            int[,] mapInstance = mapData.MakeMap();
            graph.Init(mapInstance);
            if (graphView != null)
            {
                graphView.Init(graph); 
            }

            if (graph.IsWithinBounds(startX, startY) && graph.IsWithinBounds(goalX, goalY) && pathfinder != null)
            {
                Node startNode = graph.nodes[startX, startY];
                Node goalNode = graph.nodes[goalX, goalY];
                pathfinder.Init(graph, graphView, startNode, goalNode);
                pathfinder.SearchRoutine();
            }
            if (playerUnitViewPrefab != null)
            {
                playerSpawner.SpawnPlayer(graph, playerUnitViewPrefab, startX, startY);
            }

            
        }
    }
}
