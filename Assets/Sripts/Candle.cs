using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    public GameObject effectGo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.candleNum++;
        Instantiate(effectGo, transform.position, Quaternion.identity); //������Ч��Ĭ�ϽǶ�
        if(GameManager.Instance.candleNum >= 5)
        {
            GameManager.Instance.SetContentIndex();
        }
        // gameObjectָ���ǽű����صĶ���
        Destroy(gameObject);
    }
}
