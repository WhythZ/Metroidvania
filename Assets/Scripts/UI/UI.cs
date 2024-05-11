using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI : MonoBehaviour
//此脚本放在Canvas上，用于控制各UI的切换
{
    public static UI instance;

    #region ToolTip
    //物品栏物品的详细信息窗口
    public UI_ItemToolTip itemToolTip;
    //人物属性的详细信息窗口
    public UI_StatToolTip statToolTip;
    //按键提示
    [SerializeField] private GameObject interactToolTipUI;
    //是否显示按键提示
    private bool isShowInteractToolTip; 
    #endregion

    #region UIMenus
    //记录各UI，以便使用按键切换
    public GameObject inGameUI;
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillsUI;
    [SerializeField] private GameObject optionsUI;
    public GameObject cdPlayerUI;
    #endregion

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
    private void Start()
    {
        //初始状态打开的是游戏内界面UI
        SwitchToUI(inGameUI);

        //防止TooTip在不需要的时候打开
        statToolTip.gameObject.SetActive(false);
        itemToolTip.gameObject.SetActive(false);
    }

    private void Update()
    {
        //按键控制UI的检测
        UIWithKeyController();
        //按键提示UI的检测
        CheckWhtherShowInteractToolTip();
    }

    #region UIController
    public void UIWithKeyController()
    //综合的按键控制
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchWithKeyToUI(characterUI);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //在这几个界面按ESC应该退出，切换到游戏内UI
            if (characterUI.activeSelf || skillsUI.activeSelf || cdPlayerUI.activeSelf)
            {
                SwitchToUI(inGameUI);
                //结束函数
                return;
            }

            //其他情况正常来即可
            SwitchWithKeyToUI(optionsUI);
        }

        if (Input.GetKeyDown(KeyCode.E) && isShowInteractToolTip)
        //当显示了按键提示UI时，按E打开或关闭唱片机交互UI
        {
            SwitchWithKeyToUI(cdPlayerUI);
        }
    }
    public void SwitchToUI(GameObject _menu)
    {
        //遍历Canvas的子对象
        for (int i = 0; i < transform.childCount; i++)
        {
            //保证按键提示UI可以正常显示
            if(transform.GetChild(i).gameObject != interactToolTipUI)
            {
                //关闭所有子对象
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        //开启需要转换到的非空对象
        if(_menu != null)
        {
            _menu.SetActive(true);
            //UI切换的音效
            Audio_Manager.instance.PlaySFX(8, null);
        }
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

    #region StateOfUI
    public bool ActivatedStateOfMainUIs()
    //返回主要UI的激活情况，用于比如此函数返回为真时，不能做攻击等动作
    {
        //遍历Canvas的子对象
        for (int i = 0; i < transform.childCount; i++)
        {
            //检测除了inGameUI和按键提示UI之外的UI
            if (transform.GetChild(i).gameObject != interactToolTipUI && transform.GetChild(i).gameObject != inGameUI)
            {
                if(transform.GetChild(i).gameObject.activeSelf)
                    return true;
            }
        }
        return false;
    }
    #endregion

    #region Interact
    public void SetWhetherShowInteractToolTip(bool _bool)
    //此函数用于决定是否显示按键提示UI
    {
        isShowInteractToolTip = _bool;
    }
    public void CheckWhtherShowInteractToolTip()
    {
        //是否显示按键提示UI
        interactToolTipUI.SetActive(isShowInteractToolTip);
    }
    #endregion
}