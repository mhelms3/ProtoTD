using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
public class Node
{
    public int positionX;
    public int positionY;
    public float moveCost;
    public bool isWalkable;

    public float gcost;
    public float hcost;
    public Node parent;

    public float fcost
    {
        get
        {
            return (gcost + hcost);
        }
    }
}

public class Grid
{
    Node[,] gridOfNodes;
}
*/
public class pathFindingScript : MonoBehaviour {

    public BoardSquare[,] boardGrid;
    public List<BoardSquare> finalPath;
    gameBoard cgb;

    Vector2 getGridCoord(Vector2 v)
    {
        Vector2 v2 = new Vector2();
        v2.x = Mathf.FloorToInt(v.x);
        v2.y = Mathf.CeilToInt(v.y);
        return v2;
    }

    public List<BoardSquare> getSquareNeighbors(BoardSquare b)
    {
        List<BoardSquare> neighbors = new List<BoardSquare>();
        int posX = b.positionX;
        int posY = b.positionY;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (y != 0 || x != 0)
                {
                    int newX = posX + x;
                    int newY = posY + y;
                    if (newX >= 0 && newX < cgb.tileSizeX && newY >= 0 && newY < cgb.tileSizeY)
                    {
                        
                        neighbors.Add(boardGrid[newX, newY]);
                    }
                }
            }
        }
        
       // print("Pos:" + posX + ", " + posY + "#neighbors:"+neighbors.Count);
        return neighbors;
    }

    void FindPathVectors (object[] o)
    {
        
        Vector2 v1 = (Vector2)o[0];
        Vector2 v2 = (Vector2)o[1];
        GameObject go = (GameObject)o[2];

        print("PFS Target" + v1.x+", " + v1.y);
        print("PFS Destingation" + v2.x + ", " + v2.y);
        FindPath(v1, v2, go);
        
    }

    void clearBoardGrid()
    {
        foreach (BoardSquare s in boardGrid)
        {
            s.gcost = 0;
            s.hcost = 0;
            s.parent = null;
        }
    }

    void FindPath (Vector2 _starting, Vector2 _end, GameObject mobileGO)
    {
        
        BoardSquare startNode = boardGrid[Mathf.FloorToInt(_starting.x), Mathf.CeilToInt(_starting.y)];
        BoardSquare endNode = boardGrid[Mathf.FloorToInt(_end.x), Mathf.CeilToInt(_end.y)];

        List<BoardSquare> openSet = new List<BoardSquare>();
        HashSet<BoardSquare> closedSet = new HashSet<BoardSquare>();
        openSet.Add(startNode);

        enemyBehavior eb = mobileGO.GetComponent<enemyBehavior>();

        while (openSet.Count > 0)
        {
            BoardSquare currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fcost < currentNode.fcost || openSet[i].fcost == openSet[i].fcost && openSet[i].hcost < openSet[i].hcost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                finalPath = retracePath(currentNode, startNode);
                eb.myPath = finalPath;
                eb.hasPath = true;
                clearBoardGrid();
                Debug.Log("Path Success");
                return;
            }

            foreach (BoardSquare neighbor in getSquareNeighbors(currentNode))
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
    }

    private List<BoardSquare> retracePath(BoardSquare end, BoardSquare begin)
    {
        BoardSquare currentNode = end;
        List<BoardSquare> path = new List<BoardSquare>();
        while (currentNode != begin)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private float getDistance(BoardSquare a, BoardSquare b)
    {
        float disX = Mathf.Abs(a.positionX - b.positionX);
        float disY = Mathf.Abs(a.positionY - b.positionY);
        if (disX > disY)
            return (disY * 14 + (disX - disY) * 10);
        return (disX * 14 + (disY - disX) * 10);
    }
	// Use this for initialization

	void Start () {
        cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        GameObject[,] tempGrid = cgb.boardTile;
        boardGrid = new BoardSquare[cgb.tileSizeX, cgb.tileSizeY];
        foreach(GameObject go in tempGrid)
        {
            BoardSquare tempBS = (BoardSquare)go.GetComponent(typeof(BoardSquare));
            int _x = tempBS.positionX;
            int _y = tempBS.positionY;
            boardGrid[_x, _y] = tempBS;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
