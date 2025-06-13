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
    //            //离A的距离远，因此是跳跃方向是B->A
    //            targetTrans = jumpPointA;
    //        }
    //        else
    //        {
    //            targetTrans = jumpPointB;
    //        }
    //        // 使用 DOTween 插件 让角色在 0.5 秒内跳到目标位置,并在动画完成后执行endjump回调
    //        lunaController.transform.DOMove(targetTrans.position, 0.5f).OnComplete(() => { EndJump(lunaController); });  //插件实现跳跃动画效果
    //        // 让子物体有y方向移动，实现类似跳跃功能
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
    [SerializeField] private AnimationCurve jumpCurve;  // 曲线模拟跳跃弧线

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Luna"))
        {
            LunaController luna = collision.GetComponent<LunaController>();
            luna.Jump(true);  // 开始跳跃

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

        // 跳跃前禁用物理模拟（避免物理干扰）
        Rigidbody2D rb = trans.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // 位置插值
            Vector3 horizontalPos = Vector3.Lerp(startPos, targetPos, t);

            // 垂直方向增加跳跃高度（曲线控制）
            float heightOffset = jumpCurve.Evaluate(t);  // 例如曲线在中间是最高点
            Vector3 jumpPos = horizontalPos + new Vector3(0, heightOffset, 0);

            trans.position = jumpPos;
            yield return null;  //等待一帧，每帧调用一次
        }

        trans.position = targetPos; // 确保最终对齐

        // 跳跃后恢复物理
        if (rb) rb.simulated = true;

        luna.Jump(false); // 结束跳跃
    }

}
