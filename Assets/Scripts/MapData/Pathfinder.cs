using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    Node m_startNode;
    Node m_goalNode;
    public Node GoalNode { get { return m_goalNode; } }
    Graph m_graph;
    GraphView m_graphView;

    PriorityQueue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;
    public List<Node> PathNodes { get { return m_pathNodes; } }

    public Color startColor = Color.green;
    public Color goalColor = Color.red;

    public bool exitOnGoal = true;
    public bool isComplete = false;
    int m_iterations = 0;

    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || start == null || graphView == null)
        {
            return;
        }

        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        ShowColors(graphView, start, goal);

        m_frontierNodes = new PriorityQueue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();

            }
        }

        isComplete = false;
        m_startNode.distanceTravled = 0;
        m_iterations = 0;
    }

    void ShowColors(GraphView graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }

        NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];

        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    public void SearchRoutine()
    {
        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;
                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                ExpandFrontierAStar(currentNode);                

                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_pathNodes = GetPathNodes(m_goalNode);
                    if (exitOnGoal)
                    {
                        isComplete = true;
                    }
                }
            }
            else
            {
                isComplete = true;
            }
        }
    }

    private void ExpandFrontierAStar(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTravled + (int)node.nodeType;

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTravled) || newDistanceTraveled < node.neighbors[i].distanceTravled)
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTravled = newDistanceTraveled;
                    }

                    if (!m_frontierNodes.Contains(node.neighbors[i]) && m_graph != null)
                    {
                        float distanceToGoal = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                        node.neighbors[i].priority = node.neighbors[i].distanceTravled + distanceToGoal;
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }
    }

    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }

        path.Add(endNode);

        Node currentNode = endNode.previous;

        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.previous;
        }

        return path;
    }
}
