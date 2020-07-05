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
    public PlayerAttack CurrentPlayerAttack { get => m_currentPlayerAttack; set => m_currentPlayerAttack = value; }
    public bool IsSelectingEnemy { get => isSelectingEnemy; set => isSelectingEnemy = value; }
    public static event EventHandler<OnUnitSelectedEventArgs> OnUnitSelected;
    public class OnUnitSelectedEventArgs : EventArgs
    {
        public Unit currentUnit;
        public GameObject currentUnitView;
    }

    Graph m_graph;
    GraphView m_graphView;
    Pathfinder m_pathfinder;
    MouseController m_mouseController;
    UnitDatabase m_unitDatabase;
    PlayerAttack m_currentPlayerAttack;
    Ray ray;
    Color enemyMoveRangeColor = Color.red;
    List<Node> currentPath;
    float maxDistance = 100f;
    bool isEnemySelected = false;
    bool isSelectingEnemy = false;
    // Need to find out why exactly I need this offset!
    float actionPointoffset = 1f;

    private void Start()
    {
        m_mouseController = FindObjectOfType<MouseController>();
        m_graph = FindObjectOfType<Graph>();
        m_graphView = FindObjectOfType<GraphView>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_unitDatabase = FindObjectOfType<UnitDatabase>();
        TurnManager.OnTurnEnded += TurnManager_OnTurnEnded;
    }

    private void TurnManager_OnTurnEnded(object sender, EventArgs e)
    {
        DeselectUnit();
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
                        Unit enemy = m_unitDatabase.UnitNodeMap[hitNode];
                        if (enemy.unitType == UnitType.player)
                            return;

                        PlayerAttack playerAttack = currentUnitView.GetComponent<PlayerAttack>();
                        m_currentPlayerAttack = playerAttack;
                        if (enemy != null && currentUnit != null)
                        {
                            StartCoroutine(playerAttack.Attack(currentUnit, enemy));
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (hasHitUnit && !isSelectingEnemy)
            {
                // Selecting a unit
                int xIndex = (int)hit.transform.position.x;
                int zIndex = (int)hit.transform.position.z;

                Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);
                if (hitNode != null)
                {
                    currentUnit = GetUnit(m_unitDatabase, hitNode);
                    currentUnitView = m_mouseController.hoveredGameobject;
                    OnUnitSelected?.Invoke(this, new OnUnitSelectedEventArgs { currentUnit = currentUnit, currentUnitView = currentUnitView });
                    if (!currentUnit.isWaiting)
                    {
                        if (currentUnit.unitType == UnitType.enemy)
                        {
                            isEnemySelected = true;
                            HighlightUnitMovementRange(currentUnit, enemyMoveRangeColor);
                        }
                        else if (currentUnit.unitType == UnitType.player)
                        {
                            isEnemySelected = false;
                        }

                        if (!currentUnit.hasMoved && !isEnemySelected)
                        {
                            HighlightUnitMovementRange(currentUnit);
                            currentUnit.isPathfinding = true;
                            return;
                        }

                        //if (currentUnit.hasMoved && !isEnemySelected)
                        //{
                        //    PromptUnitAction(currentUnit);
                        //}
                    }
                    else
                    {
                        Debug.Log("This unit is waiting!");
                        DeselectUnit(currentUnit);
                        return;
                    }
                }               
            }

            if (hasHitNode && currentUnit != null && !isEnemySelected && !isSelectingEnemy)
            {
                // Moving a unit.
                int xIndex = (int)hit.transform.position.x;
                int zIndex = (int)hit.transform.position.z;

                Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);

                if (hitNode != null && startNode != null && !m_unitDatabase.UnitNodeMap.ContainsKey(hitNode) && !currentUnit.isAttacking)
                {
                    currentPath = m_pathfinder.GetPath(hitNode, currentUnit);
                    if (currentUnit.actionPoints + actionPointoffset < currentPath.Count || currentUnit.hasMoved)
                    {
                        Debug.Log("Not enough action points or unit has already moved!");
                        currentPath = null;
                        return;
                    }

                    // A goal node was hit. Need to validate, move, and update data.
                    isGoalSelected = true;
                    goalNode = hitNode;
                    if (currentUnitView != null)
                    {
                        PlayerMovement playerMovement = currentUnitView.GetComponent<PlayerMovement>();
                        UnHighlightUnitMovementRange(currentUnit);
                        playerMovement.Move(hitNode, currentUnit, currentUnitView, m_pathfinder);
                    }
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
                //PromptUnitAction(unit);
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
        for (int x = unit.xIndex - (int)unit.actionPoints; x <= unit.xIndex + (int)unit.actionPoints; x++)
        {
            for (int y = unit.yIndex - (int)unit.actionPoints; y <= unit.yIndex + (int)unit.actionPoints; y++)
            {
                if (m_graph.IsWithinBounds(x, y))
                {
                    if (m_graph.GetNodeAt(x, y).nodeType == NodeType.Open)
                    {
                        Node node = m_graph.GetNodeAt(x, y);
                        if (!m_unitDatabase.UnitNodeMap.ContainsKey(node))
                        {
                            if (m_pathfinder.GetPath(node, unit).Count <= unit.actionPoints + actionPointoffset)
                            {
                                NodeView nodeView = m_graphView.nodeViews[node.xIndex, node.yIndex];
                                nodeView.ColorNode(movementRangeColor);
                            }
                            else
                            {
                                // Nothing
                            }
                        }
                    }
                }
            }
        }
    }

    private void HighlightUnitMovementRange(Unit unit, Color color)
    {
        for (int x = unit.xIndex - (int)unit.actionPoints; x <= unit.xIndex + (int)unit.actionPoints; x++)
        {
            for (int y = unit.yIndex - (int)unit.actionPoints; y <= unit.yIndex + (int)unit.actionPoints; y++)
            {
                if (m_graph.IsWithinBounds(x, y))
                {
                    if (m_graph.GetNodeAt(x, y).nodeType == NodeType.Open)
                    {
                        Node node = m_graph.GetNodeAt(x, y);
                        if (!m_unitDatabase.UnitNodeMap.ContainsKey(node))
                        {
                            if (m_pathfinder.GetPath(node, unit).Count <= unit.actionPoints + actionPointoffset)
                            {
                                NodeView nodeView = m_graphView.nodeViews[node.xIndex, node.yIndex];
                                nodeView.ColorNode(color);
                            }
                            else
                            {
                                // Nothing
                            }
                        }
                    }
                }
            }
        }
    }

    private void UnHighlightUnitMovementRange(Unit unit)
    {
        for (int x = unit.xIndex - (int)unit.actionPoints; x <= unit.xIndex + (int)unit.actionPoints; x++)
        {
            for (int y = unit.yIndex - (int)unit.actionPoints; y <= unit.yIndex + (int)unit.actionPoints; y++)
            {
                if (m_graph.IsWithinBounds(x, y))
                {
                    if (m_graph.GetNodeAt(x, y).nodeType == NodeType.Open)
                    {
                        Node node = m_graph.GetNodeAt(x, y);
                        if (!m_unitDatabase.UnitNodeMap.ContainsKey(node))
                        {
                            if (m_pathfinder.GetPath(node, unit).Count <= unit.actionPoints + actionPointoffset)
                            {
                                NodeView nodeView = m_graphView.nodeViews[node.xIndex, node.yIndex];
                                nodeView.ColorNode(m_graphView.baseColor);
                            }
                            else
                            {
                                // Nothing
                            }
                        }
                    }
                }
            }
        }
    }

    public void DeselectUnit()
    {
        DeselectUnit(currentUnit);
    }

    public void DeselectUnit(Unit unit)
    {
        if (unit != null)
        {
            unit.isSelected = false;
            unit.isAttacking = false;
            unit.isPathfinding = false;
            UnHighlightUnitMovementRange(unit);
        }
        isGoalSelected = false;
        isEnemySelected = false;
        isSelectingEnemy = false;
        m_mouseController.lineRenderer.enabled = false;
        currentUnit = null;
        currentUnitView = null;
        m_currentPlayerAttack = null;
        startNode = null;
        goalNode = null;
        currentPath = null;
        m_pathfinder.ClearPath();
    }
}