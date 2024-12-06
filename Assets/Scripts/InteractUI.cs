using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractUI : MonoBehaviour
{
    
    public GameObject _targetName;
    TextMeshProUGUI targetName;
    public GameObject _targetIntro;
    TextMeshProUGUI targetIntro;
    // Start is called before the first frame update
    void Start()
    {
        targetName = _targetName.GetComponent<TextMeshProUGUI>();
        targetIntro = _targetIntro.GetComponent<TextMeshProUGUI>();

        targetIntro.text = "";
        targetName.text = "";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey("escape")){
            Debug.Log("closing");
            PlayerStats.interacting = false;
            PlayerStats.stopInteracting = true;
            targetIntro.text = "";
            targetName.text = "";
            return;
        }

        // Update frame
        targetIntro.text = PlayerStats.target.chatIntro;
        targetName.text = PlayerStats.target.entityName;
        
    }
}
