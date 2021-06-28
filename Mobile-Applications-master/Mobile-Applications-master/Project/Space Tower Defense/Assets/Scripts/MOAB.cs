using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOAB : MonoBehaviour {

    public float MOABDamage = 20.0f;
    public float MOABSize = 5.0f;

    // Use this for initialization
    void Start ()
    {
        gameObject.transform.localScale = gameObject.transform.localScale * MOABSize;
        Destroy(this, 0.5f);

    }
	

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.gameObject.GetComponent<EnemyController>().damageHealth(MOABDamage);
    }
}
