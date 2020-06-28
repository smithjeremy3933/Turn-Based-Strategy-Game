﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject hoveredGameobject;
    public LineRenderer lineRenderer;
    public Color validLineColor = Color.cyan;
    public Color InvalidLineColor = Color.red;
    Node m_hoveredNode;
    PlayerSpawner m_playerSpawner;
    Graph m_graph;
    Pathfinder m_pathfinder;
    PlayerManager m_playerManager;
    float maxDist = 100f;

    public Node HoveredNode { get => m_hoveredNode; }

    void Start()
    {
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        m_graph = FindObjectOfType<Graph>();
        m_pathfinder = FindObjectOfType<Pathfinder>();
        m_playerManager = FindObjectOfType<PlayerManager>();
        lineRenderer.enabled = false;
        lineRenderer.material.color = validLineColor;
    }

    void Update()
    {
        LayerMask mask = LayerMask.GetMask("Unit");
        LayerMask tileMask = LayerMask.GetMask("Tile");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        RaycastHit tileHitInfo;
        bool hasHit = Physics.Raycast(ray, out hitInfo, maxDist, mask);
        bool hasHitTile = Physics.Raycast(ray, out tileHitInfo, maxDist, tileMask);

        if (hasHit)
        {
            int xIndex = (int)hitInfo.transform.position.x;
            int yIndex = (int)hitInfo.transform.position.y;
            int zIndex = (int)hitInfo.transform.position.z;


            Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);
            if (hitNode == null)
            {
                return;
            }
            m_hoveredNode = hitNode;
            if (hitNode != null && m_playerSpawner.NodeUnitViewMap[hitNode] != null)
            {
                GameObject hitUnit = m_playerSpawner.NodeUnitViewMap[hitNode];
                if (hitUnit != null)
                {
                    SelectObject(hitUnit);
                }

            }
        }
        else
        {
            ClearSelection();
        }

        if (hasHitTile)
        {
            int xIndex = (int)tileHitInfo.transform.position.x;
            int yIndex = (int)tileHitInfo.transform.position.y;
            int zIndex = (int)tileHitInfo.transform.position.z;

            Node hitNode = m_graph.GetNodeAt(xIndex, zIndex);
            if (hitNode == null)
            {
                return;
            }
            if (m_hoveredNode == hitNode)
            {
                return;
            }
            if (hitNode != null && m_playerManager.currentUnit != null)
            {
                if (m_playerManager.currentUnit.isPathfinding)
                {
                    Unit currentUnit = m_playerManager.currentUnit;
                    List<Node> currentPath = m_pathfinder.GetPath(hitNode, currentUnit);
                    if (currentUnit != null && currentPath != null)
                    {
                        DrawPath(currentPath.ToArray<Node>(), currentUnit);
                    }
                }
            }
            m_hoveredNode = hitNode;
            return;
        }
    }

    void DrawPath(Node[] path, Unit unit)
    {
        if (path.Length == 0)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;        
        float distanceBetweenNodes = m_graph.GetNodeDistance(path[0], path[path.Length - 1]);
        if (distanceBetweenNodes > unit.actionPoints)
        {
            lineRenderer.material.color = InvalidLineColor;
        } else
        {
            lineRenderer.material.color = validLineColor;
        }
       
        Vector3[] ps = new Vector3[path.Length];

        for (int i = 0; i < path.Length; i++)
        {
            ps[i] = path[i].position + (Vector3.up *0.1f);
        }

        lineRenderer.positionCount = ps.Length;
        lineRenderer.SetPositions(ps);
    }

    void SelectObject(GameObject obj)
    {
        if (hoveredGameobject != null)
        {
            if (obj == hoveredGameobject)
                return;

            ClearSelection();
        }

        hoveredGameobject = obj;
    }

    void ClearSelection()
    {
        if (hoveredGameobject == null)
            return;

        hoveredGameobject = null;
    }
}
