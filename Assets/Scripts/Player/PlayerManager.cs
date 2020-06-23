using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Node startNode;
    public Node goalNode;
    public Color movementRangeColor = Color.cyan;
    public bool isGoalSelected;
    Vector3 startPos;
    Vector3 goalPos;
    Graph m_graph;
    Pathfinder m_pathfinder;
    GraphView m_graphView;
    PlayerSpawner m_playerSpawner;
    PlayerUnitView m_playerUnitView;
    PlayerAttack m_playerAttack;
    Ray ray;
    Unit currentUnit;
    GameObject currentUnitView;
    Vector3 mouseOverPosition;
    List<Node> currentPath;
    UIController uiController;


    private void Start()
    {
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
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                int xIndex = (int)hit.transform.position.x;
                int yIndex = (int)hit.transform.position.y;
                int zIndex = (int)hit.transform.position.z;
                // Check to see if the hit node is in bounds of the created map.
                if (m_graph.IsWithinBounds(xIndex, yIndex))
                {
                    Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);
                    if (hitNode == null)
                    {
                        // Hit a node not with bounds some how.
                        Debug.Log("Invalid node");
                        return;
                    }
                    
                    if (m_playerSpawner.UnitNodeMap.ContainsKey(hitNode) && m_playerSpawner.UnitNodeMap[hitNode].unitType == UnitType.player && hitNode != null )
                    {
                        // A valid node with a player on it was selected
                        SelectPlayerUnit(m_playerSpawner, hitNode);
                    }
                    else if (currentUnit == null)
                    {
                        // No node with a player unit was selected
                        Debug.Log("Invalid node");
                        return;
                    }
                    else if (!m_playerSpawner.UnitNodeMap.ContainsKey(hitNode) && currentUnit.isSelected && currentUnit.unitType == UnitType.player)
                    {
                        // A goalnode was hit
                        float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitNode);
                        if (currentUnit.actionPoints < distanceBetweenNodes)
                        {
                            Debug.Log("Not enough action points!!");
                            return;
                        }
                        MoveToValidGoalNode(hitNode, false);
                    }
                    else if (m_playerSpawner.UnitNodeMap.ContainsKey(hitNode) && currentUnit.isSelected && m_playerSpawner.UnitNodeMap[hitNode].unitType == UnitType.enemy)
                    {
                        // player unit is selected and player hit node with enemy on it!
                        Debug.Log("hit node with enemy on it!!!");
                        float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitNode);
                        if (currentUnit.actionPoints < distanceBetweenNodes)
                        {
                            Debug.Log("Not enough action points!!");
                            return;
                        }
                        MoveToValidGoalNode(hitNode, true);                      
                    }
                    else if (m_playerSpawner.UnitNodeMap.ContainsKey(hitNode) && m_playerSpawner.UnitNodeMap[hitNode].unitType == UnitType.enemy && hitNode != null)
                    {
                        Debug.Log("enemy Selected");
                        SelectPlayerUnit(m_playerSpawner, hitNode);
                    }
                    else
                    {
                        Debug.Log("No valid node with a unit on it");
                        ResetInvalidSelection();
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("No valid node selected");
                ResetInvalidSelection();
                return;
            }
        }
    }

    private void SelectPlayerUnit(PlayerSpawner playerSpawner, Node hitNode)
    {
        Unit unit = playerSpawner.UnitNodeMap[hitNode];
        GameObject unitView = unit.gameObject;
        ResetUnitSelection(playerSpawner);

        currentUnit = unit;
        currentUnitView = unitView;
        unit.isSelected = true;
        startNode = hitNode;
        HighlightUnitMovementRange(currentUnit);

        // Update UI
        uiController.UpdateUnitSelectText(currentUnit);
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

    private void ResetInvalidSelection()
    {
        if (currentUnit != null)
        {
            currentUnit.isSelected = false;
        }
        currentUnit = null;
        currentUnitView = null;
        startNode = null;
    }

    private void MoveToValidGoalNode(Node hitNode, bool isEnemyClicked)
    {
        isGoalSelected = true;
        goalNode = hitNode;
        m_pathfinder.Init(m_graph, m_graphView, currentUnit.currentNode, goalNode);
        if (!m_playerSpawner.UnitNodeMap.ContainsKey(goalNode) && !isEnemyClicked)
        {
            m_playerSpawner.UnitNodeMap[goalNode] = currentUnit;
            m_playerSpawner.UnitNodeMap.Remove(startNode);
        }
        else if (!m_playerSpawner.UnitNodeMap.ContainsKey(goalNode) && isEnemyClicked)
        {
            m_playerSpawner.UnitNodeMap[goalNode] = currentUnit;
            m_playerSpawner.UnitNodeMap.Remove(startNode);
        }
        Debug.Log("process move");
        currentPath = CalculatePath(startNode, goalNode, currentUnit);
        if (currentPath != null && startNode != null && goalNode != null)
        {
            StartCoroutine(FollowPath(currentPath, currentUnit, currentUnitView, isEnemyClicked));
        }
        if (currentPath == null)
        {
            Debug.Log("null path");
        }
    }

    private void HighlightUnitMovementRange(Unit unit)
    {
        Debug.Log("Highlight unit's movement range");
    }

    private List<Node> CalculatePath(Node start, Node goal, Unit unit)
    {
        List<Node> calcPath = m_pathfinder.SearchRoutine(unit);
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

            if (isEnemySelected && node != goalNode)
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }

            if (!isEnemySelected)
            {
                UpdateUnitPosData(uiController, unit, unitView, node);
            }
        }
        if (isEnemySelected)
        {
            unit.Attack(m_playerSpawner, goalNode);
        }
        ResetPlayerSelectionData(unit);
        
    }

    private void ResetPlayerSelectionData(Unit unit)
    {
        unit.isSelected = false;
        isGoalSelected = false;
        startNode = null;
        goalNode = null;
        currentPath = null;
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
