using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例模板
/// </summary>
/// <typeparam name="T">MonoBehaviour的子类</typeparam>
public class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    //只读字段
    private static T instance;
    public static T Instance { get => instance;}

    protected virtual void Awake() {
        instance = this as T;
    }
}
