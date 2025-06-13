using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image hpMaskImage;
    public Image mpMaskImage;
    private float originalSize;     //血条和蓝条原始宽度
    public GameObject battlePanelGo;

    public GameObject TalkPanelGo;
    public Image characterImage;
    public Sprite[] characterSprites;
    public Text nameText;
    public Text contentText;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        originalSize = hpMaskImage.rectTransform.rect.width;
        setHPValue(1);
        setMPValue(1);
    }

    /// <summary>
    /// 血条UI显示
    /// 使用mask组件限制子物体矩形血条的显示区域，只显示在mask区域内的部分，其他部分被裁剪掉
    /// </summary>
    /// <param name="fillPercent">填充百分比</param>
    public void setHPValue(float fillPercent)
    {
        hpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }

    /// <summary>
    /// 蓝条UI显示
    /// </summary>
    /// <param name="fillPercent">填充百分比</param>
    public void setMPValue(float fillPercent)
    {
        mpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }
    /// <summary>
    /// 显示对话内容（包含人物的切换，名字的更换，对话内容的更换）
    /// </summary>
    /// <param name="content">说话内容</param>
    /// <param name="name">说话人名字</param>
    public void ShowDialog(string content = null, string name = null)
    {
        // 内容为空，关闭对话UI
        if (content == null)
        {
            TalkPanelGo.SetActive(false);
        }
        else
        {
            TalkPanelGo.SetActive(true);
            // 根据说话人切换人物UI图片
            if(name != null)
            {
                if(name == "Luna")
                {
                    characterImage.sprite = characterSprites[0];
                }
                else
                {
                    characterImage.sprite = characterSprites[1];
                }
                characterImage.SetNativeSize(); //将图片恢复为本身大小
            }
            // 显示人名和对话内容
            contentText.text = content;
            nameText.text = name;
        }
    }

    /// <summary>
    /// 是否启动战斗场景
    /// </summary>
    /// <param name="show"></param>
    public void ShowOrHideBattlePanel(bool show)
    {
        battlePanelGo.SetActive(show);
    }
}
