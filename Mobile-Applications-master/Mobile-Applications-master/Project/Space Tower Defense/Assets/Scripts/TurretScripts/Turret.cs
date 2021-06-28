using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour {

    public Transform target;

    [Header("Attributes")]
    public float range = 2f;
    public float rotateSpeed = 10f;
    public float damage = 0;
    [Header("Use Bullets")]
    public float fireRate = 1.0f;
    private float fireCountdown = 0f;
    public GameObject bulletPrefab;

    [Header("Use Laser")]
    public bool useLaser = false;
    public LineRenderer lineRenderer;

    [Header("Use Electricity")]
    public float electricityDelay = 1.0f;
    private float electricityCountdown = 0f;


    [Header("Unity Setup")]
    public string enemyTag = "Enemy";
    public Transform turretHead;
    public bool useElectric = false;
    public Transform[] firePoints;
    public bool fireFromAnimation = false;


    public enum ShootStyle { First, Last, Strongest, Weakest};
    public ShootStyle shootStyle;

    public List<GameObject> EnemiesInRange = null;

    public int swivelSpeed = 8;

    private Animator animator;
    private MenuScript menuScript;

    // Use this for initialization
    void Start()
    {
        menuScript = GameObject.Find("Canvas").GetComponent<MenuScript>();
        animator = GetComponent<Animator>();
        InvokeRepeating("UpdateTarget", 0f, 0.1f);
        transform.localPosition = Vector3.zero;
        swivelSpeed = 8;
}

    void UpdateTarget()
    {
        RemoveDeadEnemies();
        switch (shootStyle)
        {
            case ShootStyle.First:
                FirstEnemy();
                break;
            case ShootStyle.Last:
                LastEnemy();
                break;
            case ShootStyle.Strongest:
                StrongestEnemy();
                break;
            case ShootStyle.Weakest:
                WeakestEnemy();
                break;

        }

    }


    void FirstEnemy()
    {
        for (int i = 0; i < EnemiesInRange.Count; i++)
        {
            if (target != null)
            {
                target = EnemiesInRange[0].transform;
                break;
            }
        }        
    }

    void LastEnemy()
    {
        for (int i = EnemiesInRange.Count; i --> 0; )
        {
            target = EnemiesInRange[i].transform;
            break;
        }
        //if (target != null)
        //{

        //    if (EnemiesInRange.Count > 0)
        //        target = EnemiesInRange[EnemiesInRange.Count - 1].transform;
        //}
    }

    void StrongestEnemy()
    {
        if (target == null && EnemiesInRange.Count>0)
        {
            target = EnemiesInRange[0].transform;
        }
        if (target != null)
        {
            for (int i = 0; i < EnemiesInRange.Count; i++)
            {
                if (EnemiesInRange[i].GetComponent<EnemyController>().health >= target.GetComponent<EnemyController>().health)
                {
                    target = EnemiesInRange[i].transform;
                }

            }
        }
    }

    void WeakestEnemy()
    {
        if(EnemiesInRange.Count == 0)
        {
            target = null;
        }
        if (target == null && EnemiesInRange.Count > 0)
        {
            target = EnemiesInRange[0].transform;
        }
        if (target != null)
        {
            for (int i = 0; i < EnemiesInRange.Count; i++)
            {
                if (EnemiesInRange[i].GetComponent<EnemyController>().health <= target.GetComponent<EnemyController>().health)
                {
                    target = EnemiesInRange[i].transform;
                }

            }
        }
    }

    void Shoot(int counter)
    {
        // Debug.Log("Shoot");
        
            GameObject bulletGo = null;

        if (target != null)
        {
            Vector3 dir = target.position - transform.position; // postion of target minus current position to find directions
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 tmp;
            float offset = 5.0f;
            tmp = lookRotation.eulerAngles;
            tmp.y -= 90;                            // fix the rotation
            tmp.z = (tmp.x * -1) - offset;          // uses the x value to aim vertically and iff offset so it aims at the center of the target
            tmp.x = 0;                              // stops the turret from rotating in the wrong axis

            Quaternion newRot = Quaternion.Euler(tmp);
            bulletGo = Instantiate(bulletPrefab, firePoints[counter].position, newRot, this.transform);   //casting as a game object to store
            Bullet bullet = bulletGo.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.damage = damage;
                bullet.Chase(target);
            }

        }


    }

    private void FixedUpdate()
    {
            
        
    }
    // Update is called once per frame
    void Update()
    {
        RemoveDeadEnemies();
        if (EnemiesInRange.Count == 0)
            target = null;
        if (target == null)
        {
            animator.SetBool("ifInRange", false);
            if (useLaser || useElectric)
            {
                if (lineRenderer.enabled)
                    lineRenderer.enabled = false;
            }
            return;
        }
        else
        {
            animator.SetBool("ifInRange", true);
        }

        if(!fireFromAnimation)
        {
            Fire();
        }
        else
        {
            Animator anim = GetComponent<Animator>();
            anim.speed = menuScript.rateOfFireMultiplier;           
        }
        LockOnTarget();

 
        

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void LockOnTarget()
    {
        ///Target lock
        Vector3 dir = target.position - transform.position; // postion of target minus current position to find directions
        Quaternion lookRotation= Quaternion.LookRotation(dir);
        Vector3 tmp;
        float offset = 5.0f;
        tmp = lookRotation.eulerAngles;
        tmp.y -= 90;                            // fix the rotation
        tmp.z = (tmp.x * -1) - offset;          // uses the x value to aim vertically and iff offset so it aims at the center of the target
        tmp.x = 0;                              // stops the turret from rotating in the wrong axis

        Quaternion newRot = Quaternion.Euler(tmp);
        //Vector3 rotation = lookRotation.eulerAngles;
        turretHead.rotation = Quaternion.RotateTowards(turretHead.rotation, newRot, swivelSpeed);
        
        //turretHead.Rotate(0, -90.0f, 0);//rotation ;// Quaternion.Lerp(turretHead.rotation, lookRotation, rotateSpeed * Time.deltaTime).eulerAngles;
        //turretHead.rotation = Quaternion.Euler(0f, rotation.y - 90, 0f);
    }

    void Fire()
    {
        if (fireCountdown <= 0f)
        {
            int currentFirePoint = 0;
            while (currentFirePoint < firePoints.Length)
            {
                Shoot(currentFirePoint);
                //have delay inbetween series of fires 
                currentFirePoint++;
            }
            fireCountdown = 1.0f / (fireRate * menuScript.rateOfFireMultiplier);
        }

        fireCountdown -= Time.deltaTime;
    }   

    void Laser()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoints[0].position);
        lineRenderer.SetPosition(1, target.position);

    }

    void Electric()
    {
        lineRenderer.enabled = true;

        if (electricityCountdown >= electricityDelay)
        {


            electricityCountdown = 0f;
            
        }
            electricityCountdown += Time.deltaTime;
       
        //lightning.StartPosition = firePoints.position;
        //lightning.EndPosition = target.position;
    }



    private void OnTriggerEnter(Collider other)
    {
        RemoveDeadEnemies();
        if (other.tag == "Enemy" && !EnemiesInRange.Contains(other.gameObject))
        {
            EnemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveDeadEnemies();
        if(other.tag == "Enemy" && EnemiesInRange.Contains(other.gameObject))
        {
            EnemiesInRange.Remove(other.gameObject);
        }
    }
    public void RemoveDeadEnemies()
    {
        for (int i = 0; i < EnemiesInRange.Count; i++)
        {
            if(EnemiesInRange[i] == null)
            {
                EnemiesInRange.RemoveAt(i);
            }
        }
    }

}

