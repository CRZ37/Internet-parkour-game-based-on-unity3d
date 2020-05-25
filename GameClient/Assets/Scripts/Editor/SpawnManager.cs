using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnManager : EditorWindow
{
    [MenuItem("Tools/添加编辑好的pattern到Manager")]
    static void PatternSystem()
    {
        Debug.Log("添加pattern成功");
        GameObject spawnManager = GameObject.Find("PatternManager");
        if (spawnManager != null)
        {
            var patternManager = spawnManager.GetComponent<PatternManager>();
            if (Selection.gameObjects.Length == 1)
            {
                //获取当前点击到的游戏物体
                var item = Selection.gameObjects[0].transform;
                if (item != null)
                {
                    Pattern pattern = new Pattern();
                    foreach (var child in item)
                    {
                        //将选中的物体转为transform
                        Transform childTrans = child as Transform;
                        if (childTrans != null)
                        {
                            //通过场景中的gameobject找到对应的prefab
                            var prefab = PrefabUtility
                                .GetCorrespondingObjectFromSource(childTrans.gameObject);
                            if (prefab != null)
                            {
                                //创建新的item
                                PatternItem patternItem = new PatternItem
                                {
                                    pos = childTrans.localPosition,
                                    prefabName = prefab.name
                                };
                                //将item加入方案
                                pattern.PatternItems.Add(patternItem);
                            }
                        }
                    }
                    //将方案加入方案list
                    patternManager.Patterns.Add(pattern);
                }
            }
        }
    }
}
