using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    player = 0,
    enemy = 1
}
public class Unit
{
    public UnitType unitType = UnitType.player;
    public int xIndex = -1;
    public int yIndex = -1;
    public string name = "Steve";
    public float movementRange = 3f;

    public Vector3 position;

    public Node currentNode;

    public Unit( int xIndex, int yIndex, UnitType unitType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.unitType = unitType;
    }
}
