using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class BasePanel : MonoBehaviour
{
    private UIManager uiMng;
    private BGAnim bGAnim;

    public UIManager UIMng { get => uiMng; set => uiMng = value; }
    public BGAnim BGAnim { get => bGAnim; set => bGAnim = value; }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {

    }

    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume()
    {

    }

    /// <summary>
    /// 界面不显示,退出这个界面。
    /// </summary>
    public virtual void OnExit()
    {
        gameObject.SetActive(false);
    }


    protected void PausePanelByScale()
    {
        transform.DOScale(0, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }

    protected void ResumePanelByScale()
    {
        gameObject.SetActive(true);
        transform.DOScale(1, 0.1f);
    }

    protected void OpenPanelPUNISHING(RectTransform panelBG, RectTransform content)
    {
        OpenPanelPUNISHING(panelBG, content, 720, 120, 1200);
    }
    protected void OpenPanelPUNISHING(RectTransform panelBG, RectTransform content, int maxY, int minY, int width)
    {
        gameObject.SetActive(true);
        panelBG.sizeDelta = new Vector2(width, minY);
        Image panelImg = panelBG.GetComponent<Image>();
        panelImg.DOColor(new Color(1, 1, 1, 0.5f), 0.05f).OnComplete(() =>
        {
            panelImg.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(() =>
            {
                panelImg.DOColor(new Color(1, 1, 1, 1), 0.05f).OnComplete(() =>
                {
                    panelBG.DOSizeDelta(new Vector2(width, maxY), 0.5f).OnComplete(() =>
                    {
                        content.gameObject.SetActive(true);
                    });
                });
            });
        });
    }
    protected void ClosePanelPUNISHING(RectTransform panelBG, RectTransform content)
    {
        ClosePanelPUNISHING(panelBG, content, 120, 1200);
    }
    protected void ClosePanelPUNISHING(RectTransform panelBG, RectTransform content, int minY, int width)
    {
        //内容重置回透明
        content.gameObject.SetActive(false);
        Image panelImg = panelBG.GetComponent<Image>();
        panelBG.DOSizeDelta(new Vector2(width, minY), 0.3f).OnComplete(() =>
        {
            panelImg.DOColor(new Color(1, 1, 1, 0.5f), 0.05f).OnComplete(() =>
            {
                panelImg.DOColor(new Color(1, 1, 1, 1), 0.05f).OnComplete(() =>
                {
                    panelImg.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(() =>
                    {
                        //这里pop就要调用OnExit，会将active设为false
                        UIMng.PopPanel();
                    });
                });
            });
        });
    }
    //暂停以及恢复panel不闪烁
    protected void PausePanelPUNISHING(RectTransform panelBG, RectTransform content, UIPanelType type, Action<BasePanel> SetParams = null)
    {
        PausePanelPUNISHING(panelBG, content, type, 120, 1200, SetParams);
    }
    protected void PausePanelPUNISHING(RectTransform panelBG, RectTransform content, UIPanelType type,
        int minY, int width, Action<BasePanel> SetParams = null)
    {
        //内容重置回透明
        content.gameObject.SetActive(false);
        Image panelImg = panelBG.GetComponent<Image>();
        panelBG.DOSizeDelta(new Vector2(width, minY), 0.35f).OnComplete(() =>
        {
            panelImg.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(() =>
            {
                //这里active直接设为false
                gameObject.SetActive(false);
                BasePanel panel = UIMng.PushPanel(type);
                //如果委托不为空，调用委托
                SetParams?.Invoke(panel);
            });
        });
    }
    protected void ResumePanelPUNISHING(RectTransform panelBG, RectTransform content)
    {
        ResumePanelPUNISHING(panelBG, content, 720, 1200);
    }
    protected void ResumePanelPUNISHING(RectTransform panelBG, RectTransform content, int maxY, int width)
    {
        gameObject.SetActive(true);
        Image panelImg = panelBG.GetComponent<Image>();
        panelImg.DOColor(new Color(1, 1, 1, 1), 0.05f).OnComplete(() =>
        {
            panelBG.DOSizeDelta(new Vector2(width, maxY), 0.5f).OnComplete(() =>
            {
                content.gameObject.SetActive(true);
            });
        });
    }
}
