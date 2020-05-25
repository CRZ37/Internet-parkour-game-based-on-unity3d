using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadChange : MonoBehaviour
{
    //发送生成新跑道和道具/障碍的请求
    private CreateRoadRequest createRoadRequest;
    //记录目前的跑道和下一个跑道
    private GameObject roadNow;
    private GameObject roadNext;
    private GameObject parent;
    //记录所有道路的长度
    private int[] roadLength;
    //上一条路的长度
    private int lastLength;
    private int roadIndex = -1;
    private int itemIndex = -1;
    void Start()
    {
        parent = GameObject.Find("Road");
        if (parent == null)
        {
            parent = new GameObject();
            parent.transform.position = Vector3.zero;
            parent.name = "Road";
        }
    }
    public void SetCreateRoadRequest(CreateRoadRequest createRoadRequest, int index1, int index2)
    {
        this.createRoadRequest = createRoadRequest;
        
        //初始化跑道长度
        roadLength = new int[4];
        roadLength[0] = 120;
        roadLength[1] = 135;
        roadLength[2] = 130;
        roadLength[3] = 110;
        lastLength = roadLength[1];
        //第一条跑道的位置要设置为（0，0，0）,第二条跑道要加上第一条跑道的长度，前两条跑道先固定
        //TODO: 这里最好也做成随机的
        roadNow = Game.Instance.objectPool.Spawn("runway1", parent.transform);
        //第二次开始游戏时的归零
        roadNow.transform.position = Vector3.zero;
        roadNext = Game.Instance.objectPool.Spawn("runway2", parent.transform);
        roadNext.transform.position = roadNow.transform.position + new Vector3(0, 0, roadLength[0]);
        //添加道具
        AddItem(roadNow, index1);
        AddItem(roadNext, index2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Road)
        {
            //回收旧的跑道，生成新的跑道
            Game.Instance.objectPool.Unspwan(other.gameObject);
            createRoadRequest.SendRequest();
        }
    }
    private void Update()
    {
        if (roadIndex != -1 && itemIndex != -1)
        {
            SpawnNewRoadAndItem();
            roadIndex = -1;
            itemIndex = -1;
        }
    }
    public void SpawnNewRoadAndItemSync(int roadIndex, int itemIndex)
    {
        this.roadIndex = roadIndex;
        this.itemIndex = itemIndex;
    }
    /// <summary>
    /// lastLength记录了上一条跑道的长度，随机生成一条新的跑道的时候，新跑道的位置要先加上上一条跑道的长度
    /// </summary>
    private void SpawnNewRoadAndItem()
    {
        roadNow = roadNext;
        roadNext = Game.Instance.objectPool.Spawn("runway" + roadIndex.ToString(), parent.transform);
        roadNext.transform.position = roadNow.transform.position + new Vector3(0, 0, lastLength);
        lastLength = roadLength[roadIndex - 1];
        AddItem(roadNext);
    }
    /// <summary>
    /// 生成障碍物。
    /// </summary>
    /// <param name="obj"></param>
    private void AddItem(GameObject obj)
    {
        AddItem(obj, itemIndex);
    }
    /// <summary>
    /// 生成障碍物重载，用于初次生成障碍物
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="index"></param>
    private void AddItem(GameObject obj, int index)
    {
        var itemFather = obj.transform.Find("Item");
        //如果找到
        if (itemFather != null)
        {
            var patternManager = PatternManager.Instance;
            if (patternManager != null && patternManager.Patterns != null && patternManager.Patterns.Count > 0)
            {
                //Random.Range(0, patternManager.Patterns.Count)
                var pattern = patternManager.Patterns[index];
                if (pattern != null && pattern.PatternItems.Count > 0)
                {
                    foreach (var itemList in pattern.PatternItems)
                    {
                        GameObject go = Game.Instance.objectPool.Spawn(itemList.prefabName, itemFather);
                        //由于生成道路与生成道具时，父物体对对象池物体管理方式不同(ObjectPool类中有解释)，这里要重新指定一下父物体
                        go.transform.parent = itemFather;
                        //设定障碍物的位置
                        go.transform.localPosition = itemList.pos;
                    }
                }
            }
        }
    }
}
