using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Node startNode;
    public Node goalNode;
    public Color movementRangeColor = Color.cyan;
    public bool isSelected;
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


    private void Start()
    {
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_graphView = FindObjectOfType<GraphView>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        m_playerAttack = FindObjectOfType<PlayerAttack>();
        m_playerUnitView = FindObjectOfType<PlayerUnitView>();
        
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

                if (m_graph.IsWithinBounds(xIndex, yIndex))
                {
                    Node hitNode = m_graph.GetNodeAt((int)hit.transform.position.x, (int)hit.transform.position.z);
                    if (hitNode == null)
                    {
                        Debug.Log("Invalid node");
                        return;
                    }
                    
                    if (m_playerSpawner.UnitNodeMap.ContainsKey(hitNode) && m_playerSpawner.UnitNodeMap[hitNode].unitType == UnitType.player && hitNode != null )
                    {
                        var hitNodesNieghbors = m_graph.GetNeighbors(hitNode.xIndex, hitNode.yIndex);
                        
                        foreach (Node node in hitNodesNieghbors)
                        {
                            if (m_playerSpawner.UnitNodeMap.ContainsKey(node))
                            {
                                Debug.Log(node.position);
                            }
                        }
                        Unit unit = m_playerSpawner.UnitNodeMap[hitNode];
                        GameObject unitView = unit.gameObject;
                        if (unit.isSelected == false && currentUnit == null)
                        {
                            unit.isSelected = true;
                            currentUnit = unit;
                            currentUnitView = unitView;
                            startNode = hitNode;
                            HighlightUnitMovementRange(currentUnit);
                        }
                        else if (unit.isSelected == false && currentUnit != null)
                        {
                            currentUnit.isSelected = false;
                            currentUnit = unit;
                            currentUnitView = unitView;
                            unit.isSelected = true;
                            startNode = hitNode;
                            HighlightUnitMovementRange(currentUnit);
                        }                     
                    }
                    else if (currentUnit == null)
                    {
                        Debug.Log("Invalid node");
                        return;
                    }
                    else if (!m_playerSpawner.UnitNodeMap.ContainsKey(hitNode) && currentUnit.isSelected && currentUnit.unitType == UnitType.player)
                    {
                        // goalnode hit
                        float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitNode);
                        if (distanceBetweenNodes > currentUnit.movementRange)
                        {
                            Debug.Log("Cannot select goal node outside of the current unit movement Range " + distanceBetweenNodes);
                            return;                          
                        }
                        if (currentUnit.actionPoints < distanceBetweenNodes)
                        {
                            Debug.Log("Not enough action points!!");
                            return;
                        }
                        isGoalSelected = true;
                        goalNode = hitNode;
                        m_pathfinder.Init(m_graph, m_graphView, currentUnit.currentNode, goalNode);
                        if (!m_playerSpawner.UnitNodeMap.ContainsKey(goalNode))
                        {
                            m_playerSpawner.UnitNodeMap[goalNode] = currentUnit;
                            m_playerSpawner.UnitNodeMap.Remove(startNode);
                        }
                        Debug.Log("process move");
                        currentPath = CalculatePath(startNode, goalNode, currentUnit);
                        if (currentPath != null && startNode != null && goalNode != null)
                        {
                            StartCoroutine(FollowPath(currentPath, currentUnit, currentUnitView));
                        }
                        if (currentPath == null)
                        {
                            Debug.Log("null path");
                        }
                        UpdateUnitData();
                    }
                    else
                    {
                        Debug.Log("No valid node with a unit on it");
                        if (currentUnit != null)
                        {
                            currentUnit.isSelected = false;
                        }
                        currentUnit = null;
                        currentUnitView = null;
                        startNode = null;
                        hitNode = null;
                    }
                }
            }
            else
            {
                Debug.Log("No valid node selected");
                if (currentUnit != null)
                {
                    currentUnit.isSelected = false;
                }
                currentUnit = null;
                currentUnitView = null;
                startNode = null;
            }
        }
        //    else if (hasHit && isSelected == true && m_playerAttack.tag.Equals("Enemy"))
        //    {
        //        m_playerAttack.MeleeAttackEnemy();
        //    }
    }

    private void UpdateUnitData()
    {
        currentUnit.position = goalPos;
        currentUnit.currentNode = goalNode;
        currentUnit.xIndex = (int)goalPos.x;
        currentUnit.yIndex = (int)goalPos.z;
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

    IEnumerator FollowPath(List<Node> path, Unit unit, GameObject unitView)
    {
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(0.5f);
            if (node != startNode)
            {
                unit.actionPoints -= node.movementCost;
            }
            unitView.transform.position = node.position;
            unit.position = node.position;
            unit.currentNode = node;
        }
        unit.isSelected = false;
        isGoalSelected = false;
        startNode = null;
        goalNode = null;
        currentPath = null;
        m_pathfinder.ClearPath();
    }
}
