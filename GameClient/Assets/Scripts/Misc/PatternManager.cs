using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PatternManager : MonoSingleton<PatternManager>
{
    public List<Pattern> Patterns;
}

//一个游戏物体的信息
[Serializable]
public class PatternItem
{
    public string prefabName;
    public Vector3 pos;
}

//一个片段
[Serializable]
public class Pattern
{
    public List<PatternItem> PatternItems = new List<PatternItem>();
}
