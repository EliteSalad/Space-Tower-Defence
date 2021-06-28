using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    float rate = 1.0f;
    float damage = 1.0f;
    //float distanceToEnemy;

    // Use this for initialization
    void Start ()
    {
        Invoke("DestroyObject", rate);
	}
	
	// Update is called once per frame
	void Update () {

        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            float distanceToEnemy = Vector3.Distance(transform.position, other.transform.position);
            other.gameObject.SendMessage("damageHealth", damage/distanceToEnemy);
            
        }

    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
