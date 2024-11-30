using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : EntityGeneric
{
    public string entityName;
    public bool friendly;
    public float damage;
    float currentAttackInterval = 0f;
    public float attackInterval;
    public int xpReward;
    public int level;
    public float aggroRange;
    public LayerMask playerLayer;
    GameObject target; //TODO: Target can be chased
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
    RespawnHandler rp;
    Vector3 respawnPosition;

    void AttackTarget()
    {
        if (currentAttackInterval != 0) return;
        if (target == null) return;
        Player pl = target.GetComponent<Player>();
        if (pl == null) return;
        pl.TakeDamage(damage);
        currentAttackInterval++;
    }

    public void ResetEntity()
    {
        target = null;
        chaseTarget = false;
        transform.position = respawnPosition;
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        if (patrolPath.Count > 0 && !stationary){
            transform.position = GridCalculator.GetWorldPosFromGrid(patrolPath[0]);
            currentPatrolIndex++;
            StartCoroutine(myWaitCoroutine( () => StartPatrollingIteration(), 0.5f));
        }else {
            transform.position = GridCalculator.GetWorldPosFromGrid(GridCalculator.GetGridPos(transform.position));
        }
    }

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
        if (patrolPath.Count == 1) currentPatrolIndex = 0;
        patrolDestination = patrolPath[currentPatrolIndex];
        if (chaseTarget) {
            patrolDestination = GridCalculator.GetGridPos(target.transform.position);
        }
        pathToDestination = pathfinder.DoPathfinding(GridCalculator.GetGridPos(transform.position), patrolDestination);
        if (pathToDestination.Count > 0) waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
        patrolling = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        respawnPosition = transform.position;
        
        rp = GameObject.Find("RespawnHandler").GetComponent<RespawnHandler>();
        if (!WorldHandler.ChangeEntityStatus(gameObject)){
            DateTime dt = WorldHandler.GetTimeOfDeath(gameObject.name);
            DateTime ct = DateTime.Now;
            TimeSpan diff = ct-dt;
            if (diff.TotalSeconds < respawnTimerSeconds){
                rp.RespawnGameObject(gameObject, (float) (respawnTimerSeconds - diff.TotalSeconds));
                gameObject.SetActive(false);
                return;
            }
        }
        
        ResetEntity();
    }

    void EntityDies()
    {
        Debug.Log("DEAD");
        rp.RespawnGameObject(gameObject, (float) respawnTimerSeconds);
        WorldHandler.ChangeEntityStatus(gameObject, false);
        StartCoroutine(myWaitCoroutine( () => gameObject.SetActive(false), 2f));
        PlayerStats.GainXp(xpReward);
    }

    public float TakeDamage(float dmg, string type = "default"){
        float diff = dmg/damageReductionMultiplier;
        hp -= diff;
        if (hp <= 0) {
            hp = 0;
            EntityDies();
        }
        SpawnDamagePopup(diff, type);
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
        if (!isAlive) return;
        // if (gameObject.layer == deathLayer) return;
        //Will return to patrolling if current target is dead
        // Does not work for stationary targets so stationary shouldnt be used for now
        if (target && target.layer == deathLayer){
            target = null;
            chaseTarget = false;
            patrolling = true;
        }
        if (!friendly && !chaseTarget) CheckForPlayersInAggroRange();
        if (isAlive && (patrolling || chaseTarget)) DoPatrolling();
        if (currentAttackInterval > 0){
            currentAttackInterval++;
            if (currentAttackInterval == attackInterval) currentAttackInterval = 0;
        }
        if (target != null && chaseTarget && IsWithinRange(GridCalculator.GetGridPos(transform.position), GridCalculator.GetGridPos(target.transform.position))) AttackTarget();
    }
}
