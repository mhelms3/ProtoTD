using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class castleScript : MonoBehaviour {

    public string castleName = "Player's Castle";
    public int castleLevel = 1;
    public string material = "Stone";
    public float integrity;
    public float percentComplete;

    public Slider aSlider;
    public Text aText;


    void UpdateStructurePanel()
    {
        GameObject temp = GameObject.Find("Health Slider");
        if (temp != null)
            aSlider = temp.GetComponent<Slider>();
        aSlider.value = integrity;

       
        temp = GameObject.Find("Construction Slider");
        if (temp != null)
        {
            aSlider = temp.GetComponent<Slider>();

            if (percentComplete < 100)
            {
                aSlider.value = percentComplete;
            }
            else
            {
                aSlider.enabled = false;
            }
        }
        

        temp = GameObject.Find("Structure Name");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = castleName + " Level:" + castleLevel.ToString();

        temp = GameObject.Find("Materials Label");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = "Material: " + material;

        temp = GameObject.Find("Special Properties");
        if (temp != null)
            aText = temp.GetComponent<Text>();
        aText.text = "+100% everything";

        //Debug.Log("Position["+positionX+"," + positionY +"] Name:"+squareName+" W:"+wood +" S:"+ stone +" F:"+ food);
    }
    // Use this for initialization
    void Start () {
        integrity = 100;
        percentComplete = 100;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
