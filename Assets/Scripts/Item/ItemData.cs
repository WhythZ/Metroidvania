//已删除掉了不需要的using
using UnityEngine;

//使得可以在Unity内右键相应的路径内，创建出此类的相关对象文件
[CreateAssetMenu(fileName ="New Item Data", menuName ="Item Data/New Item")]

public class ItemData : ScriptableObject
//继承自ScriptableObject这个类，是一种很好用的模板
{
    //物品的名称
    public string itemName;
    //物品的类型
    public string itemType;
    //物品的描述
    [TextArea]
    public string itemDescription;
    //物品的贴图
    public Sprite itemIcon;
}
