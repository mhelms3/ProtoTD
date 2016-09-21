using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StructureBehavior : MonoBehaviour {

    public int positionX;
    public int positionY;
    public float workerSpeed;

    public float percentComplete;
    public float integrity;

    public bool buildFlag;
    public bool updatedButtonFlag;
    public bool hasDestroyOrder;

    public string structureName;
    public string buildingType;
    public string buildingSubType;
    public BoardSquare homeSquare;

    public string structureMaterial;
    public string foundationMaterial;

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
        print("Hit building");
        if (col.gameObject.tag == "EnemyAmmo")
        {
            ammoScript aScript = col.gameObject.GetComponent<ammoScript>();
            float damage = Mathf.Round(Random.value * (aScript.damageUpper - aScript.damageLower) + aScript.damageLower);
            integrity -= damage;
            print("Building hit for " + damage + " damage");
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

    void SquareAssignment(BoardSquare bs)
    {
        homeSquare = bs;
    }

    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UpdateStructurePanel();
            homeSquare.SendMessage("updateTerrainUI");
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
            if (structureMaterial == null || structureMaterial == "")
                aText.text = "Material: " + foundationMaterial;
            else
                aText.text = "Material: " + foundationMaterial + "(F) and " + structureMaterial;
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
        barSprites = this.GetComponentsInChildren<SpriteRenderer>();        
    }

    void updateStructureName()
    {
        if (buildingType == null)
            structureName = foundationMaterial + " Foundation";
        else if(buildingType == "Ruin")
            structureName = foundationMaterial + " Ruin";
        else if(buildingSubType == null || buildingSubType == "")
            structureName = structureMaterial + " " + buildingType + "(" + foundationMaterial + "[F])";
        else
             structureName = structureMaterial + " " + buildingType + ", " + buildingSubType + "(" + foundationMaterial + "[F])";
    }

    void updateMaterialType(string s, bool isFoundation)
    {
        if (isFoundation)
            foundationMaterial = s;            
        else
            structureMaterial = s;
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

    // Use this for initialization
    void Start () {
        updateStructureName();
        hasDestroyOrder = false;
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
       cgb.playerWorkers += buildingScript.workers;
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
                    print("TOWER COMPLETE -- ACTIVATING");
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
            if (buildingType != "Castle")
            {
                float refundPercentage = 1.00f;
                if (!buildFlag)
                    refundPercentage = .50f;                
                refund(refundPercentage, cgb);
                freeWorkers(cgb);
                destroyThis();
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
