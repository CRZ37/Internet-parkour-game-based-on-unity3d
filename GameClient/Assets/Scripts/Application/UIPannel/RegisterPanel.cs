using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RegisterPanel : BasePanel
{
    private RectTransform panelBG;
    private RectTransform content;
    private InputField usernameIF;
    private InputField passwordIF;
    private InputField confirmIF;
    private RegisterRequest registerRequest;

    private void Awake()
    {
        registerRequest = GetComponent<RegisterRequest>();
        panelBG = transform.GetComponent<RectTransform>();
        content = transform.Find("Content").GetComponent<RectTransform>();
        usernameIF = content.Find("UserLabel/UserInput").GetComponent<InputField>();
        passwordIF = content.Find("PassWordLabel/PwdInput").GetComponent<InputField>();
        confirmIF = content.Find("ConfirmLabel/ConfirmInput").GetComponent<InputField>();

        content.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick);
        content.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
    }
    public override void OnEnter()
    {
        OpenPanelPUNISHING(panelBG, content);
    }

    private void OnRegisterClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        string msg = "";
        if (string.IsNullOrEmpty(usernameIF.text))
        {
            msg = "用户名不能为空！";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            msg = "密码不能为空！";
        }
        if(passwordIF.text != confirmIF.text)
        {
            msg = "密码与确认密码不一致！";
        }
        if (msg != "")
        {
            UIMng.ShowMessage(msg);
            return;
        }
        //进行注册，发送到服务器端
        registerRequest.SendRequest(usernameIF.text, passwordIF.text);
    }

    private void OnCloseClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        ClosePanelPUNISHING(panelBG, content);
    }

    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            UIMng.ShowMessageSync("注册成功");
        }
        else
        {
            UIMng.ShowMessageSync("用户名重复，注册失败");
        }
    }
}
