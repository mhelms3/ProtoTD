using UnityEngine;
using UnityEngine.UI;

public class BuldingMenuScript : MonoBehaviour {

    public Toggle tog;
    public Animator anim;

	// Use this for initialization
	void Start () {

        tog = GetComponentInChildren<Toggle>();
        anim = GetComponent<Animator>();
	}

    public void slidePanel()
    {
        if (tog.isOn)
        {
            anim.Play("slideIn");
            tog.GetComponentInChildren<Text>().text = "Close";
        }
        else
        {
            anim.Play("slideOut");
            tog.GetComponentInChildren<Text>().text = "Open";
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
