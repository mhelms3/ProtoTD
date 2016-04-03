using UnityEngine;
using System.Collections;

public class foundationScript : MonoBehaviour {

    public int positionX;
    public int positionY;

    public float percentComplete;
    public float integrity;

    public string material;

	// Use this for initialization
	void Start () {
        percentComplete = 1;
        integrity = 1;
	}
	
	// Update is called once per frame
	void Update () {

        Component[] children;
        Component child;
        children = this.GetComponentsInChildren<SpriteRenderer>();
        child = children.
        if (percentComplete < 100)
        {
            percentComplete += Time.deltaTime;
            integrity += Time.deltaTime;
        }
    }
}
