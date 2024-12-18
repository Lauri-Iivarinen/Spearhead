using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/*

ISSUE TRACKER:
- Grant xp
- Friendly entity
- 

*/

public class Player : EntityGeneric
{
    // Reduces amount of clicks player can do, mby reduces load idk 
    public int currentClickInterval = 0;
    public const int CLICKINTERVAL = 30;
    public LayerMask entityLayer; //Mobs etc.
    public LayerMask collisionLayer; //Player cant enter collisionLayer objects
    public Vector3 destination = new Vector3(); //Final destination of mouse click
    public List<Pathfinder.Node> pathToDestination = new List<Pathfinder.Node>();
    public Vector3 waypointDestination = new Vector3(); //Current index of pathfinding nodes
    [SerializeField]
    private GameObject cursorOKPrefab;
    [SerializeField]
    private GameObject cursorNOPrefab;
    private GameObject currentCursor;
    private Pathfinder pathfinder;

    GameObject target;
    bool chaseTarget = false;
    bool isInteracting = false;
    int attackInterval = 60;
    int currentAttackInterval = 0;
	PlayerStats PlayerStats;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = GridCalculator.GetWorldPosFromGrid(WorldHandler.playerGridPos);
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    //Check if player is on edge of the map and loads the next area if possible
    void CheckBorderContact()
    {
        Vector3 gridPos = GridCalculator.GetGridPos(transform.position);
        string nextWorld = "";
        int xMultiplier = 1;
        int yMultiplier = 1;
        if (gridPos.x == -15) {
            xMultiplier = -1;
            nextWorld = WorldHandler.ChangeWorld(-1, 0);
        }
        if( gridPos.x == 15) {
            xMultiplier = -1;
            nextWorld = WorldHandler.ChangeWorld(1, 0);
        }
        if (gridPos.y == -8) {
            yMultiplier = -1;
            nextWorld = WorldHandler.ChangeWorld(0, 1);
        }
        if (gridPos.y == 8) {
            yMultiplier = -1;
            nextWorld = WorldHandler.ChangeWorld(0, -1);
        }
        
        if (nextWorld != "") {
            WorldHandler.loadingNextWorld = true;
            //Change polayer to be on the opposite side
            WorldHandler.playerGridPos = new Vector3(gridPos.x*xMultiplier, gridPos.y*yMultiplier, gridPos.z);
            Debug.Log("LOADING NEW MAP " + nextWorld);
            SceneManager.LoadSceneAsync(nextWorld);
        }
    }

    //Check if player has destination to move to and does single iteration of movement toward the goal
    void CheckMovement()
    {   
        if (pathToDestination.Count == 0 || (pathToDestination.Count == 1 && chaseTarget)) {
            CheckBorderContact();
            return;
        }
        
        // Move from waypoint to waypoint
        int posX = (int) transform.position.x;
        int posY = (int) transform.position.y;
        var pos = transform.position;
        
        if (posX != waypointDestination.x){
            if (posX < waypointDestination.x) pos.x = pos.x + 1;
            else pos.x = pos.x - 1;   
        } else if (posY != waypointDestination.y){
            if (posY < waypointDestination.y) pos.y = pos.y + 1;
            else pos.y = pos.y - 1;   
        }
        transform.position = pos;
        if (transform.position.x == waypointDestination.x && transform.position.y == waypointDestination.y) {
            // Change waypoint
            pathToDestination.RemoveAt(0);
            if (pathToDestination.Count > 0) waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
        }
    }

    void Die()
    {
        // StartCoroutine(myWaitCoroutine(() => gameObject.layer = deathLayer, 1f));
        // gameObject.layer = deathLayer;
        Debug.Log("Player died");
        PlayerStats.hp = 0;
    }

    public float TakeDamage(float dmg, string _type = "hostile")
    {
        float diff = dmg/PlayerStats.damageReductionMultiplier;
        PlayerStats.hp -= diff;
        if (PlayerStats.hp <= 0) Die();
        SpawnDamagePopup(diff, _type);
        return diff;
    }

    void MoveToTarget()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        pos.x = (int) pos.x;
        pos.y = (int) pos.y;
        destination = GridCalculator.GetWorldPosFromGrid(GridCalculator.GetGridPos(pos));

