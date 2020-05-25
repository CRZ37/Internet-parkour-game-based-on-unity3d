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
        Game.Instance.sound.PlayEffect("Click");
        Application.Quit();
    }

    private void onLoginClick()
    {
        Game.Instance.sound.PlayEffect("Click");
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
