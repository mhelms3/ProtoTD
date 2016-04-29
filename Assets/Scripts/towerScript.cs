using UnityEngine;
using System.Collections;

public class towerScript : MonoBehaviour {
    public int towerLevel;
    public int towerType;
    public int soldiers;
    public int maxSoldiers;

    public bool isActive;

    public float rateOfFire;
    public float attackTimer;
    public float attackRange;

    public GameObject towerAmmo;
    public GameObject attackTarget;
    public string targetType = "Enemy";

    public void makeActive()
    {
        isActive = true;
    }

    public void findTarget()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetType);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in targetObjects)
        {
            //print("---checking target "+ go.name +" at " + go.transform.position);
            float curDistance = getDistance(go.transform.position, position);
            //print("---checking target " + go.name + " at " + go.transform.position + " distance:" + curDistance);
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        if (closest != null)
        {
            attackTarget = closest;
            //print("---attack target " + closest.name + " at " + closest.transform.position + " distance:" + distance);
        }
   }

    public float getDistance(Vector3 a, Vector3 b)
    {
        Vector3 diff = a - b;
        return (diff.sqrMagnitude);
    }

    public void shootAmmo()
    {
        if(getDistance(attackTarget.transform.position, transform.position)<attackRange)
        {
            //Vector3 relativePos = attackTarget.transform.position - transform.position;
            //Quaternion rotation = Quaternion.LookRotation(relativePos);
            Vector3 startingPos = new Vector3(transform.position.x +.25f, transform.position.y - .5f, transform.position.z);
            GameObject ammo = Instantiate(towerAmmo, startingPos, Quaternion.identity) as GameObject;
            ammoScript aScript = ammo.GetComponent<ammoScript>();
            aScript.attackTarget = attackTarget;
            attackTimer = 0;
            findTarget();

            //giveammoChars
        }


    }
    public void calculateMaxSoldiers()
    {
        maxSoldiers = 2 * towerLevel;
    }
    // Use this for initialization
    void Start () {
        soldiers = 0;
        maxSoldiers = 2;
        attackTimer = 0;
        attackRange = 5000;
        isActive = false;                
	}
	
	// Update is called once per frame
	void Update () {
	
        if(attackTarget == null)
        {
            findTarget();
        }

        if (attackTimer < rateOfFire && isActive)
        {
            attackTimer += Time.deltaTime;
        }
        
        if(attackTimer > rateOfFire && attackTarget != null && isActive)
        {
            shootAmmo();
        }


    }
}
