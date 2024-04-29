using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//对于后面两个父类，alt+enter键选择“实现接口”即可使用（会分别多出一个函数）
{
    //获取UI组件
    private UI ui;

    #region SlotContent
    //需要显示的属性的名字，Hierarchy内对象的名字，也会一并赋值给UI内显示的该数值名字，即statNameText
    [SerializeField] private string statName;

    //UI中显示的属性的名字的文本，将此脚本使用者下的Name显示者拖入赋值此变量
    [SerializeField] TextMeshProUGUI statNameText;
    //UI中显示的属性的值的文本，在Unity内使用此脚本的对象同时也是Value的显示者，需要把自己拖入赋值此变量
    [SerializeField] TextMeshProUGUI statValueText;

    //此变量在Unity内可以手动选取定义在enum StatType内的各内容，再通过GetValueOfStatType函数获取该内容对应的最终值
    [SerializeField] StatType statType;
    #endregion

    #region ToolTip
    //对于这个slot的具体描述信息，需要手动输入；[TextArea]使得我们在Hierarchy内可以分多行输入，而不是只有一行
    [TextArea]
    [SerializeField] string statDescription;
    #endregion

    private void OnValidate()
    {
        gameObject.name = "Stat  " + statName;

        if(statNameText != null )
            statNameText.text = statName;
    }

    private void Start()
    {
        //开始时获取UI组件
        ui = GetComponentInParent<UI>();

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

    #region PointerEnter&Exit
    public void OnPointerEnter(PointerEventData eventData)
    {
        //这是默认生成的语句，我们不用这个
        //throw new System.NotImplementedException();

        //当鼠标悬停在这个slot上时，显示这个窗口
        ui.statToolTip.ShowStatToolTipAs(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //这是默认生成的语句，我们不用这个
        //throw new System.NotImplementedException();

        //离开时，关闭
        ui.statToolTip.HideStatToolTip();
    }
    #endregion
}
