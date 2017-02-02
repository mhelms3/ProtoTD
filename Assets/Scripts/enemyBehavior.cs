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
    public float sizeOfEnemy;

    private SpriteRenderer healthBar;
    public GameObject popUp;
    public float reaquireFrequency;
    public float reaquireClock = 0;

    private pathFindingScript pfs;
    private bool triggerDeathFlag = false;
    public Vector2 target;
    public GameObject targetObject;
    public GameObject tempTarget;
    public GameObject enemyAmmo;

    public string enemyType;
    public string enemySubType;

    public float resistCrush;
    public float resistPierce;
    public float resistFire;
    public float resistHoly;

    public List<node> myPath;
    public string targetType;
    public bool isConfounded; //use this if enemy cannot find path to closest target type
    public bool pathIsCurrent; //if nothing in the world changes, path should remain current, and can skip pathfinding
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

            GameObject dText = Instantiate(popUp, transform.position , Quaternion.identity) as GameObject;
            dText.GetComponent<numberPop>().updateText(damage.ToString("0"));
            dText.GetComponent<numberPop>().updateColor(Color.red);
            dText.GetComponent<numberPop>().setTextPos(transform.position); 

            


            //print("Enemy hit for " + damage + " damage");
            updateStatusBars();
            Destroy(col.gameObject);
        }
    }

    private void updateStatusBars()
    {
        
        float ratio = hitPoints / maxPoints;
        //print("updatebarAAA: dmg "+ ratio);
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
            float curDistance = Mathf.Sqrt(diff.sqrMagnitude);
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
            float curDistance = Mathf.Sqrt(diff.sqrMagnitude);
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
        //print("TARGET: " + targetObject.name);
    }

    public void attackTarget()
    {
        if (attackCycle < .001)
        {
            //print("Attacking");
            Vector3 startingPos = new Vector3(transform.position.x + .25f, transform.position.y - .5f, transform.position.z);
            GameObject ammo = Instantiate(enemyAmmo, startingPos, Quaternion.identity) as GameObject;
            ammoScript aScript = ammo.GetComponent<ammoScript>();
            aScript.attackTarget = targetObject;
            aScript.updateMaxDistance(attackRange);
            aScript.isEnemyAttack = true;
            aScript.isRotating = true;
            attackCycle = attackSpeed;
        }
    }

    public void findPath()
    {
        /*
        gameBoard cgb = (gameBoard)FindObjectOfType(typeof(gameBoard));
        pathFindingScript pfs = cgb.GetComponent<pathFindingScript>();

        
        object[] passVectors = new object[3];
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        passVectors[0] = currentPosition;
        target.x = targetObject.transform.position.x;
        target.y = targetObject.transform.position.y;

        passVectors[1] = target;
        passVectors[2] = gameObject;

        //Debug.Log(" Target:" + target);
        pfs.SendMessage("FindPathVectors", passVectors);
        */
        target.x = targetObject.transform.position.x;
        target.y = targetObject.transform.position.y;
        myPath = pfs.FindPath(new Vector2(transform.position.x, transform.position.y), target);

        if (myPath == null)
            print("Path Null Problem");
        else if (myPath.Count > 0)
            pathIsCurrent = true;
        else if (myPath.Count == 0)
            print("Path Zero Problem");
        
    }

    public void moveToTarget()
    {
        Vector3 myTarget = new Vector3(myPath[0].positionX+(.5f*sizeOfEnemy), myPath[0].positionY-(.5f*sizeOfEnemy), 0);
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
        float curDistance = Mathf.Sqrt(diff.sqrMagnitude);
        //print("ePos:"+position+" tPos"+ targetObject.transform.position+ " diff" + diff+ " Distance to target:" + curDistance);
        if (curDistance < (attackRange*1.43))
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
        
        maxPoints = hitPoints;
        attackCycle = attackSpeed;
        targetObject = null;
        targetType = "Castle";
        target = new Vector2(0, 0);
        hasPath = false;
        getHealthBar();
        updateStatusBars();
        acquireTarget();
        pfs = GetComponentInParent<pathFindingScript>();
        isAsleep = false;
        attackRange = attackRange + Random.value * .1f;

    }	

    void accelerate()
    {
        reaquireClock = (.70f + (Random.value *.25f)) * reaquireFrequency;
        pathIsCurrent = false;
    }

	// Update is called once per frame
	void Update () {

        reaquireClock += Time.deltaTime;
        if (reaquireClock > reaquireFrequency)
        {
            tempTarget = targetObject;
            reaquireClock = 0;
            acquireTarget();
            if (tempTarget != targetObject || pathIsCurrent == false)
                hasPath = false;
        }

        if (attackCycle > 0)
            attackCycle -= Time.deltaTime;

        
        if (isAsleep)
            sleepCounter += Time.deltaTime;
        if (sleepCounter > 5)
        {
            isAsleep = false;
            sleepCounter = 0;
        }
        


        if (targetObject == null && !isAsleep)
        {
            hasPath = false;
            if(myPath!=null)
                myPath.Clear();
            acquireTarget();
            
            if (targetObject == null)
            {
                isAsleep = true;
            }
            else
            {
                hasPath = false;
            }

        }

        if (targetObject != null)
        {
            if (!hasPath)
            {
                findPath();
                if (myPath == null || myPath.Count == 0)
                {
                    hasPath = false;
                    changeTargetClosest();
                    if (targetObject != null)
                        print("No path found. Changing target to:" + targetObject.name + " at" + targetObject.transform.position);
                }
                else
                    hasPath = true;
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
