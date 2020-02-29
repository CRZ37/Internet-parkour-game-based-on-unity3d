using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System.Text;
using System.Linq;

public class Message
{
    //消息的内容,只读。
    private byte[] data = new byte[1024];
    //标志位，存了多少字节的数据在数组里，只读。
    private int endIndex = 0;
    public byte[] Data { get => data; }
    public int EndIndex { get => endIndex; }
    //剩余空间
    public int RemainSize
    {
        get => data.Length - endIndex;
    }
    /// <summary>
    /// 解析数据,处理粘包以及分包问题
    /// </summary>
    public void ReadMessage(int newDataAmout, Action<ActionCode, string> processDataCallBack)
    {
        //更新索引
        endIndex += newDataAmout;
        //数据是完整的，默认当作粘包处理
        while (true)
        {
            //数据不完整，说明分包了，不解析（异步接收的时候没有发现这个现象，感觉其实不会有这个问题）
            //或解析完成
            if (endIndex <= 4) return;
            //读取数据长度,从0向后四个字节是存储数据长度的
            int count = BitConverter.ToInt32(data, 0);
            if (endIndex - 4 >= count)
            {
                //从8的位置解析actionCode
                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 4);
                string s = Encoding.UTF8.GetString(data, 8, count - 4);
                processDataCallBack(actionCode, s);
                //把解析过的数据用后面还没解析过的数据覆盖掉
                Array.Copy(data, count + 4, data, 0, endIndex - 4 - count);
                //更新endIndex
                endIndex -= (count + 4);
            }
            else
            {
                break;
            }
        }
    }

    public static byte[] PackData(RequestCode requestData,ActionCode actionCode,string data)
    {
        //controller、方法、数据
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestData);
        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        //数据包总大小
        int dataAmount = requestCodeBytes.Length + dataBytes.Length + actionCodeBytes.Length;
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
        //返回打包后的数据
        return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>()
            .Concat(actionCodeBytes).ToArray<byte>()
            .Concat(dataBytes).ToArray<byte>();
    }
}
