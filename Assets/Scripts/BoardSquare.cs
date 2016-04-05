using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoardSquare : MonoBehaviour {

    public int positionX;
    public int positionY;
    public BoardTerrain squareTerrain;

    private SpriteRenderer squareSprite;

    public GameObject foundation = null;
    public GameObject structure = null;
    public GameObject thisSquare = null;
    public string squareName;
    public float wood;
    public float stone;
    public float food;

    public Slider WoodSlider;
    public Slider StoneSlider;
    public Slider FoodSlider;
    public Text terrainText;

    public Slider aSlider;
    public Text aText;
    //public specialItem[] specials;

    // Use this for initialization
    void UpdateStructurePanel()
    {
        GameObject temp = GameObject.Find("Health Slider");
        if (temp != null)
            aSlider = temp.GetComponent<Slider>();
        aSlider.value = 0;

       
        temp = GameObject.Find("Construction Slider");
        if (temp != null)
            aSlider = temp.GetComponent<Slider>();
        aSlider.value = 0;

        temp = GameObject.Find("Structure Name");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = "No structure here";

        temp = GameObject.Find("Materials Label");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = "";

        temp = GameObject.Find("Special Properties");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = "";

        //Debug.Log("Position["+positionX+"," + positionY +"] Name:"+squareName+" W:"+wood +" S:"+ stone +" F:"+ food);
    }

    void Start () {
    }

    void OnMouseDown()
    {
        GameObject temp;
        if (WoodSlider == null)
        {
            temp = GameObject.Find("Wood Slider");
            if (temp != null)
                WoodSlider = temp.GetComponent<Slider>();
        }

        if (WoodSlider != null)
            WoodSlider.value = wood;

        if (StoneSlider == null)
        {
            temp = GameObject.Find("Stone Slider");
            if (temp != null)
                StoneSlider = temp.GetComponent<Slider>();
        }

        if (StoneSlider != null)
            StoneSlider.value = stone;

        if (FoodSlider == null)
        {
            temp = GameObject.Find("Food Slider");
            if (temp != null)
                FoodSlider = temp.GetComponent<Slider>();
        }

        if (FoodSlider != null)
            FoodSlider.value = food;

        if (terrainText == null)
        {
            temp = GameObject.Find("Terrain Name");
            if (temp != null)
                terrainText = temp.GetComponent<Text>();
        }

        if (terrainText != null)
            terrainText.text = squareName;

        //temp = GameObject.Find("");


        if (structure != null)
        {
            Debug.Log("structure");
            structure.SendMessage("UpdateStructurePanel");
        }       
        else if (foundation != null)
        {
            Debug.Log("foundation");
            foundation.SendMessage("UpdateStructurePanel");
        }
        else
        {
            UpdateStructurePanel();
        }

        Debug.Log("Position["+positionX+"," + positionY +"] Name:"+squareName+" W:"+wood +" S:"+ stone +" F:"+ food);
    }
    // Update is called once per frame
    void Update()
    {

    }
       
}
