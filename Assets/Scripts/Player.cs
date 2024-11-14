using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{

    public int count = 0;
    public int currentClickInterval = 0;
    public const int CLICKINTERVAL = 60;
    public LayerMask collisionLayer;
    public Vector3 destination = new Vector3(); //Final destination
    public List<Pathfinder.Node> pathToDestination = new List<Pathfinder.Node>();
    public Vector3 waypointDestination = new Vector3();
    
    [SerializeField]
    private GameObject cursorPrefab;

    private GameObject currentCursor;

    private Pathfinder pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 v_1 = new Vector3(2, 2, 0);
        Vector3 v_2 = new Vector3(0, 4, 0);
        
        Debug.Log(v_1 - v_2);

        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    void checkMovement()
    {   
        if (pathToDestination.Count == 0) return;
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
            if (pathToDestination.Count > 0) waypointDestination = GridCalculatoor.GetWorldPosFromGrid(pathToDestination[0].pos);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {   

        if (Input.GetMouseButton(0) && currentClickInterval == 0) 
        {   
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 1;
            pos.x = (int) pos.x;
            pos.y = (int) pos.y;
            destination = GridCalculatoor.GetWorldPosFromGrid(GridCalculatoor.GetGridPos(pos));
            Destroy(currentCursor);
            currentCursor = Instantiate(cursorPrefab, transform);
            currentCursor.transform.parent = null;
            currentCursor.transform.position = destination;

            List<Pathfinder.Node> path = pathfinder.DoPathfinding(GridCalculatoor.GetGridPos(transform.position), GridCalculatoor.GetGridPos(pos));
            if (path.Count > 0) {
                pathToDestination = path;
                waypointDestination = GridCalculatoor.GetWorldPosFromGrid(pathToDestination[0].pos);
            }

            // foreach (Pathfinder.Node node in pathToDestination) Debug.Log(node.pos + " - " + node.playable);

            currentClickInterval++;
        }
        if (Input.GetMouseButton(1)) 
        {   
            
            //Debug.Log(pos);
        }
        if (currentClickInterval > 0) currentClickInterval++;
        if (currentClickInterval == CLICKINTERVAL) currentClickInterval = 0; 
        checkMovement();
        // Debug.Log(count);
        // count++;
    }
}
