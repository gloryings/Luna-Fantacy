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
        StartCoroutine(PerformAttackLogic());   //启动协程
    }

    /// <summary>
    /// 定义攻击逻辑和动画，使用协程实现异步控制
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformAttackLogic()
    {
        // 隐藏
        UIManager.Instance.ShowOrHideBattlePanel(false);
        // luna前进
        lunaAnimator.SetBool("MoveState", true);
        lunaAnimator.SetFloat("MoveValue", -1);
        // 注意DOLocalMove函数是异步非阻塞的，因此需要后续yield renturn进行阻塞
        lunaTrans.DOLocalMove(monsterInitPos + new Vector3(1.5f, 0, 0), 0.5f).OnComplete
            (

                () =>
                {
                    // luna攻击
                    lunaAnimator.SetBool("MoveState", false);
                    lunaAnimator.SetFloat("MoveValue", 0);
                    lunaAnimator.CrossFade("Attack", 0);    //使用CrossFade实现动画播放的平滑过渡
                    monsterSR.DOFade(0.3f, 0.2f).OnComplete(() => { JudgeMonsterHP(-20); });     //实现怪物被攻击后淡入淡出效果
                }
            );
        // 等待0.5+0.667s后luna返回
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
                    // luna被攻击
                    lunaAnimator.CrossFade("Hit", 0);    //使用CrossFade实现动画播放的平滑过渡
                    lunaSR.DOFade(0.3f, 0.2f).OnComplete(() => { lunaSR.DOFade(1, 0.2f); });     //实现luna被攻击后淡入淡出效果
                    JudgePlayerHP(-20);
                }
            );
        yield return new WaitForSeconds(0.4f);
        // 怪物返回并显示行动ui
        monsterTrans.DOLocalMove(monsterInitPos, 0.5f).OnComplete(() =>
        {
            UIManager.Instance.ShowOrHideBattlePanel(true);
        });
    }

    /// <summary>
    /// luna防御
    /// </summary>
    public void LunaDefend()
    {
        StartCoroutine(PerformDefendLogic());   //启动协程
    }

    /// <summary>
    /// luna防御逻辑
    /// </summary>
    IEnumerator PerformDefendLogic()
    {
        // 隐藏
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaAnimator.SetBool("Defend", true);
        // 怪物攻击
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
        // 怪物返回并显示行动ui
        monsterTrans.DOLocalMove(monsterInitPos, 0.5f).OnComplete(() =>
        {
            UIManager.Instance.ShowOrHideBattlePanel(true);
            lunaAnimator.SetBool("Defend", false);
        });
    }

    /// <summary>
    /// 使用技能
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
    /// 技能逻辑
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformSkillLogic()
    {
        // 播放技能动画
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaAnimator.CrossFade("Skill", 0);
        GameManager.Instance.AddOrDecreaseMP(-30);
        yield return new WaitForSeconds(0.35f);
        // 生成技能特效
        GameObject go = Instantiate(skillEffectGO, monsterTrans);
        go.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(0.4f);
        monsterSR.DOFade(0.3f, 0.2f).OnComplete(() => { JudgeMonsterHP(-40); });     //实现怪物被攻击后淡入淡出效果
        yield return new WaitForSeconds(0.5f);
        // 怪物攻击
        StartCoroutine(MonsterAttack());

    }

    /// <summary>
    /// 回血
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
    /// 回血逻辑
    /// </summary>
    /// <returns></returns>
    IEnumerator PerformRecoverHPLogic()
    {
        // 播放回血动画
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaAnimator.CrossFade("RecoverHP", 0);
        GameManager.Instance.AddOrDecreaseMP(-50);
        yield return new WaitForSeconds(0.1f);
        GameObject go = Instantiate(healEffectGO, lunaTrans);
        go.transform.localPosition = Vector3.zero;  //将起始坐标设置在luna处
        GameManager.Instance.AddOrDecreaseHP(40);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MonsterAttack());

    }

    /// <summary>
    /// 判断怪物扣血逻辑
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
    /// 判断luna扣血逻辑
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
    /// luna逃跑
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
