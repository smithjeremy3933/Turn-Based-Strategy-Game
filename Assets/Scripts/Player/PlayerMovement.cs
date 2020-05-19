using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    PlayerAttack m_playerAttack;
    Ray ray;
    Unit currentUnit;
    Vector3 mouseOverPosition;
    List<Node> currentPath;


    private void Start()
    {
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_graphView = FindObjectOfType<GraphView>();
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        m_playerAttack = FindObjectOfType<PlayerAttack>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit && isSelected == true && hit.rigidbody == null && startNode != null && startPos != hit.transform.position && isGoalSelected == false)
            {
                // goalnode hit
                Node hitGoalNode = m_graph.GetNodeAt((int)hit.transform.position.x, (int)hit.transform.position.z);
                float distanceBetweenNodes = m_graph.GetNodeDistance(startNode, hitGoalNode);
                if (distanceBetweenNodes > currentUnit.movementRange)
                {
                    Debug.Log("Cannot select goal node outside of the current unit movement Range " + distanceBetweenNodes);
                    isSelected = false;
                    return;
                }
                if (m_playerSpawner.unitNodeMap.ContainsKey(hitGoalNode))
                {
                    Debug.Log("Node already occupied");
                    startNode = null;
                    isSelected = false;
                    return;
                }
                if (currentUnit.actionPoints < distanceBetweenNodes)
                {
                    Debug.Log("Not enough action points!!");
                    isSelected = false;
                    return;
                }
                ProcessMoveToValidGoal(hit, startNode);
            }
            else if (hasHit && isSelected == true && m_playerAttack.tag.Equals("Enemy"))
            {        
                m_playerAttack.MeleeAttackEnemy();
            }
            else if (hasHit && isSelected == false && mouseOverPosition == hit.transform.position && tag.Equals("Player"))
            {
                ProcessUnitMoveSelection(hit);
            }
           
            else
            {
                Debug.Log("Nothing Valid Selected");
                isSelected = false;
            }
        }
    }

    private void ProcessUnitMoveSelection(RaycastHit hit)
    {
        isSelected = true;
        startPos = hit.transform.position;
        startNode = m_graph.GetNodeAt((int)startPos.x, (int)startPos.z);
        if (m_playerSpawner.unitNodeMap.ContainsKey(startNode))
        {
            currentUnit = m_playerSpawner.unitNodeMap[startNode];
        }
        HighlightUnitMovementRange(currentUnit);
    }

    private void ProcessMoveToValidGoal(RaycastHit hit, Node start)
    {
        isGoalSelected = true;
        goalPos = hit.transform.position;
        goalNode = m_graph.GetNodeAt((int)goalPos.x, (int)goalPos.z);
        m_pathfinder.Init(m_graph, m_graphView, start, goalNode);
        if (!m_playerSpawner.unitNodeMap.ContainsKey(goalNode))
        {
            m_playerSpawner.unitNodeMap[goalNode] = currentUnit;
            m_playerSpawner.unitNodeMap.Remove(startNode);
        }
        currentPath = CalculatePath(startNode, goalNode, currentUnit);
        if (currentPath != null && startNode != null && goalNode != null)
        {
            StartCoroutine(FollowPath(currentPath, currentUnit));
        }
        if (currentPath == null)
        {
            Debug.Log("null path");
        }
        UpdateUnitData();
    }

    private void UpdateUnitData()
    {
        currentUnit.position = goalPos;
        currentUnit.currentNode = goalNode;
        currentUnit.xIndex = (int)goalPos.x;
        currentUnit.yIndex = (int)goalPos.z;
    }

    private void OnMouseOver()
    {
        mouseOverPosition = gameObject.transform.position;
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

    IEnumerator FollowPath(List<Node> path, Unit unit)
    {
        foreach (Node node in path)
        {
            yield return new WaitForSeconds(0.5f);
            if (node != startNode)
            {
                unit.actionPoints -= node.movementCost;
            }

            transform.position = node.position;
            unit.position = node.position;
            unit.currentNode = node;   
        }
        isGoalSelected = false;
        isSelected = false;
        startNode = null;
        goalNode = null;
        currentPath = null;
        m_pathfinder.ClearPath();
    }
}