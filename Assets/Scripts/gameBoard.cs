﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class gameBoard : MonoBehaviour
{
    public int startingEnemies;
    public int difficultySetting;
    public string playerRace;
    public int playerLevel;
    public int playerBuildSpeed;
    public int playerWorkers;
    public int playerSoldiers;

    public GameObject arrowTower;
    public GameObject cannonTower;
    public GameObject wizardtower;
    public GameObject holytower;

    public GameObject mill;
    public GameObject mine;
    public GameObject farm;
    public GameObject market;


    public GameObject buildMenuPanel;
    public GameObject structureMenuPanel;
    public GameObject buildTowerMenuPanel;
    public CanvasGroup buildMenuCG;
    public CanvasGroup structureMenuCG;
    public CanvasGroup buildTowerMenuCG;
    public bool buildMenuFlag;
    public bool buildTowerMenuFlag;
    public bool structureMenuFlag;


    public float playerWood;
    public float playerStone;
    public float playerFood;
    public float playerGold;
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

    public bool currentWalkability = false;
    public bool currentEnemy = false;


    public GameObject[] terrainTiles;
    private IDictionary<string, GameObject> terrainDictionary = new Dictionary<string, GameObject>();
    

    public GameObject squarePrototype;
    public GameObject[,] boardTile;
    public Vector2 selectedTile;
    public GameObject selectorAnimation;
    public GameObject selector;
    

    public GameObject foundationPrototype;
    public GameObject ruinsPrototype;

    public GameObject playerCastle;
    public Vector3 home; //keep

    public GameObject[] enemyType;
    public List<GameObject> playerStructures = new List<GameObject>();
    public List<GameObject> enemies;




    //private Vector3 mousePosition = new Vector3();
    //private Vector3 moveDirection = new Vector3();
    //private bool moveFlag = false;
    //private float maxPositionX;
    //private float maxPositionY;

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
        playerWood = 3000 + baseResources;
        playerStone = 3000 + baseResources;
        playerFood = 3000 + baseResources;

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
        playerWoodText.text = Mathf.Round(playerWood).ToString();
        playerStoneText.text = Mathf.Round(playerStone).ToString();
        playerFoodText.text = Mathf.Round(playerFood).ToString();
    }

    void assignStructure(int x, int y, GameObject g, bool overwrite)
    {
        playerStructures.Add(g);
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

    void popStructure(Vector2 v2)
    {
        int x = Mathf.FloorToInt(v2.x);
        int y = Mathf.FloorToInt(v2.y);
        GameObject tile = boardTile[x,y];
        BoardSquare thisSquare = (BoardSquare)tile.GetComponent(typeof(BoardSquare));
        thisSquare.structure = null;
        Debug.Log("Structure popped");
    }

    bool hasStructure(Vector2 v2)
    {
        int x = Mathf.FloorToInt(v2.x);
        int y = Mathf.FloorToInt(v2.y);
        GameObject tile = boardTile[x, y];
        BoardSquare thisSquare = (BoardSquare)tile.GetComponent(typeof(BoardSquare));
        if (thisSquare.structure == null)
            return false;
        else
            return true;
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

        selectedTile = new Vector2(startingPositionX-1, startingPositionY-1);
        selector.transform.position = new Vector3(startingPositionX, startingPositionY, 0);       
        sb.isSelected = true;

        GameObject bt = boardTile[startingPositionX, startingPositionY];
        BoardSquare bs = bt.GetComponent<BoardSquare>();
        bs.SendMessage("UpdateStructurePanel");
            


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
        
        selector = Instantiate(selectorAnimation, new Vector3(startingPositionX+1, startingPositionY+1, 0), Quaternion.identity) as GameObject;


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

    public void popEnemy(GameObject e)
    {
        print("Enemy# " + enemies.Count);
        enemies.Remove(e);
        print("Enemy new # " + enemies.Count);
    }

    void spawnEnemies()
    {
        float xPo, yPo;
        GameObject eGo;
        for(int i=0; i<startingEnemies; i++)
        {
            xPo = Random.value * 7 +  8 ;
            yPo = Random.value * 7 + 8;
            eGo = Instantiate(enemyType[0], new Vector3(xPo, yPo, 1), Quaternion.identity) as GameObject;
            enemies.Add(eGo);
        }
    }

    public void enemyUpdate()
    {
        foreach (GameObject e in enemies)
        {
            e.SendMessage("acquireTarget");
            e.SendMessage("findPath");
        }
        currentEnemy = true;
    }

    void deleteFromPlayerStructures(GameObject g)
    {
        playerStructures.Remove(g);
    }

    void initializeNewStructure(StructureBehavior sb)
    {
        sb.positionX = Mathf.FloorToInt(selectedTile.x);
        sb.positionY = Mathf.FloorToInt(selectedTile.y);
        sb.workerSpeed = playerBuildSpeed;
        sb.integrity = 1;
        sb.percentComplete = 1;
        sb.structureMaterial = "Wood";
        sb.foundationMaterial = "Stone";
        sb.buildFlag = true;
        sb.isSelected = true;
    }

    void deselectCurrentStructure()
    {
        GameObject tempTile = boardTile[Mathf.FloorToInt(selectedTile.x), Mathf.FloorToInt(selectedTile.y)];
        BoardSquare tempBS = tempTile.GetComponent<BoardSquare>();
        if (tempBS.structure != null)
        {
            StructureBehavior tempSB = tempBS.structure.GetComponent<StructureBehavior>();
            tempSB.isSelected = false;
            //Debug.Log("Deselected " + tempSB.structureName);
        }
        //getWalkableSquares();
    }

    void buildTower(string subType, GameObject tower, float stoneRec, float woodRec)
    {
        
        if (hasStructure(selectedTile))
        {
            Debug.Log("Tile has structure already");
        }
        else
        {
         

            Debug.Log("Building Tower");
            if (playerWood > woodRec && playerStone > stoneRec)
            {
                Vector3 currentLocation = new Vector3(selectedTile.x, selectedTile.y, 0);

                GameObject thisTower = Instantiate(tower, new Vector3(selectedTile.x + 1, selectedTile.y + 1, 0), Quaternion.identity) as GameObject;

                assignStructure(Mathf.FloorToInt(selectedTile.x), Mathf.FloorToInt(selectedTile.y), thisTower, false);
                StructureBehavior sb = (StructureBehavior)thisTower.GetComponent(typeof(StructureBehavior));
                initializeNewStructure(sb);
                sb.buildingType = "Tower";
                sb.buildingSubType = subType;
                sb.woodCost = woodRec;
                sb.stoneCost = stoneRec;
                playerWood -= woodRec;
                playerStone -= stoneRec;
                Debug.Log("Tower Built at " + currentLocation);
                openMenu("Structure");
                currentWalkability = false;
                currentEnemy = false;
            }
            else
            {
                if (playerWood < woodRec)
                    Debug.Log("Insufficient Wood");
                if (playerStone < stoneRec)
                    Debug.Log("Insufficient Stone");
            }
        }

    }

   
    void buildArrowTower()
    {
        buildTower("Arrow", arrowTower, 100, 200);
    }

    void buildCannonTower()
    {
        buildTower("Cannon", cannonTower, 250, 150);
    }

    void buildWizardTower()
    {
        buildTower("Wizard", wizardtower, 400, 300);
    }

    void buildHolyTower()
    {
        buildTower("Holy", holytower, 200, 300);
    }

    void buildResourceBuilding(string buildingType, GameObject resBuilding,  float woodReq, float stoneReq)
    {
        if (hasStructure(selectedTile))
        {
            Debug.Log("Tile has structure already");
        }
        else
        {
            GameObject thisBuilding;
            Debug.Log("buildingType");
            if (playerWood >= woodReq && playerStone >= stoneReq && playerWorkers > 0)
            {
                thisBuilding = Instantiate(resBuilding, new Vector3(selectedTile.x + 1, selectedTile.y + 1, 0), Quaternion.identity) as GameObject;
                Vector3 currentLocation = new Vector3(selectedTile.x, selectedTile.y, 0);
                assignStructure(Mathf.FloorToInt(selectedTile.x), Mathf.FloorToInt(selectedTile.y), thisBuilding, false);
                StructureBehavior sb = (StructureBehavior)thisBuilding.GetComponent(typeof(StructureBehavior));
                sb.buildingType = buildingType;
                initializeNewStructure(sb);

                sb.woodCost = woodReq;
                sb.stoneCost = stoneReq;
                playerWood -= woodReq;
                playerStone -= stoneReq;
                playerWorkers -= 1;
                Debug.Log(buildingType + " built at " + currentLocation);
                openMenu("Structure");
            }
            else if (playerWorkers < 1)
            {
                Debug.Log("Insufficient Workers");
            }
            else
            {
                if (playerWood < woodReq)
                    Debug.Log("Insufficient Wood");
                if (playerStone < stoneReq)
                    Debug.Log("Insufficient Stone");
            }
        }
    }

    void buildMine()
    {
        buildResourceBuilding("Mine", mine, 100, 100);
        Debug.Log("Mine");
    }   
    void buildMill()
    {
        buildResourceBuilding("Mill", mill, 100, 100);
        Debug.Log("Mill");
    }
    void buildFarm()
    {
        buildResourceBuilding("Farm", farm, 100, 100);
        Debug.Log("Farm");
    }
    void buildMarket()
    {
        buildResourceBuilding("Market", market, 100, 100);
        Debug.Log("Market");
    }



    void initializeGUI()
    {
        buildMenuPanel = GameObject.Find("BuildMenuPanel");
        structureMenuPanel = GameObject.Find("StructureMenuPanel");
        buildTowerMenuPanel = GameObject.Find("BuildTowerMenuPanel");
        buildMenuCG  = (CanvasGroup)buildMenuPanel.GetComponent<CanvasGroup>();
        structureMenuCG = (CanvasGroup)structureMenuPanel.GetComponent<CanvasGroup>();
        buildTowerMenuCG = (CanvasGroup)buildTowerMenuPanel.GetComponent<CanvasGroup>();
    }

    void getWalkableSquares()
    {
        print("updating walkability");
        foreach (GameObject go in boardTile)
        {
            int layerMask = 1 << 8;
            BoardSquare b = go.GetComponent<BoardSquare>();
            Vector3 checkArea = new Vector3(b.positionX+.5f, b.positionY-.5f, 0);            
            b.isWalkable = !(Physics.CheckBox(checkArea, Vector3.one*.2f, Quaternion.identity, layerMask));
            
            
        }
        currentWalkability = true;
    }

    void setWalkableCheck()
    {
        if (currentWalkability)
            currentWalkability = false;
    }

    void setEnemyCheck()
    {
        if (currentEnemy)
            currentEnemy = false;
    }

    void OnDrawGizmos()
    {
        List<BoardSquare> bs = new List<BoardSquare>();
        pathFindingScript pfs = gameObject.GetComponent<pathFindingScript>();
        bs = pfs.finalPath;
        if (boardTile != null)
        {
            foreach (GameObject go in boardTile)
            {
                BoardSquare b = go.GetComponent<BoardSquare>();
                if (!b.isWalkable)
                    Gizmos.color = Color.red;
                else if (bs.Contains(b))
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(b.positionX + .5f, b.positionY - .5f, 0), Vector3.one * .9f);
            }
        }
    }
    

    void openCanvasGroup(CanvasGroup this_cg)
    {
        this_cg.interactable = true;
        this_cg.alpha = 1;
        this_cg.blocksRaycasts = true;
    }

    void closeCanvasGroup(CanvasGroup this_cg)
    {
        this_cg.interactable = false;
        this_cg.alpha = 0;
        this_cg.blocksRaycasts = false;
    }

    void toggleTowerMenu ()
    {
        if (buildTowerMenuFlag)
            closeCanvasGroup(buildTowerMenuCG);
        else
            openCanvasGroup(buildTowerMenuCG);
        buildTowerMenuFlag = !buildTowerMenuFlag;
    }

    void openMenu(string menu)
    {
        
        if(menu == "Build")
        {
            //openCanvasGroup(buildMenuCG);
            buildMenuFlag = true;
            structureMenuFlag = false;
        }   
        else if(menu=="Structure")
        {
            //closeCanvasGroup(buildMenuCG);
            buildMenuFlag = false;
            structureMenuFlag = true;
        }
    }

    void Awake()
    {
        enemies = new List<GameObject>();
        initializePlayer();
        updateResourceText();
        playerRaceText.text = playerRace;
        initializeGameBoard();
        createCastle();
        initializeGUI();
        getWalkableSquares();
        spawnEnemies();

        
    }
    
    private void updateMarketIncome()
    {
        float marketIncome = playerMarketValue / 1000 * Time.deltaTime;
        playerWood += marketIncome;
        playerStone += marketIncome;
        playerFood += marketIncome;

        if (playerRace == "Dwarf")
        {
            playerStone += playerLevel / 50f * Time.deltaTime;
        }
        else if (playerRace == "Elf")
        {
            playerWood += playerLevel / 50f * Time.deltaTime;
        }
        else if (playerRace == "Gnome")
        {
            playerFood += playerLevel / 50f * Time.deltaTime;
        }
        else if (playerRace == "Human")
        {
            playerWood += marketIncome * playerLevel * .33f;
            playerStone += marketIncome * playerLevel * .33f;
            playerFood += marketIncome * playerLevel * .33f;
        }
    }
    // Update is called once per frame

    void LateUpdate()
    {
        if (!currentWalkability)
        {
            getWalkableSquares();
        }

        if (!currentEnemy)
        {
            enemyUpdate();
        }
        if (playerStructures.Count == 0)
        {
            print("Quit");
            Application.Quit();
        }
    }

    void Update()
    {
        updateMarketIncome();       
        updateResourceText();

        if (Input.GetKeyDown(KeyCode.A))
        {
            buildArrowTower();
            Debug.Log(buildMenuFlag);            
        }

        
       
    }
}