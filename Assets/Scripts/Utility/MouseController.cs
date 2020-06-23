using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject hoveredGameobject;
    Node m_hoveredNode;
    PlayerSpawner m_playerSpawner;
    Graph m_graph;

    public Node HoveredNode { get => m_hoveredNode; set => m_hoveredNode = value; }

    void Start()
    {
        m_playerSpawner = FindObjectOfType<PlayerSpawner>();
        m_graph = FindObjectOfType<Graph>();
    }

    void Update()
    {
        LayerMask mask = LayerMask.GetMask("Unit");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hasHit = Physics.Raycast(ray, out hitInfo, 100f, mask);

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
