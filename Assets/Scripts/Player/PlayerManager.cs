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

    Graph m_graph;
    Pathfinder m_pathfinder;
    MouseController m_mouseController;
    PlayerSpawner m_playerSpawner;
    PlayerAttack m_playerAttack;
    PlayerMovement m_playerMovement;
    Ray ray;
    List<Node> currentPath;
    UIController uiController;
    float maxDistance = 100f;
    bool isEnemySelected = false;

    public List<Node> CurrentPath { get => currentPath; set => currentPath = value; }

    private void Start()
    {
        m_mouseController = FindObjectOfType<MouseController>();
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
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
                    PlayerMovement playerMovement = currentUnitView.GetComponent<PlayerMovement>();
                    playerMovement.Move(hitNode, currentUnit, currentUnitView, m_pathfinder);
                }
            }         
        }

        if (Input.GetMouseButtonDown(1) && currentUnit != null)
        {
            Debug.Log("unit deselected");
            DeselectUnit(currentUnit);
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

    public void DeselectUnit(Unit unit)
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
}