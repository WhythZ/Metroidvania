using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    public void ShowItemToolTip(ItemData _item)
    //传入物品信息
    {
        itemNameText.text = _item.itemName;
        itemTypeText.text = _item.itemType.ToString();
        itemDescriptionText.text = _item.itemDescription;

        //显示这个ToolTip
        gameObject.SetActive(true);

        //UI音效
        AudioManager.instance.PlaySFX(5, null);
    }

    public void HideItemToolTip()
    {
        gameObject.SetActive(false);
    }
}
