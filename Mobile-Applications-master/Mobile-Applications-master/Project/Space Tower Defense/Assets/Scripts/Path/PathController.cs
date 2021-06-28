using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour {

    public List<MoveAlongPath> Emitters;

    public List<GameObject> WayPoints;

    public float ChangeWPDistance = 2.0f;
	
	// Update is called once per frame
	void Update () {
        foreach (MoveAlongPath emitter in Emitters)
        {
            if (emitter.DistanceFromWayPoint <= ChangeWPDistance)
            {
                // Made it to destination, change if not at end
                if (emitter.TargetNum < WayPoints.Count - 1)
                {
                    emitter.Target = WayPoints[emitter.TargetNum + 1];
                    emitter.TargetNum += 1;
                }
                else
                {                    
                    emitter.NME.Warp(new Vector3(WayPoints[0].transform.position.x, -0.29f, WayPoints[0].transform.position.z));
                    emitter.TargetNum = 0;
                    emitter.Target = WayPoints[emitter.TargetNum];
                }
            }
        }
	}
}
