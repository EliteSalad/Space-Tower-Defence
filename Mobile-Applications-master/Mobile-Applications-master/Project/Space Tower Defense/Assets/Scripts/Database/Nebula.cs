using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nebula : MonoBehaviour {

    public float NebulaTime = 5.0f;
    public float Nebulafactor = 0.5f;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, NebulaTime);
    }
	

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            other.gameObject.GetComponent<EnemyController>().changeSpeed(Nebulafactor);
    }
}
