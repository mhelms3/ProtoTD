using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;



public class pathFindingScript : MonoBehaviour {

    //public BoardSquare[,] boardGrid;
    public node[,] boardGrid;
    public List<node> finalPath;
    public int dimX, dimY;
    Stopwatch sw;
    
    
    Vector2 getGridCoord(Vector2 v)
    {
        Vector2 v2 = new Vector2();
        v2.x = Mathf.FloorToInt(v.x);
        v2.y = Mathf.CeilToInt(v.y);
        return v2;
    }

    public List<node> getSquareNeighbors(node b)
    {
        List<node> neighbors = new List<node>();
        int posX = b.positionX;
        int posY = b.positionY;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {

                //if (y != 0 || x != 0)

                if (y == 0 || x == 0)
                {
                    int newX = posX + x;
                    int newY = posY + y;
                    if (newX >= 0 && newX < dimX && newY >= 0 && newY < dimY)
                    {
                        
                        neighbors.Add(boardGrid[newX, newY]);
                    }
                }
            }
        }
        
       //print("Pos:" + posX + ", " + posY + "#neighbors:"+neighbors.Count);
        return neighbors;
    }

    /*
    void FindPathVectors (object[] o)
    {   
        
        Vector2 v1 = (Vector2)o[0];
        Vector2 v2 = (Vector2)o[1];
        GameObject go = (GameObject)o[2];

        //print("PFS Target" + v1.x+", " + v1.y);
        //print("PFS Destingation" + v2.x + ", " + v2.y);
        //print("PFS GameObject Name" + go.name +" Position:"+ go.transform.position);

        FindPath(v1, v2, go);
        
    }
    */

    void clearBoardGrid()
    {
        foreach (node s in boardGrid)
        {
            s.gcost = 0;
            s.hcost = 0;
            s.parent = null;
        }
    }

    public List<node> FindPath (Vector2 _starting, Vector2 _end)
    {
        sw.Reset();
        sw.Start();
        //print("FP Target" + _starting.x+", " + _starting.y);
        //print("FP Destingation" + _end.x + ", " + _end.y);
        //print("FP GameObject Name" + mobileGO.name +" Position:"+ mobileGO.transform.position);

        node startNode = boardGrid[Mathf.FloorToInt(_starting.x), Mathf.CeilToInt(_starting.y)];
        node endNode = boardGrid[Mathf.FloorToInt(_end.x), Mathf.CeilToInt(_end.y)];
        
        //print("Start Node (Target)" + startNode.positionX + ", " + startNode.positionY);
        //print("End Node (Dest)" + endNode.positionX + ", " + endNode.positionY);

        List<node> openSet = new List<node>();
        HashSet<node> closedSet = new HashSet<node>();
        openSet.Add(startNode);

        //enemyBehavior eb = mobileGO.GetComponent<enemyBehavior>();

        
        while (openSet.Count > 0)
        {
            node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fcost < currentNode.fcost || openSet[i].fcost == openSet[i].fcost && openSet[i].hcost < openSet[i].hcost)
                {
                    currentNode = openSet[i];
                     //print("Current Node X:" + currentNode.positionX + " Y:" + currentNode.positionY + " fCost"+currentNode.fcost);
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            

            if (checkDistanceOne(currentNode, endNode))
            {
                finalPath = retracePath(currentNode, startNode);
                clearBoardGrid();
                //Debug.Log("Path Success");
                sw.Stop();
                print("Path Time" + sw.ElapsedMilliseconds + " in ms");
                return finalPath;
            }

            foreach (node neighbor in getSquareNeighbors(currentNode))
            {
                if(!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                //float newMoveCostToNeighbor = currentNode.gcost + getDistance(currentNode, neighbor) + (neighbor.moveCost *.5f);
                //print("Current Node g:" + currentNode.gcost + " Current Node h:" + currentNode.hcost);
                float newMoveCostToNeighbor = currentNode.gcost + getDistance(currentNode, neighbor);
                if (newMoveCostToNeighbor < neighbor.gcost || !openSet.Contains(neighbor))
                {
                    //neighbor.gcost = getDistance(currentNode, neighbor) + (neighbor.moveCost * .5f);
                    //neighbor.hcost = getDistance(neighbor, endNode) + (neighbor.moveCost * .5f);
                    neighbor.gcost = getDistance(currentNode, neighbor);
                    neighbor.hcost = getDistance(neighbor, endNode);
                    neighbor.parent = currentNode;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        //Debug.Log("No Path Found!!");
        return openSet;
    }

    private List<node> retracePath(node end, node begin)
    {
        node currentNode = end;
        List<node> path = new List<node>();
        while (currentNode != begin)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }


    private bool checkDistanceOne (node a, node b)
    {
        if (getDistance(a, b) <= 14)
            return true;
        else
            return false;
    }
    private float getDistance(node a, node b)
    {
        float disX = Mathf.Abs(a.positionX - b.positionX);
        float disY = Mathf.Abs(a.positionY - b.positionY);
        if (disX > disY)
            return (disY * 14 + (disX - disY) * 10);
        return (disX * 14 + (disY - disX) * 10);
    }
	// Use this for initialization

	void Start () {
        
        gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        dimX = cgb.tileSizeX;
        dimY = cgb.tileSizeY;
        boardGrid = cgb.pathGrid;
        sw = new Stopwatch();
        /*
        node[,] boardGrid = cgb.pathGrid;
        boardGrid = new node[cgb.tileSizeX, cgb.tileSizeY];
        foreach(node n in boardGrid)
        {
            /////////////////node tempBS = (node)go.GetComponent(typeof(node));!!!!!!!!!!!!!!!!!!!!!!!!!
            int _x = tempBS.positionX;
            int _y = tempBS.positionY;
            boardGrid[_x, _y] = tempBS;
        }
        */
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
