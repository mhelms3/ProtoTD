using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuScript : MonoBehaviour {

    public Toggle tog;
    public Animator anim;

    public GameObject buildingImagePanel;
    public Image buildingImage;

    // Use this for initialization
    void Start() {

        tog = GetComponentInChildren<Toggle>();
        anim = GetComponent<Animator>();
        buildingImagePanel = GameObject.Find("BuildingImage");
        buildingImage = buildingImagePanel.GetComponentInChildren<Image>();
    }

    public void slidePanel()
    {
        if (tog.isOn)
        {
            anim.Play("slideOut");
            tog.GetComponentInChildren<Text>().text = "Open Building Menu";
        }
        else
        {
            anim.Play("slideIn");
            tog.GetComponentInChildren<Text>().text = "Close Building Menu";
        }
    }

    public void updatePanel(StructureBehavior sb)
    {
        if (sb.buildingType == "Tower")
        {
            if (sb.buildingSubType == "Arrow")
            {
                buildingImage = ""
            }
            else if (sb.buildingSubType == "Cannon")
            { }
            else if (sb.buildingSubType == "Wizard")
            { }
            else if (sb.buildingSubType == "Holy")
            { }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
