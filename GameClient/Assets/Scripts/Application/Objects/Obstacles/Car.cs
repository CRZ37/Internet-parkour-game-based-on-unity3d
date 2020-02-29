using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Obstacles
{
    public bool canMove;
    bool isBlock = false;
    public float speed = 10;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void HitPlayer(Vector3 pos)
    {
        base.HitPlayer(pos);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
    }
    //碰撞触发区域
    protected void HitTrigger()
    {
        isBlock = true;
    }

    public override void OnUnSpawn()
    {
        //数据还原
        isBlock = false;
    }

    private void Update()
    {
        if(isBlock && canMove)
        {
            transform.Translate(-transform.forward * speed * Time.deltaTime);
        }
    }
}
