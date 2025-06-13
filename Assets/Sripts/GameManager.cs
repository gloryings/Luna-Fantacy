using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject battleGo;         //战场景游戏物体
    // Luna属性
    public int lunaHP;                  //Luna最大血量
    public int lunaMP;                  //Luna最大蓝量
    public float lunaCurrentHP;         //Luna当前血量
    public float lunaCurrentMP;         //Luna当前蓝量
    // Monster属性
    public int monsterCurrentHP;        //怪物当前血量
    // 任务物品记录
    public int dialogInfoIndex;         //对话索引
    public bool canControlLuna;         //是否允许玩家控制Luna
    public bool hasPetTheDog;           //是否触发摸狗事件
    public int candleNum;               //玩家捡到蜡烛的数量
    public int killNum;                 //玩家杀死怪物的数量
    public GameObject monsterGo;        //控制怪物群的出现
    public GameObject battleMonsterGo;  //当前战斗的怪物
    public NPCDialog npc;               //存储npc对话（用来任务完成设置索引）
    public bool enterBattle;            //是否进入战斗

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
                AddOrDecreaseHP(Time.deltaTime);    //每帧逐渐上涨血量
            }
            if(lunaCurrentMP <= 100)
            {
                AddOrDecreaseMP(Time.deltaTime);    //每帧逐渐下降血量
            }
        }
    }

    /// <summary>
    /// 变化Luna血量
    /// </summary>
    /// <param name="value">变化值</param>
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
    /// 变化Luna蓝量
    /// </summary>
    /// <param name="value">变化值</param>
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
    /// 显示怪物
    /// </summary>
    public void ShowMonsters()
    {
        if (!monsterGo.activeSelf)
        {
            monsterGo.SetActive(true);
        }
    }

    /// <summary>
    /// 怪物血量改变
    /// </summary>
    /// <param name="value">血量变化值</param>
    /// <returns>怪物当前血量</returns>
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
        if (!enter) //战斗结束
        {
            killNum += addKillNum;
            battleMonsterGo.transform.position += new Vector3(1, 1, 0);     //移动一下，避免和luna重叠
            if (addKillNum > 0)
            {
                DestoryMonster();
            }
            monsterCurrentHP = 50;
            // luna战斗失败
            if(lunaCurrentHP <= 0)
            {
                lunaCurrentHP = 100;
                lunaCurrentMP = 0;
                //battleMonsterGo.transform.position += new Vector3(2, 2, 0);     //移动一下，避免和luna重叠
            }
        }
        enterBattle = enter;
    }


    /// <summary>
    /// luna是否可以使用当前技能
    /// </summary>
    /// <param name="value">技能需要MP值</param>
    /// <returns></returns>
    public bool CanUsePlayerMP(int value)
    {
        return lunaCurrentMP >= value;
    }


    /// <summary>
    /// 任务完成设置索引
    /// </summary>
    public void SetContentIndex()
    {
        npc.SetContentIndex();
    }
}
