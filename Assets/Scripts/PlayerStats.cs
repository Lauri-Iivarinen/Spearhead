using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public static float baseDamage = 4f;
    public static float damageMultiplier = 1f;
    public static float damage {
        get {
            return baseDamage * damageMultiplier;
        }
    }
    public static float maxHp = 50f;
    public static float hp = maxHp;
    

}
