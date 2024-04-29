using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatToolTip : MonoBehaviour
//此脚本控制CharacterUI界面的鼠标悬停在Stats上的产生的描述信息文本窗口
{
    //使用子文件中的TextMeshProUGUI组件拖入此变量赋值
    [SerializeField] private TextMeshProUGUI statDescription;

    public void ShowStatToolTipAs(string _content)
    //显示具体描述窗口
    {
        //赋予描述信息以具体的文本
        statDescription.text = _content;

        //激活这个窗口
        gameObject.SetActive(true);
    }

    public void HideStatToolTip()
    //关闭这个窗口
    {
        //清空文本
        statDescription.text = "";
        gameObject.SetActive(false);
    }
}
