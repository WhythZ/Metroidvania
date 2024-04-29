using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
//此脚本放在Canvas上，用于控制各UI的切换
{
    //人物属性的详细信息窗口
    public UI_StatToolTip statToolTip;

    #region UIMenus
    //记录各UI，以便使用按键切换
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillsUI;
    [SerializeField] private GameObject optionsUI;
    #endregion

    private void Start()
    {
        //初始状态打开的是游戏内界面UI
        SwitchToUI(inGameUI);

        //防止TooTip在不需要的时候打开
        statToolTip.gameObject.SetActive(false);
    }

    private void Update()
    {
        //按键控制UI的检测
        UIWithKeyController();
    }

    #region UIController
    public void UIWithKeyController()
    //综合的按键控制
    {
        if (Input.GetKeyDown(KeyCode.I))
            SwitchWithKeyToUI(characterUI);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //在这两个界面按ESC应该退出，切换到游戏内UI
            if (characterUI.activeSelf || skillsUI.activeSelf)
            {
                SwitchToUI(inGameUI);
                //结束函数
                return;
            }

            //其他情况正常来即可
            SwitchWithKeyToUI(optionsUI);
        }
    }
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
    public void SwitchWithKeyToUI(GameObject _menu)
    //使用按键控制的进入或者退出UI界面
    {
        //如果传入的UI界面是激活状态的，则关闭此界面
        if (_menu != null && _menu.activeSelf)
        {
            //传入UI若和当前UI重叠，则切换到游戏内UI
            SwitchToUI(inGameUI);
            //结束此函数
            return;
        }

        //否则打开此传入界面（并关闭其他界面）
        SwitchToUI(_menu);
    }
    #endregion
}
