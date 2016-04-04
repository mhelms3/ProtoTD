using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class foundationScript : MonoBehaviour {

    public int positionX;
    public int positionY;
    public float workerSpeed;

    public float percentComplete;
    public float integrity;

    public bool buildFlag;

    public string foundationName;
    public BoardSquare homeSquare;

    public string material;
    public IList<string> specials = new List<string>();
    

    SpriteRenderer[] spriteChildren;

    public Slider aSlider;
    public Text   aText;

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

        if (percentComplete<100)
        {
            temp = GameObject.Find("Construction Slider");
            if (temp != null)
                aSlider = temp.GetComponent<Slider>();
            aSlider.value = integrity;
        }

        temp = GameObject.Find("Structure Name");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = foundationName;

        temp = GameObject.Find("Materials Label");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = "Material: "+ material;

        temp = GameObject.Find("Special Properties");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        string specialsTemp = makeSpecialsString();
        aText.text = specialsTemp;

        //Debug.Log("Position["+positionX+"," + positionY +"] Name:"+squareName+" W:"+wood +" S:"+ stone +" F:"+ food);
    }


    // Use this for initialization
    void Awake () {
        percentComplete = 1;
        integrity = 1;
        buildFlag = false;
        material = "Wood";
        foundationName = material+" "+"Foundation";
        spriteChildren = this.GetComponentsInChildren<SpriteRenderer>();
    }

    void AddMaterialSpecials (string s)
    {
        string[] ruinMaterial = { "Marble", "Granite", "Obsidian", "Limestone", "Basalt", "Brownstone", "Flagstone", "Quadratum" };
        switch (s)
        {
            case "Marble":
                specials.Add("Marble: +1 defense");
                specials.Add("Marble:+100 value");
                break;
            case "Granite":
                specials.Add("Granite: +3 defense");
                specials.Add("Granite: half damage from ALL physical");
                break;
            case "Obsidian":
                specials.Add("Obsidian: +3 dark damage");
                specials.Add("Obsidian: +3 magic defense");
                specials.Add("Obsidian: blocks ethereal");
                break;
            case "Limestone":
                specials.Add("Limestone: +2 defense");
                specials.Add("Limestone: +3 acid damage");
                break;
            case "Basalt":
                specials.Add("Basalt: +2 defense");
                specials.Add("Basalt: +3 frost damage");
                break;
            case "BrownStone":
                specials.Add("BrownStone: +2 defense");
                specials.Add("BrownStone: +3 crushing damange");
                break;
            case "FlagStone":
                specials.Add("FlagStone: +2 defense");
                specials.Add("FlagStone: +1 crushing damange");
                break;
            case "Quadratum":
                specials.Add("Quadratum: +2 defense");
                specials.Add("Quadratum: +1 ALL damange");
                break;
        }
    }

	void RuinStart (string s)
    {
        Debug.Log("RuinStarted");
        material = s;
        foundationName = material + " " + "Ruin";
        buildFlag = false;
        integrity = 50f;
        percentComplete = 50f;
        updateStatusBars();
        specials.Add("Ruins: no material cost");
        specials.Add("Ruins: double labor cost");
        specials.Add("Ruins: +5% for undead/level");
        specials.Add("Ruins: +5 defense");
        specials.Add("Ruins: +25% of item on explore");
        AddMaterialSpecials(s);
        
    }

    void updateMaterialType(string s)
    {
        material = s;
        foundationName = material + " " + "Foundation";
    }
    // Update is called once per frame


    void updateStatusBars()
    {
        foreach (SpriteRenderer c in spriteChildren)
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

	void Update () {
        
        if (percentComplete < 100 && buildFlag)
        {
            percentComplete += Time.deltaTime*workerSpeed;
            if (percentComplete > 100)
                percentComplete = 100;

            integrity += Time.deltaTime * workerSpeed;
            if (integrity > percentComplete)
                integrity = percentComplete;

            updateStatusBars();
            
        }
    }
}
