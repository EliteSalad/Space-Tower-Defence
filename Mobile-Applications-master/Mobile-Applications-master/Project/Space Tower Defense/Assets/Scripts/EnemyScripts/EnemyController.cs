using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public NavMeshAgent NMA;
    public GameObject EndPath;
    private int currentPathNode = 0;
    public int childNumber = 0;
    public float health = 10;
    private float enemySpeed = 0;
    public int goldOnDeath = 0;
    public float distanceLeft;
    public bool inList = false;
    public int currentPlace = -1;
    public bool inRangeFirstTurret = false;
    private GameObject goldController;
    public GameController gameController;
    private List<GameObject> Path;

    // Use this for initialization
    void Start()
    {
        
        childNumber = Random.Range(0, 3);
        //;EndPath = ; // GameObject.Find("PathWNodes (" + currentPathNode + ")");
        //NMA.SetDestination(EndPath.transform.GetChild(childNumber).transform.position);
        enemySpeed = NMA.speed;
        goldController = GameObject.Find("GoldText");
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            goldController.GetComponent<GoldController>().ChangeGoldAmount(goldOnDeath);
           // Debug.Log("I died!");            
            Destroy(gameObject);
        }
        //Debug.Log("num children:"+EndPath.transform.childCount);
        childNumber = Random.Range(0, 3);
        //Debug.Log(childNumber);

        if (Vector3.Distance(transform.position, NMA.destination) <= 0.5f)
        {
            currentPathNode++;
            if (currentPathNode < Path.Count)
            {
                EndPath = Path[currentPathNode];// GameObject.Find("PathWNodes (" + currentPathNode + ")");               
                NMA.SetDestination(EndPath.transform.GetChild(childNumber).transform.position);                
            }
            else
            {
                GameObject.Find("GameManager").GetComponent<GameController>().reduceHealth();
                //this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }

        distanceLeft = Vector3.Distance(transform.position, NMA.destination);
    }

    ///   <summary>
    ///   Decreases health by input amount.
    ///   </summary>
    ///<param name="damage">Amount to decrease health by.
    ///</param>
    public void damageHealth(float damage)
    {
        health -= damage;
    }
    public void Targetable(bool inRange)
    {
        inRangeFirstTurret = inRange;
    }
    public void changeSpeed(float speed)
    {
        NMA.speed /= speed;
    }
    void resetSpeed()
    {
        NMA.speed = enemySpeed;
    }
    public void setPath(List<GameObject> path)
    {
        Path = path;
        EndPath = Path[currentPathNode];
        NMA.SetDestination(EndPath.transform.GetChild(childNumber).transform.position);
    }
}
