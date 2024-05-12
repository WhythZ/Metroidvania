using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavesManager
//创建一个接口（名字一般以I开头），需要在SavesManager被继承；在ToolTip用到过的IPointIn啥的名字带I的都是接口
{
    void LoadData(GameData _data);

    //注意这里是引用，使得可以改变传入对象
    void SaveData(ref GameData _data);
}
