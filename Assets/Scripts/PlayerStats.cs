using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    // Add more levelcaps and actual logic for it
    static float[] xpCaps = { 0, 200, 300, 400, 500, 600 };
    static float[] baseDamagePerLevel = { 0, 6f, 7f, 8f, 9f, 10f };
    static float[] baseMaxHpPerLevel = { 0, 50f, 60f, 65f, 75f, 80f };
    public static bool interacting = false;
    public static bool stopInteracting = false;
    //Level specific stuff ends
    public static int level = 1;
    public static float baseDamage {
        get {
            return baseDamagePerLevel[level];
        }
    }
    public static float damageMultiplier = 1f;
    public static float damage {
        get {
            return baseDamage * damageMultiplier;
        }
    }
    public static float maxHpAdditions = 0f;
    public static float maxHp {
        get {
            return baseMaxHpPerLevel[level] + maxHpAdditions;
        }
    }
    public static float hp = maxHp;
    public static float damageReductionMultiplier = 1f;
	public static float xp = 0;
    
	public static float maxLevel{
        get{
            return xpCaps.Length;
        }
    }
	public static float attackSpeed = 1.1f;
    public static string playerName = "Hero";
    public static Entity target;
    public static float xpToNextLvl{
        get{
            return xpCaps[level];
        }
    }

    public static void LevelUp()
    {
        xp -= xpToNextLvl;
        level++;
        hp = maxHp;
    }
    
    public static void GainXp(float amount){
        if (level == maxLevel){
            xp = 0;
            return;
        }
        xp += amount;
        if (xp >= xpToNextLvl){
            LevelUp();
        }
    }
}
