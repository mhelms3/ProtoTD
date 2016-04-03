using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoardSquare : MonoBehaviour {

    public int positionX;
    public int positionY;
    //public BoardTerrain squareTerrain;

    private SpriteRenderer squareSprite;
    public string squareName;
    public float wood;
    public float stone;
    public float food;

    public Slider WoodSlider;
    public Slider StoneSlider;
    public Slider FoodSlider;
    //public specialItem[] specials;

    // Use this for initialization
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
        
        Debug.Log("Position["+positionX+"," + positionY +"] Name:"+squareName+" W:"+wood +" S:"+ stone +" F:"+ food);
    }
    // Update is called once per frame
    void Update()
    {

    }
       
}
