using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public GameObject player;
    PlayerManager playerManager;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");
        playerManager = player.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerManager.playerDied)
            enabled = false;
            
        transform.position = new Vector3(player.transform.position.x, 0, -10);//プレイヤーに追随するカメラ
        if (transform.position.x < -13)//見切れぬように適宜値変更
        {
            transform.position = new Vector3(-13, 5, -10);
        }
        if (transform.position.x > 1000)
        {
            transform.position = new Vector3(13, 5, -10);
        }
    }
}
