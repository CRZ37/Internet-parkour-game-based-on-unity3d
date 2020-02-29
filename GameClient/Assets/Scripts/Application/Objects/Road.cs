using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : ReusableObject
{
    public override void OnSpawn()
    {
        
    }

    public override void OnUnSpawn()
    {
        //回收Item下的东西
        var itemFather = transform.Find("Item");
        if (itemFather != null)
        {
            foreach (Transform child in itemFather)
            {
                
                if (child != null)
                {
                    Game.Instance.objectPool.Unspwan(child.gameObject);
                }
            }
        }
    }
}
