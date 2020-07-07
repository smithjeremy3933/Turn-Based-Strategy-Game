using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public List<Unit> AllUnits { get => m_allUnits; }
    public List<Unit> PlayerUnits { get => m_playerUnits; }
    public List<Unit> EnemyUnits { get => m_enemyUnits; }
    public List<Unit> KilledPlayerUnits { get => m_killedPlayerUnits; }
    public Dictionary<Node, Unit> UnitNodeMap { get => unitNodeMap; }
    public Dictionary<Node, GameObject> NodeUnitViewMap { get => nodeUnitViewMap; }
    public Dictionary<Unit, GameObject> UnitGOMap { get => unitGOMap; }

    List<Unit> m_allUnits = new List<Unit>();
    List<Unit> m_playerUnits = new List<Unit>();
    List<Unit> m_enemyUnits = new List<Unit>();
    List<Unit> m_killedPlayerUnits = new List<Unit>();
    Dictionary<Node, Unit> unitNodeMap = new Dictionary<Node, Unit>();
    Dictionary<Node, GameObject> nodeUnitViewMap = new Dictionary<Node, GameObject>();
    Dictionary<Unit, GameObject> unitGOMap = new Dictionary<Unit, GameObject>();

    private void Awake()
    {
        PlayerSpawner playerSpawner = FindObjectOfType<PlayerSpawner>();
        playerSpawner.OnUnitSpawned += PlayerSpawner_OnUnitSpawned;
    }

    private void Start()
    {
        PlayerMovement.OnUnitMoved += PlayerMovement_OnUnitMoved;
        EnemyMovement.OnEnemyMoved += EnemyMovement_OnEnemyMoved;
        UnitView.OnUnitDeath += PlayerUnitView_OnUnitDeath;
    }

    private void PlayerUnitView_OnUnitDeath(object sender, UnitView.OnUnitDeathEventArgs e)
    {
        KilledPlayerUnits.Add(e.deadUnit);
        AllUnits.Remove(e.deadUnit);
        PlayerUnits.Remove(e.deadUnit);
        UnitNodeMap.Remove(e.deadUnit.currentNode);
        NodeUnitViewMap.Remove(e.deadUnit.currentNode);
        UnitGOMap.Remove(e.deadUnit);
        Debug.Log("Player unit deleted from active list of unit");
    }

    private void EnemyMovement_OnEnemyMoved(object sender, EnemyMovement.OnEnemyMovedEventArgs e)
    {
        UpdateDicts(e.currentEnemy, e.startNode, e.endNode);
    }

    private void PlayerMovement_OnUnitMoved(object sender, PlayerMovement.OnUnitMovedEventArgs e)
    {
        Debug.Log("Player unit just moved");
        Unit movedUnit = e.currentUnit;
        Node startNode = e.startNode;
        Node endNode = e.endNode;
        UpdateDicts(movedUnit, startNode, endNode);
    }

    private void PlayerSpawner_OnUnitSpawned(object sender, PlayerSpawner.OnUnitSpawnedEventArgs e)
    {
        Unit newUnit = e.newUnit;
        Node node = e.node;
        GameObject instance = e.instance;
        newUnit.currentNode = node;
        newUnit.position = node.position;
        newUnit.gameObject = instance;

        if (newUnit.unitType == UnitType.player)
        {
            PlayerUnits.Add(newUnit);
        }
        else
        {
            EnemyUnits.Add(newUnit);
        }

        AllUnits.Add(newUnit);
        NodeUnitViewMap[node] = newUnit.gameObject;
        UnitGOMap[newUnit] = instance;
        UnitNodeMap[node] = newUnit;       
    }

    public Queue<Unit> GetEnemeiesForTurn()
    {
        Queue<Unit> enemies = new Queue<Unit>();
        foreach (Unit enemy in m_enemyUnits)
        {
            enemies.Enqueue(enemy);
        }
        return enemies;
    }

    public void UpdateDicts(Unit unit, Node startNode, Node goalNode)
    {
        if (!UnitNodeMap.ContainsKey(goalNode))
        {
            UnitNodeMap[goalNode] = unit;
            NodeUnitViewMap[goalNode] = unit.gameObject;
            UnitNodeMap.Remove(startNode);
            NodeUnitViewMap.Remove(startNode);
        }
    }
}