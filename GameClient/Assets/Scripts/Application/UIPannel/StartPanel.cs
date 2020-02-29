using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class StartPanel : BasePanel
{
    private Button loginButton;
    private Button exitButton;
    public override void OnEnter()
    {
        loginButton = transform.Find("LoginButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        loginButton.onClick.AddListener(onLoginClick);
        exitButton.onClick.AddListener(onExitClick);
    }

    private void onExitClick()
    {
        Application.Quit();
    }

    private void onLoginClick()
    {
        UIMng.PushPanel(UIPanelType.Login);
    }
    public override void OnPause()
    {
        PausePanelByScale();
    }
    public override void OnResume()
    {
        ResumePanelByScale();
    }
}
