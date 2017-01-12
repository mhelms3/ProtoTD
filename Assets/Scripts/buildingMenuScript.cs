using UnityEngine;
using UnityEngine.UI;

public class buildingMenuScript : MonoBehaviour
{

    public Toggle tog;
    public Animator anim;

    public GameObject buildingImagePanel;
    public Sprite []buildingImages;
    private Image currentImage;

    // Use this for initialization
    void Awake()
    {

        tog = GetComponentInChildren<Toggle>();
        anim = GetComponent<Animator>();
        buildingImagePanel = GameObject.Find("BuildingImage");
        currentImage = buildingImagePanel.GetComponentInChildren<Image>();
    }

    public void slidePanel()
    {
        if (tog.isOn)
        {
            anim.Play("slideOut");
            Debug.Log("Slide out");
            tog.GetComponentInChildren<Text>().text = "Building Details";
        }
        else
        {
            anim.Play("slideIn");
            Debug.Log("Slide in");
            tog.GetComponentInChildren<Text>().text = "Close Details";
        }
    }

    private void updatePanelImage(string buildingType, string buildingSubType)
    {
        if (buildingType == "Tower")
        {
            if (buildingSubType == "Arrow")
            {
                currentImage.sprite = buildingImages[1];
            }
            else if (buildingSubType == "Cannon")
            {
                currentImage.sprite = buildingImages[2];
            }
            else if (buildingSubType == "Holy")
            {
                currentImage.sprite = buildingImages[3];
            }
            else if (buildingSubType == "Wizard")
            {
                currentImage.sprite = buildingImages[4];
            }
        }
        else if (buildingType == "Wall")
        {
            currentImage.sprite = buildingImages[5];
        }
        else if (buildingType == "Castle")
        {
            currentImage.sprite = buildingImages[0];
        }
    }

    private void updateBasicInfo(StructureBehavior sb)
    {

    }

    private void updateTowerInfo(towerScript ts)
    {

    }


    public void updatePanel(StructureBehavior sb, towerScript ts)
    {
        updatePanelImage(sb.buildingType, sb.buildingSubType);
        updateBasicInfo(sb);
        if (ts != null)
            updateTowerInfo(ts);
    }

    public void updatePanel(StructureBehavior sb, resourceBuildingScript rbs)
    {
        updatePanelImage(sb.buildingType, sb.buildingSubType);
        updateBasicInfo(sb);
        //if (rbs != null)
            //updateResourceBuildingInfo(rbs);
    }

    public void updatePanel(StructureBehavior sb)
    {
        updatePanelImage(sb.buildingType, sb.buildingSubType);
        updateBasicInfo(sb);
    }
}

