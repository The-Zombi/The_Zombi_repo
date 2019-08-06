using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {


    int damagePoint = 20;   //アイテムでの攻撃ダメージ量
    string state;   //アイテムの状態

	void Start () {
        state = "normal";   //通常状態
	}
	
	// Update is called once per frame
    void Update () {

	}

    public void changeState(){
        if (state == "normal")
        {
            state = "attack";
        }
        else if (state == "attack")
        {
            state = "normal";
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy" && state == "attack"){
            //敵にダメージを与える
            EnemyManager enemyManagerScript;
            enemyManagerScript = collision.gameObject.GetComponent<EnemyManager>();
            enemyManagerScript.damaged(damagePoint);
            enemyManagerScript.knockBack(gameObject);
        }

        if (collision.gameObject.tag == "Ground")
            state = "normal";
    }
}
