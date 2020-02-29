using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : Obstacles
{
    Animator peopleAnim;
    Transform peopleEffectParent;
    //初始位置
    Vector3 originPos;
    //碰撞玩家
    protected override void HitPlayer(Vector3 pos)
    {
        //播放完动画后再进行回收
        GameObject go = Game.Instance.objectPool.Spawn("FX_ZhuangJi", peopleEffectParent);
        go.transform.position = pos;
        peopleAnim.SetBool("canDown", true);
        Invoke("DestroyPeople", 1.5f);
    }

    private void DestroyPeople()
    {
        //回收人物障碍
        Game.Instance.objectPool.Unspwan(gameObject);
    }

    //碰撞触发区域
    protected void HitTrigger()
    {
        peopleAnim.SetBool("canMove", true);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
    }

    public override void OnUnSpawn()
    {
        base.OnUnSpawn();
        //恢复数据
        peopleAnim.SetBool("canDown", false);
        peopleAnim.SetBool("canMove", false);
        transform.position = originPos;
    }

    protected override void Awake()
    {
        base.Awake();
        peopleAnim = GetComponent<Animator>();
        peopleEffectParent = GameObject.Find("EffectParent").transform;
        originPos = transform.position;
    }
}
