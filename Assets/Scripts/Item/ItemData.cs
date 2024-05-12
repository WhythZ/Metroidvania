//已删除掉了不需要的using
using UnityEditor;
using UnityEngine;

//使得可以在Unity内右键相应的路径内，创建出此类的相关对象文件
[CreateAssetMenu(fileName ="New Item Data", menuName ="Item Data/New Item")]

public class ItemData : ScriptableObject
//继承自ScriptableObject这个类，是一种很好用的模板
{
    //物品的名称
    public string itemName;
    //物品的类型
    public ItemType itemType;
    //物品的描述
    [TextArea]
    public string itemDescription;
    //物品的贴图
    public Sprite itemIcon;

    //每个物品特有的的id序列，用于存档记录物品栏内物品相关功能
    public string itemID;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string _path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(_path);
#endif
    }
}

public enum ItemType
{
    Weapon,
    Potion,
    CD
}
