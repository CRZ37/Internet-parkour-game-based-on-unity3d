using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;
using System;

public class GamePanel : BasePanel
{
    private Text timer;
    private int time = -1;
    private string data;
    private Transform gameResult;
    private Text winnerName;
    private Text winnerScore;
    private Text loserName;
    private Text loserScore;
    private Text result;
    private Button confirmButton;
    private Button exitButton;
    private QuitBattleRequest quitBattleRequest;
    private bool isGameOver = false;
    private bool isExit = false;
    private void Awake()
    {       
        timer = transform.Find("Timer").GetComponent<Text>();
        gameResult = transform.Find("GameResult").transform;
        winnerName = gameResult.Find("Winner/WinnerName").GetComponent<Text>();
        winnerScore = gameResult.Find("Winner/WinnerScore").GetComponent<Text>();
        loserName = gameResult.Find("Loser/LoserName").GetComponent<Text>();
        loserScore = gameResult.Find("Loser/LoserScore").GetComponent<Text>();
        result = gameResult.Find("Result").GetComponent<Text>();
        confirmButton = gameResult.Find("ConfirmButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
        quitBattleRequest = GetComponent<QuitBattleRequest>();
        exitButton.gameObject.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmClick);
        exitButton.onClick.AddListener(OnExitClick);
    }
    private void OnExitClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        quitBattleRequest.SendRequest();
    }
    public void OnExitResponseSync()
    {
        isExit = true;
    }
    private void OnExitResponse()
    {
        exitButton.gameObject.SetActive(false);
        //分别pop game和room 
        UIMng.PopPanel();
        UIMng.PopPanel();
        Game.Instance.GameOver();
    }
    private void OnConfirmClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        exitButton.gameObject.SetActive(false);
        gameResult.gameObject.SetActive(false);
        //分别pop game和room 
        UIMng.PopPanel();
        UIMng.PopPanel();
        Game.Instance.GameOver();
    }     
    private void Update()
    {
        if (time > -1)
        {
            ShowTime(time);
            time = -1;
        }
        if (isGameOver)
        {
            OnGameOverResponse();
            isGameOver = false;
            data = null;
        }
        if (isExit)
        {
            OnExitResponse();
            isExit = false;
        }
    }
    public void ShowTimeSync(int time)
    {
        this.time = time;
    }
    public void ShowTime(int time)
    {
        timer.text = time.ToString();
        //重置状态
        timer.transform.localScale = Vector3.one;
        Color temp = timer.color;
        temp.a = 1;
        timer.color = temp;
        //每次计时停留1秒
        timer.transform.DOScale(2, 0.4f).SetDelay(0.4f);
        Tween tween = timer.DOFade(0, 0.4f).SetDelay(0.4f);
        //播放音效
        Game.Instance.sound.PlayEffect("Timer");
        //在1之后显示开始
        if (time == 1)
        {            
            //显示开始
            tween.OnComplete(() =>
            {   //重置状态
                timer.transform.localScale = Vector3.one;
                temp.a = 1;
                timer.color = temp;
                timer.text = "开始！";
                timer.DOFade(0, 0.4f).SetDelay(0.4f);
                exitButton.gameObject.SetActive(true);
            });
        }
    }
    public void OnGameOverResponseSync(string data)
    {
        this.data = data;
        isGameOver = true;
    }
    public void OnGameOverResponse()
    {
        gameResult.gameObject.SetActive(true);
        string[] res = data.Split(',');
        winnerName.text = res[0].Split('|')[0];
        winnerScore.text = res[0].Split('|')[1];
        loserName.text = res[1].Split('|')[0];
        loserScore.text = res[1].Split('|')[1];
        //判断result参数是否为Tie
        if((Role_ResultRoleType)int.Parse(res[2]) != Role_ResultRoleType.Tie)
        {
            result.text = winnerName.text + "胜利";
        }
        else
        {
            result.text = "平手";
        }
    }
}
