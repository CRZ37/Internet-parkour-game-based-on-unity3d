using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowPlayer : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;
    private float speed = 15;
    private bool isFollow = false;

    private void Start()
    {

    }
    public void FollowRole()
    {
        player = Game.Instance.GetCurrentRoleGameObject().transform;
        offset = new Vector3(0,8.66f,-9.53f);
        isFollow = true;
    }
    public void StopFollow()
    {
        isFollow = false;
    }
    private void Update()
    {
        if (isFollow)
        {
            transform.position = Vector3.Lerp(transform.position, offset + player.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, offset + player.position) <= 0.1)
            {
                transform.position = offset + player.position;
            }
        }       
    }
}
