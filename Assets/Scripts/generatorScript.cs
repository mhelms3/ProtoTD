using UnityEngine;
using System.Collections;

public class generatorScript : MonoBehaviour {

    public GameObject enemy;
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

    void generateEnemy()
    {
        
        float newPosx = positionX + (Random.value * 2) - 1;
        float newPosy = positionY + (Random.value * 2) - 1;
        GameObject e = Instantiate(enemy, new Vector3(newPosx, newPosy, 1), Quaternion.identity) as GameObject;
        cgb.SendMessage("addEnemyToBoard", e);
    }

	void Start () {        
        StructureBehavior bs = GetComponent<StructureBehavior>();
        cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        positionX = bs.positionX;
        positionY = bs.positionY;
        pauseCounter = 0;
        isActive = true;
}
	
	// Update is called once per frame
	void Update () {

        if (isActive)
        {
            enemyCheckCounter+= Time.deltaTime;
            if (enemyCheckCounter > 1)
            {
                enemyCumulativeFrequency += enemyFrequency;
                if (Random.value < enemyCumulativeFrequency)
                {
                    productionOn = true;
                    numberRemaining = Mathf.CeilToInt(baseEnemyNumber + (Random.value * enemyVariableNumber));
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
                    generateEnemy();
                    numberRemaining -= 1;
                    if (numberRemaining < 1)
                        productionOn = false;
                }
            }
        }
	
	}
}
