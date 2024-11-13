using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public class Node {
        public Vector3 pos;
        public bool playable;
        public float gCost;
        public float hCost;
        public float fCost {
            get { 
                return gCost+hCost; 
            }
        }
        public Node(Vector3 _pos, bool _playable) { 
            pos = _pos;
            playable = _playable;
        }
    }

    public const int WORLDSIZE_X = 30;
    public const int WORLDSIZE_Y = 16;
    
    Node[,] worldGrid;

    /** Play grid is [-15, 8] -> [15,-8], convert it to start from [0,0] */
    public int ConvertToGrid(int x, int size) {
        return x + (int) size/2;
    }

    /** Converts [0,0] starting grid to center it in play world*/
    public int ConvertToPlayGrid(int x, int size) {
        return x - (int) size/2;
    }
    
    /** Divide large world into smaller chunks that are easier to handle */
    public void createWorldGrid()
    {
        worldGrid = new Node[WORLDSIZE_X+1, WORLDSIZE_Y+1];
        int startX = (int) WORLDSIZE_X / 2 * -1;
        int startY = (int) (WORLDSIZE_Y / 2);
        for (int x = startX; x <= startX*-1; x++)
        {
            for (int y = startY; y >= startY*-1; y--) {
                Vector3 pos = new Vector3((float) x, (float) y, 0f);
                //TODO: find out if area playable
                Node node = new Node(pos, true);
                // Debug.Log(ConvertToGrid(x, WORLDSIZE_X) + " - " + ConvertToGrid(y, WORLDSIZE_Y));
                worldGrid[ConvertToGrid(x, WORLDSIZE_X), ConvertToGrid(y, WORLDSIZE_Y)] = node;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        createWorldGrid();
        Debug.Log(worldGrid);
    }

    public float CalculateDistance(float x, float y){
        
        
        return 0f;
    }

    public int DoPathfinding(Vector3 currPos, Vector3 targetPos){
        //Check all nodes next to current
        //If better cost, do X
        Debug.Log("CurrPos, " + ConvertToGrid((int) currPos.x, WORLDSIZE_X) + " " + ConvertToGrid((int) currPos.y, WORLDSIZE_Y));
        Debug.Log("TargetPos, " + ConvertToGrid((int) targetPos.x, WORLDSIZE_X) + " " + ConvertToGrid((int) targetPos.y, WORLDSIZE_Y));
        Node startNode;
        Node targetNode;
        try
        {
            startNode = worldGrid[ConvertToGrid((int) currPos.x, WORLDSIZE_X), ConvertToGrid((int) currPos.y, WORLDSIZE_Y)];
            targetNode = worldGrid[ConvertToGrid((int) targetPos.x, WORLDSIZE_X), ConvertToGrid((int) targetPos.y, WORLDSIZE_Y)];
        }
        catch (System.Exception)
        {
            Debug.Log("OOB");
        }
        
        //TODO: actually do pathfinding logic

        return 1;
    }

}


