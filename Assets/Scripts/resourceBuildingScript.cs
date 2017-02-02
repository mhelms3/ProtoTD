using UnityEngine;
using System.Collections;

public class resourceBuildingScript : MonoBehaviour {

    public bool isActive;

    public string subType;
    public gameBoard cgb;

    private BoardSquare home;
    
    
    private StructureBehavior sb;
    private float [] resourceModifier = { 0f, 0f, 0f, 0f, 0f, 0f};
    public float resourceStore;
    public float resourceShip;
    public string resourceType;

    
    public float currentProduction()
    {
        return resourceModifier[sb.buildingLevel]*5*sb.supplyLevel;
    }


    // Use this for initialization
    void Awake()
    {
        resourceStore = 0;
        resourceShip = 25;
        isActive = false;
        
    }

    void Start()
    {
        cgb = FindObjectOfType<gameBoard>();
        sb = this.GetComponentInParent<StructureBehavior>();
        home = sb.homeSquare;
        calculateResourceValue();
    }

    float getNeighborMaterialValue (int i, string m)
    {
        int rank=0;
        if (i == 1)
            rank = 8;
        else
            rank = 24;
        BoardSquare[] neighbors = new BoardSquare[rank];
        int count = 0;
        int xCoord = home.positionX;
        int yCoord = home.positionY;
        int checkCoordX, checkCoordY;

        for(int kfor = -i; kfor<i+1; kfor++)
            for (int jfor = -i; jfor<i+1; jfor++)
            {
                if ((kfor != 0) || (jfor != 0))
                {
                    checkCoordX = Mathf.Clamp(xCoord + kfor, 0, gameBoard.tileSizeX);
                    checkCoordY = Mathf.Clamp(xCoord + kfor, 0, gameBoard.tileSizeY);
                    neighbors[count] = cgb.boardTile[checkCoordX, checkCoordY].GetComponent<BoardSquare>();
                    count++;
                }
            }

        float sumTotal = 0;
        foreach (BoardSquare bsn in neighbors)
        {
            if (bsn != null)
            {
                if (m == "Wood")
                {
                    sumTotal = sumTotal + bsn.wood;
                }
                else if (m == "Stone")
                {
                    sumTotal = sumTotal + bsn.stone;
                }
                else if (m == "Food")
                {
                    sumTotal = sumTotal + bsn.food;
                }
                else if (m == "Gold")
                {
                    sumTotal = sumTotal + (bsn.food + bsn.stone + bsn.wood) / 3;
                }
            }
            
        }
        return sumTotal;
    }
    
    void calculateResourceValue()
    {
        float level1materialValue = getNeighborMaterialValue(1, resourceType);
        float level2materialValue = getNeighborMaterialValue(2, resourceType); ;
        
        if (resourceType == "Wood")
        {
            resourceModifier[0] = home.wood / 200;
            resourceModifier[1] = resourceModifier[0] * 2;
        }
        else if (resourceType == "Stone")
        {
            resourceModifier[0] = home.stone / 200;
            resourceModifier[1] = resourceModifier[0] * 2;
        }
        else if (resourceType == "Food")
        {
            resourceModifier[0] = home.food / 200;
            resourceModifier[1] = resourceModifier[0] * 2;
        }
        else if (resourceType == "Gold")
        {
            resourceModifier[0] = (home.food + home.wood + home.stone) / 600;
            resourceModifier[1] = resourceModifier[0] * 2;
        }

        resourceModifier[2] = resourceModifier[1] * 1.2f + level1materialValue * .0025f;
        resourceModifier[3] = resourceModifier[1] * 1.4f + level1materialValue * .0055f;
        resourceModifier[4] = resourceModifier[1] * 1.6f + level1materialValue * .01f;
        resourceModifier[5] = resourceModifier[4] * 1.2f + (level2materialValue - level1materialValue) * .005f;
    }

    void Update()
    {
     
        if (isActive)
        {
            resourceStore += 5*resourceModifier[sb.buildingLevel] * sb.supplyLevel * Time.deltaTime;
            if (resourceStore > resourceShip)
            {
                if (resourceType == "Stone")
                {
                    cgb.playerStone += resourceStore;
                }
                else if (resourceType == "Wood")
                {
                    cgb.playerWood += resourceStore;
                }
                else if (resourceType == "Food")
                {
                    cgb.playerFood += resourceStore;
                }
                {
                    cgb.playerGold += resourceStore;
                }
                resourceStore = 0;
                cgb.SendMessage("updateResourceText");
            }

        }
        else
        {
            if (sb.buildFlag == false && sb.percentComplete > 99)
                isActive = true;
        }

    }
}
