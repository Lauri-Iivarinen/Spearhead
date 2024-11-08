using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int count = 0;
    public int currentClickInterval = 0;
    public const int CLICKINTERVAL = 60;

    public Vector3 destination = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game start");
    }

    void checkMovement()
    {   
        int posX = (int) transform.position.x;
        int posY = (int) transform.position.y;
        var pos = transform.position;
        if (posX != destination.x){
            if (posX < destination.x) pos.x = pos.x + 1;
            else pos.x = pos.x - 1;   
        }
        if (posY != destination.y){
            if (posY < destination.y) pos.y = pos.y + 1;
            else pos.y = pos.y - 1;   
        }
        transform.position = pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   

        if (Input.GetMouseButton(0) && currentClickInterval == 0) 
        {   
            Debug.Log("Mouse1");
            //var pos = Input.mousePosition;
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 1;
            pos.x = (int) pos.x;
            pos.y = (int) pos.y;
            destination = pos;
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
