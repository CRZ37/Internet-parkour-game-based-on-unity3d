using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ReusableObject 
{
    //一秒后回收
    public float time = 1;

    public override void OnSpawn()
    {
        StartCoroutine(DestroyCoroutine());
    }

    public override void OnUnSpawn()
    {
        //为了安全起见停下携程
        StopAllCoroutines();
    }
    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(time);
        //规定时间后回收自己
        Game.Instance.objectPool.Unspwan(gameObject);
    }
}
