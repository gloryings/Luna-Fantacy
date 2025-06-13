using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaController : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    public float moveSpeed;
    private int maxHealth;      //最大生命值
    private int currentHealth;  //Luna的当前生命值
    private Animator animator;
    private Vector2 lookDirection = new Vector2(0, -1);     //luna朝向，初始朝下
    private float moveScale;
    private Vector2 move = new Vector2(0, 0);               //luna移动方向


    // 设置对外属性
    public int MaxHealth {  get { return maxHealth; } }
    public int Health {  get { return currentHealth; } }

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 30;
        rigidbody2D = GetComponent<Rigidbody2D>();
        maxHealth = 5;
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();  //找到子对象们中的第一个Animator并返回
        moveSpeed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //float horizontal = Input.GetAxis("Horizontal");
        //float vetrical = Input.GetAxis("Vertical");
        ////Debug.Log(horizontal);
        //Vector2 position = transform.position;
        ////Time.deltaTime代表上一帧耗时，使用Time.deltaTime可以将每帧移动定量逻辑改为每秒移动定量，因为函数调用是一帧执行一次
        //position.x = position.x + 60f * Time.deltaTime * horizontal;
        //position.y = position.y + 60f * Time.deltaTime * vetrical;
        ////transform.position = position;    碰撞器会产生抖动
        //rigidbody2D.MovePosition(position);
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        if (!GameManager.Instance.canControlLuna)
        {
            return;
        }

        // 监听玩家输入
        float horizontal = Input.GetAxisRaw("Horizontal"); // 只返回-1,0,+1，响应更快
        float vertical = Input.GetAxisRaw("Vertical");
        move = new Vector2(horizontal, vertical);

        //用户存在键盘输入，播放对应动画
        if (!(Mathf.Approximately(move.x, 0) && Mathf.Approximately(move.y, 0)))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize(); //归一化
            //animator.SetFloat("MoveValue", 1);
        }
        animator.SetFloat("LookX", lookDirection.x);
        animator.SetFloat("LookY", lookDirection.y);
        moveScale = move.magnitude;
        // 判断是走路还是跑步
        if (move.magnitude > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveScale = 2;
                moveSpeed = 3;
            }
            else
            {
                moveScale = 1;
                moveSpeed = 1.5f;
            }
        }

        animator.SetFloat("MoveValue", moveScale);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Talk();
        }

    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    private int GetCurrentLunaHP()
    {
        return currentHealth;
    }

    // 存放跟物理系统相关的代码
    void FixedUpdate()
    {
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        if (!GameManager.Instance.canControlLuna)
        {
            return;
        }
        Vector2 position = rigidbody2D.position; // 更准确地获取物理位置
        position += moveSpeed * move * Time.fixedDeltaTime;  // 直接使用向量乘法
        rigidbody2D.MovePosition(position);
    }

    public void Climb(bool flag)
    {
        animator.SetBool("Climb", flag);
    }

    public void Jump(bool flag)
    {
        animator.SetBool("Jump", flag);
        rigidbody2D.simulated = !flag;
    }

    /// <summary>
    ///  用于触发空格事件
    /// </summary>
    public void Talk()
    {
        Collider2D collider = Physics2D.OverlapCircle(rigidbody2D.position,
            0.5f, LayerMask.GetMask("NPC"));    //获取Luna附近是否有NPC图层的GameObject
        if(collider != null)
        {
            //Debug.Log(GameManager.Instance.dialogInfoIndex);
            if (collider.name == "Nala")
            {
                GameManager.Instance.canControlLuna = false;
                collider.GetComponent<NPCDialog>().DisplayDialog();     //调用其播放对话函数
            }
            else if(collider.name == "Dog" && !GameManager.Instance.hasPetTheDog && GameManager.Instance.dialogInfoIndex == 2)
            {
                PetTheDog();
                GameManager.Instance.canControlLuna = false;
                collider.GetComponent<Dog>().BeHappy();
            }
        }
    }

    private void PetTheDog()
    {
        animator.CrossFade("PetTheDog", 0);     //播放摸狗的动画
        transform.position = new Vector3(-1.74f, -8.3f, 0);    //移动角色到特定位置
    }
}
