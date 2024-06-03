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

    #region Stats
    //注意，在EntityStat内这些属性的数据类型是"Stat"，而存档文本里无法使用，所以用回int，注意是在没有用到Stat内置的modifiers的情况下才可如此
    //实际的属性值（加成后）会在背包UI界面显示，这里记录的都是未经加成前的原始值，也是需要保存的值
    
    //角色能力值
    public int strength;
    public int agility;
    public int vitality;
    public int intelligence;

    //原始最大生命值（未经加成）
    public int originalMaxHealth;

    //暴击相关属性（未经加成）
    public int criticPower;
    public int criticChance;

    //物理和法术攻击力（未经加成）
    public int primaryPhysicalDamage;
    public int fireAttackDamage;
    public int iceAttackDamage;
    public int lightningAttackDamage;

    //技能攻击力（未经加成）
    public int swordDamage;

    //闪避与防御（未经加成）
    public int evasionChance;
    public int physicalArmor;
    public int magicalResistance;
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
        #region Currency
        //创建游戏存档时，默认货币为量0
        this.currency = 0;
        #endregion

        #region Stats
        //设定这些属性的初始默认值
        this.strength = 0;
        this.agility = 0;
        this.vitality = 0;
        this.intelligence = 0;

        this.originalMaxHealth = 100;

        this.criticPower = 150;
        this.criticChance = 5;

        this.primaryPhysicalDamage = 20;
        this.fireAttackDamage = 0;
        this.iceAttackDamage = 0;
        this.lightningAttackDamage = 0;

        this.swordDamage = 10;

        this.evasionChance = 5;
        this.physicalArmor = 0;
        this.magicalResistance = 0;
        #endregion

        #region Inventory
        this.inventory = new SerializableDictionary<string, int>();
        #endregion

        #region CheckPoints
        this.checkpointsDict = new SerializableDictionary<string, bool>();
        this.closestCheckPointID = string.Empty;
        #endregion

        #region Settings
        this.volumeSettings = new SerializableDictionary<string, float>();
        #endregion
    }
}
