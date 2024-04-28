using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
//这个脚本赋值给Player下的Inventory，不仅仅是管理CharacterUI内的Inventory的Slots，还使用着相同的方法管理着CharacterUI内的数据的更新
{
    //确保所有地方都能调用此（唯一的）物品栏脚本，直接使用instance即可
    //不同于PlayerManager的通过instance.player来调用，因为在那里instance代表的是PlayerManager而非Player类型对象
    public static Inventory instance;

    [Header("Character UI")]
    //整体物品栏的位置
    [SerializeField] private Transform inventorySlotParent;
    //单个物品栏UI的列表
    private UI_ItemSlot[] itemSlotsUIList;
    
    //记录Stat各个格子
    [SerializeField] private Transform statSlotParent;
    //记录各slot的列表
    private UI_StatSlot[] statSlotsUIList;

    [Header("Inventory Items")]
    //记录“物品栏物品”（一种类似Stat的自定义数据类型，而不是直接使用不好管理的ItemData）信息的列表
    public List<InventoryStoragedItem> inventoryItemsList;
    //使用字典来存储ItemData与InventoryItem一一对应的关系
    public Dictionary<ItemData,InventoryStoragedItem> inventoryItemsDictionary;

    private void Awake()
    {
        //赋予物品栏以及确保只有一个此脚本的instance
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        //初始化物品栏物品列表以及物品栏字典
        inventoryItemsList = new List<InventoryStoragedItem>();
        inventoryItemsDictionary = new Dictionary<ItemData,InventoryStoragedItem>();
    
        //注意这里是Components，有s，因为左值是列表，记录多个对象
        itemSlotsUIList = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        statSlotsUIList = statSlotParent.GetComponentsInChildren<UI_StatSlot>();
    }

    #region UpdateSlotUIs
    private void UpdateItemSlotUI()
    //此函数更新物品栏的格子，在物品变化（Add与Remove两处）时被调用一次
    {
        //对物品栏列表内的物品进行遍历
        for (int i = 0; i < inventoryItemsList.Count; i++)
        {
            //将物品栏列表内的物品一一赋予物品栏UI列表
            itemSlotsUIList[i].UpdateItemSlotUI(inventoryItemsList[i]);
        }
    }
    private void UpdateStatSlotUI()
    //此函数更新CharacterUI内的人物数值格子更新，在数值变化的时候被调用更新
    {
        for (int i = 0; i < statSlotsUIList.Length; i++)
        {
            statSlotsUIList[i].UpdateStatValueSlotUI();
        }
    }
    #endregion

    #region ChangeInventoryItems
    public bool CanAddNewItem()
    //检测物品栏是否还有余留空间
    {
        if (inventoryItemsList.Count >= itemSlotsUIList.Length)
        {
            Debug.Log("Inventory No More Space");
            return false;
        }
        else
        {
            //Debug.Log("Inventory Has Space");
            return true;
        }
    }
    public void AddItem(ItemData _newItemData)
    {
        //若原来在物品栏字典内有这个物品了，那么就在此基础上增加一个堆叠数量即可（注意是否有堆叠上限）
        if(inventoryItemsDictionary.TryGetValue(_newItemData, out InventoryStoragedItem _value))
        {
            //一次加上一个
            _value.AddStackSizeBy(1);
        }
        //若原来没有这个物品，则在列表中增加新物品，前提是背包没满
        else
        {
            if(CanAddNewItem())
            {
                //C#中创建自定义类新对象的方式
                InventoryStoragedItem _newInventoryStoragedItem = new InventoryStoragedItem(_newItemData);

                //物品栏新创建一个物品栏格子装填这个新物品，堆叠数在构造函数中被默认初始化为1
                inventoryItemsList.Add(_newInventoryStoragedItem);

                //添加新的映射关系，以便下次被检测到并直接将堆叠数进行增加
                inventoryItemsDictionary.Add(_newItemData, _newInventoryStoragedItem);
            }
        }

        //刷新物品栏UI
        UpdateItemSlotUI();
    }
    public void RemoveItemTotally(ItemData _itemData)
    {
        //若存在这个需要被删除的物品，则删除这个物品的全部，不需计数
        if(inventoryItemsDictionary.TryGetValue(_itemData, out InventoryStoragedItem _InvItem))
        {
            //若此格子处物品为0或者1，则销毁这个物品格子
            if(_InvItem.stackSize <= 1)
            {
                //移除物品栏格子列表中的这个ItemData
                inventoryItemsList.Remove(_InvItem);
                //移除字典中的这个作为key的ItemData
                inventoryItemsDictionary.Remove(_itemData);
            }
            else
            {
                //反之，减少这个堆叠的数量
                _InvItem.DecreaseStackSizeBy(1);
            }
        }

        //刷新物品栏UI
        UpdateItemSlotUI();
    }
    #endregion

}
