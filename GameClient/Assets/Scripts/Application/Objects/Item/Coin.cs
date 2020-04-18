using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    protected Transform effectParent;
    public override void HitPlayer(Transform pos)
    {
        base.HitPlayer(pos);
        //吃到金币时的特效
        GameObject go = Game.Instance.objectPool.Spawn("FX_JinBi", effectParent);
        go.transform.position = pos.position;
        //播放声音
        Game.Instance.sound.PlayEffect("Coin");
        //回收
        Game.Instance.objectPool.Unspwan(gameObject);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
    }

    public override void OnUnSpawn()
    {
        base.OnUnSpawn();
    }

    private void Awake()
    {
        effectParent = GameObject.Find("EffectParent").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tag.Player)
        {
            HitPlayer(other.transform);
            other.SendMessage("HitCoin", SendMessageOptions.DontRequireReceiver);
        }
    }
}
