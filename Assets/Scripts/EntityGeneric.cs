using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityGeneric : MonoBehaviour
{
    public GameObject damagePopup;
    public LayerMask deathLayer;
    public float maxHp;
    public float hp;
    public bool isAlive {
        get {
            return hp > 0;
        }
    }

    public IEnumerator myWaitCoroutine(Action func, float wait)
    {
        yield return new WaitForSeconds(wait);// Wait for one second
        func();
    }

    public bool IsWithinRange(Vector3 aPos, Vector3 bPos)
    {
        if (aPos.x != bPos.x) {
            return (aPos.x-1 == bPos.x || aPos.x+1 == bPos.x) && aPos.y == bPos.y;
        }
        if (aPos.y != bPos.y) {
            return (aPos.y == bPos.y-1 || aPos.y == bPos.y+1) && aPos.x == bPos.x;
        }
        return aPos.x == bPos.x && aPos.y == bPos.y;
    }

    // For future check if player has ranged attack and is withing range
    public bool IsWithinFarRange(Vector3 aPos, Vector3 bPos)
    {
        return false;
    }

    public void SpawnDamagePopup(float amount, string type)
    {
        GameObject popup = Instantiate(damagePopup);
        Vector3 popupPos = popup.transform.position;
        popupPos.x = transform.position.x;
        popupPos.y = transform.position.y + 10;
        popupPos.z = -1f;
        popup.transform.position = popupPos;
        DamagePopupHandler dph = popup.GetComponent<DamagePopupHandler>();
        dph.ChangeData(type, ""+((int) amount));
        
    }


}
