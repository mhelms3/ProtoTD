using UnityEngine;
using System.Collections;

public class node {

    public bool isWalkable;
    public float hcost, gcost;
    public node parent;
    public int positionX, positionY;
    public float moveCost;
    
    public node (int _x, int _y)
    {
        positionX = _x;
        positionY = _y;
        hcost = 0;
        gcost = 0;
        moveCost = 1;
        isWalkable = true;
    }
    public float fcost
    {
        get
        {
            return (gcost + hcost);
        }
    }
}
