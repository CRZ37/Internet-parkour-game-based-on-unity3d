using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : ReusableObject
{
    Transform effectParent;
    protected virtual void Awake()
    {
        effectParent = GameObject.Find("EffectParent").transform;
    }
    public override void OnSpawn()
    {
        
    }

    public override void OnUnSpawn()
    {
        
    }

    protected virtual void HitPlayer(Vector3 pos)
    {
        //1.撞击时要有特效
        GameObject go = Game.Instance.objectPool.Spawn("FX_ZhuangJi",effectParent);
        go.transform.position = pos;
        //3.回收
        Game.Instance.objectPool.Unspwan(gameObject);
        //Destroy(gameObject);
    } 
}
