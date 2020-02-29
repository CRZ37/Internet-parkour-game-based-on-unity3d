using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BGAnim : MonoBehaviour
{
    private bool canPop = false;
    private bool canPush = false;
    private RectTransform left;
    private RectTransform middle;
    private RectTransform right;
    private RectTransform blackBG;
    private GameObject hostPlayerState;
    private GameObject clientPlayerState;
    private void Awake()
    {
        blackBG = transform.Find("BlackBG").GetComponent<RectTransform>();
        left = transform.Find("Left").GetComponent<RectTransform>();
        middle = transform.Find("Middle").GetComponent<RectTransform>();
        right = transform.Find("Right").GetComponent<RectTransform>();
        clientPlayerState = left.Find("ClientPlayerState").gameObject;
        hostPlayerState = right.Find("HostPlayerState").gameObject;
    }
    private void Update()
    {
        if (canPop)
        {
            ShowPopAnim();
            canPop = false;
        }
        if (canPush)
        {
            ShowPushAnim();
            canPush = false;
        }
    }
    private void ShowPushAnim()
    {
        blackBG.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        middle.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
        left.DOSizeDelta(new Vector2(360, 1080), 0.5f);
        left.GetComponent<Image>().DOColor(new Color(0.2392157f, 0.282353f, 0.3058824f, 1), 0.5f);
        right.DOSizeDelta(new Vector2(360, 1080), 1);
        right.GetComponent<Image>().DOColor(new Color(0.2392157f, 0.282353f, 0.3058824f, 1), 0.5f);
        hostPlayerState.SetActive(false);
        clientPlayerState.SetActive(false);
    }
    private void ShowPopAnim()
    {
        //改动：把状态栏放到黑色背景后面
        middle.DOLocalMove(new Vector3(0, 1080, 0), 1).OnComplete(() =>
        {
            blackBG.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 0.4f);
        });
        left.DOSizeDelta(new Vector2(360, 400), 1);
        left.GetComponent<Image>().DOColor(new Color(0.2392157f, 0.282353f, 0.3058824f, 0.5f), 1);
        right.DOSizeDelta(new Vector2(360, 400), 1);
        right.GetComponent<Image>().DOColor(new Color(0.2392157f, 0.282353f, 0.3058824f, 0.5f), 1);
        hostPlayerState.SetActive(true);
        clientPlayerState.SetActive(true);
        //停下RoomListPanel的bgm
        Game.Instance.sound.StopBG();
    }
    public void ShowPushAnimSync()
    {
        canPush = true;
    }
    public void ShowPopAnimSync()
    {
        canPop = true;
    }
}
