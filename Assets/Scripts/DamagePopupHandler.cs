using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class DamagePopupHandler : EntityGeneric
{
    public string amount;
    Dictionary<string, Color> colorMap = new Dictionary<string, Color>(){
        {"default", Color.white},
        {"hostile", Color.red},
        {"ability", Color.blue},
        {"critical", Color.yellow},
        {"heal", Color.green}
    };
    TextMeshPro tm;
    string txt = "";
    string key = "default";
    // Start is called before the first frame update
    void Start()
    {
        tm = GetComponent<TextMeshPro>();
        StartCoroutine(myWaitCoroutine(() => Destroy(gameObject), 3f));
    }

    public void ChangeData(string _key, string _txt)
    {
        key = _key;
        txt = _txt;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        // Debug.Log(col + " " + _txt);
        if (colorMap.ContainsKey(key)){
            tm.color = colorMap[key];
            tm.text = txt;
        }
        
        // Debug.Log("changed");
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
    }
}
