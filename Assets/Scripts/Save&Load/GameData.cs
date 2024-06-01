using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    #region Currency
    //玩家持有的货币
    public int currency;
    #endregion

    #region Inventory
    //玩家的物品栏，使用自定义的可序列化字典
    public SerializableDictionary<string, int> inventory;
    #endregion

    #region CheckPoints
    //储存激活的存档点的字典
    public SerializableDictionary<string, bool> checkpointsDict;
    //储存里玩家最近的存档点id
    public string closestCheckPointID;
    #endregion

    #region Settings
    //存储音乐（bgm和cd）和音效（sfx）音量大小设置的字典
    public SerializableDictionary<string, float> volumeSettings; 
    #endregion

    public GameData()
    //构造函数
    {
        //创建游戏存档时，默认货币为量0
        this.currency = 0;
        this.inventory = new SerializableDictionary<string, int>();
        this.checkpointsDict = new SerializableDictionary<string, bool>();
        this.closestCheckPointID = string.Empty;
        this.volumeSettings = new SerializableDictionary<string, float>();
    }
}
