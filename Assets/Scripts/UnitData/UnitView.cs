using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    public static event EventHandler<OnUnitDeathEventArgs> OnUnitDeath;
    public class OnUnitDeathEventArgs : EventArgs
    {
        public Unit deadUnit;
    }

    Node m_startNode;

    public void Init(Unit unit)
    {
        gameObject.name = unit.name;
        gameObject.transform.position = unit.position;
        m_startNode = unit.currentNode;
    }

    public void UnitDeath(Unit unit)
    {
        if (unit.health <= 0)
        {
            Destroy(gameObject);
            OnUnitDeath?.Invoke(this, new OnUnitDeathEventArgs { deadUnit = unit });
        }
    }
}
