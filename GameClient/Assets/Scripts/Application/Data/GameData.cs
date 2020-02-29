using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class GameData
{
    //是否结束游戏
    public bool IsPlay { get; set; }

    public GameData(bool isPlay)
    { 
        IsPlay = isPlay;
    }
}
