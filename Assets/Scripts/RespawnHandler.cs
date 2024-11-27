using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator myWaitCoroutine(Action func, float wait)
    {
        yield return new WaitForSeconds(wait);// Wait for one second
        func();
    }

    void Respawn(GameObject go)
    {
        Entity ent = go.GetComponent<Entity>();
        ent.hp = ent.maxHp;
        WorldHandler.ChangeEntityStatus(go, false);
        go.SetActive(true);
        ent.ResetEntity();
    }

    public void RespawnGameObject(GameObject go, float seconds)
    {
        StartCoroutine(myWaitCoroutine(() => Respawn(go), seconds));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
