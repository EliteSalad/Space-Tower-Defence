using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour {

    public List<GameObject> staticShock = null;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //void OnDestroy()
    //{
    //    Debug.Log("On Destroy works");
    //    //SendMessageUpwards("ThunderStrike", staticShock);
    //    //SendMessageUpwards("TestingSend");
    //    for (int i = 0; i < staticShock.Count; i++)
    //    {
    //        if (staticShock[i] == null)
    //            staticShock.RemoveAt(i);
    //    }
    //    this.transform.parent.GetComponent<Bullet>().ThunderStrike(staticShock);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !staticShock.Contains(other.gameObject))
        {
            staticShock.Add(other.gameObject);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && staticShock.Contains(other.gameObject))
        {
            staticShock.Remove(other.gameObject);
        }
    }

}
