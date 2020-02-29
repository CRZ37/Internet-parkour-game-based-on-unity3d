using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReusableObject : MonoBehaviour, Ireusable
{
 public abstract void OnSpawn();

 public abstract void OnUnSpawn();
}
