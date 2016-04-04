using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class CreateGameBoard : MonoBehaviour
{
    public int difficultySetting;
    public string playerRace;
    public int playerLevel;

    public float playerWood;
    public float playerStone;
    public float playerFood;

    public Text playerWoodText;
    public Text playerStoneText;
    public Text playerFoodText;
    public Text playerRaceText;
    public Text playerLevelText;

    public int tileSizeX;
    public int tileSizeY;
    private float currentRnd = 0;
    private int x, y;    

    public GameObject[] terrainTiles;
    private IDictionary<string, GameObject> terrainDictionary = new Dictionary<string, GameObject>();

    public GameObject squarePrototype;
    public GameObject[,] boardTile;

    public GameObject foundationPrototype;

    public GameObject playerCastle;

    

    public Transform camTransform;
    public Camera camera1;
    private GameObject currentTile;
    public float maxCameraDistance = 50f;
    public float minCameraDistance = 5f;
    public float cameraDistance = 10f;
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
            terrainDictionary.Add(tempTerrainName, g);
        }
    }

    float initializeResources(float average)
    {
        float newValue = Mathf.Clamp((average + Random.Range(-10, 10) + Random.Range(-10, 10) + Random.Range(-10, 10)), 1, 200);
        return newValue;
    }

    void initializePlayer()
    {
        float baseResources = (5 - difficultySetting) * 200;
        if (baseResources < 0)
            baseResources = 0;
        playerWood = 100 + baseResources;
        playerStone = 100 + baseResources;
        playerFood = 100 + baseResources;

        if (playerRace == "Dwarf")
            playerStone += 200;
        else if (playerRace == "Elf")
            playerWood += 200;
        else if (playerRace == "Gnome")
            playerFood += 200;
        else if (playerRace == "Human")
        {
            playerWood += 50;
            playerStone += 50;
            playerFood += 50;
        }
    }

    void updateLevelText()
    {
        playerLevelText.text = "Level "+ playerLevel.ToString();
    }

    void updateResourceText()
    {
        playerWoodText.text = playerWood.ToString();
        playerStoneText.text = playerStone.ToString();
        playerFoodText.text = playerFood.ToString();
    }

    void assignStructure(int x, int y, GameObject g, bool overwrite)
    {
        GameObject tile = boardTile[x, y];
        BoardSquare thisSquare = (BoardSquare)tile.GetComponent(typeof(BoardSquare));
        if (thisSquare.structure == null)
            thisSquare.structure = g;
        else if (overwrite)
        {
            Destroy(thisSquare.structure);
            Debug.Log("Structure Destroyed by Overwrite");
            thisSquare.structure = g;
        }
        else
            Debug.Log("Overloaded Structure Error");
    }

    void createCastle()
    {
        Vector3 startingPosition = camera1.transform.position;
        GameObject tempObject = Instantiate(playerCastle, startingPosition, Quaternion.identity) as GameObject;
        assignStructure(Mathf.RoundToInt(camera1.transform.position.x-1), Mathf.RoundToInt(camera1.transform.position.y-1), tempObject, true);
    }

    void initializeGameBoard()
    {
        initializeTerrainDictionary();
        string tempTile;
        GameObject currentTerrain;
        BoardTerrain bt;
        BoardSquare bs;
        SpriteRenderer srTerrain;
        SpriteRenderer srSquare;
        int currentRnd;
        string[] ruinMaterial = { "Marble", "Granite", "Obsidian", "Limestone", "Basalt", "Brownstone", "Flagstone", "Quadratum" };

        boardTile = new GameObject[tileSizeX, tileSizeY];

        for (int i = 0; i < tileSizeX; i++)
            for (int j = 0; j < tileSizeY; j++)
            {
                currentRnd = Mathf.CeilToInt(Random.value * 9 + .49f);
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

                boardTile[i, j] = Instantiate(squarePrototype, new Vector3(i + 1, j + 1, 0), Quaternion.identity) as GameObject;
                bs = (BoardSquare)boardTile[i, j].GetComponent(typeof(BoardSquare));
                srSquare = (SpriteRenderer)boardTile[i, j].GetComponent(typeof(SpriteRenderer));

                bs.squareName = bt.terrainName;
                bs.wood = initializeResources(bt.averageWoodValue);
                bs.stone = initializeResources(bt.averageStoneValue);
                bs.food = initializeResources(bt.averageFoodValue);
                bs.positionX = i;
                bs.positionY = j;

                srSquare.sprite = srTerrain.sprite;

                currentRnd = Mathf.CeilToInt(Random.value * 100 + .49f);
                if (currentRnd < 3)
                {
                    GameObject tempObject = Instantiate(foundationPrototype, new Vector3(i + 1, j + 1, 0), Quaternion.identity) as GameObject;
                    bs.foundation = tempObject;
                    //foundationScript fs = tempObject.GetComponent<foundationScript>();
                    currentRnd = Mathf.CeilToInt(Random.value * ruinMaterial.Length);
                    //Debug.Log("No Mats:"+ruinMaterial.Length + " Rnd:" + currentRnd);
                    tempObject.SendMessage("RuinStart", ruinMaterial[currentRnd - 1]);
                    tempObject.SendMessage("SquareAssignment", bs);
                }
            }
    }

    void Awake()

    {
        initializePlayer();
        updateResourceText();
        playerRaceText.text = playerRace;
        initializeGameBoard();
        camera1.transform.position = new Vector3(tileSizeX / 2, tileSizeY / 2, -1);
        camera1.orthographicSize = cameraDistance;
        createCastle();
        


    }

    void Start()
    {

    }

    void adjustCamera()
    {
        float height = (camera1.orthographicSize * 2.0f);
        float width = height * Screen.width / Screen.height;

        float maxX = tileSizeX-width/2+7;
        float minX = width/2;

        float maxY = tileSizeY - height / 2;
        float minY = height/2;


        if (camera1.transform.position.x > maxX)
            camera1.transform.position = new Vector3(maxX, camera1.transform.position.y, camera1.transform.position.z);

        if (camera1.transform.position.y > maxY)
            camera1.transform.position = new Vector3(camera1.transform.position.x, maxY, camera1.transform.position.z);

        if (camera1.transform.position.x < minX)
            camera1.transform.position = new Vector3(minX, camera1.transform.position.y, camera1.transform.position.z);

        if (camera1.transform.position.y < minY)
            camera1.transform.position = new Vector3(camera1.transform.position.x, minY, camera1.transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {

        playerWood += 1 * Time.deltaTime;
        playerStone += 1 * Time.deltaTime;
        playerFood += 1 * Time.deltaTime;

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
            maxPositionX = tileSizeX - (cameraDistance*0.7f);
            maxPositionY = tileSizeY - (cameraDistance*0.85f);
            adjustCamera();
        }


        moveDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * cameraDistance / 20;
        if (moveFlag)
        {
            camera1.transform.position -= moveDirection;
            //Debug.Log(moveDirection + " " + camera1.transform.position+" "+maxPositionX+" "+maxPositionY);
            adjustCamera();
        }
    }
}