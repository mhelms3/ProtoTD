using UnityEngine;
using System.Collections;

public class node {

    public bool isWalkable;
    public float hcost, fcost, gcost;
    public node parent;
    public int positionX, positionY;
    public float moveCost;
    
    public node (int _x, int _y)
    {
        positionX = _x;
        positionY = _y;
        hcost = 0;
        fcost = 0;
        gcost = 0;
        moveCost = 1;
    }
}
