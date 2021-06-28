using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBeam : MonoBehaviour {

    public float beamDamage = 4.0f;
    private float beamDuration = 3.0f;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, beamDuration);
    }

    private void OnTriggerStay(Collider other)
    {
        other.gameObject.GetComponent<EnemyController>().damageHealth(beamDamage);
    }

  
}
