using UnityEngine;
using System.Collections;

public class BoardTerrain : MonoBehaviour {
    public string terrainName;
    public int averageWoodValue;    //base resources   
    public int averageStoneValue;   //base resources
    public int averageFoodValue;    //base resources
    public float moveCost;          //base cost to move into square

    /*
    public float bonusWood;         //odds of finding a bonus resource of this type
    public float bonusStone;        //odds of finding a bonus resource of this type
    public float bonusFood;         //odds of finding a bonus resource of this type
    public float constructionCost;  //cost modifier to build foundations, roads and tunnels
    public float defenseValue;      //firing into
    public float offenseValue;      //fighting from
    public float scryValue;         //base sight distance from
    public float obscureValue;      //odds of seeing something here
   
    public bool impassable_land;
    public bool impassable_air;
    public bool impassable_sea;
    public bool canHazTunnel;
    public bool canHazRoad;
    public bool canHazBridge;
    

    public void initTerrain(string nm, int aw, int ast, int af, float bw, float bs, float bf, float cc, float dv, float ov, float sv, float obscureV, float moveC, bool il, bool ia, bool isea, bool hazr, bool hazt, bool hazb)
    {
        terrainName = nm;
        averageWoodValue = aw;    //base resources   
        averageStoneValue = ast;   //base resources
        averageFoodValue = af;    //base resources
        bonusWood = bw;         //odds of finding a bonus resource of this type
        bonusStone = bs;        //odds of finding a bonus resource of this type
        bonusFood = bf;         //odds of finding a bonus resource of this type

        constructionCost = cc;  //cost modifier to build foundations, roads and tunnels

        defenseValue = dv;      //firing into
        offenseValue = ov;      //fighting from

        scryValue = sv;         //base sight distance from
        obscureValue = obscureV;      //odds of seeing something here

        moveCost = moveC;          //base cost to move into square

        impassable_land = il;
        impassable_air = ia;
        impassable_sea = isea;
        canHazTunnel = hazt;
        canHazRoad = hazr;
        canHazBridge = hazb;
    }
    */

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
