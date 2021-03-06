﻿using System.Collections;
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
    [SerializeField] GameObject enemyUnitViewPrefab;
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
            if (playerUnitViewPrefab != null)
            {
                playerSpawner.SpawnPlayer(graph, playerUnitViewPrefab, 4, 6, "Player 1");
                playerSpawner.SpawnPlayer(graph, playerUnitViewPrefab, 5, 5, "Player 2");
                playerSpawner.SpawnEnemy(graph, enemyUnitViewPrefab, 9, 6, "Enemy 1");
                playerSpawner.SpawnEnemy(graph, enemyUnitViewPrefab, 8, 4, "Enemy 2");
            }          
        }
    }
}
