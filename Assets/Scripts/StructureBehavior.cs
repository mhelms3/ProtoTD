using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StructureBehavior : MonoBehaviour {

    public int positionX;
    public int positionY;
    public float workerSpeed;

    public float percentComplete;
    public float integrity;

    public bool buildFlag;

    public string structureName;
    public string buildingType;
    public BoardSquare homeSquare;

    public string structureMaterial;
    public string foundationMaterial;

    public float marketValue;

    public IList<string> specials = new List<string>();

    bool isFoundationOnly;


    SpriteRenderer[] barSprites;  //for healthbars

    public Slider aSlider;
    public Text aText;

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

    void UpdateStructurePanel()
    {
        GameObject temp = GameObject.Find("Health Slider");
        if (temp != null)
            aSlider = temp.GetComponent<Slider>();
        aSlider.value = integrity;

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
        percentComplete = 1;
        integrity = 1;
        buildFlag = false;
        barSprites = this.GetComponentsInChildren<SpriteRenderer>();
        updateStructureName();
    }

    void updateStructureName()
    {
        if (buildingType == null)
            structureName = foundationMaterial + " Foundation";
        else if(buildingType == "Ruin")
            structureName = foundationMaterial + " Ruin";
        else
            structureName = structureMaterial + " " + buildingType + "(" + foundationMaterial + " Foundation)";
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
                if (percentComplete > 66)
                    c.color = Color.green;
                else if (percentComplete > 33)
                    c.color = Color.yellow;
                else
                    c.color = Color.red;
                c.transform.localScale = new Vector3(percentComplete / 100, 1, 1);
            }
            if (c.name == "BuildBar")
            {
                if (percentComplete == 100 || !buildFlag)
                {                    
                    c.enabled = false;
                    buildFlag = false;
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
	
	}
	
	// Update is called once per frame
	void Update () {
        if (percentComplete < 100 && buildFlag)
        {
            percentComplete += Time.deltaTime * workerSpeed;
            if (percentComplete > 100)
                percentComplete = 100;

            integrity += Time.deltaTime * workerSpeed;
            if (integrity > percentComplete)
                integrity = percentComplete;

            updateStatusBars();

        }

    }
}