        Destroy(currentCursor);
        List<Pathfinder.Node> path = pathfinder.DoPathfinding(GridCalculator.GetGridPos(transform.position), GridCalculator.GetGridPos(pos));
        if (path.Count > 0) { //Path found
            currentCursor = Instantiate(cursorOKPrefab, transform);
            pathToDestination = path;
            waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
            WorldHandler.loadingNextWorld = false;
        } else currentCursor = Instantiate(cursorNOPrefab, transform);

        currentCursor.transform.parent = null;
        currentCursor.transform.position = destination;

        currentClickInterval++;
    }

    void AquireTarget()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        pos.x = (int) pos.x;
        pos.y = (int) pos.y;
        pos = GridCalculator.GetWorldPosFromGrid(GridCalculator.GetGridPos(pos));
        Collider2D collider = Physics2D.OverlapCircle(pos, 5f, entityLayer);
        if (collider) {
            target = collider.gameObject;
            PlayerStats.target = target.GetComponent<Entity>();
            chaseTarget = true;
        }else{
            target = null;
            PlayerStats.target = null;
        }
    }

    void PathfindToTarget()
    {   
        if (!chaseTarget || target == null) return;
        //Check if close enough, else chase
        Vector3 playerGrid = GridCalculator.GetGridPos(transform.position);
        Vector3 targetGrid = GridCalculator.GetGridPos(target.transform.position);
        if (!IsWithinRange(playerGrid, targetGrid)) {
            List<Pathfinder.Node> path = pathfinder.DoPathfinding(playerGrid, targetGrid);
            if (path.Count > 1) {
                //Sheesh, debugged bad pathfinding from 22:30 to 01:20 do not touch lol
                pathToDestination = path;
                waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
                //Player will stay stationary because its targeting its own location as the first step on path
                if (waypointDestination == GridCalculator.GetWorldPosFromGrid(GridCalculator.GetGridPos(transform.position))) {
                    pathToDestination.RemoveAt(0);
                    waypointDestination = GridCalculator.GetWorldPosFromGrid(pathToDestination[0].pos);
                }
            }
            return;
        }
    }

    void AttackTarget(Entity e)
    {   
        if (currentAttackInterval != 0 ) return;
        if (!e.isAlive) {
            target = null;
            return;
        }
        Debug.Log("Attack");
        e.TakeDamage(PlayerStats.damage);
        currentAttackInterval++;
    }

    void CheckTargetInteraction()
    {
        // Debug.Log("Made it to Interact");
        Vector3 playerGrid = GridCalculator.GetGridPos(transform.position);
        Vector3 targetGrid = GridCalculator.GetGridPos(target.transform.position);
        if (!IsWithinRange(playerGrid, targetGrid) && !IsWithinFarRange(playerGrid, targetGrid))
        {
            PlayerStats.interacting = false;
            return;
        }
        Entity tg = target.GetComponent<Entity>();
        if (tg.friendly && !isInteracting) {
            Debug.Log("Interacting");
            PlayerStats.interacting = true;
        }else if (!tg.friendly) {
            AttackTarget(tg);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerStats.stopInteracting)
        {
            target = null;
            PlayerStats.target = null;
            chaseTarget = false;
            PlayerStats.stopInteracting = false;
            pathToDestination.Clear();
        }
        if (Input.GetMouseButton(0) && currentClickInterval == 0 && !PlayerStats.interacting) 
        {   
            target = null;
            PlayerStats.target = null;
            chaseTarget = false;
            MoveToTarget();
        } else if (Input.GetMouseButton(1) && currentClickInterval == 0 && !PlayerStats.interacting) 
        {   
            chaseTarget = false;
            AquireTarget();
            MoveToTarget();
        } else if (currentClickInterval % 30 == 0) PathfindToTarget(); //No need to pathfind every frame
        if (currentClickInterval > 0) currentClickInterval++;
        if (currentClickInterval == CLICKINTERVAL) currentClickInterval = 0;
        if (currentAttackInterval > 0) currentAttackInterval++;
        if (currentAttackInterval == attackInterval) currentAttackInterval = 0;
    
        if (target != null && chaseTarget && !PlayerStats.interacting) CheckTargetInteraction();
        CheckMovement();
    }
}
