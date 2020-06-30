using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    MouseController m_mouseController;

    void Start()
    {
        m_mouseController = FindObjectOfType<MouseController>();
    }

    void Update()
    {
        if (m_mouseController.HoveredNode != null)
        {
            transform.position = m_mouseController.HoveredNode.position;
        }
    }
}
