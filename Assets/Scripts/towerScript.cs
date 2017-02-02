using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class towerScript : MonoBehaviour {
    public string towerType;
    public bool isActive;

    public float rateOfFire;
    public float attackTimer;
    public float attackRange;
    public float damageModifierLower;
    public float damageModifierUpper;


    public GameObject towerAmmo;
    public GameObject attackTarget;
    public StructureBehavior sb;
    public string targetType = "Enemy";
    public string targetSubType;

    public void makeActive()
    {
        isActive = true;
    }

    GameObject findClosestTarget(GameObject[] targetList)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in targetList)
        {
            float curDistance = getDistance(go.transform.position, position);
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        if (closest != null && distance<attackRange)
        {
            return (closest);
        }
        else
        {
            return (null);
        }
    }

    public void findTarget()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetType);
        if (targetObjects.Length > 0)
        {
            GameObject[] tempObjects = new GameObject[targetObjects.Length];
            int indexSubType = 0;

            foreach (GameObject go in targetObjects)
            {
                enemyBehavior eb = go.GetComponent<enemyBehavior>();
                if (eb.enemySubType == targetSubType)
                {
                    tempObjects[indexSubType] = go;
                    indexSubType++;
                }
            }

            if (indexSubType > 0)
            {
                GameObject[] subTypeObjects = new GameObject[indexSubType];
                int counter = 0;
                foreach (GameObject go2 in subTypeObjects)
                {
                    subTypeObjects[counter] = tempObjects[counter];
                    counter++;
                }
                attackTarget = findClosestTarget(subTypeObjects);
            }

            if (attackTarget == null)
            {
                attackTarget = findClosestTarget(targetObjects);
            }

        }
        else
        {
            attackTarget = null;
        }
   }

    public float getDistance(Vector3 a, Vector3 b)
    {
        Vector3 diff = a - b;
        return (Mathf.Sqrt(diff.sqrMagnitude));
    }

    public void updateDamageModifiers()
    {
        damageModifierLower = Mathf.Pow(sb.buildingLevel, 1.1f);
        damageModifierUpper = Mathf.Pow(sb.buildingLevel, 1.3f);
    }

    private void modifyDamage(ammoScript a)
    {
        a.damageLower = a.damageLower * (1+damageModifierLower);
        a.damageUpper = a.damageUpper * (1+damageModifierUpper);
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
            aScript.updateMaxDistance(attackRange);
            aScript.attackTarget = attackTarget;
            modifyDamage(aScript);
            attackTimer = 0;
            findTarget();

            //giveammoChars
        }
    }
    
    // Use this for initialization
    void Start ()
    {
        attackTimer = 0;
        //attackRange = 5000;
        isActive = false;
        sb = GetComponentInParent<StructureBehavior>();
        updateDamageModifiers();
    }

  

    // Update is called once per frame
    void Update () {
        if (isActive && gameBoard.hasEnemies)
        {
            if (attackTimer < rateOfFire)
            {
                attackTimer += Time.deltaTime;
            }
            else if (attackTimer >= rateOfFire && attackTarget == null)
            {
                findTarget();
            }
            if (attackTimer > rateOfFire && attackTarget != null)
            {
                shootAmmo();
            }
        }
    }

}
