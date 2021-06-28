using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSystemEditor : MonoBehaviour {

    private ParticleSystem ps;
    private int TurretChildNumber = 2;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.childCount > 2)
        {
            if (transform.parent.GetChild(TurretChildNumber).tag == "StandardTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.red;
            }
            else if (transform.parent.GetChild(TurretChildNumber).tag == "ElectricityTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.blue;
            }
            else if (transform.parent.GetChild(TurretChildNumber).tag == "RocketTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.yellow;
            }
            else if (transform.parent.GetChild(TurretChildNumber).tag == "LaserTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.magenta;
            }
            else if (transform.parent.GetChild(TurretChildNumber).tag == "SniperTurret")
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = Color.white;
            }
        }
        else
        {
            ParticleSystem.MainModule main = ps.main;
            main.startColor = Color.green;
        }
	}
}
