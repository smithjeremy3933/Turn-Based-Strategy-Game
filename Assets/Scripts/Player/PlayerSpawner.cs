using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    Dictionary<Node, Unit> unitNodeMap;
    Dictionary<Node, GameObject> nodeUnitViewMap = new Dictionary<Node, GameObject>();
    List<Unit> playerUnits = new List<Unit>();
    private List<Unit> allUnits = new List<Unit>();

    public Graph graph;
    public PlayerUnitView playerUnitView;
    public EnemyUnitView enemyUnitView;
    public Dictionary<Node, GameObject> NodeUnitViewMap { get => nodeUnitViewMap; set => nodeUnitViewMap = value; }
    public Dictionary<Node, Unit> UnitNodeMap { get => unitNodeMap; set => unitNodeMap = value; }
    public List<Unit> PlayerUnits { get => playerUnits; set => playerUnits = value; }
    public List<Unit> AllUnits { get => allUnits; set => allUnits = value; }

    public void SpawnPlayer(Graph graph, GameObject player, int xIndex, int yIndex)
    {
        Node node = graph.GetNodeAt(xIndex, yIndex);
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.player);
        newUnit.currentNode = node;
        newUnit.position = node.position;
        playerUnits.Add(newUnit);
        allUnits.Add(newUnit);
        GameObject instance = Instantiate(player, node.position, Quaternion.identity, this.transform);
        newUnit.gameObject = instance;
        playerUnitView.Init(newUnit);
        nodeUnitViewMap[node] = newUnit.gameObject;

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
        Unit newUnit = new Unit(xIndex, yIndex, node, UnitType.enemy);
        newUnit.currentNode = node;
        newUnit.position = node.position;
        allUnits.Add(newUnit);

        GameObject instance = Instantiate(enemy, node.position, Quaternion.identity, this.transform);      
        enemyUnitView.Init(newUnit);
        newUnit.gameObject = instance;
        nodeUnitViewMap[node] = newUnit.gameObject;

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
