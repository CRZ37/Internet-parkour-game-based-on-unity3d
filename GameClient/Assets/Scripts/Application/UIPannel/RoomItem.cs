using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    private Text username;
    private Text totalCount;
    private Text winCount;
    private int id;
    private RoomListPanel roomListPanel;
    private void Awake()
    {
        username = transform.Find("Username").GetComponent<Text>();
        totalCount = transform.Find("TotalCount").GetComponent<Text>();
        winCount = transform.Find("WinCount").GetComponent<Text>();

        transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(OnJoinClick);
    }
    public void SetRoomInfo(int id, string username, int totalCount, int winCount, RoomListPanel roomListPanel)
    {
        this.id = id;
        this.username.text = username;
        this.totalCount.text = "总场数：" + totalCount;
        this.winCount.text = "胜场：" + winCount;
        this.roomListPanel = roomListPanel;
    }
    public void SetRoomInfo(int id, string username, string totalCount, string winCount, RoomListPanel roomListPanel)
    {
        this.roomListPanel = roomListPanel;
        this.id = id;
        this.username.text = username;
        this.totalCount.text = "总场数：" + totalCount;
        this.winCount.text = "胜场：" + winCount;
    }

    private void OnJoinClick()
    {
        Game.Instance.sound.PlayEffect("Join");
        roomListPanel.OnJoinClick(id);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
