using UnityEngine;
using System.Collections;

public class foundationScript : MonoBehaviour {

    public int positionX;
    public int positionY;
    public float workerSpeed;

    public float percentComplete;
    public float integrity;

    public bool buildFlag;

    public string foundationName;

    public string material;
    SpriteRenderer[] spriteChildren;

    // Use this for initialization
    void Awake () {
        percentComplete = 1;
        integrity = 1;
        buildFlag = false;
        material = "Wood";
        foundationName = material+" "+"Foundation";
        spriteChildren = this.GetComponentsInChildren<SpriteRenderer>();
    }

	void RuinStart (string s)
    {
        Debug.Log("RuinStarted");
        material = s;
        foundationName = material + " " + "Ruin";
        buildFlag = false;
        integrity = 60f;
        percentComplete = 30f;
        updateStatusBars();
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
                if (percentComplete == 100)
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
