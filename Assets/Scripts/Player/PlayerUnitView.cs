using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitView : MonoBehaviour
{
    Node m_startNode;

    public void Init(Unit unit)
    {
        gameObject.name = "Player Unit " + unit.name;
        gameObject.transform.position = unit.position;
        m_startNode = unit.currentNode;
    }

}
