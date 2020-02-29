using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 1.创建子对象池的同时也指定了取出对象时对象的默认父物体，多个子对象池可以有同一个父物体。
/// 2.子对象池一旦生成后prefab名字作为区分（也就是池名），生成时直接生成在一开始指定的父物体下（跑道），可以在生成后再修改父物体（道具）。
/// </summary>
public class ObjectPool : MonoBehaviour
{
    //资源目录
    public string ResourceDir = "";
    //存储所有对象池的字典
    Dictionary<string,SubPool> pools = new Dictionary<string, SubPool>();
    /// <summary>
    /// 从总对象池中取子对象池
    /// </summary>
    /// <param name="name">子对象池的名字</param>
    /// <returns>取出的对象</returns>
    public GameObject Spawn(string name,Transform transform){
        SubPool pool = null;
        //如果不含子对象池，则新建一个
        if(!pools.ContainsKey(name)){
            RegieterNew(name,transform);
        }
        //取出对象池，并从对象池中取出对象
        pool = pools[name];
        return pool.Spawn();
    }
    /// <summary>
    /// 将物体回收回对象池
    /// </summary>
    /// <param name="obj">对象的名字</param>
    public void Unspwan(GameObject obj){
        SubPool pool = null;
        //判断对象在哪一个对象池内
        foreach (var p in pools.Values)
        {
            if(p.Contain(obj)){
                pool = p;
                break;
            }
        }
        pool.UnSpawn(obj);
    }
    /// <summary>
    /// 回收所有对象
    /// </summary>
    public void UnSpawnAll(){
        foreach (var p in pools.Values)
        {
            p.UnSpawnAll();  
        }
    }
    /// <summary>
    /// 新建对象池
    /// </summary>
    /// <param name="name">对象池名称</param>
    /// <param name="transform">父物体</param>
    void RegieterNew(string name,Transform transform){
        //资源目录
        string path = ResourceDir + "/" + name;
        //新建子对象池
        GameObject obj = Resources.Load(path) as GameObject;
        SubPool pool = new SubPool(transform,obj);
        //将子对象池添加到总对象池中
        pools.Add(pool.Name,pool);
    }
}
