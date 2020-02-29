using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ReusableObject
{
    //旋转速度
    public float speed = 60;
    public override void OnSpawn()
    {
        
    }

    public override void OnUnSpawn()
    {
        //回收时旋转量为0
        transform.localEulerAngles = Vector3.zero;
    }

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }

    public virtual void HitPlayer(Transform pos)
    {

    }
}
