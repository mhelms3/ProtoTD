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
    public float playerMarketValue;

    public Text playerWoodText;
    public Text playerStoneText;
    public Text playerFoodText;
    public Text playerRaceText;
    public Text playerLevelText;

    public int tileSizeX;
    public int tileSizeY;
    public int startingPositionX;
    public int startingPositionY;
    //private float currentRnd = 0;
    private int x, y;    

    public GameObject[] terrainTiles;
    private IDictionary<string, GameObject> terrainDictionary = new Dictionary<string, GameObject>();

    public GameObject squarePrototype;
    public GameObject[,] boardTile;

    public GameObject foundationPrototype;
    public GameObject ruinsPrototype;

    public GameObject playerCastle;
    public Vector3 home; //keep

    

    

    //private Vector3 mousePosition = new Vector3();
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

    void addMarketValue(float v)
    {
        playerMarketValue += v;
        //add some cha-ching effect here, and highlight additional market value
    }
    void createCastle()
    {
        Vector3 startingPosition = new Vector3(startingPositionX, startingPositionY,0);
        GameObject tempObject = Instantiate(playerCastle, startingPosition, Quaternion.identity) as GameObject;
        StructureBehavior sb = (StructureBehavior)tempObject.GetComponent(typeof(StructureBehavior));
        sb.foundationMaterial = "Granite";
        sb.structureMaterial = "Granite";
        sb.SendMessage("updateStructureName");
        sb.marketValue = 100;
        assignStructure(Mathf.RoundToInt(startingPositionX-1), Mathf.RoundToInt(startingPositionY-1), tempObject, true);        
        addMarketValue(100);
        home = new Vector3(startingPositionX, startingPositionY, -1);
        
        Debug.Log(home);
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
                    GameObject tempObject = Instantiate(ruinsPrototype, new Vector3(i + 1, j + 1, 0), Quaternion.identity) as GameObject;
                    bs.foundation = tempObject;
                    currentRnd = Mathf.CeilToInt(Random.value * ruinMaterial.Length);                    
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
        createCastle();

    }

    void Start()
    {

    }

    
    // Update is called once per frame
    void Update()
    {
        playerWood += 1 * Time.deltaTime;
        playerStone += 1 * Time.deltaTime;
        playerFood += 1 * Time.deltaTime;
    }
}