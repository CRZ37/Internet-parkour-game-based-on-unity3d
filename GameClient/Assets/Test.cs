using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log("CLR Version:" + Environment.Version);
        Type type = Type.GetType("Mono.Runtime");
        if (type != null)
        {
            MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            if (displayName != null)
                Debug.Log(displayName.Invoke(null, null));
        }
    }
}
