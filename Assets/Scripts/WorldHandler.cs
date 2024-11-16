using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldHandler
{
    //World map stores index of all levels aivailable, "" == doesnt exist
    public static int x = 1;
    public static int y = 1;
    public static bool loadingNextWorld = false;
    public static Vector3 playerGridPos;
    public static string currentLevel {
        get {
            return worldMap[y,x];
        }
    }

    //Startworld, TODO: Create map of all smaller maps
    public static string[,] worldMap = {
        {""             ,"Level_ind_2"  ,"Level_ind_3"  },
        {"Level_ind_4"  ,"Level_ind_5"  ,""             },
        {"Level_ind_7"  ,"Level_ind_8"  ,"Level_ind_9"  },
    };

    public static string ChangeWorld(int _x, int _y){
        if (loadingNextWorld) return "";
        x += _x;
        y += _y;
        return currentLevel;
    }
}
