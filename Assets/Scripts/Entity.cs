using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public bool friendly;
    public float hp;
    public float maxhp;
    public float damage;
    public int level;
    public bool isAlive{
        get { return hp > 0; }
    }
    public GameObject target; //TODO: Target can be chased
    public List<Vector3> patrolPath; //Detault path entity will follow, if none then stationary

    //List of nodes to the current path marked by patrolPath[currentIndex] or patrolDestination
    List <Pathfinder.Node> pathToDestination = new List<Pathfinder.Node>(); 
    public Vector3 patrolDestination; //patrolPath[currentIndex]
    Vector3 waypointDestination; //current target from list of waypoints from pathfinding
    public int currentPatrolIndex = 0; //Tracks patrol path
    Pathfinder pathfinder;
    bool patrolling = false; //Statoinary -> false

    public float speed = 0.5f; //Cant go lower than 0.5, maybe needs fixing in the future
    public float damageReductionMultiplier = 1f; // Higher -> take less dmg

    void DoPatrolling(){
        // Move from waypoint to waypoint
        float posX =  transform.position.x;
        float posY = transform.position.y;
        var pos = transform.position;
        if (posX != waypointDestination.x){
            if (posX < waypointDestination.x) pos.x = pos.x + speed;
            else pos.x = pos.x - speed;   
        } else if (posY != waypointDestination.y){
            if (posY < waypointDestination.y) pos.y = pos.y + speed;
            else pos.y = pos.y - speed;   
        }
        transform.position = pos;
        if (transform.position.x == waypointDestination.x && transform.position.y == waypointDestination.y) {
            // Change waypoint
            pathToDestination.RemoveAt(0);
            if (pathToDestination.Count > 0) waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
            else {
                //Change patrol waypoint
                currentPatrolIndex++;
                patrolling = false;
                StartCoroutine(myWaitCoroutine( () => StartPatrollingIteration(), 2f));
            }
        }
    }

    void StartPatrollingIteration(){
        if (currentPatrolIndex == patrolPath.Count){
            currentPatrolIndex = 1;
            patrolPath.Reverse();
        }
        patrolDestination = patrolPath[currentPatrolIndex];
        // Debug.Log(GridCalculator.GetGridPos(transform.position) + " -> " + patrolDestination);
        pathToDestination = pathfinder.DoPathfinding(GridCalculator.GetGridPos(transform.position), patrolDestination);
        // Debug.Log(pathToDestination);
        waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
        patrolling = true;
    }

    IEnumerator myWaitCoroutine(Action func, float wait)
    {
        yield return new WaitForSeconds(wait);// Wait for one second
        func();
    }

    // Start is called before the first frame update
    void Start()
    {
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        if (patrolPath.Count > 0){
            transform.position = GridCalculator.GetWorldPosFromGrid(patrolPath[0]);
            currentPatrolIndex++;
            StartCoroutine(myWaitCoroutine( () => StartPatrollingIteration(), 0.5f));
            
        }else {
            transform.position = GridCalculator.GetWorldPosFromGrid(GridCalculator.GetGridPos(transform.position));
        }
    }

    void EntityDies()
    {
        Debug.Log("DEAD");
        StartCoroutine(myWaitCoroutine( () => Destroy(gameObject), 2f));
    }

    public float TakeDamage(float dmg){
        float diff = dmg/damageReductionMultiplier;
        hp -= diff;
        if (hp <= 0) EntityDies();
        return diff;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( isAlive && patrolling) DoPatrolling();
    }
}
