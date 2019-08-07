using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{

    
    //移動速度
    const float normalSpeed = 0.1f;
    const float slowSpeed = 0.02f;
    
    //ジャンプ力
    const float jumpForce = 300f;

    //回復アイテム
    const int pillsHP = 40;
    const int potionErosion = 20;


    public int HP;
    public int HPMax = 100;

    public int erosion;
    public int erosionMax = 100;

    bool invisibleFlag;
    float blinkTimer;
    float blinkInterval = 0.1f;
    float invisibleTimer;
    float invisibleInterval = 0.5f;
    Collider2D attackcollider;
    float Timer = 0.0f;
    public float walkSpeed;

    bool isJumping = false;


    Vector3 playerScale;
    float playDir = 0;

    //アイテムを持っているか
    bool HasItem;
    GameObject item;

    public Animator animator;
    Rigidbody2D rb2d;

    private AudioSource audioSource;
    public AudioClip walkSE;
    public AudioClip revivalSE;
    public AudioClip itemSE;
    public AudioClip damagedSE;
    public AudioClip gameOverSE;
    

    bool isAttacking;

    string state;

    public bool playerDied = false;



    // Use this for initialization
    void Start()
    {
        HP = HPMax;
        erosion = 10;

        invisibleFlag = false;
        attackcollider = GameObject.Find("attackCollider").GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        HasItem = false;
        playerScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

        animator = gameObject.GetComponent<Animator>();
        animator.speed = 0;

        isAttacking = false;
        state = "right";
        walkSpeed = normalSpeed;

        audioSource = gameObject.GetComponent<AudioSource>();
        

    }

    // Update is called once per frame
    void Update()
    {
        playDir = Input.GetAxis("Horizontal");
        if (isAttacking == false)
        {
            //移動
            move(playDir);

            //↑キーでジャンプ
            if (Input.GetKeyDown("up"))
                jump();

            // 攻撃
            if (Input.GetKeyDown(KeyCode.Z) && !attackcollider.enabled)
                attack(playDir);
        }


        // 攻撃クールタイム
        if (attackcollider.enabled)
        {
            Timer += Time.deltaTime;
            if (Timer > 0.5f)
            {
                Timer = 0;
                attackcollider.enabled = false;
                isAttacking = false;
                animator.speed = 0;
                animator.SetBool("attackBool", false); 
            }
        }

        // アイテムを投げる
        if (Input.GetKeyDown(KeyCode.X) && HasItem)
        {
            item.transform.parent = null;

            Rigidbody2D rb2d = item.GetComponent<Rigidbody2D>();

            Vector3 force;
            Vector3 itemPosition = item.transform.position;
            Vector3 offset = new Vector3(-2, 0, 0);

            if (state == "right")
            {
                force = new Vector3(400, 60, 0);
            }
            else
            {
                force = new Vector3(-400, 60, 0);
                offset.x *= -1;
            }
            item.active = true;
            rb2d.AddForceAtPosition(force, itemPosition + offset);
            StartCoroutine(changeHasItem());

            ItemManager itemManagerScript;
            itemManagerScript = item.GetComponent<ItemManager>();
            itemManagerScript.changeState();

        }


        //無敵状態
        if (invisibleFlag && !playerDied)
        {
            Renderer playerRenderer = GetComponent<Renderer>();

            blinkTimer += Time.deltaTime;
            if (blinkTimer > blinkInterval)
            {
                // 点滅させる
                playerRenderer.enabled = !playerRenderer.enabled;
                blinkTimer = 0;
            }

            invisibleTimer += Time.deltaTime;
            if (invisibleTimer > invisibleInterval && !playerDied)
            {
                // 無敵時間終了
                invisibleTimer = 0;
                invisibleFlag = false;

                playerRenderer.enabled = true;
            }
        }

        //侵食率100以上で死亡
        if ((erosion >= 100 || transform.position.y < -10) && !playerDied)
        {
            gameOver();
        }
        else if (HP <= 0 && !playerDied) //侵食率100以上でない時，HP０になると侵食率を10上げて復活
        {
            HP = 100;
            erosion += 10;
            audioSource.PlayOneShot(revivalSE);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Item"))
        {
            isJumping = false;
            walkSpeed = normalSpeed;
        }

        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.name.Contains("Forest_deco_covers"));

        //沼地にいる場合
        if(collision.gameObject.tag == "Swamp" || collision.gameObject.name.Contains("Forest_deco_covers")){
            isJumping = false;
            walkSpeed = slowSpeed;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 敵との接触判定
        if (collision.gameObject.tag == "Enemy" && !invisibleFlag)
        {
            GameObject enemy = collision.gameObject;
            EnemyManager enemyManagerScript;
            enemyManagerScript = enemy.GetComponent<EnemyManager>();
            int damagePoint = enemyManagerScript.damagePoint;
            //int erosionPoint = enemyManagerScript.erosionPoint;
            HP -= damagePoint;
            audioSource.PlayOneShot(damagedSE);
            

            invisibleFlag = true;
            knockBack(enemy);
        }

        //沼地にいる場合
        if(collision.gameObject.tag == "Swamp" || collision.gameObject.name.Contains("Forest_deco_covers")){
            walkSpeed = slowSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Goal"){
            Debug.Log("goal");
            SceneManager.LoadScene("stage1-1");
        }
    }

    void OnTriggerStay2D(Collider2D other){
        //　アイテム接触判定
        if (other.gameObject.tag == "Item" && !invisibleFlag)
        {
            //　アイテムを持つ
            if (Input.GetKeyDown(KeyCode.X) && !HasItem)
            {
                item = other.gameObject;
                item.active = false;
                other.isTrigger = false;
                
                if(item.name == "pills"){
                    HP += pillsHP;
                    audioSource.PlayOneShot(itemSE);
                }else if(item.name == "potion_1"){
                    erosion -= potionErosion;
                    audioSource.PlayOneShot(itemSE);
                }else{
                //アイテムを動かせるようにする
                Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
                itemRb.bodyType = RigidbodyType2D.Dynamic;

                Vector3 offset;
                if(state == "right")
                    offset = new Vector3(0.7f, 0.5f, 0);
                else
                    offset = new Vector3(-0.7f, 0.5f, 0);
                item.transform.position = transform.position + offset;
                item.transform.parent = transform;
                StartCoroutine(changeHasItem());
                }
            }
        }
    }

    private void knockBack(GameObject enemy)
    {
        // 体当たりしてきた敵とプレイヤーの座標からノックバックする方向を取得する
        Vector3 knockBackDirection = (enemy.transform.position - transform.position).normalized;

        // ノックバックの方向を逆転させる
        knockBackDirection.x *= -1;
        knockBackDirection.y = 1;

        // ノックバックさせる
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(knockBackDirection * 100);
    }

    private void attack(float dir)
    {

        attackcollider.enabled = !attackcollider.enabled;
        animator.speed = 1.0f;
        animator.SetBool("attackBool", true);
        isAttacking = true;
    }

    private void move(float direction)
    {
        //プレイヤー方向の変更
        if (direction > 0 && !isAttacking)
        {
            transform.localScale = playerScale;
            animator.speed = direction;
            animator.SetBool("rightWalkBool", true);
            animator.SetBool("leftWalkBool", false);
            state = "right";
        }
        else if (direction < 0 && !isAttacking)
        {
            transform.localScale = new Vector3(-playerScale.x, playerScale.y, playerScale.z);
            animator.speed = -direction;
            animator.SetBool("rightWalkBool", false);
            animator.SetBool("leftWalkBool", true);
            state = "left";
  
        }
        //プレイヤーの移動
        Vector2 speed = new Vector2(walkSpeed*direction, 0);
        transform.Translate(speed);
    }

    //x=2.5fをギリギリ飛び越せる
    private void jump()
    {
        if (isJumping == false)
        {
            rb2d.AddForce(new Vector2(0, jumpForce));
            isJumping = true;
        }
    }


    IEnumerator changeHasItem()
    {
        yield return new WaitForSeconds(0.1f);
        HasItem = !HasItem;
        yield break;
    }

    public void gameOver(){
        playerDied = true;
        HP = 0;
        erosion = erosionMax;
        audioSource.PlayOneShot(gameOverSE);
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(SceneReload());
        
    }

      IEnumerator SceneReload()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
        yield break;
    }








}
