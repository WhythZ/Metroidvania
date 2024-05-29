using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //玩家持有的货币
    public int currency;

    //玩家的物品栏，使用自定义的可序列化字典
    public SerializableDictionary<string, int> inventory;

    //储存激活的存档点的字典
    public SerializableDictionary<string, bool> checkpointsDict;

    public GameData()
    //构造函数
    {
        //创建游戏存档时，默认货币为量0
        this.currency = 0;

        this.inventory = new SerializableDictionary<string, int>();
        this.checkpointsDict = new SerializableDictionary<string, bool>();
    }
}
