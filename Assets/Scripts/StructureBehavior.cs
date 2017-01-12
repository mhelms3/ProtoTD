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

    public float percentComplete;
    public float integrity;

    public bool buildFlag;
    public bool updatedButtonFlag;
    public bool hasDestroyOrder;

    public string structureName;
    public string buildingType;
    public string buildingSubType;
    public BoardSquare homeSquare;
    public GameObject closestSource; // for now, default to Castle; change this when supply depots become available.

    private string []structureMaterial = new string[5];

    //private _material[] structureMaterial = new _material[5];
    //private _unit[] occupants = new _unit[10];
    //private _item[] placedItem = new _unit[5];

    public float woodCost;
    public float stoneCost;
    public float marketValue;
    public bool isSelected = false;

    public IList<string> specials = new List<string>();

    

    //bool isFoundationOnly;


    SpriteRenderer[] barSprites;  //for healthbars

    public Slider aSlider;
    public Text aText;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        //print("Hit building");
        if (col.gameObject.tag == "EnemyAmmo")
        {
            ammoScript aScript = col.gameObject.GetComponent<ammoScript>();
            float damage = Mathf.Round(Random.value * (aScript.damageUpper - aScript.damageLower) + aScript.damageLower);
            integrity -= damage;
            //print("Building hit for " + damage + " damage");
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

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UpdateStructurePanel();
            homeSquare.SendMessage("updateTerrainUI");
            homeSquare.SendMessage("updateStructureUI");
            buildingMenuScript bms = buildMenu.GetComponent<buildingMenuScript>();

            if (buildingType == "Tower")
            {
                towerScript ts = gameObject.GetComponent<towerScript>();
                bms.updatePanel(this, ts);
            }
            else if (buildingType == "Resource")
            {
                resourceBuildingScript rbs = gameObject.GetComponent<resourceBuildingScript>();
                bms.updatePanel(this, rbs);
            }
            else 
            {
                bms.updatePanel(this);
            }


        }
    }

    void UpdateStructurePanel()
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
        if (temp != null)
        {
            aText = temp.GetComponent<Text>();
            int count = 0;
            foreach (string s in structureMaterial)
            {
                if (s != null)
                {
                    aText.text = "Level "+count+ " Material: " + s +"\n";
                    count++;
                }
                else
                    break;
            }            
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
        workerSpeed = 25;
        buildingLevel = 0;
        barSprites = this.GetComponentsInChildren<SpriteRenderer>();
        buildMenu = GameObject.Find("BuildingMenu");
        
       

    }

    void updateStructureName()
    {
        if (buildingType == null)
            structureName = "Null void [error]";
        else if (buildingType == "Ruin")
        {
            structureName = "Ruin";
        }
        else if (buildingSubType == null || buildingSubType == "")
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
    // Update is called once per frame


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
                        towerScript ts = gameObject.GetComponent<towerScript>();
                        ts.isActive = true;
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
                    if (this.transform.position != s.transform.position) //supply depots should not count themselves
                    {
                        distanceToThis = Vector3.Distance(this.transform.position, s.transform.position);
                        //Debug.Log("Supply: " + s.name + " distance: " + distanceToThis.ToString("0.00"));
                        if (distanceToThis < minDistance)
                        {
                            minDistance = distanceToThis;
                            theSource = s;
                        }
                    }
                }
            }
            return (theSource);
        }
    }

    private void calculateSupplyLevel()
    {
        GameObject closestSource = findClosestSource();
        float distanceToThis = 0;
        if (closestSource != null)
        {
            distanceToThis = Vector3.Distance(this.transform.position, closestSource.transform.position);
            //Debug.Log("Calculated Supply: " + closestSource.name + " distance: " + distanceToThis.ToString("0.00"));
        }
        if (distanceToThis < 3)
            supplyLevel = 1;
        else if (distanceToThis < 20)
            supplyLevel = 1 - ((distanceToThis - 3) / 20);
        else
            supplyLevel = .05f;

    }

    private void calculateWorkerSpeed()
    {
        workerSpeed = (workerSpeed/homeSquare.buildTimeModifier) * supplyLevel;
        //Debug.Log("Calculated Speed: " + workerSpeed + " supply: " + supplyLevel + "terrain effect: "+ homeSquare.buildTimeModifier);
    }

    void Start () {
        updateStructureName();
        hasDestroyOrder = false;
        calculateSupplyLevel();
        calculateWorkerSpeed();        
    }

    void refund(float percentRefund, gameBoard cgb)
    {
        //give player resources back
        cgb.playerWood += percentRefund * woodCost;
        cgb.playerStone += percentRefund * stoneCost;
    }

    void freeWorkers(gameBoard cgb)
    {
       resourceBuildingScript buildingScript = gameObject.GetComponent<resourceBuildingScript>();
       //cgb.playerWorkers += buildingScript.workers;
    }
    

    void destroyThis()
    {
        //print("Death");
        Destroy(gameObject);
        gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        cgb.SendMessage("popStructure", new Vector2(positionX, positionY));
        cgb.SendMessage("deleteFromPlayerStructures", gameObject);
        if (isSelected)
            cgb.SendMessage("openMenu", "Build");
        cgb.SendMessage("setWalkableCheck");
        cgb.SendMessage("setEnemyCheck");
        //Debug.Log("Destroying " + buildingType + " at [" + positionX + "," + positionY + "]");
    }
   
    private void updateButtons()
    {
        //gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        //cgb.SendMessage("updateButtonStatus", gameObject);
    }
    // Update is called once per frame
    void Update () {

        


        if(isSelected && !updatedButtonFlag)
        {
            updateButtons();
            updatedButtonFlag = true;
        }
        else if(!isSelected && updatedButtonFlag)
        {
            updatedButtonFlag = false;
        }

        if (percentComplete < 100 && buildFlag)
        {
            percentComplete += Time.deltaTime * workerSpeed;
            if (percentComplete >= 100)
            {
                percentComplete = 100;
                if (buildingType == "Tower")
                {
                    //print("TOWER COMPLETE -- ACTIVATING");
                    towerScript ts = gameObject.GetComponent<towerScript>();
                    ts.isActive = true;
                }
            }

            integrity += Time.deltaTime * workerSpeed;
            if (integrity > percentComplete)
                integrity = percentComplete;

            updateStatusBars();
            if(isSelected)
                UpdateStructurePanel();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && isSelected)
        {
            gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
            if (buildingType != "Castle" && cgb!=null)
            {
                float refundPercentage = 1.00f;
                if (!buildFlag)
                    refundPercentage = .50f;                
                refund(refundPercentage, cgb);
                if(buildingType == "Resource")
                    freeWorkers(cgb);
                destroyThis();
            }
            else if (cgb == null)
            {
                Debug.Log("No CGB in StructureBehavior");
            }
            else
            {
                Debug.Log("DONT DESTROY YOUR OWN CASTLE!!!");
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
