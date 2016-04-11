using UnityEngine;
using System.Collections;

public class towerScript : MonoBehaviour {



    public int towerLevel;
    public int towerType;
    public int soldiers;
    public int maxSoldiers;

    public void calculateMaxSoldiers()
    {
        maxSoldiers = 2 * towerLevel;
    }
    // Use this for initialization
    void Start () {
        soldiers = 0;
        maxSoldiers = 2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
