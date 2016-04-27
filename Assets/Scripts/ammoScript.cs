using UnityEngine;
using System.Collections;

public class ammoScript : MonoBehaviour
{


    public float speed = 5;
    public float maxDistance = 5.6f;
    public float distanceTraveled;
    public float damageLower = 2;
    public float damageUpper = 7;
    public string damageType;
    public float angle = 90;
    public bool isHoming;
    public bool hasTarget;

    public GameObject attackTarget;
    public Vector3 attackPosition;

    // Use this for initialization
    void Start()
    {
        hasTarget = true;
        isHoming = false;

        //TrailRenderer tr = gameObject.GetComponentInChildren<TrailRenderer>();
        //tr.sortingLayerName = "Structure";
        ParticleSystemRenderer psr = gameObject.GetComponentInChildren<ParticleSystemRenderer>();
        psr.sortingLayerName = "Structure";
        

    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            //enemyBehavior enemyInfo = col.gameObject.GetComponent<enemyBehavior>();
            //float damage = Mathf.Round(Random.value * (damageUpper - damageLower) + damageLower);
            //enemyInfo.hitPoints -= damage;
            Destroy(gameObject);
        }

    }

    public float getDistance(Vector3 a, Vector3 b)
    {
        Vector3 diff = a - b;
        return (diff.sqrMagnitude);
    }

    bool getNewTarget()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("Enemy");
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
            print("---attack target " + closest.name + " at " + closest.transform.position + " distance:" + distance);
            return true;
        }
        else
        {
            print("!!! no target found for ammo");
            return false;
        }
    }


    void rotateToTarget(Vector3 tp)
    {
        Vector3 targetDir = tp - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg ;
        transform.rotation = Quaternion.identity;
        transform.Rotate(0, 0, angle-90);
    }

    void moveAmmo(float s)
    {
        if(attackTarget != null)
            attackPosition = attackTarget.transform.position + new Vector3(0.5f, -0.5f);

        transform.position = Vector3.MoveTowards(transform.position, attackPosition, s);
        rotateToTarget(attackPosition);
            
    }
    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        if (attackTarget == null && isHoming)
        {
            hasTarget = getNewTarget();
        }
        else
        {
            moveAmmo(step);
            distanceTraveled += step;
        }

        
        if (distanceTraveled > maxDistance || hasTarget == false || getDistance(attackPosition, transform.position)<.00001)
            Destroy(gameObject);
        

    }
}
