using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject hoveredGameobject;
    Node m_hoveredNode;
    PlayerSpawner m_playerSpawner;
    Graph m_graph;
    float maxDist = 100f;

    public Node HoveredNode { get => m_hoveredNode; }

    void Start()
    {
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        m_graph = FindObjectOfType<Graph>();
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
            m_hoveredNode = hitNode;
            return;
        }
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
