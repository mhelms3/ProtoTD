using UnityEngine;
using System.Collections;

public class generatorScript : MonoBehaviour {

    
    public GameObject[] enemy;
    public int baseEnemyNumber;
    public int enemyVariableNumber;

    public float enemyFrequency;    
    public float enemyRest;
    public float enemyCumulativeFrequency;

    public float enemyCheckCounter;
    public float enemyPause;
    public float pauseCounter;

    private bool productionOn;
    private int numberRemaining;

    public bool isActive;
    public int level;
    private gameBoard cgb;

    
    private int positionX;
    private int positionY;
	// Use this for initialization

    void generateEnemy(int type)
    {
        float newPosx = positionX + 1+(Random.value * .5f)-.25f;
        float newPosy = positionY + 1+(Random.value * .5f)-.25f;
        //GameObject e = Instantiate(enemy[type], new Vector3(newPosx, newPosy, 1), Quaternion.identity) as GameObject;

        //print("Enemy@ X:"+positionX + " Y:" + positionY);
        GameObject e = Instantiate(enemy[type], new Vector3(newPosx, newPosy, 1), Quaternion.identity) as GameObject;        
        cgb.SendMessage("addEnemyToBoard", e);
    }


    void Awake ()
    {
        isActive = false;
    }

	void Start () {        
        StructureBehavior bs = GetComponentInParent<StructureBehavior>();
        cgb = FindObjectOfType<gameBoard>();
        //enemy = cgb.enemyType;
        positionX = bs.positionX;
        positionY = bs.positionY;
        pauseCounter = 0;
}
	
	// Update is called once per frame
	void Update () {

        int rndEnemy;
        if (isActive)
        {
            enemyCheckCounter+= Time.deltaTime;
            if (enemyCheckCounter > 2)
            {
                enemyCumulativeFrequency += enemyFrequency;
                if (Random.value < enemyCumulativeFrequency)
                {
                    productionOn = true;
                    numberRemaining = Mathf.CeilToInt(baseEnemyNumber + (Random.value * enemyVariableNumber));
                    //numberRemaining = 1;
                    enemyCumulativeFrequency = 0;
                }
                enemyCheckCounter = 0;
            }

            if (productionOn)
            {
                pauseCounter += Time.deltaTime;
                enemyCheckCounter = 0;
                if (pauseCounter > enemyPause)
                {
                    pauseCounter = 0;
                    rndEnemy = Mathf.FloorToInt(Random.value * enemy.Length);
                    generateEnemy(rndEnemy);
                    numberRemaining -= 1;
                    if (numberRemaining < 1)
                        productionOn = false;
                }
            }
        }
	
	}
}
