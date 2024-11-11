using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class GridCalculatoor
{

    public static float RoundFloat(float a) { 
        if (a > 0) {
            return (float) Math.Ceiling(a);
        }
	    return (float) Math.Floor(a);
    }

    public static float CenterPos (float a) {
        return a * 4 - 2;
    }

    public static Vector3 GetGridPos( Vector3 pos )
    {
        // Divide pos by 4 to get smaller grid pos
        Vector3 gridpos = pos / 4;
        
        // Round pos to get actual pos
        gridpos.x = RoundFloat(gridpos.x);
        gridpos.y = RoundFloat(gridpos.y);

        // center on this grid
        gridpos.x = CenterPos(gridpos.x);
        gridpos.y = CenterPos(gridpos.y);
        
        return gridpos;
    }


}
