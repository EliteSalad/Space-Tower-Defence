using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public enum TurretType { Standard, Laser, Rocket, Electric, Sniper };
    public Transform target;
    public float speed = 10f;
    public float damage = 2f;

    [Header("Effects")]
    public GameObject impactEffect;
    public GameObject launchEffect;
    public GameObject trailEffect;

    [Header("Turret Type")]
    public TurretType shootStyle;
    
    public List<GameObject> staticShock = null;
    public GameObject staticBullet = null;
    public int electricBounce = 3;
    public float shockWeaknesFactor = 2.0f;
    public void Chase(Transform _target)
    {
        target = _target;
    }

	
	// Update is called once per frame
	void Update ()
    {
       
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 dir = target.position - transform.position;
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = look;
            float distanceThisFrame = speed * Time.deltaTime;

          

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
      
    }

    void HitTarget()
    {

        switch (shootStyle)
        {
            case TurretType.Standard:
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;
            case TurretType.Electric:
                ThunderStrike(transform.GetChild(0).GetComponent<Shock>().staticShock);
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;
            case TurretType.Rocket:
                Debug.Log("Rocket Hit");
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;
            case TurretType.Laser:
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;
            case TurretType.Sniper:
                target.gameObject.SendMessage("damageHealth", damage);
                Destroy(gameObject);
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == target.tag)
        {
         //Debug.Log("Hit");
            HitTarget();
        }
    }
    public void ThunderStrike(List<GameObject> targets)
    {
        electricBounce += 1;

        for (int i = 0; i < staticShock.Count; i++)
        {
            if (staticShock[i] == null)
                staticShock.RemoveAt(i);
        }

        Debug.Log("Begining lightning Attack");

        if (targets.Count < electricBounce) //if potential bounce targets less than amout of times we can bouce 
            electricBounce = targets.Count; //set to smaller number

        for (int i = 1; i < electricBounce; i++)    //create bullet for each bounce
        {
            GameObject staticGo = null;
            staticGo = Instantiate(staticBullet, this.transform.position, this.transform.rotation);   //
            Bullet sBullet = staticGo.GetComponent<Bullet>();

            if (sBullet != null)
            {

                sBullet.damage = damage / shockWeaknesFactor;
                sBullet.Chase(targets[i].transform);

            }

        }
    }
}
