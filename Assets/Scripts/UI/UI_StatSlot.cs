using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatSlot : MonoBehaviour
{
    //需要显示的属性的名字
    [SerializeField] private string statName;
    //UI中显示的属性的值的文本
    [SerializeField] TextMeshProUGUI statValueText;
    //UI中显示的属性的名字的文本
    [SerializeField] TextMeshProUGUI statNameText;

    private void OnValidate()
    {
        gameObject.name = "Stat  " + statName;

        if(statValueText != null )
            statValueText.text = statName;
    }
}
