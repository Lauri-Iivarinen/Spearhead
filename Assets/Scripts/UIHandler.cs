using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public GameObject _playerName;
    public GameObject _playerLevel;
    public GameObject _playerHp;
    public GameObject _targetName;
    public GameObject _targetHp;
    public GameObject _targetLevel;
    public GameObject targetUI;
    public GameObject interactUI;

    TextMeshProUGUI playerName;
    TextMeshProUGUI playerLevel;
    TextMeshProUGUI playerHp;
    TextMeshProUGUI targetName;
    TextMeshProUGUI targetHp;
    TextMeshProUGUI targetLevel;
    public Slider hpSlider;
    public Slider xpSlider;
    public Slider manaSlider;
    // Start is called before the first frame update
    public Color friendlyTarget;
    public Color hostileTarget;
    Image targetPanel;
    void Start()
    {
        playerName = _playerName.GetComponent<TextMeshProUGUI>();
        playerLevel = _playerLevel.GetComponent<TextMeshProUGUI>();
        playerHp = _playerHp.GetComponent<TextMeshProUGUI>();
        targetName = _targetName.GetComponent<TextMeshProUGUI>();
        targetHp = _targetHp.GetComponent<TextMeshProUGUI>();
        targetLevel = _targetLevel.GetComponent<TextMeshProUGUI>();
        //hpSlider = _playerHpSlider.GetComponent<Slider>();
        //xpSlider = _playerXpSlider.GetComponent<Slider>();
        targetUI.SetActive(false);
        targetPanel = targetUI.GetComponent<Image>();
        UpdatePlayerUI();
        interactUI.SetActive(false);
    }

    void UpdatePlayerUI()
    {
        playerName.text = "" + PlayerStats.playerName;
        playerLevel.text = "" + PlayerStats.level;
        playerHp.text = "" + PlayerStats.hp + "/" + PlayerStats.maxHp;

        hpSlider.value = PlayerStats.hp / PlayerStats.maxHp;
        xpSlider.value = PlayerStats.xp / PlayerStats.xpToNextLvl;
    }

    void UpdateTargetUI()
    {
        if (PlayerStats.target.friendly){
            targetPanel.color = friendlyTarget;
        }else{
            targetPanel.color = hostileTarget;
        }
        targetName.text = "" + PlayerStats.target.entityName;
        targetHp.text = "" + PlayerStats.target.hp + "/" + PlayerStats.target.maxHp;
        targetLevel.text = "" + PlayerStats.target.level;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerUI();
        if (PlayerStats.target){
            UpdateTargetUI();
            targetUI.SetActive(true);
        }else{
            targetUI.SetActive(false);
        }
        interactUI.SetActive(PlayerStats.interacting);
    }
}
