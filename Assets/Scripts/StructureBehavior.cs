using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StructureBehavior : MonoBehaviour {

    public GameObject buildMenu;    
    public int positionX;
    public int positionY;
    public float workerSpeed;
    public int buildingLevel;
    public float supplyLevel;
    public float costRamp;

    public float percentComplete;
    public float integrity;

    public bool isActiveBuilding;
    public bool buildFlag;
    public bool upgradeFlag;
    public bool updatedButtonFlag;
    public bool hasDestroyOrder;

    public string structureName;
    public string buildingType;
    public string buildingSubType;
    public BoardSquare homeSquare;
    public GameObject closestSource; // for now, default to Castle; change this when supply depots become available.
    public GameObject popUp;

    private string[] structureMaterial;
    private int[] _units = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //private int[] _items = new int[5];

    private float costMultiplier;

    //private _material[] structureMaterial = new _material[5];
    //private _unit[] occupants = new _unit[10];
    //private _item[] placedItem = new _unit[5];

    public float woodCost;
    public float[] woodUpgradeCosts = new float[5];
    public float stoneCost;
    public float[] stoneUpgradeCosts = new float[5];
    public float marketValue;
    public bool isSelected = false;

    public IList<string> specials = new List<string>();

    

    //bool isFoundationOnly;


    SpriteRenderer[] barSprites;  //for healthbars

    public Slider aSlider;
    public Text aText;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "EnemyAmmo" && gameObject.tag != "Generator")
        {
            ammoScript aScript = col.gameObject.GetComponent<ammoScript>();
            float damage = Mathf.Round(UnityEngine.Random.value * (aScript.damageUpper - aScript.damageLower) + aScript.damageLower);
            GameObject dText = Instantiate(popUp, transform.position, Quaternion.identity) as GameObject;
            dText.GetComponent<numberPop>().updateText(damage.ToString("0"));
            dText.GetComponent<numberPop>().updateColor(Color.red);
            dText.GetComponent<numberPop>().setTextPos(transform.position);
            integrity -= damage;
            updateStatusBars();
            Destroy(col.gameObject);
        }
    }
    string makeSpecialsString()
    {
        string tempString = "";
        foreach (string s in specials)
        {
            tempString = tempString + s + "\r\n";
        }

        return (tempString);
    }

    public void SquareAssignment(BoardSquare bs)
    {
        homeSquare = bs;
    }

    public void updateBuildingMenuPanel()
    {
        buildingMenuScript bms = buildMenu.GetComponent<buildingMenuScript>();
        if (buildingType == "Tower")
        {
            print("Tower:" + structureName);
            towerScript ts = gameObject.GetComponent<towerScript>();
            if(ts==null)
            {
                print("null towerscript error");
                bms.updatePanel(this);
            }
            bms.updatePanelTower(this, ts);
        }
        else if (buildingType == "Resource")
        {
            print("Resource:" + structureName);
            resourceBuildingScript rbs = gameObject.GetComponent<resourceBuildingScript>();
            if (rbs == null)
            {
                print("null resourceScript error");
                bms.updatePanel(this);
            }
            bms.updatePanelResource(this, rbs);
        }
        else
        {
            bms.updatePanel(this);
        }
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UpdateStructurePanel();
            updateBuildingMenuPanel();
            homeSquare.SendMessage("updateTerrainUI");
            homeSquare.SendMessage("updateStructureUI");
        }
    }

     
    public bool canBeUpgraded(gameBoard gb)
    {

        if (buildFlag || percentComplete < 100 || upgradeFlag || buildingLevel>4)
        {
            print("Cannot upgrade incomplete building.");
            return false;
        }
        else if (!isActiveBuilding)
        {
            print("Cannot upgrade inactive building.");
            return false;
        }
        else
        {
            if ((gb.playerStone >= stoneCost * costMultiplier) && (gb.playerWood >= woodCost * costMultiplier))
                return true;
            else
            {
                if (gb.playerStone < stoneCost * costMultiplier)
                {
                    print("Insufficient STONE to upgrade." + (stoneCost * costMultiplier).ToString("0.0") + " needed.");
                }
                if (gb.playerWood < woodCost * costMultiplier)
                {
                    print("Insufficient WOOD to upgrade." + (woodCost * costMultiplier).ToString("0.0") + " needed.");
                }
                return false;
            }
        }
    }

    public void changeColor()
    {
        SpriteRenderer s = this.GetComponentInParent<SpriteRenderer>();
        switch(buildingLevel)
        {
            case 1:
                s.color = Color.yellow;
                break;
            case 2:
                s.color = Color.green;
                break;
            case 3:
                s.color = Color.cyan;
                break;
            case 4:
                s.color = Color.blue;
                break;
            case 5:
                s.color = Color.red;
                break;
            default:
                s.color = Color.white;
                break;
        }
    }

    private void beginUpgrade()
    {
        isActiveBuilding = false;
        buildFlag = true;
        upgradeFlag = true;
        percentComplete = 0.1f;
        if (buildingType == "Tower")
        {
            GetComponentInParent<towerScript>().isActive = false;
        }
    }

    private void completeUpgrade()
    {
        upgradeFlag = false;
        buildingLevel++;
        changeColor();
        marketValue += stoneCost * costMultiplier + woodCost * costMultiplier;
        if (buildingType == "Tower")
        {
            GetComponentInParent<towerScript>().isActive = true;
            GetComponentInParent<towerScript>().updateDamageModifiers();
        }
        updateBuildingMenuPanel();
    }

    public void updgradeBuilding()
    {
        costMultiplier = Mathf.Pow((buildingLevel + 1.1f), 1.2f);
        gameBoard cgb = GameObject.FindObjectOfType<gameBoard>();
        if(canBeUpgraded(cgb))
        {
            cgb.playerStone -= stoneCost * costMultiplier;
            cgb.playerWood -= woodCost * costMultiplier;
            beginUpgrade();
        }
    }

    public void UpdateStructurePanel()
    {
        GameObject temp = GameObject.Find("Health Slider");
        if (temp != null)
            aSlider = temp.GetComponent<Slider>();
        aSlider.value = integrity;

        if (integrity < 100)
            updateStatusBars();

        if (percentComplete < 100)
        {
            temp = GameObject.Find("Construction Slider");
            if (temp != null)
            {
                aSlider = temp.GetComponent<Slider>();
                aSlider.value = integrity;
            }
        }

        temp = GameObject.Find("Structure Name");
        if (temp != null)
        {
            aText = temp.GetComponent<Text>();
            aText.text = structureName;
        }

        temp = GameObject.Find("Materials Label");
        string tempString = "";
        if (temp != null)
        {
            aText = temp.GetComponent<Text>();
            int count = 0;
            foreach (string s in structureMaterial)
            {
                if (s != null && s!= "")
                {
                    tempString = tempString + "Level "+count+ " Material: " + s +"\n";
                    count++;
                }
                else
                    break;
            }
            aText.text = tempString;
        }

        temp = GameObject.Find("Special Properties");
        if (temp != null)
        {
            aText = temp.GetComponent<Text>();
            string specialsTemp = makeSpecialsString();
            aText.text = specialsTemp;
        }       
    }

   

    
    // Use this for initialization
    void Awake()
    {
        percentComplete = 99;
        integrity = 99;
        buildFlag = true;
        isActiveBuilding = false;
        workerSpeed = 25;
        buildingLevel = 0;
        barSprites = this.GetComponentsInChildren<SpriteRenderer>();
        buildMenu = GameObject.Find("BuildingMenu");
        costMultiplier = 1;
        costRamp = 1.2f;

        //int inc = 0;
        structureMaterial = new string[5];
        for (int i=0; i<5; i++)
            structureMaterial[i] = "";
        /*
         foreach (string s in structureMaterial)
         {

             inc++;
         }
         */
    }

    void updateStructureName()
    {
        if (buildingType == null)
            structureName = "Null void [error]";
        else if (buildingType == "Ruin")
        {
            structureName = "Ruin";
        }
        else if (buildingSubType == null || buildingSubType == "" || buildingType == "Wall")
        {
            structureName = buildingType;
        }
        else
        {
            structureName = buildingSubType + " " + buildingType;
        }
    }

    public void updateMaterialType(string s, int materialLevel)
    {
        structureMaterial[materialLevel] = s;
        updateStructureName();
    }
   


    void updateStatusBars()
    {
        foreach (SpriteRenderer c in barSprites)
        {      
            if (c.name == "HealthBar")
            {      
                if (integrity > 99.9)
                    c.enabled = false;
                else
                    c.enabled = true;

                if (integrity > 66)
                    c.color = Color.green;
                else if (integrity > 33)
                    c.color = Color.yellow;
                else
                    c.color = Color.red;
                c.transform.localScale = new Vector3(integrity / 100, 1, 1);
            }
            if (c.name == "BuildBar")
            {
                if (percentComplete == 100 || !buildFlag)
                {                    
                    c.enabled = false;
                    buildFlag = false;
                    if (buildingType == "Tower")
                    {
                        GetComponentInParent<towerScript>().isActive = true;
                    }
                }
                else
                {
                    c.transform.localScale = new Vector3(percentComplete / 100, 1, 1);
                }
            }
        }
    }

    private GameObject findClosestSource()
    {
        
        if (this.tag == "Castle")
            return null;
        else
        {
            float minDistance = 999999;
            float distanceToThis = 0;
            GameObject theSource = null;
            GameObject[] supplies = GameObject.FindGameObjectsWithTag("Supply");
            GameObject[] castles = GameObject.FindGameObjectsWithTag("Castle");
            GameObject[] allSupply = new GameObject[supplies.Length + castles.Length];

            supplies.CopyTo(allSupply, 0);
            castles.CopyTo(allSupply, supplies.Length);
            if (allSupply.Length > 0)
            {
                foreach (GameObject s in allSupply)
                {
                    //Debug.Log("Is in Supply: " + s.name + " distance: " + distanceToThis.ToString("0.00"));
                    if (this.transform.position != s.transform.position) //supply depots should not count themselves
                    {
                        if (!s.GetComponent<StructureBehavior>().buildFlag) //make sure supply depot is not still being built
                        {
                            distanceToThis = Vector3.Distance(this.transform.position, s.transform.position);
                            //Debug.Log("Counts as Supply: " + s.name + " distance: " + distanceToThis.ToString("0.00"));
                            if (distanceToThis < minDistance)
                            {
                                minDistance = distanceToThis;
                                theSource = s;
                            }
                        }
                    }
                }
            }
            return (theSource);
        }
    }

    public void calculateSupplyLevel()
    {
        GameObject closestSource = findClosestSource();
        float distanceToThis = 0;
        if (closestSource != null)
        {
            distanceToThis = Vector3.Distance(this.transform.position, closestSource.transform.position);
            //Debug.Log("Name:"+structureName+"Calculated Supply: " + closestSource.name+ " distance: " + distanceToThis.ToString("0.00"));
        }
        if (distanceToThis < 3)
            supplyLevel = 1;
        else if (distanceToThis < 20)
            supplyLevel = 1 - ((distanceToThis - 3) / 20);
        else
            supplyLevel = .05f;
    }

    public void calculateWorkerSpeed()
    {
        if(buildingType!="Ruin")
            workerSpeed = (workerSpeed/homeSquare.buildTimeModifier) * supplyLevel;
        //Debug.Log("Calculated Speed: " + workerSpeed + " supply: " + supplyLevel + "terrain effect: "+ homeSquare.buildTimeModifier);
    }

    public void calculateUpgradeCosts()
    {
        float cm = 0;
        if (buildingType != "Ruin")
            for (int count = 0; count<woodUpgradeCosts.Length; count++)
            {
                cm = Mathf.Pow((count + 1.1f), costRamp);
                woodUpgradeCosts[count] = cm * woodCost;
                stoneUpgradeCosts[count] = cm * stoneCost;
            }
    }


    void Start () {
        updateStructureName();
        hasDestroyOrder = false;
        if (buildingType != "Ruin")
        {
            calculateSupplyLevel();
            calculateUpgradeCosts();
        }
        else if(buildingType == "Ruin")
        {
            integrity = 100;
            percentComplete = 100;
            updateStatusBars();
        }
    }

    void refund(float percentRefund)
    {
        //give player resources back
        FindObjectOfType<gameBoard>().playerWood += percentRefund * woodCost;
        FindObjectOfType<gameBoard>().playerStone += percentRefund * stoneCost;
    }

    public int unitCount()
    {
        int count = 0;
        foreach (int u in _units)
        {
            if (u > 0)
                count++;
        }
        return count;
    }

    void freeUnits()
    {
        //resourceBuildingScript buildingScript = gameObject.GetComponent<resourceBuildingScript>();
        int unitsToFree = unitCount();
        if (buildingType == "Resource")
            FindObjectOfType<gameBoard>().playerWorkers += unitsToFree;
        else if (buildingType == "Tower")
            FindObjectOfType<gameBoard>().playerSoldiers += unitsToFree;
    }
    

    void destroyThis()
    {
        //print("Death");
        Destroy(gameObject);
        gameBoard cgb = FindObjectOfType<gameBoard>();
        cgb.SendMessage("popStructure", new Vector2(positionX, positionY));
        if (buildingType != "Ruin")
        {
            freeUnits();
            cgb.SendMessage("deleteFromPlayerStructures", gameObject);
            if (isSelected)
                cgb.SendMessage("openMenu", "Build");
            cgb.SendMessage("setWalkableCheck");
            cgb.SendMessage("setEnemyCheck");
        }
        //Debug.Log("Destroying " + buildingType + " at [" + positionX + "," + positionY + "]");
    }
   
    private void updateButtons()
    {
        
        //use this to disable/enble buttons, depending on whether they are available or not according to resource availability
        //maybe this should be a function of the button, not the building??? Probably.

        //gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        //cgb.SendMessage("updateButtonStatus", gameObject);
    }
    // Update is called once per frame
    void Update () {

        if (buildingType != "Ruin")
        {
            if (isSelected && !updatedButtonFlag)
            {
                updateButtons();
                updatedButtonFlag = true;
            }
            else if (!isSelected && updatedButtonFlag)
            {
                updatedButtonFlag = false;
            }

            if (percentComplete < 100 && buildFlag)
            {
                percentComplete += (Time.deltaTime * workerSpeed) / (buildingLevel + 1);
                if (percentComplete >= 100)
                {
                    percentComplete = 100;
                    buildFlag = false;
                    isActiveBuilding = true;
                    if (upgradeFlag)
                        completeUpgrade();

                    if (buildingType == "Tower")
                    {
                        GetComponentInParent<towerScript>().isActive = true;
                    }
                    else if (buildingSubType == "Depot")
                    {
                        FindObjectOfType<gameBoard>().recalculateSupply();
                    }
                }

                integrity += Time.deltaTime * workerSpeed;
                if (integrity > percentComplete)
                    integrity = percentComplete;

                updateStatusBars();
                if (isSelected)
                {
                    UpdateStructurePanel();
                    updateBuildingMenuPanel();
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace) && isSelected)
            {
                //gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
                if (buildingType != "Castle")
                {
                    float refundPercentage = 1.00f;
                    if (!buildFlag)
                        refundPercentage = .50f;
                    refund(refundPercentage);
                    if (buildingType == "Resource")
                        freeUnits();
                    destroyThis();
                }
                else if (buildingType != "Castle")
                {
                    Debug.Log("DONT DESTROY YOUR OWN CASTLE!!!");
                }

            }
        }

        if (integrity<0)
        {
            if (!hasDestroyOrder)
                destroyThis();
            hasDestroyOrder = true;
        }
    }
}
