using System.Collections;
using System.Collections.Generic;

public interface Ireusable {
    //从对象池中取出对象
    void OnSpawn();
    //将物体回收回对象池
    void OnUnSpawn();
}
