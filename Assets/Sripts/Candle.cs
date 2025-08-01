using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    public GameObject effectGo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[hotfix] Luna pick up one candle");
        GameManager.Instance.candleNum++;
        Debug.Log("[hotfix] Luna has candleNum = " + GameManager.Instance.candleNum);
        Instantiate(effectGo, transform.position, Quaternion.identity); //生成特效，默认角度
        if(GameManager.Instance.candleNum >= 5)
        {
            GameManager.Instance.SetContentIndex();
            Debug.Log("[hotfix] Luna finish the pick candle task");
        }
        // gameObject指的是脚本挂载的对象
        Destroy(gameObject);
    }
}
