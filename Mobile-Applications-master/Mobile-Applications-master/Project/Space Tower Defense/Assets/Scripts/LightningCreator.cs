using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningCreator : MonoBehaviour {

    public Transform GO;
    public Transform StartGO;
    public LineRenderer LR;
    public float arcLength = 2.0f;
    public float arcVariation = 2.0f;
    public float inaccuracy = 1.0f;
	// Use this for initialization
	void Start () {
        LR = this.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lastPoint = StartGO.position;
        int i = 1;
        LR.SetPosition(0, StartGO.position);
        while(Vector3.Distance(GO.position, lastPoint)>0.5f)
        {
            LR.positionCount= i+1;
            Vector3 fwd = GO.position - lastPoint;
            fwd.Normalize();
            fwd = Randomize(fwd, inaccuracy);
            fwd *= Random.Range(arcLength * arcVariation, arcLength);
            fwd += lastPoint;
            LR.SetPosition(i, fwd);
            i++;
            lastPoint = fwd;
        }
	}

    Vector3 Randomize(Vector3 v3, float inaccuracy2)
    {
        v3 += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * inaccuracy2;
        v3.Normalize();
        return v3;
    }
}
