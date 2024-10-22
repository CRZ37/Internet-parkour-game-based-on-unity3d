﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiply : Item
{
    public override void HitPlayer(Transform pos)
    {
        base.HitPlayer(pos);
        //播放声音
        Game.Instance.sound.PlayEffect("Multiply");
        //TODO: 回收
        Game.Instance.objectPool.Unspwan(gameObject);
        //Destroy(gameObject);
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
        if (other.tag == Tag.Player)
        {
            HitPlayer(other.transform);
            other.SendMessage("HitMultiply", SendMessageOptions.DontRequireReceiver);
        }
    }
}
