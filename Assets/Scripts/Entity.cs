using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public bool friendly;
    public float hp;
    public float maxHp;
    public float damage;
    public int level;
    public float aggroRange;
    public LayerMask playerLayer;
    public bool isAlive{
        get { return hp > 0; }
    }
    public GameObject target; //TODO: Target can be chased
    bool chaseTarget;
    public List<Vector3> patrolPath; //Detault path entity will follow, if none then stationary

    //List of nodes to the current path marked by patrolPath[currentIndex] or patrolDestination
    List <Pathfinder.Node> pathToDestination = new List<Pathfinder.Node>(); 
    public Vector3 patrolDestination; //patrolPath[currentIndex]
    Vector3 waypointDestination; //current target from list of waypoints from pathfinding
    public int currentPatrolIndex = 0; //Tracks patrol path
    Pathfinder pathfinder;
    bool patrolling = false; //Statoinary -> false
    public bool stationary;
    public float speed = 0.5f; //Cant go lower than 0.5, maybe needs fixing in the future
    public float damageReductionMultiplier = 1f; // Higher -> take less dmg

    void DoPatrolling(){ //Follows patrol path OR chases player if aggroed
        float timer = 2f;
        if (chaseTarget) timer = 0f;

        if (stationary && !chaseTarget) return;
        if (chaseTarget && pathToDestination.Count < 2 ){
            StartCoroutine(myWaitCoroutine( () => StartPatrollingIteration(), timer));
            return;
        }
        // Move from waypoint to waypoint
        float posX = transform.position.x;
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
                StartCoroutine(myWaitCoroutine( () => StartPatrollingIteration(), timer));
            }
        }
    }

    void StartPatrollingIteration(){
        if ( currentPatrolIndex == patrolPath.Count){
            currentPatrolIndex = 1;
            patrolPath.Reverse();
        }
        patrolDestination = patrolPath[currentPatrolIndex];
        if (chaseTarget) {
            patrolDestination = GridCalculator.GetGridPos(target.transform.position);
        }
        pathToDestination = pathfinder.DoPathfinding(GridCalculator.GetGridPos(transform.position), patrolDestination);
        if (pathToDestination.Count > 0) waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
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
        if (patrolPath.Count > 0 && !stationary){
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

    void CheckForPlayersInAggroRange()
    {
        // Debug.Log("Checking aggro");
        Collider2D collider = Physics2D.OverlapCircle(transform.position, aggroRange, playerLayer);
        if (collider) {
            target = collider.gameObject;
            chaseTarget = true;
            patrolling = false;
            // Player pl = target.GetComponent<Player>();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (!friendly && !chaseTarget) CheckForPlayersInAggroRange();
        if (isAlive && (patrolling || chaseTarget)) DoPatrolling();
    }
}
