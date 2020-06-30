using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    UnitDatabase m_unitDatabase;
    PlayerAttack m_playerAttack;
    PlayerMovement m_playerMovement;
    ActionList m_actionList;
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
        m_unitDatabase = FindObjectOfType<UnitDatabase>();
        m_actionList = FindObjectOfType<ActionList>();
        uiController = FindObjectOfType<UIController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

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

            if (hasHitUnit && currentUnit != null && currentUnit.isAttacking)
            {
                // Attacking an enemy.
                int xIndex = (int)hit.transform.position.x;
                int zIndex = (int)hit.transform.position.z;

                Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);
                if (hitNode != null)
                {
                    if (currentUnit.isSurrEnemies && m_unitDatabase.UnitNodeMap.ContainsKey(hitNode))
                    {
                        PlayerAttack playerAttack = currentUnitView.GetComponent<PlayerAttack>();
                        playerAttack.Attack(m_unitDatabase, hitNode, currentUnit);
                    }
                    else
                    {
                        return;
                    }
                }
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
                    currentUnit = GetUnit(m_unitDatabase, hitNode);
                    if (!currentUnit.isWaiting)
                    {
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
                            currentUnit.isPathfinding = true;
                            uiController.UpdateUnitSelectText(currentUnit);
                            return;
                        }

                        if (currentUnit.hasMoved && !isEnemySelected)
                        {
                            PromptUnitAction(currentUnit);
                        }
                        uiController.UpdateUnitSelectText(currentUnit);
                    }
                    else
                    {
                        Debug.Log("This unit is waiting!");
                        DeselectUnit(currentUnit);
                        return;
                    }
                }               
            }

            if (hasHitNode && currentUnit != null && !isEnemySelected)
            {
                // Moving a unit.
                int xIndex = (int)hit.transform.position.x;
                int zIndex = (int)hit.transform.position.z;

                Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);

                if (hitNode != null && startNode != null && !m_unitDatabase.UnitNodeMap.ContainsKey(hitNode))
                {
                    float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitNode);
                    if (currentUnit.actionPoints < distanceBetweenNodes || currentUnit.hasMoved)
                    {
                        Debug.Log("Not enough action points or unit has already moved!");
                        return;
                    }
                    // A goal node was hit. Need to validate, move, and update data.
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

    private void PromptUnitAction(Unit unit)
    {
        Debug.Log("Pick an action for the unit or deselect");
        ActionList actionList = FindObjectOfType<ActionList>();
        actionList.HandleMovedUnit(unit);
    }

    private Unit GetUnit(UnitDatabase unitDatabase, Node node)
    {
        if (unitDatabase.NodeUnitViewMap.ContainsKey(node))
        {
            Unit unit = unitDatabase.UnitNodeMap[node];
            // Check to see if we clicked the same unit again,
            // which means we chose to stay at the current pos
            // and chose to attack or use an item;
            if (unit == currentUnit)
            {
                unit.hasMoved = true;
                PromptUnitAction(unit);
            }

            ResetUnitSelection(unitDatabase);
            unit.isSelected = true;
            if (unit.unitType == UnitType.player && unit.hasMoved)
            {
                unit.GetUnitNeighbors(node, unit, m_graph, unitDatabase);
            }
            startNode = node;
            return unit;
        }
        return null;
    }

    private static void ResetUnitSelection(UnitDatabase unitDatabase)
    {
        foreach (Unit u in unitDatabase.AllUnits)
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
        unit.isAttacking = false;
        unit.isPathfinding = false;
        isGoalSelected = false;
        isEnemySelected = false;
        m_mouseController.lineRenderer.enabled = false;
        currentUnit = null;
        currentUnitView = null;
        startNode = null;
        goalNode = null;
        currentPath = null;
        m_pathfinder.ClearPath();
        m_actionList.actionList.SetActive(false);
    }
}