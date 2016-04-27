using UnityEngine;
using System.Collections.Generic;

public class enemyBehavior : MonoBehaviour {

    
    public float hitPoints;
    public float maxPoints;
    public float moveSpeed;
    public float attackRange;
    public float damage;
    public float attackCycle;
    public float attackSpeed;

    private SpriteRenderer healthBar;
    public float reaquireFrequency;
    public float reaquireClock = 0;

    private bool triggerDeathFlag = false;
    public Vector2 target;
    public GameObject targetObject;

    public List<BoardSquare> myPath;
    public string targetType;
    public bool isConfounded; //use this if enemy cannot find path to closest target type
    public bool hasPath;
    public bool isAsleep;
    public float sleepCounter;

    public int countPath = 0;

    void killEnemy()
    {
        gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        cgb.SendMessage("popEnemy", gameObject);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Ammo")
        {
            //enemyBehavior enemyInfo = col.gameObject.GetComponent<enemyBehavior>();
            ammoScript aScript = col.gameObject.GetComponent<ammoScript>();
            float damage = Mathf.Round(Random.value * (aScript.damageUpper - aScript.damageLower) + aScript.damageLower);
            hitPoints -= damage;
            //print("Hit for " + damage + " damage");
            updateStatusBars();
            Destroy(col.gameObject);
        }
    }

    private void updateStatusBars()
    {
        
        float ratio = hitPoints / maxPoints;
        print("updatebarAAA: dmg "+ ratio);
        if (ratio > .66)
            healthBar.color = Color.green;
        else if (ratio > .33)
            healthBar.color = Color.yellow;
        else
            healthBar.color = Color.red;
        healthBar.transform.localScale = new Vector3(ratio, 1, 1);
    }

    private GameObject findClosestObject(List<GameObject> objs)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in objs)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    private GameObject findClosestObject(GameObject[] objs)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in objs)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public bool changeTargetClosest()
    {
        gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        GameObject closest = null;
        closest = findClosestObject(cgb.playerStructures);
        if (closest != null)
        {
            target.x = closest.transform.position.x;
            target.y = closest.transform.position.y;
            targetObject = closest;
            return true;
        }
        else
            return false;
    }

    public void acquireTarget()
    {
        //print("finding target");
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetType);
        GameObject closest = null;
        closest = findClosestObject(targetObjects);

        if (closest == null && targetType != "Building")
        {
            targetObjects = GameObject.FindGameObjectsWithTag("Building");
            closest = findClosestObject(targetObjects);
        }

        if (closest != null)
        {
            target.x = closest.transform.position.x;
            target.y = closest.transform.position.y;
            targetObject = closest;
        }
        else
        {
            print("Enemy failed to find Primar or Secondary Targets!!!");
            bool anyTargetsLeft = changeTargetClosest();
            if (!anyTargetsLeft)
                print("END GAME");
        }
    }

    public void attackTarget()
    {
        if (attackCycle < .001)
        {
            StructureBehavior tsb = targetObject.GetComponent<StructureBehavior>();
            tsb.integrity -= damage;
            tsb.SendMessage("updateStatusBars");            
            attackCycle = attackSpeed;
        }
    }

    public void findPath()
    {
        gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        pathFindingScript pfs = cgb.GetComponent<pathFindingScript>();
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        object[] passVectors = new object[3];
        passVectors[0] = currentPosition;
        target.x = targetObject.transform.position.x;
        target.y = targetObject.transform.position.y;
        
        passVectors[1] = target;
        passVectors[2] = gameObject;

        //Debug.Log(" Target:" + target);
        pfs.SendMessage("FindPathVectors", passVectors);
    }

    public void moveToTarget()
    {
        Vector3 myTarget = new Vector3(myPath[0].positionX, myPath[0].positionY, 0);
        //Debug.Log("MyTarget:"+myTarget);
        gameObject.transform.position = Vector3.MoveTowards(transform.position, myTarget, moveSpeed*Time.deltaTime);
        if (Vector3.Distance(transform.position, myTarget)<.005)
        {
            myPath.Remove(myPath[0]);
        }        
    }

 

    public void engageTarget()
    {
        Vector3 position = transform.position;
        Vector3 diff = targetObject.transform.position - position;
        float curDistance = diff.sqrMagnitude;
        if (curDistance < (attackRange*1.42))
        {
            attackTarget();
        }
        else
        {
            if (myPath.Count > 0)
                moveToTarget();           
        }


        //move to target, if close enough, attack target
    }

    private void getHealthBar()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer c in sprites)
        {
            if (c.name == "HealthBar")
                healthBar = c;
        }
    }

	// Use this for initialization
	void Start () {
        hitPoints = 20;
        maxPoints = hitPoints;
        attackRange = 2;
        attackSpeed = .5f;
        attackCycle = attackSpeed;
        damage = 10;
        targetObject = null;
        targetType = "Tower";
        target = new Vector2(0, 0);
        hasPath = false;

        getHealthBar();
        updateStatusBars();
}	
	// Update is called once per frame
	void Update () {

        reaquireClock += Time.deltaTime;
        if (reaquireClock > reaquireFrequency)
        {
            reaquireClock = 0;
            acquireTarget();
        }

        if (attackCycle > 0)
            attackCycle -= Time.deltaTime;
        if (isAsleep)
            sleepCounter += Time.deltaTime;
        if (sleepCounter > 15)
        {
            isAsleep = false;
            sleepCounter = 0;
        }



        if (targetObject == null && !isAsleep)
        {
         
            hasPath = false;
            myPath.Clear();
            acquireTarget();
            if (targetObject == null)
            {
                isAsleep = true;
            }

        }

        if (targetObject != null)
        {
            if (!hasPath)
            {
                findPath();
                if (!hasPath)
                {
                    print("Bollux.No path found");  
                    changeTargetClosest();
                }
            }
                
            
            engageTarget();
        }

        if(hitPoints<0)
        {
            triggerDeathFlag = true;
        }

        if(triggerDeathFlag)
        {
            killEnemy();
        }
	
	}
}
