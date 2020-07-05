using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] public Inventory inventory;

    public Graph graph;
    public PlayerUnitView playerUnitView;
    public EnemyUnitView enemyUnitView;
    public event EventHandler<OnUnitSpawnedEventArgs> OnUnitSpawned;
    public class OnUnitSpawnedEventArgs : EventArgs
    {
        public Unit newUnit;
        public Node node;
        public GameObject instance;
    }

    public void SpawnPlayer(Graph graph, GameObject player, int xIndex, int yIndex, string name)
    {
        ItemDatabase itemDatabase = FindObjectOfType<ItemDatabase>();
        itemDatabase.BuildDatabase();
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.player, name);
        GameObject instance = Instantiate(player, node.position, Quaternion.identity, this.transform);
        SetUnitWeapons(newUnit);
        playerUnitView.Init(newUnit);
        OnUnitSpawned?.Invoke(this, new OnUnitSpawnedEventArgs { newUnit = newUnit, instance = instance, node = node });
    }

    public void SpawnEnemy(Graph graph, GameObject enemy, int xIndex, int yIndex, string name)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.enemy, name);
        GameObject instance = Instantiate(enemy, node.position, Quaternion.identity, this.transform);
        SetUnitWeapons(newUnit);
        enemyUnitView.Init(newUnit);
        OnUnitSpawned?.Invoke(this, new OnUnitSpawnedEventArgs { newUnit = newUnit, instance = instance, node = node });
    }

    private void SetUnitWeapons(Unit newUnit)
    {
        if (newUnit != null)
        {
            inventory.SpawnItemToUnit(newUnit, 0);
            inventory.SpawnItemToUnit(newUnit, 1);
            newUnit.GetWeaponStats(newUnit, newUnit.unitInventory[0]);
        }
    }
}
