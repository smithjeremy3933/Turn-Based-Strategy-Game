using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Unit currentUnit;
    public GameObject currentUnitView;
    public Node startNode;
    public Node goalNode;
    public Color movementRangeColor = Color.cyan;
    public bool isGoalSelected;
    Vector3 startPos;
    Vector3 goalPos;
    Graph m_graph;
    Pathfinder m_pathfinder;
    GraphView m_graphView;
    MouseController m_mouseController;
    PlayerSpawner m_playerSpawner;
    PlayerUnitView m_playerUnitView;
    PlayerAttack m_playerAttack;
    Ray ray;
    Vector3 mouseOverPosition;
    List<Node> currentPath;
    UIController uiController;
    float maxDistance = 100f;
    bool isEnemySelected = false;


    private void Start()
    {
        m_mouseController = FindObjectOfType<MouseController>();
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_graphView = FindObjectOfType<GraphView>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        m_playerUnitView = FindObjectOfType<PlayerUnitView>();
        uiController = FindObjectOfType<UIController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LayerMask unitMask = LayerMask.GetMask("Unit");
            LayerMask tileMask = LayerMask.GetMask("Tile");
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHitUnit = Physics.Raycast(ray, out hit, maxDistance, unitMask);
            bool hasHitNode = Physics.Raycast(ray, out hit, maxDistance, tileMask);
            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHitNode && isEnemySelected)
            {
                Debug.Log("unit deselected");
                DeselectUnit(currentUnit);
            }

            if (hasHitUnit)
            {
                int xIndex = (int)hit.transform.position.x;
                int zIndex = (int)hit.transform.position.z;

                Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);
                if (hitNode != null)
                {
                    currentUnitView = m_mouseController.hoveredGameobject;
                    currentUnit = GetUnit(m_playerSpawner, hitNode);
                    if (currentUnit.unitType == UnitType.enemy)
                    {
                        isEnemySelected = true;
                    }
                    else if (currentUnit.unitType == UnitType.player)
                    {
                        isEnemySelected = false;
                    }
                    uiController.UpdateUnitSelectText(currentUnit);
                    HighlightUnitMovementRange(currentUnit);
                }               
            }

            if (hasHitNode && currentUnit != null && !isEnemySelected)
            {
                int xIndex = (int)hit.transform.position.x;
                int zIndex = (int)hit.transform.position.z;

                Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);

                if (hitNode != null && startNode != null && !m_playerSpawner.UnitNodeMap.ContainsKey(hitNode))
                {
                    float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitNode);
                    if (currentUnit.actionPoints < distanceBetweenNodes)
                    {
                        Debug.Log("Not enough action points!!");
                        return;
                    }
                    // A goal node was hit.Need to validate, move, and update data.
                    isGoalSelected = true;
                    goalNode = hitNode;
                    Move(hitNode, currentUnit, currentUnitView, m_pathfinder, m_playerSpawner);
                }
            }         
        }

        if (Input.GetMouseButtonDown(1) && currentUnit != null)
        {
            Debug.Log("unit deselected");
            DeselectUnit(currentUnit);
        }
    }

    private void Move(Node hitNode, Unit unit, GameObject unitView, Pathfinder pathfinder, PlayerSpawner playerSpawner)
    {
        GetPath(hitNode, unit, pathfinder, playerSpawner);
        if (currentPath != null && hitNode != null)
        {
            StartCoroutine(FollowPath(currentPath, unit, unitView, false));
        }
        if (currentPath == null)
        {
            Debug.Log("null path");
        }
    }

    private void GetPath(Node hitNode, Unit unit, Pathfinder pathfinder, PlayerSpawner playerSpawner)
    {
        pathfinder.Init(m_graph, m_graphView, unit.currentNode, hitNode);
        UpdateDicts(unit, playerSpawner, hitNode);
        Debug.Log("process move");
        currentPath = CalculatePath(unit.currentNode, hitNode, unit, pathfinder);
    }

    private void UpdateDicts(Unit unit, PlayerSpawner playerSpawner, Node clickedNode)
    {
        if (!playerSpawner.UnitNodeMap.ContainsKey(clickedNode))
        {
            playerSpawner.UnitNodeMap[clickedNode] = unit;
            playerSpawner.NodeUnitViewMap[clickedNode] = unit.gameObject;
            playerSpawner.UnitNodeMap.Remove(startNode);
            playerSpawner.NodeUnitViewMap.Remove(startNode);
        }
    }

    private Unit GetUnit(PlayerSpawner playerSpawner, Node node)
    {
        if (playerSpawner.NodeUnitViewMap.ContainsKey(node))
        {
            Unit unit = playerSpawner.UnitNodeMap[node];
            ResetUnitSelection(playerSpawner);
            unit.isSelected = true;
            startNode = node;
            return unit;
        }
        return null;
    }

    private static void ResetUnitSelection(PlayerSpawner playerSpawner)
    {
        foreach (Unit u in playerSpawner.AllUnits)
        {
            u.isSelected = false;
        }
    }

    private void GetUnitNeighbors(Node hitNode, Unit unit)
    {
        var hitNodesNieghbors = m_graph.GetNeighbors(hitNode.xIndex, hitNode.yIndex);

        foreach (Node node in hitNodesNieghbors)
        {
            if (m_playerSpawner.UnitNodeMap.ContainsKey(node))
            {
                // unit.surroundingEnemies.Add(m_playerSpawner.UnitNodeMap[node]);
            }
        }
    }

    private void HighlightUnitMovementRange(Unit unit)
    {
        Debug.Log("Highlight unit's movement range");
    }

    private List<Node> CalculatePath(Node start, Node goal, Unit unit, Pathfinder pathfinder)
    {
        List<Node> calcPath = pathfinder.SearchRoutine(unit);
        if (calcPath.Count <= 1)
        {
            Debug.Log("There should never be less then two node in the path");
            return null;
        }
        return calcPath;
    }

    IEnumerator FollowPath(List<Node> path, Unit unit, GameObject unitView, bool isEnemySelected)
    {
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(0.5f);
            if (node != startNode)
            {
                unit.actionPoints -= node.movementCost;
            }

            if (isEnemySelected)
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }

            if (!isEnemySelected)
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }
        }
        //if (isEnemySelected)
        //{
        //    unit.Attack(m_playerSpawner, goalNode);
        //}
        DeselectUnit(unit);
        
    }

    private void DeselectUnit(Unit unit)
    {
        unit.isSelected = false;
        isGoalSelected = false;
        currentUnit = null;
        currentUnitView = null;
        startNode = null;
        goalNode = null;
        currentPath = null;
        isEnemySelected = false;
        m_pathfinder.ClearPath();
    }

    private static void UpdateUnitPosData(UIController uiController, Unit unit, GameObject unitView, Node node)
    {
        uiController.UpdateUnitSelectText(unit);
        unitView.transform.position = node.position;
        unit.position = node.position;
        unit.currentNode = node;
        unit.xIndex = node.xIndex;
        unit.yIndex = node.yIndex;
    }
}