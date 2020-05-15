using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitView : MonoBehaviour
{
    public void Init(Unit unit)
    {

            gameObject.name = "Player Unit " + unit.xIndex + "," + unit.yIndex + ")";
        
    }
}
