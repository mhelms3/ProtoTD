using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoardSquare : MonoBehaviour {

    public int positionX;
    public int positionY;
    //public BoardTerrain squareTerrain;

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
        
        GameObject temp = GameObject.Find("Wood Slider");
        if (temp != null)
            WoodSlider = temp.GetComponent<Slider>();
        WoodSlider.value = wood;

        temp = GameObject.Find("Stone Slider");
        if (temp != null)
            StoneSlider = temp.GetComponent<Slider>();
        StoneSlider.value = stone;

        
        temp = GameObject.Find("Food Slider");
        if (temp != null)
            FoodSlider = temp.GetComponent<Slider>();
        FoodSlider.value = food;


        if (structure != null)
        {
            Debug.Log("Structure Name"+structure.name);
            structure.SendMessage("UpdateStructurePanel");
        }
        else if (foundation != null)
        {
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
