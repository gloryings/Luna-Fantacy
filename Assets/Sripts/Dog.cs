using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private Animator animator;
    public GameObject starEffect;       //星星特效
    public AudioClip petClip;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void BeHappy()
    {
        animator.CrossFade("Comfoted", 0);          //播放狗狗动画
        GameManager.Instance.hasPetTheDog = true;
        GameManager.Instance.dialogInfoIndex++;     //对话组id++
        Debug.Log(GameManager.Instance.dialogInfoIndex);
        Destroy(starEffect);
        Invoke("CanControlLuna", 1.75f);            //播放完动画才允许控制luna
    }

    private void CanControlLuna()
    {
        GameManager.Instance.canControlLuna = true;
    }
}
