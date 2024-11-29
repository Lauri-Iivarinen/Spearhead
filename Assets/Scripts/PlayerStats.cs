using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public static float baseDamage = 6f;
    public static float damageMultiplier = 1f;
    public static float damage {
        get {
            return baseDamage * damageMultiplier;
        }
    }
    public static float maxHp = 50f;
    public static float hp = maxHp;
    public static float damageReductionMultiplier = 1f;
	public static int xp = 0;
	public static int xpToNextLvl = 200;
	public static int level = 1;
	public static float attackSpeed = 1.1f;
    public static string playerName = "Hero";
    public static Entity target;
}
