using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincible : Item
{
    public override void HitPlayer(Transform pos)
    {
        base.HitPlayer(pos);
        //播放声音
        Game.Instance.sound.PlayEffect("Invincible");
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tag.Player)
        {
            HitPlayer(other.transform);
            other.SendMessage("HitInvincible", SendMessageOptions.DontRequireReceiver);
        }
    }
}
