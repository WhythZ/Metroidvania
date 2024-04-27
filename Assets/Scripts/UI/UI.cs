using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
//此脚本放在Canvas上，用于控制各UI的切换
{
    public void SwitchToUI(GameObject _menu)
    {
        //遍历Canvas的子对象
        for (int i = 0; i < transform.childCount; i++)
        {
            //关闭所有子对象
            transform.GetChild(i).gameObject.SetActive(false);
        }
        
        //开启需要转换到的非空对象
        if(_menu != null)
            _menu.SetActive(true);
    }
}
