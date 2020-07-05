using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public List<Unit> AllUnits { get => m_allUnits; set => m_allUnits = value; }
    public List<Unit> PlayerUnits { get => m_playerUnits; set => m_playerUnits = value; }
    public List<Unit> EnemyUnits { get => m_enemyUnits; set => m_enemyUnits = value; }
    public List<Unit> KilledPlayerUnits { get => m_killedPlayerUnits; set => m_killedPlayerUnits = value; }
    public Dictionary<Node, Unit> UnitNodeMap { get => unitNodeMap; set => unitNodeMap = value; }
    public Dictionary<Node, GameObject> NodeUnitViewMap { get => nodeUnitViewMap; set => nodeUnitViewMap = value; }
    public Dictionary<Unit, GameObject> UnitGOMap { get => unitGOMap; set => unitGOMap = value; }

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
        Debug.Log("Unit was spawned on the map " + e.newUnit.name);
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

        if (UnitNodeMap == null)
        {
            UnitNodeMap = new Dictionary<Node, Unit>();
            UnitNodeMap[node] = newUnit;
        }
        else
        {
            UnitNodeMap[node] = newUnit;
        }
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
