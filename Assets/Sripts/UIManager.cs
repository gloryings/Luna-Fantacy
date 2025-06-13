using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image hpMaskImage;
    public Image mpMaskImage;
    private float originalSize;     //Ѫ��������ԭʼ���
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
    /// Ѫ��UI��ʾ
    /// ʹ��mask����������������Ѫ������ʾ����ֻ��ʾ��mask�����ڵĲ��֣��������ֱ��ü���
    /// </summary>
    /// <param name="fillPercent">���ٷֱ�</param>
    public void setHPValue(float fillPercent)
    {
        hpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }

    /// <summary>
    /// ����UI��ʾ
    /// </summary>
    /// <param name="fillPercent">���ٷֱ�</param>
    public void setMPValue(float fillPercent)
    {
        mpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }
    /// <summary>
    /// ��ʾ�Ի����ݣ�����������л������ֵĸ������Ի����ݵĸ�����
    /// </summary>
    /// <param name="content">˵������</param>
    /// <param name="name">˵��������</param>
    public void ShowDialog(string content = null, string name = null)
    {
        // ����Ϊ�գ��رնԻ�UI
        if (content == null)
        {
            TalkPanelGo.SetActive(false);
        }
        else
        {
            TalkPanelGo.SetActive(true);
            // ����˵�����л�����UIͼƬ
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
                characterImage.SetNativeSize(); //��ͼƬ�ָ�Ϊ�����С
            }
            // ��ʾ�����ͶԻ�����
            contentText.text = content;
            nameText.text = name;
        }
    }

    /// <summary>
    /// �Ƿ�����ս������
    /// </summary>
    /// <param name="show"></param>
    public void ShowOrHideBattlePanel(bool show)
    {
        battlePanelGo.SetActive(show);
    }
}
