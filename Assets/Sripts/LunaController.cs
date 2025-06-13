using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaController : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    public float moveSpeed;
    private int maxHealth;      //�������ֵ
    private int currentHealth;  //Luna�ĵ�ǰ����ֵ
    private Animator animator;
    private Vector2 lookDirection = new Vector2(0, -1);     //luna���򣬳�ʼ����
    private float moveScale;
    private Vector2 move = new Vector2(0, 0);               //luna�ƶ�����


    // ���ö�������
    public int MaxHealth {  get { return maxHealth; } }
    public int Health {  get { return currentHealth; } }

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 30;
        rigidbody2D = GetComponent<Rigidbody2D>();
        maxHealth = 5;
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();  //�ҵ��Ӷ������еĵ�һ��Animator������
        moveSpeed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //float horizontal = Input.GetAxis("Horizontal");
        //float vetrical = Input.GetAxis("Vertical");
        ////Debug.Log(horizontal);
        //Vector2 position = transform.position;
        ////Time.deltaTime������һ֡��ʱ��ʹ��Time.deltaTime���Խ�ÿ֡�ƶ������߼���Ϊÿ���ƶ���������Ϊ����������һִ֡��һ��
        //position.x = position.x + 60f * Time.deltaTime * horizontal;
        //position.y = position.y + 60f * Time.deltaTime * vetrical;
        ////transform.position = position;    ��ײ�����������
        //rigidbody2D.MovePosition(position);
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        if (!GameManager.Instance.canControlLuna)
        {
            return;
        }

        // �����������
        float horizontal = Input.GetAxisRaw("Horizontal"); // ֻ����-1,0,+1����Ӧ����
        float vertical = Input.GetAxisRaw("Vertical");
        move = new Vector2(horizontal, vertical);

        //�û����ڼ������룬���Ŷ�Ӧ����
        if (!(Mathf.Approximately(move.x, 0) && Mathf.Approximately(move.y, 0)))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize(); //��һ��
            //animator.SetFloat("MoveValue", 1);
        }
        animator.SetFloat("LookX", lookDirection.x);
        animator.SetFloat("LookY", lookDirection.y);
        moveScale = move.magnitude;
        // �ж�����·�����ܲ�
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

    // ��Ÿ�����ϵͳ��صĴ���
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
        Vector2 position = rigidbody2D.position; // ��׼ȷ�ػ�ȡ����λ��
        position += moveSpeed * move * Time.fixedDeltaTime;  // ֱ��ʹ�������˷�
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
    ///  ���ڴ����ո��¼�
    /// </summary>
    public void Talk()
    {
        Collider2D collider = Physics2D.OverlapCircle(rigidbody2D.position,
            0.5f, LayerMask.GetMask("NPC"));    //��ȡLuna�����Ƿ���NPCͼ���GameObject
        if(collider != null)
        {
            //Debug.Log(GameManager.Instance.dialogInfoIndex);
            if (collider.name == "Nala")
            {
                GameManager.Instance.canControlLuna = false;
                collider.GetComponent<NPCDialog>().DisplayDialog();     //�����䲥�ŶԻ�����
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
        animator.CrossFade("PetTheDog", 0);     //���������Ķ���
        transform.position = new Vector3(-1.74f, -8.3f, 0);    //�ƶ���ɫ���ض�λ��
    }
}
