using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject battleGo;         //ս������Ϸ����
    // Luna����
    public int lunaHP;                  //Luna���Ѫ��
    public int lunaMP;                  //Luna�������
    public float lunaCurrentHP;         //Luna��ǰѪ��
    public float lunaCurrentMP;         //Luna��ǰ����
    // Monster����
    public int monsterCurrentHP;        //���ﵱǰѪ��
    // ������Ʒ��¼
    public int dialogInfoIndex;         //�Ի�����
    public bool canControlLuna;         //�Ƿ�������ҿ���Luna
    public bool hasPetTheDog;           //�Ƿ񴥷������¼�
    public int candleNum;               //��Ҽ����������
    public int killNum;                 //���ɱ�����������
    public GameObject monsterGo;        //���ƹ���Ⱥ�ĳ���
    public GameObject battleMonsterGo;  //��ǰս���Ĺ���
    public NPCDialog npc;               //�洢npc�Ի������������������������
    public bool enterBattle;            //�Ƿ����ս��

    private void Awake()
    {
        Instance = this;
        lunaCurrentHP = 100;
        lunaCurrentMP = 100;
        lunaHP = 100;
        lunaMP = 100;
        monsterCurrentHP = 50;
    }

    private void Update()
    {
        if (!enterBattle)
        {
            if(lunaCurrentMP <= 100)
            {
                AddOrDecreaseHP(Time.deltaTime);    //ÿ֡������Ѫ��
            }
            if(lunaCurrentMP <= 100)
            {
                AddOrDecreaseMP(Time.deltaTime);    //ÿ֡���½�Ѫ��
            }
        }
    }

    /// <summary>
    /// �仯LunaѪ��
    /// </summary>
    /// <param name="value">�仯ֵ</param>
    public void AddOrDecreaseHP(float value)
    {
        lunaCurrentHP += value;
        if(lunaCurrentHP >= lunaHP)
        {
            lunaCurrentHP = lunaHP;
        }
        if(lunaCurrentHP <= 0)
        {
            lunaCurrentHP = 0;
        }
        UIManager.Instance.setHPValue((float)lunaCurrentHP / lunaHP);
    }

    /// <summary>
    /// �仯Luna����
    /// </summary>
    /// <param name="value">�仯ֵ</param>
    public void AddOrDecreaseMP(float value)
    {
        lunaCurrentMP += value;
        if (lunaCurrentMP >= lunaMP)
        {
            lunaCurrentMP = lunaMP;
        }
        if (lunaCurrentMP <= 0)
        {
            lunaCurrentMP = 0;
        }
        UIManager.Instance.setMPValue((float)lunaCurrentMP / lunaMP);
    }

    /// <summary>
    /// ��ʾ����
    /// </summary>
    public void ShowMonsters()
    {
        if (!monsterGo.activeSelf)
        {
            monsterGo.SetActive(true);
        }
    }

    /// <summary>
    /// ����Ѫ���ı�
    /// </summary>
    /// <param name="value">Ѫ���仯ֵ</param>
    /// <returns>���ﵱǰѪ��</returns>
    public int AddOrDecreaseMonsterHP(int value)
    {
        monsterCurrentHP += value;
        return monsterCurrentHP;
    }

    public void DestoryMonster()
    {
        Destroy(battleMonsterGo);
    }

    public void SetMonster(GameObject go)
    {
        battleMonsterGo = go;
    }

    public void EnterOrExitBattle(bool enter = true, int addKillNum = 0)
    {
        UIManager.Instance.ShowOrHideBattlePanel(enter);
        battleGo.SetActive(enter);
        if (!enter) //ս������
        {
            killNum += addKillNum;
            battleMonsterGo.transform.position += new Vector3(1, 1, 0);     //�ƶ�һ�£������luna�ص�
            if (addKillNum > 0)
            {
                DestoryMonster();
            }
            monsterCurrentHP = 50;
            // lunaս��ʧ��
            if(lunaCurrentHP <= 0)
            {
                lunaCurrentHP = 100;
                lunaCurrentMP = 0;
                //battleMonsterGo.transform.position += new Vector3(2, 2, 0);     //�ƶ�һ�£������luna�ص�
            }
        }
        enterBattle = enter;
    }


    /// <summary>
    /// luna�Ƿ����ʹ�õ�ǰ����
    /// </summary>
    /// <param name="value">������ҪMPֵ</param>
    /// <returns></returns>
    public bool CanUsePlayerMP(int value)
    {
        return lunaCurrentMP >= value;
    }


    /// <summary>
    /// ���������������
    /// </summary>
    public void SetContentIndex()
    {
        npc.SetContentIndex();
    }
}
