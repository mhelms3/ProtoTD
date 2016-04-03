using UnityEngine;
using System.Collections.Generic;

public class CreateGameBoard : MonoBehaviour
{
    public int tileSizeX;
    public int tileSizeY;
    private float currentRnd = 0;
    private int x, y;    

    public GameObject[] terrainTiles;
    private IDictionary<string, GameObject> terrainDictionary = new Dictionary<string, GameObject>();

    public GameObject squarePrototype;
    public GameObject[,] boardTile;
    
    public Transform camTransform;
    public Camera camera1;
    private GameObject currentTile;
    public float maxCameraDistance = 50f;
    public float minCameraDistance = 5f;
    public float cameraDistance = 25f;
    public float scrollSpeed = 2.0f;

    private Vector3 mousePosition = new Vector3();
    private Vector3 moveDirection = new Vector3();
    private bool moveFlag = false;
    private float maxPositionX;
    private float maxPositionY;

    void initializeTerrainDictionary()
    {
        string tempTerrainName;
        BoardTerrain bt;
        foreach (GameObject g in terrainTiles)
        {
            bt = (BoardTerrain)g.GetComponent(typeof(BoardTerrain));
            tempTerrainName = bt.terrainName;
            Debug.Log(tempTerrainName);
            terrainDictionary.Add(tempTerrainName, g);
        }
    }

    float initializeResources(float average)
    {
        float newValue = Mathf.Clamp((average + Random.Range(-10, 10) + Random.Range(-10, 10) + Random.Range(-10, 10)), 1, 200);
        return newValue;
    }

    void Start()
    {

        initializeTerrainDictionary();
        maxPositionX = tileSizeX - cameraDistance;
        maxPositionY = tileSizeY - (cameraDistance * 0.9f);
        string tempTile;
        GameObject currentTerrain;
        BoardTerrain bt;
        BoardSquare bs;
        SpriteRenderer srTerrain;
        SpriteRenderer srSquare;
        int currentRnd;

        boardTile = new GameObject[tileSizeX,tileSizeY];

        for (int i = 0; i < tileSizeX; i++)
            for (int j = 0; j < tileSizeY; j++)
            {
                currentRnd = Mathf.CeilToInt(Random.value * 9 +.49f);
                switch (currentRnd)
                {
                    case 1:
                        tempTile = "Grasslands";
                        break;
                    case 2:
                        tempTile = "LightWoods";
                        break;
                    case 3:
                        tempTile = "LowHills";
                        break;
                    case 4:
                        tempTile = "Mountains";
                        break;
                    case 5:
                        tempTile = "RoughHills";
                        break;
                    case 6:
                        tempTile = "StoneHills";
                        break;
                    case 7:
                        tempTile = "Swamp";
                        break;
                    case 8:
                        tempTile = "WoodedHills";
                        break;
                    case 9:
                        tempTile = "Woods";
                        break;
                    default:
                        tempTile = "Grasslands";
                        break;
                }

                currentTerrain = terrainDictionary[tempTile];
                
                bt = (BoardTerrain)currentTerrain.GetComponent(typeof(BoardTerrain));
                srTerrain = (SpriteRenderer)currentTerrain.GetComponent(typeof(SpriteRenderer));

                boardTile[i,j] = Instantiate(squarePrototype, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                bs = (BoardSquare)boardTile[i, j].GetComponent(typeof(BoardSquare));
                srSquare = (SpriteRenderer)boardTile[i, j].GetComponent(typeof(SpriteRenderer));

                bs.name = bt.terrainName;
                bs.wood = initializeResources(bt.averageWoodValue);
                bs.stone = initializeResources(bt.averageStoneValue);
                bs.food = initializeResources(bt.averageFoodValue);
                bs.positionX = i;
                bs.positionY = j;

                srSquare.sprite = srTerrain.sprite;
            }

        camera1.transform.position = new Vector3(tileSizeX / 2, tileSizeY / 2, -1);
        camera1.orthographicSize = cameraDistance;

    }

    void adjustCamera()
    {

        if (camera1.transform.position.x > maxPositionX)
            camera1.transform.position = new Vector3(maxPositionX, camera1.transform.position.y, camera1.transform.position.z);
        if (camera1.transform.position.y > maxPositionY)
            camera1.transform.position = new Vector3(camera1.transform.position.x, maxPositionY, camera1.transform.position.z);
        if (camera1.transform.position.x < cameraDistance)
            camera1.transform.position = new Vector3(cameraDistance, camera1.transform.position.y, camera1.transform.position.z);
        if (camera1.transform.position.y < cameraDistance * 0.9f)
            camera1.transform.position = new Vector3(camera1.transform.position.x, cameraDistance * 0.9f, camera1.transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            moveFlag = true;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            moveFlag = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel")!=0)
        {
            cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
            camera1.orthographicSize = cameraDistance;
            maxPositionX = tileSizeX - cameraDistance;
            maxPositionY = tileSizeY - (cameraDistance*0.9f);
            adjustCamera();
        }


        moveDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * cameraDistance / 20;
        if (moveFlag)
        {
            camera1.transform.position -= moveDirection;
            Debug.Log(moveDirection + " " + camera1.transform.position+" "+maxPositionX+" "+maxPositionY);
            adjustCamera();
        }
    }
}