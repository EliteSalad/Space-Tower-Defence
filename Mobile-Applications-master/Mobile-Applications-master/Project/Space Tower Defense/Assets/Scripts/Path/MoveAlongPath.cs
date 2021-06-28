using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAlongPath : MonoBehaviour {

    public NavMeshAgent NME;

    public GameObject Target;
    public int TargetNum = 0;

    public float DistanceFromWayPoint = 0.0f;

	// Use this for initialization
	void Start () {
        NME = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Target != null)
        {
            
        DistanceFromWayPoint = Vector3.Distance(Target.transform.position, transform.position);
        NME.SetDestination(Target.transform.position);
        }
	}
}
