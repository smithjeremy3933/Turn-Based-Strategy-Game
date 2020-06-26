using System;
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
    public List<Node> CurrentPath { get => currentPath; set => currentPath = value; }

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
                // Selecting a unit
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

                    if (!currentUnit.hasMoved && !isEnemySelected)
                    {
                        HighlightUnitMovementRange(currentUnit);
                        uiController.UpdateUnitSelectText(currentUnit);
                        return;
                    }

                    if (!isEnemySelected)
                    {
                        PromptUnitAction(currentUnit);
                    }
                    uiController.UpdateUnitSelectText(currentUnit);
                }               
            }

            if (hasHitNode && currentUnit != null && !isEnemySelected)
            {
                // Moving a unit.
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
                    //if (!currentUnit.isWaiting)
                    //{
                    //    PromptUnitAction(currentUnit);
                    //}
                }
            }         
        }

        if (Input.GetMouseButtonDown(1) && currentUnit != null)
        {
            Debug.Log("unit deselected");
            DeselectUnit(currentUnit);
        }
    }

    private void PromptUnitAction(Unit unit)
    {
        Debug.Log("Pick an action for the unit or deselect");
        ActionList actionList = FindObjectOfType<ActionList>();
        actionList.HandleMovedUnit(unit);
    }

    private Unit GetUnit(PlayerSpawner playerSpawner, Node node)
    {
        if (playerSpawner.NodeUnitViewMap.ContainsKey(node))
        {
            Unit unit = playerSpawner.UnitNodeMap[node];
            // Check to see if we clicked the same unit again,
            // which means we chose to stay at the current pos
            // and chose to attack or use an item;
            if (unit == currentUnit)
            {
                unit.hasMoved = true;
                PromptUnitAction(unit);
            }

            ResetUnitSelection(playerSpawner);
            unit.isSelected = true;
            if (unit.unitType == UnitType.player && unit.hasMoved)
            {
                unit.GetUnitNeighbors(node, unit, m_graph, playerSpawner);
            }
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