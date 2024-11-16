using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class GridCalculator
{
    public const int GRIDSIZE = 10;
    public static float RoundFloat(float a) { 
        if (a > 0) {
            return (float) Math.Ceiling(a);
        }
	    return (float) Math.Floor(a);
    }

    //Centers position in the middle of current gridPos
    public static float CenterPos (float a) {
        if (a >= 0) return a * GRIDSIZE - GRIDSIZE/2;
        return a * GRIDSIZE + GRIDSIZE/2;
    }

    // Convert grid pos to position in the play world
    public static Vector3 GetWorldPosFromGrid(Vector3 gridpos){
        gridpos.x = CenterPos(gridpos.x);
        gridpos.y = CenterPos(gridpos.y);
        return gridpos;
    }

    public static Vector3 GetGridPos( Vector3 pos )
    {
        Vector3 gridpos = pos / GRIDSIZE;
        
        // Round pos to get actual pos
        gridpos.x = RoundFloat(gridpos.x);
        gridpos.y = RoundFloat(gridpos.y);
        return gridpos;
    }


}
