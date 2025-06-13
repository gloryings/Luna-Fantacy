using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{

    public GameObject effectGo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ³ÔÑªÆ¿»ØÑª
        if (GameManager.Instance.lunaCurrentHP < GameManager.Instance.lunaHP)
        {
            GameManager.Instance.AddOrDecreaseHP(40);
            Instantiate(effectGo, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
