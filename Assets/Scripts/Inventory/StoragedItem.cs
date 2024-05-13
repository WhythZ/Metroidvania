using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这样才使得该类可见
[System.Serializable]
public class StoragedItem
//此处不需继承，可以发现，这个数据类型和Stat这个类的结构很相似，都是含有所谓baseValue和modifiers的类
//这个类代表的是人物物品栏内的一处格子，而ItemData则代表着一种物品的类型，ItemObject则是代表物品在游戏内的显示与碰撞相关的层面
{
    //表示这个物品栏格子处的物品是什么
    public ItemData itemData;
    //表示此处格子的物品堆叠数量
    public int stackSize;

    public StoragedItem(ItemData _newItemData)
    //构造函数，接收一个ItemData作为这个格子的物品类型
    {
        itemData = _newItemData;
        //当这个物品栏格子被创建时，默认堆叠数量应该从1开始，而不是0
        stackSize = 1;
    }

    //两个函数控制stackSize的值，和Stat中的modifiers很相似的用法
    public void AddStackSizeBy(int _num) => stackSize += _num;
    public void DecreaseStackSizeBy(int _num) => stackSize -= _num;
}
