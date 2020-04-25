using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class GameData
{
    //是否结束游戏
    public bool IsPlay { get; set; }
    //是否购买属性商店的道具
    public bool IsLoadProperty { get; set; }
    //是否购买人物商店的道具
    public bool IsLoadRole { get; set; }
    public GameData(bool isPlay,bool isLoadProperty,bool isLoadRole)
    { 
        IsPlay = isPlay;
        IsLoadProperty = isLoadProperty;
        IsLoadRole = isLoadRole;
    }
}
