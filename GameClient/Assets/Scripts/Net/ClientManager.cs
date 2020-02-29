using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using Common;
using System.Text;
/// <summary>
/// 管理和服务器socket的连接
/// </summary>
public class ClientManager : BaseManager
{
    //服务器：（公网）139.199.133.2（私网）172.21.0.13
    private const string IP = "127.0.0.1";
    private const int PORT = 6666;
    private Socket clientSocket;
    private Message msg = new Message();
    //创建socket，建立与服务器的连接
    public override void OnInit()
    {
        base.OnInit();
        //跟服务器端建立连接
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(IP, PORT);
            Start();
        }
        catch (Exception e)
        {
            Debug.Log("连不上服务器，服务器未开启" + e);
        }        
    }
    //向服务器发送数据
    public void SendRequest(RequestCode requestCode,ActionCode actionCode,string data)
    {
        byte[] bytes = Message.PackData(requestCode, actionCode, data);
        clientSocket.Send(bytes);
    }
    private void Start()
    {
        //从服务器接收数据
        clientSocket.BeginReceive(msg.Data,msg.EndIndex,msg.RemainSize, SocketFlags.None, ReceiveCallback,null);
    }
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket.Connected == false)
            {
                //Scoket Close的时候会收到一个消息，此时clientSocket已释放，直接return
                Debug.Log("客户端连接已关闭");
                return;
            }
            int count = clientSocket.EndReceive(ar);
            msg.ReadMessage(count, OnProcessDataCallback);
            Start();
        }
        catch (Exception e)
        {
            Debug.Log("接收出错，" + e);
        }
    }
    private void OnProcessDataCallback(ActionCode actionCode,string data)
    {
        Game.Instance.HandleResponse(actionCode,data);
    }
    //关闭与服务器的连接
    public override void OnDestroy()
    {
        try
        {
            clientSocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log("无法关闭连接" + e);
        }
    }
}
