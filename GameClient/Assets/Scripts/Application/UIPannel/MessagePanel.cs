using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessagePanel : BasePanel
{
    private Text text;
    private float showTime = 1f;
    private string message = null;
    public override void OnEnter()
    {
        text = GetComponent<Text>();
        text.enabled = false;
        UIMng.InjectMsgPanel(this);
    }
    private void Update()
    {
        if (message != null)
        {
            ShowMessage(message);
            message = null;
        }
    }
    public void ShowMessageSync(string msg)
    {
        message = msg;
    }
    public void ShowMessage(string msg)
    {
        text.enabled = true;
        text.text = msg;
        text.DOColor(new Color(1, 1, 1, 1), 0.05f).OnComplete(() =>
        {
            text.DOColor(new Color(1, 1, 1, 0), 0.05f).OnComplete(() =>
            {
                text.DOColor(new Color(1, 1, 1, 1), 0.05f);
            });
        });      
        //一秒后隐藏
        Invoke("Hide", showTime);
    }
    public override void OnPause()
    {

    }
    public override void OnResume()
    {

    }
    private void Hide()
    {
        text.DOColor(new Color(1, 1, 1, 0), 0.5f);
    }
}
