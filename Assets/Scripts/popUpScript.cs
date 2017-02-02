using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class popUpScript : MonoBehaviour {

    // Use this for initialization
    public float duration;
    private float expiration;
    private Text thisText;

    public void updateText(string s)
    {
        print(s);
        print(thisText);
        print(thisText.text);
        thisText.text = s;
    }

    public void setTextPos (Vector3 v)
    {
        thisText.transform.position = v;
    }

	void Start () {
        expiration = 0;
        Canvas c = GetComponentInChildren<Canvas>();
        print("C"+c.name);
        thisText = c.GetComponentInChildren<Text>();
        print(thisText);
        print(thisText.text);
    }

    
	
	// Update is called once per frame
	void Update () {
        if(thisText == null)
        {
            thisText = GetComponentInParent<Text>();
        }
        thisText.transform.Translate(new Vector3(0,Time.deltaTime,0));
        expiration += Time.deltaTime;
        if (expiration > duration)
            Destroy(gameObject);
	
	}
}
