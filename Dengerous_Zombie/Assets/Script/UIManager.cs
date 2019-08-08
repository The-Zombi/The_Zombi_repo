using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Image HPgage;
    public Image erosionGage;
    public GameObject[] gameOverImage;
    PlayerManager playerManager;


	void Start () {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        HPgage = GameObject.Find("HPgage").GetComponent<Image>();
        erosionGage = GameObject.Find("Erosiongage").GetComponent<Image>();
    }
	

	void Update () {
        int HPvalue = playerManager.HP;
        int HPMax = playerManager.HPMax;
        int erosionValue = playerManager.erosion;
        int erosionMax = playerManager.erosionMax;
        HPgage.fillAmount = (float)HPvalue/HPMax;
        erosionGage.fillAmount = (float)erosionValue / erosionMax;

	}

    public void gameOver(){
        GameObject.Find("gameOverImage").GetComponent<Image>().enabled = true;
    }
}
