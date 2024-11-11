using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class GridCalculatoor
{
    public const int GRIDSIZE = 10;
    public static float RoundFloat(float a) { 
        if (a > 0) {
            return (float) Math.Ceiling(a);
        }
	    return (float) Math.Floor(a);
    }

    public static float CenterPos (float a) {
        if (a >= 0) return a * GRIDSIZE - GRIDSIZE/2;
        return a * GRIDSIZE + GRIDSIZE/2;
    }

    public static Vector3 GetGridPos( Vector3 pos )
    {
        // Divide pos by 4 to get smaller grid pos
        Vector3 gridpos = pos / GRIDSIZE;
        
        // Round pos to get actual pos
        gridpos.x = RoundFloat(gridpos.x);
        gridpos.y = RoundFloat(gridpos.y);
        Debug.Log("" + gridpos);
        // center on this grid
        gridpos.x = CenterPos(gridpos.x);
        gridpos.y = CenterPos(gridpos.y);
        
        return gridpos;
    }


}
