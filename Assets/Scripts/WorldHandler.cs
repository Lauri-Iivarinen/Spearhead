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

    public static Dictionary<string, string> aliveEntities = new Dictionary<string, string>();
    public static Dictionary<string, DateTime> deadEntities = new Dictionary<string, DateTime>();

    //Startworld, TODO: Create map of all smaller maps
    public static string[,] worldMap = {
        {""             ,"Level_ind_2"  ,"Level_ind_3"  },
        {"Level_ind_4"  ,"Level_ind_5"  ,""             },
        {"Level_ind_7"  ,"Level_ind_8"  ,"Level_ind_9"  },
    };

    // Change status by entityname, name in world, not the entity displayed name
    // eg. Hungry Wolf (2) etc.
    public static bool ChangeEntityStatus(GameObject obj, bool checkStatus = true) 
    {
        string entityName = obj.name;
        EntityGeneric eg = obj.GetComponent<EntityGeneric>();
        if (checkStatus && aliveEntities.ContainsKey(entityName)) return true;
        if (checkStatus && deadEntities.ContainsKey(entityName)) return false;


        if (!aliveEntities.ContainsKey(entityName) && !deadEntities.ContainsKey(entityName)){
            if (eg.isAlive){
                aliveEntities[entityName] = "?";
                return true;
            }else{
                deadEntities[entityName] = DateTime.Now;
                return false;
            }
        }
        if (aliveEntities.ContainsKey(entityName) && !deadEntities.ContainsKey(entityName)){
            aliveEntities.Remove(entityName);
            deadEntities.Add(entityName, DateTime.Now);
            return false;
        }
        if (!aliveEntities.ContainsKey(entityName) && deadEntities.ContainsKey(entityName)){
            deadEntities.Remove(entityName);
            aliveEntities.Add(entityName, "?");
            return true;
        }

        return eg.isAlive;
    }

    public static DateTime GetTimeOfDeath(string k){
        if (!deadEntities.ContainsKey(k)) return DateTime.Now;
        return deadEntities[k];
    }

    public static string ChangeWorld(int _x, int _y){
        if (loadingNextWorld) return "";
        x += _x;
        y += _y;
        return currentLevel;
    }
}
