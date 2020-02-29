using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPool
{
    //储存对象池对象的list
    List<GameObject> objPool = new List<GameObject>();
    //对象的prefab
    GameObject prefab;
    //对象的名字,一个只读的属性,同时作为对象池的名字
    public string Name
    {
        get
        {
            return prefab.name;
        }
    }
    //父对象的位置信息
    Transform parent;
    //构造函数
    public SubPool(Transform parent, GameObject prefab)
    {
        this.parent = parent;
        this.prefab = prefab;
    }

    /// <summary>
    /// 从对象池中取出对象
    /// </summary>
    /// <returns>取出的对象</returns>
    public GameObject Spawn()
    {
        GameObject obj_spawn = null;
        //遍历list取出可用的对象
        foreach (var obj in objPool)
        {
            if (!obj.activeSelf)
            {
                obj_spawn = obj;
            }
        }
        //如果没有可用的对象
        if (obj_spawn == null)
        {
            //从预制体生成对象并储存在对象池中
            obj_spawn = GameObject.Instantiate<GameObject>(prefab);
            //设置生成物体的父物体
            obj_spawn.transform.parent = parent;
            //将物体添加到对象池中
            objPool.Add(obj_spawn);
        }
        //取出对象的时候要将active设置为true,并发送消息调用OnSpawn方法
        obj_spawn.SetActive(true);
        obj_spawn.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
        return obj_spawn;
    }

    /// <summary>
    /// 将对象回收到对象池内
    /// </summary>
    /// <param name="obj_unSpawn">回收的对象</param>
    public void UnSpawn(GameObject obj_unSpawn)
    {
        //如果物体在对象池内,回收对象并将active设置为false
        if (objPool.Contains(obj_unSpawn))
        {
            obj_unSpawn.SendMessage("OnUnSpawn", SendMessageOptions.DontRequireReceiver);
            obj_unSpawn.SetActive(false);
        }
    }

    /// <summary>
    /// 将所有的对象回收到对象池中
    /// </summary>
    public void UnSpawnAll()
    {
        foreach (var obj in objPool)
        {
            if (obj.activeSelf)
            {
                UnSpawn(obj);
            }
        }
    }
    /// <summary>
    /// 判断对象是否在对象池内
    /// </summary>
    /// <param name="obj">对象的名字</param>
    /// <returns></returns>
    public bool Contain(GameObject obj)
    {
        return (objPool.Contains(obj));
    }
}
