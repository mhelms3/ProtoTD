using UnityEngine;
using System.Collections.Generic;

public class enemyBehavior : MonoBehaviour {

    
    public float hitPoints;
    public float moveSpeed;
    public float attackRange;
    public float damage;
    public float attackCycle;
    public float attackSpeed;
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
            print("Hit for " + damage + " damage");
            Destroy(col.gameObject);
        }

    }

    public void acquireTarget()
    {
        //print("finding target");
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetType);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in targetObjects)
        {
            //print("---checking target "+ go.name +" at " + go.transform.position);
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            //print("---checking target " + go.name + " at " + go.transform.position + " distance:" + curDistance);
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        
        if(closest == null && targetType != "Building")
        {
            targetObjects = GameObject.FindGameObjectsWithTag("Building");
            foreach (GameObject go in targetObjects)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        
        

        if (closest != null)
        {
            target.x = closest.transform.position.x;
            target.y = closest.transform.position.y;
            //Debug.Log("MyTarget:" + closest.transform.position + " Target:" + target);
            targetObject = closest;
        }
        else
        {
            print("Failed to find Target!!!");
            target.x = position.x;
            target.y = position.y;
            targetObject = null;
            Application.Quit();


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
	// Use this for initialization
	void Start () {
        hitPoints = 20;
        attackRange = 2;
        attackSpeed = .5f;
        attackCycle = attackSpeed;
        damage = 10;
        moveSpeed = .5f;
        targetObject = null;
        targetType = "Tower";
        target = new Vector2(0, 0);
        hasPath = false;
}	
	// Update is called once per frame
	void Update () {

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
            //print("acquiring new target");
            hasPath = false;
            myPath.Clear();
            acquireTarget();
            if (targetObject == null)
            {
                //SleepTimeout
            }

        }

        if (targetObject != null)
        {
            if(!hasPath)
                    findPath();
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
