using UnityEngine;
using System.Collections;

public class mineBehavior : MonoBehaviour {

    public int mineLevel;
    public int workers;
    public int maxWorkers;
    public bool isActive;
    public GameObject gb;
    public gameBoard cgb;
    public BoardSquare bs; 

    public float stoneStore;
    public float stoneShip;

    public void calculateMaxWorkers()
    {
        maxWorkers = 2^mineLevel;
    }
    // Use this for initialization
    void Awake () {
        stoneStore = 0;
        stoneShip = 25;
        mineLevel = 1;
        workers = 1;
        maxWorkers = 2;
        gb = GameObject.Find("GameBoard");
        cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        isActive = false;
    }
	
	// Update is called once per frame
	void Update () {
        //cgb.playerStone += (bs.wood/20) * mineLevel * Time.deltaTime;
        if (isActive)
        {
            stoneStore += 5 * workers * Time.deltaTime;
            if (stoneStore > stoneShip)
            {
                cgb.playerStone += stoneStore;
                stoneStore = 0;
            }

        }
        else
        {
            StructureBehavior sb = GetComponent<StructureBehavior>();
            if (sb.buildFlag == false && sb.percentComplete > 99)
                isActive = true;
        }

    }
}
