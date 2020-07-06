using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitView : MonoBehaviour
{
    Node m_startNode;

    public void Init(Unit unit)
    {
        gameObject.name = "Enemy Unit " + unit.name;
        gameObject.transform.position = unit.position;
        m_startNode = unit.currentNode;
    }

    public void UnitDeath()
    {

    }
}
