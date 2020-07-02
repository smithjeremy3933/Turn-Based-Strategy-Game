using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    MouseController m_mouseController;
    [SerializeField] GameObject selectionIndicator;

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

    public void HideSelectionIndicator()
    {
        selectionIndicator.SetActive(false);
    }

    public void ShowSelectionIndicator()
    {
        selectionIndicator.SetActive(true);
    }
}
