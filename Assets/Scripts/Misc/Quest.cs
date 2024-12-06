using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public string title;
    public float xpReward;
    public string description;
    public bool completed = false;

    //Target who grants the rewards and completes the quest
    public bool completerName;

    public Quest(){

    }
}
