using UnityEngine;
using System.Collections.Generic;

public class enemyBehavior : MonoBehaviour {

    
    public float hitPoints;
    public float moveSpeed;
    public float attackRange;
    public float damage;
    public float attackCycle;
    public float attackSpeed;
    public Vector2 target;
    public GameObject targetObject;

    public List<BoardSquare> myPath;
    public string targetType;
    public bool hasPath;

    public bool isAsleep;
    public float sleepCounter;



    public void acquireTarget()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetType);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
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

        if (closest != null)
        {
            target.x = closest.transform.position.x;
            target.y = closest.transform.position.y;
            Debug.Log("MyTarget:" + closest.transform.position + " Target:" + target);
            targetObject = closest;
        }
        else
        {
            target.x = position.x;
            target.y = position.y;
            targetObject = null;
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
        Debug.Log(" Target:" + target);
        passVectors[1] = target;
        passVectors[2] = gameObject;

        pfs.SendMessage("FindPathVectors", passVectors);
        //myPath = pfs.finalPath;        
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
        if (curDistance < attackRange)
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
            print("acquiring new target");
            hasPath = false;
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
	
	}
}
