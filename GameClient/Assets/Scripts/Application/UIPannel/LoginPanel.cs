using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;

public class LoginPanel : BasePanel
{
    private RectTransform panelBG;
    private RectTransform content;
    private Button closeButton;
    private InputField usernameIF;
    private InputField passwordIF;
    private LoginRequest loginRequest;
    private ReturnCode returnCode = ReturnCode.Null;

    private void Awake()
    {
        loginRequest = GetComponent<LoginRequest>();
        panelBG = transform.GetComponent<RectTransform>();
        content = transform.Find("Content").GetComponent<RectTransform>();
        usernameIF = content.Find("UsernameInput").GetComponent<InputField>();
        passwordIF = content.Find("PasswordInput").GetComponent<InputField>();
        closeButton = content.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(OnCloseClick);

        content.Find("LoginButton").GetComponent<Button>().onClick.AddListener(OnLoginClick);
        content.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick);
    }
    public override void OnEnter()
    {
        OpenPanelPUNISHING(panelBG, content);
        Game.Instance.sound.PlayBg("LoginPanel");
    }
    private void Update()
    {
        if (returnCode != ReturnCode.Null)
        {
            OnLoginResponse();
            returnCode = ReturnCode.Null;
        }
    }
    public void OnLoginResponseSync(ReturnCode returnCode)
    {
        this.returnCode = returnCode;
    }
    private void OnLoginResponse()
    {
        switch (returnCode)
        {
            case ReturnCode.IsLogin:
                //重复登陆账号
                UIMng.ShowMessage("该用户已登录");
                break;
            case ReturnCode.Fail:
                //登陆失败提示错误
                UIMng.ShowMessage("用户名或密码错误，无法登录\n请重新输入！");
                break;
            case ReturnCode.Success:
                // 登录成功进入房间列表
                UIMng.ShowMessage("登录成功！");
                PausePanelPUNISHING(panelBG, content, UIPanelType.RoomList);
                break;
        }
    }
    private void OnLoginClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        string msg = "";
        if (string.IsNullOrEmpty(usernameIF.text))
        {
            msg = "用户名不能为空！";
        }
        else if (string.IsNullOrEmpty(passwordIF.text))
        {
            msg = "密码不能为空！";
        }

        if (msg != "")
        {
            UIMng.ShowMessage(msg);
            return;
        }
        loginRequest.SendRequest(usernameIF.text, passwordIF.text);
    }
    private void OnRegisterClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        PausePanelPUNISHING(panelBG, content, UIPanelType.Register);     
    }
    private void OnCloseClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        ClosePanelPUNISHING(panelBG, content);
    }
    public override void OnResume()
    {
        ResumePanelPUNISHING(panelBG, content);
        Game.Instance.sound.PlayBg("LoginPanel");
    }
    public override void OnExit()
    {
        base.OnExit();
        Game.Instance.sound.StopBG();
    }
}
