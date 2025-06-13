using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public Animator lunaAnimator;
    public Transform lunaTrans;
    public Transform monsterTrans;
    private Vector3 lunaInitPos;
    private Vector3 monsterInitPos;
    public SpriteRenderer monsterSR;
    public SpriteRenderer lunaSR;
    public GameObject skillEffectGO;
    public GameObject healEffectGO;
    public Button button1;

    // Start is called before the first frame update
    void Awake()
    {
        monsterInitPos = monsterTrans.localPosition;
        lunaInitPos = lunaTrans.localPosition;
    }

    private void OnEnable()
    {
        monsterSR.DOFade(1, 0.01f);
        lunaSR.DOFade(1, 0.01f);
        lunaTrans.localPosition = lunaInitPos;
        monsterTrans.localPosition = monsterInitPos;
        button1.onClick.AddListener(LunaAttack);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LunaAttack() 
    { 
        StartCoroutine(PerformAttackLogic());   //����Э��
    }

    /// <summary>
    /// ���幥���߼��Ͷ�����ʹ��Э��ʵ���첽����
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformAttackLogic()
    {
        // ����
        UIManager.Instance.ShowOrHideBattlePanel(false);
        // lunaǰ��
        lunaAnimator.SetBool("MoveState", true);
        lunaAnimator.SetFloat("MoveValue", -1);
        // ע��DOLocalMove�������첽�������ģ������Ҫ����yield renturn��������
        lunaTrans.DOLocalMove(monsterInitPos + new Vector3(1.5f, 0, 0), 0.5f).OnComplete
            (

                () =>
                {
                    // luna����
                    lunaAnimator.SetBool("MoveState", false);
                    lunaAnimator.SetFloat("MoveValue", 0);
                    lunaAnimator.CrossFade("Attack", 0);    //ʹ��CrossFadeʵ�ֶ������ŵ�ƽ������
                    monsterSR.DOFade(0.3f, 0.2f).OnComplete(() => { JudgeMonsterHP(-20); });     //ʵ�ֹ��ﱻ�������뵭��Ч��
                }
            );
        // �ȴ�0.5+0.667s��luna����
        yield return new WaitForSeconds(1.167f);
        lunaAnimator.SetBool("MoveState", true);
        lunaAnimator.SetFloat("MoveValue", 1);
        lunaTrans.DOLocalMove(lunaInitPos, 0.5f).OnComplete(() => { lunaAnimator.SetBool("MoveState", false); });
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MonsterAttack());

    }

    IEnumerator MonsterAttack()
    {
        monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        monsterTrans.DOLocalMove(lunaInitPos, 0.2f).OnComplete
            (
                () =>
                {
                    monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.2f);
                    // luna������
                    lunaAnimator.CrossFade("Hit", 0);    //ʹ��CrossFadeʵ�ֶ������ŵ�ƽ������
                    lunaSR.DOFade(0.3f, 0.2f).OnComplete(() => { lunaSR.DOFade(1, 0.2f); });     //ʵ��luna���������뵭��Ч��
                    JudgePlayerHP(-20);
                }
            );
        yield return new WaitForSeconds(0.4f);
        // ���ﷵ�ز���ʾ�ж�ui
        monsterTrans.DOLocalMove(monsterInitPos, 0.5f).OnComplete(() =>
        {
            UIManager.Instance.ShowOrHideBattlePanel(true);
        });
    }

    /// <summary>
    /// luna����
    /// </summary>
    public void LunaDefend()
    {
        StartCoroutine(PerformDefendLogic());   //����Э��
    }

    /// <summary>
    /// luna�����߼�
    /// </summary>
    IEnumerator PerformDefendLogic()
    {
        // ����
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaAnimator.SetBool("Defend", true);
        // ���﹥��
        monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        monsterTrans.DOLocalMove(lunaInitPos, 0.2f).OnComplete
            (
                () =>
                {
                    monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.2f);
                    lunaTrans.DOLocalMove(lunaInitPos + new Vector3(1, 0, 0), 0.2f).OnComplete(
                        () =>
                        {
                            lunaTrans.DOLocalMove(lunaInitPos, 0.2f);
                        });

                }
            );
        yield return new WaitForSeconds(0.4f);
        // ���ﷵ�ز���ʾ�ж�ui
        monsterTrans.DOLocalMove(monsterInitPos, 0.5f).OnComplete(() =>
        {
            UIManager.Instance.ShowOrHideBattlePanel(true);
            lunaAnimator.SetBool("Defend", false);
        });
    }

    /// <summary>
    /// ʹ�ü���
    /// </summary>
    public void LunaUseSkill()
    {
        if (!GameManager.Instance.CanUsePlayerMP(30))
        {
            return;
        }
        StartCoroutine(PerformSkillLogic());
    }

    /// <summary>
    /// �����߼�
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformSkillLogic()
    {
        // ���ż��ܶ���
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaAnimator.CrossFade("Skill", 0);
        GameManager.Instance.AddOrDecreaseMP(-30);
        yield return new WaitForSeconds(0.35f);
        // ���ɼ�����Ч
        GameObject go = Instantiate(skillEffectGO, monsterTrans);
        go.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(0.4f);
        monsterSR.DOFade(0.3f, 0.2f).OnComplete(() => { JudgeMonsterHP(-40); });     //ʵ�ֹ��ﱻ�������뵭��Ч��
        yield return new WaitForSeconds(0.5f);
        // ���﹥��
        StartCoroutine(MonsterAttack());

    }

    /// <summary>
    /// ��Ѫ
    /// </summary>
    public void LunaRecoverHP()
    {
        if (!GameManager.Instance.CanUsePlayerMP(50))
        {
            return;
        }
        StartCoroutine(PerformRecoverHPLogic());
    }

    /// <summary>
    /// ��Ѫ�߼�
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformRecoverHPLogic()
    {
        // ���Ż�Ѫ����
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaAnimator.CrossFade("RecoverHP", 0);
        GameManager.Instance.AddOrDecreaseMP(-50);
        yield return new WaitForSeconds(0.1f);
        GameObject go = Instantiate(healEffectGO, lunaTrans);
        go.transform.localPosition = Vector3.zero;  //����ʼ����������luna��
        GameManager.Instance.AddOrDecreaseHP(40);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MonsterAttack());

    }

    /// <summary>
    /// �жϹ����Ѫ�߼�
    /// </summary>
    /// <param name="value"></param>
    private void JudgeMonsterHP(int value)
    {
        if (GameManager.Instance.AddOrDecreaseMonsterHP(value) <= 0)
        {
            monsterSR.DOFade(0, 0.4f).OnComplete(() => { GameManager.Instance.EnterOrExitBattle(false, 1); });
        }
        else
        {
            monsterSR.DOFade(1, 0.2f);
        }

    }

    /// <summary>
    /// �ж�luna��Ѫ�߼�
    /// </summary>
    /// <param name="value"></param>
    private void JudgePlayerHP(int value)
    {
        GameManager.Instance.AddOrDecreaseHP(value);
        if(GameManager.Instance.lunaCurrentHP <= 0)
        {
            lunaAnimator.CrossFade("Die", 0);
            lunaSR.DOFade(0, 0.8f).OnComplete(() => { GameManager.Instance.EnterOrExitBattle(false); });
        }

    }


    /// <summary>
    /// luna����
    /// </summary>
    public void LunaEscape()
    {
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaTrans.DOLocalMove(lunaInitPos + new Vector3(5, 0, 0), 0.5f).OnComplete(

            () =>
            {
                GameManager.Instance.EnterOrExitBattle(false);
            }
        );
        lunaAnimator.SetBool("MoveState", true);
        lunaAnimator.SetFloat("MoveValue", 1);

    }
}
