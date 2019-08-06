using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* ３回攻撃で死亡　３秒後復活*/

public class EnemyManager : MonoBehaviour {

    public int damagePoint = 20;
    public int erosionPoint = 0;
    GameObject player;
    int HP = 150;
    string enemyDir;
    Vector3 enemyScale;

    bool invisibleFlag;
    float blinkTimer;
    float blinkInterval = 0.1f;
    float invisibleTimer;
    float invisibleInterval = 0.5f;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        enemyScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        invisibleFlag = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(transform.position.x < player.transform.position.x){
            enemyDir = "right";
            transform.Translate(new Vector3(0.03f, 0, 0));
        }else if(transform.position.x > player.transform.position.x){
            enemyDir = "left";
            transform.Translate(new Vector3(-0.03f, 0, 0));
        }

        if(enemyDir == "right"){
            transform.localScale = enemyScale;
        }else if(enemyDir == "left"){
            transform.localScale = new Vector3(-enemyScale.x, enemyScale.y, enemyScale.z);
        }

        if (HP <= 0)
        {
            //HPが0以下になるとオブジェクトを非アクティブ化
            this.gameObject.SetActive(false);
            //DelayMethodを3秒後に呼び出す 
            Invoke("DelayMethod", 3f);
        }

        //無敵状態
        if (invisibleFlag)
        {
            Renderer enemyRenderer = GetComponent<Renderer>();

            blinkTimer += Time.deltaTime;
            if (blinkTimer > blinkInterval)
            {
                // 点滅させる
                enemyRenderer.enabled = !enemyRenderer.enabled;
                blinkTimer = 0;
            }

            invisibleTimer += Time.deltaTime;
            if (invisibleTimer > invisibleInterval)
            {
                // 無敵時間終了
                invisibleTimer = 0;
                invisibleFlag = false;

                enemyRenderer.enabled = true;
            }
        }
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (other.gameObject.name == "attackCollider" )
        {
            knockBack(other.gameObject);
            damaged(50);

        }
        
	}
	
    public void knockBack(GameObject enemy)
    {
        // 体当たりしてきた敵とプレイヤーの座標からノックバックする方向を取得する
        Vector3 knockBackDirection = (transform.position - enemy.transform.position).normalized;

        // ノックバックの方向
        knockBackDirection.x *= 1;
        knockBackDirection.y = 1;

        // ノックバックさせる
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(knockBackDirection * 300);
        
    }

    public void damaged(int damage){
        HP -= damage;
        invisibleFlag = true;
    }

    void DelayMethod()
    {
        //オブジェクトをアクティブ化し復活させる
        HP = 150;
        this.gameObject.SetActive(true);
    }
}

