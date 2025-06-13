using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JumpArea : MonoBehaviour
{
    //public Transform jumpPointA;
    //public Transform jumpPointB;
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Luna"))
    //    {
    //        LunaController lunaController = collision.transform.GetComponent<LunaController>();
    //        lunaController.Jump(true);
    //        float disA = Vector3.Distance(lunaController.transform.position, jumpPointA.position);
    //        float disB = Vector3.Distance(lunaController.transform.position, jumpPointB.position);
    //        Transform targetTrans;
    //        if(disA > disB)
    //        {
    //            //��A�ľ���Զ���������Ծ������B->A
    //            targetTrans = jumpPointA;
    //        }
    //        else
    //        {
    //            targetTrans = jumpPointB;
    //        }
    //        // ʹ�� DOTween ��� �ý�ɫ�� 0.5 ��������Ŀ��λ��,���ڶ�����ɺ�ִ��endjump�ص�
    //        lunaController.transform.DOMove(targetTrans.position, 0.5f).OnComplete(() => { EndJump(lunaController); });  //���ʵ����Ծ����Ч��
    //        // ����������y�����ƶ���ʵ��������Ծ����
    //        Transform lunaLocalTrans = lunaController.transform.GetChild(0);
    //        Sequence sequence = DOTween.Sequence();
    //        sequence.Append(lunaLocalTrans.DOLocalMoveY(1.5f, 0.25f));
    //        sequence.Append(lunaLocalTrans.DOLocalMoveY(0.61f, 0.25f));
    //        sequence.Play();

    //    }
    //}

    //private void EndJump(LunaController lunaController)
    //{
    //    lunaController.Jump(false);
    //}

    [SerializeField] private Transform jumpPointA;
    [SerializeField] private Transform jumpPointB;
    [SerializeField] private AnimationCurve jumpCurve;  // ����ģ����Ծ����

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Luna"))
        {
            LunaController luna = collision.GetComponent<LunaController>();
            luna.Jump(true);  // ��ʼ��Ծ

            float disA = Vector3.Distance(luna.transform.position, jumpPointA.position);
            float disB = Vector3.Distance(luna.transform.position, jumpPointB.position);
            Transform target = disA > disB ? jumpPointA : jumpPointB;

            StartCoroutine(JumpToTarget(luna, target.position));
        }
    }

    IEnumerator JumpToTarget(LunaController luna, Vector3 targetPos)
    {
        Transform trans = luna.transform;
        Vector3 startPos = trans.position;
        float duration = 0.5f;
        float timer = 0f;

        // ��Ծǰ��������ģ�⣨����������ţ�
        Rigidbody2D rb = trans.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // λ�ò�ֵ
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, t);

            // ��ֱ����������Ծ�߶ȣ����߿��ƣ�
            float heightOffset = jumpCurve.Evaluate(t);  // �����������м�����ߵ�
            Vector3 jumpPos = horizontalPos + new Vector3(0, heightOffset, 0);

            trans.position = jumpPos;
            yield return null;  //�ȴ�һ֡��ÿ֡����һ��
        }

        trans.position = targetPos; // ȷ�����ն���

        // ��Ծ��ָ�����
        if (rb) rb.simulated = true;

        luna.Jump(false); // ������Ծ
    }

}
