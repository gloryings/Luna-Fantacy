using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //轴向控制
    public bool vertical;
    public float speed = 5;
    //方向控制
    private int direction = 1;
    //方向改变时间间隔
    public float changeTime = 3;
    //计时器
    private float timer;
    //刚体组件引用，为了使用刚体进行移动
    private Rigidbody2D rigidbody2d;
    //动画控制器组件引用，为了播放动画
    private Animator animator;


    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        // 实现转向
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        Vector3 pos = rigidbody2d.position;

        //垂直移动
        if (vertical)
        {
            animator.SetFloat("LookX", 0);
            animator.SetFloat("LookY", direction);
            pos.y += speed * direction * Time.fixedDeltaTime;
        }
        else 
        {
            animator.SetFloat("LookX", direction);
            animator.SetFloat("LookY", 0);
            pos.x += speed * direction * Time.fixedDeltaTime;
        }
        rigidbody2d.MovePosition(pos);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Luna"))
        {
            GameManager.Instance.EnterOrExitBattle(true);
            GameManager.Instance.SetMonster(gameObject);
            Debug.Log("[hotfix] Luna enter the battle");
        }
    }
}
