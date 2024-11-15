using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{

    public class Node {
        public Vector3 pos;
        public bool playable;
        public int gridX;
        public int gridY;
        public float gCost;
        public float hCost;
        public Node parent;
        public float fCost {
            get { 
                return gCost+hCost; 
            }
        }
        public Node() {}
        public Node(Vector3 _pos, bool _playable, int _x, int _y) { 
            pos = _pos;
            playable = _playable;
            gridX = _x;
            gridY = _y;
        }
    }

    public const int WORLDSIZE_X = 30;
    public const int WORLDSIZE_Y = 16;
    Node[,] worldGrid;
    public LayerMask collisionLayer;

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
                //Get real world values for x and y
                Vector3 worldPoint = GridCalculatoor.GetWorldPosFromGrid(pos);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, 6f, collisionLayer); //TODO Change 10 to be configured
                Debug.Log(worldPoint + " " + walkable);
                Node node = new Node(pos, walkable, ConvertToGrid(x, WORLDSIZE_X), ConvertToGrid(y, WORLDSIZE_Y));
                // Debug.Log(ConvertToGrid(x, WORLDSIZE_X) + " - " + ConvertToGrid(y, WORLDSIZE_Y));
                worldGrid[ConvertToGrid(x, WORLDSIZE_X), ConvertToGrid(y, WORLDSIZE_Y)] = node;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        createWorldGrid();
        // Debug.Log(worldGrid);
    }

    public float CalculateDistance(Vector3 a, Vector3 b){
        
        float a_x = (float) a.x;
        float a_y = (float) a.y;
        float b_x = (float) b.x;
        float b_y = (float) b.y;

        float width = 0f;
        float height = 0f;

        if (a_x < b_x) width = b_x - a_x;
        else width = a_x - b_x;
        if (a_y < b_y) height = b_y - a_y;
        else height = a_y - b_y;

        return (float) Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
    }

    public List<Node> DoPathfinding(Vector3 currPos, Vector3 targetPos){
        //Check all nodes next to current
        //If better cost, do X
        Node startNode = new Node();
        Node targetNode = new Node();
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

        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();
        open.Add(startNode);

        while (open.Count > 0){
            Node current = open[0];
            // Debug.Log("Current node: " + current.gridX + "," + current.gridY);
            foreach (Node n in open) {
                if (n.fCost < current.fCost) current = n;
                else if (n.fCost == current.fCost && n.hCost < current.hCost) current = n;
            }
            open.Remove(current);
            closed.Add(current);

            if (current == targetNode) {
                //Path found
                // Debug.Log("PATH");
                // return new List<Node>();
                List<Node> path = new List<Node>();
                Node cn = current;
                while (cn != startNode) {
                    path.Add(cn);
                    cn = cn.parent;
                }
                path.Reverse();
                return path;
            }

            Vector2[] _neighbours = { 
                new Vector2(current.gridX, current.gridY+1),
                new Vector2(current.gridX, current.gridY-1),
                new Vector2(current.gridX+1, current.gridY),
                new Vector2(current.gridX-1, current.gridY)
            };

            List<Vector2> neighbours = new List<Vector2>(_neighbours);

            // Debug.Log("Checker");
            foreach (Vector2 v in neighbours) {
                // Debug.Log(v);
                if (v.x < 0 || v.x > 30 || v.y < 0 || v.y > 16) continue;
                int nodeX = (int)v.x;
                int nodeY = (int)v.y;
                Node neighbor = worldGrid[nodeX, nodeY];
                if (closed.Contains(neighbor) || !neighbor.playable) continue;
                // Debug.Log("Do Logic");
                float newG = current.gCost + CalculateDistance(new Vector3(current.gridX, current.gridY), new Vector3(neighbor.gridX, neighbor.gridY));
                if (newG < neighbor.gCost || !open.Contains(neighbor)){
                    float newH = CalculateDistance(new Vector3(neighbor.gridX, neighbor.gridY), new Vector3(targetNode.gridX, targetNode.gridY));
                    neighbor.hCost = newH;
                    neighbor.gCost = newG;
                    neighbor.parent = current;

                    if (!open.Contains(neighbor)) open.Add(neighbor);
                }
                
            }
        }

        return new List<Node>();
    }

}


