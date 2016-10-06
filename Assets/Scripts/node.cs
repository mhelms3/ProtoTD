using UnityEngine;
using System.Collections;

public class node :IHeapItem<node> {

    public bool isWalkable;
    public float hcost, gcost;
    public node parent;
    public int positionX, positionY;
    public float moveCost;
    int heapIndex;
    
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

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(node nodeToCompare)
    {
        int compare = fcost.CompareTo(nodeToCompare.fcost);
        if (compare == 0)
        {
            compare = hcost.CompareTo(nodeToCompare.hcost);
        }
        return -compare;
    }

}
