using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    private Animator animator;
    public GameObject starEffect;       //������Ч
    public AudioClip petClip;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void BeHappy()
    {
        animator.CrossFade("Comfoted", 0);          //���Ź�������
        GameManager.Instance.hasPetTheDog = true;
        GameManager.Instance.dialogInfoIndex++;     //�Ի���id++
        Debug.Log(GameManager.Instance.dialogInfoIndex);
        Destroy(starEffect);
        Invoke("CanControlLuna", 1.75f);            //�����궯�����������luna
    }

    private void CanControlLuna()
    {
        GameManager.Instance.canControlLuna = true;
    }
}
