using UnityEngine;
using System.Collections;

public class resourceBuildingScript : MonoBehaviour {

    public int resourceBuldingLevel;
    public int workers;
    public int maxWorkers;
    public bool isActive;

    public string subType;
    public GameObject gb;
    public gameBoard cgb;
    public BoardSquare bs;

    public float resourceStore;
    public float resourceShip;
    public string resourceType;

    public void calculateMaxWorkers()
    {
        maxWorkers = 2 ^ resourceBuldingLevel;
    }
    // Use this for initialization
    void Awake()
    {
        resourceStore = 0;
        resourceShip = 25;
        resourceBuldingLevel = 1;
        workers = 1;
        maxWorkers = 2;
        gb = GameObject.Find("GameBoard");
        cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        //cgb.playerStone += (bs.wood/20) * mineLevel * Time.deltaTime;
        if (isActive)
        {
            resourceStore += 5 * workers * Time.deltaTime;
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
                cgb.SendMessage("UpdateResourceText");
            }

        }
        else
        {
            StructureBehavior sb = GetComponent<StructureBehavior>();
            if (sb.buildFlag == false && sb.percentComplete > 99)
                isActive = true;
        }

    }
}
