using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public GameObject player;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 5, -10);//プレイヤーに追随するカメラ
        if (transform.position.x < -13)//見切れぬように適宜値変更
        {
            transform.position = new Vector3(-13, 5, -10);
        }
        if (transform.position.x > 13)
        {
            transform.position = new Vector3(13, 5, -10);
        }
    }
}
