using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
//字体相关与UI相关的包记得要导入

public class UI_ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//这是单个的物品栏格子UI，与InventoryStoragedItem一一对应
{
    //Image是UI相关的图像，而不是Sprite
    [SerializeField] private Image itemImageInSlot;
    //物品栏UI显示的物品的相关文本
    [SerializeField] private TextMeshProUGUI itemText;

    //链接到物品栏存储的物品的信息
    public InventoryStoragedItem item;

    private void Start()
    {
        itemImageInSlot = GetComponent<Image>();
        itemText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateItemSlotUI(InventoryStoragedItem _newItem)
    //在Inventory中被调用更新
    {
        //接受输入的物品栏内物品
        item = _newItem;
        //注意这个非空判定
        if (item != null)
        {
            //赋予这个物品栏上的物品图像
            itemImageInSlot.sprite = item.itemData.itemIcon;
            //并把物品栏从半透明状态转化为白色，不然的话物品图像也会透明
            itemImageInSlot.color = Color.white;

            //物品数大于一的时候才显示数量，否则只有一个时不显示数量，这样好看
            if (item.stackSize > 1)
            {
                //显示这个物品的堆叠数量，注意从整型到字符串的转变
                itemText.text = item.stackSize.ToString();
            }
            if(item.stackSize == 1)
            {
                //空文本
                itemText.text = "";
            }
        }
    }

    #region ItemToolTip
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item.itemData != null)
            UI.instance.itemToolTip.ShowItemToolTip(item.itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item.itemData != null)
            UI.instance.itemToolTip.HideItemToolTip();
    }
    #endregion
}
