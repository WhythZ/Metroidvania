using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatSlot : MonoBehaviour
{
    //需要显示的属性的名字，Hierarchy内对象的名字，也会一并赋值给UI内显示的该数值名字，即statNameText
    [SerializeField] private string statName;

    //UI中显示的属性的名字的文本，将此脚本使用者下的Name显示者拖入赋值此变量
    [SerializeField] TextMeshProUGUI statNameText;
    //UI中显示的属性的值的文本，在Unity内使用此脚本的对象同时也是Value的显示者，需要把自己拖入赋值此变量
    [SerializeField] TextMeshProUGUI statValueText;

    //此变量在Unity内可以手动选取定义在enum StatType内的各内容，再通过GetValueOfStatType函数获取该内容对应的最终值
    [SerializeField] StatType statType;

    private void OnValidate()
    {
        gameObject.name = "Stat  " + statName;

        if(statNameText != null )
            statNameText.text = statName;
    }

    private void Start()
    {
        //开始时更新一次数据值
        UpdateStatValueSlotUI();
    }

    public void UpdateStatValueSlotUI()
    //每次数值产生变化时，都要调用此函数更新一次
    {
        //获取人物统计数据脚本
        PlayerStats pStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
    
        if( pStats != null )
        {
            //通过此函数获取选取的内容对应的最终值，再转化为字符串
            statValueText.text = pStats.GetValueOfStatType(statType).ToString();
        }
    }
}
