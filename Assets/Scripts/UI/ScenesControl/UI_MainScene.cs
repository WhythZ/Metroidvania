using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class UI_MainScene : MonoBehaviour, ISavesManager
//此脚本放在Canvas上，用于控制各UI的切换
{
    public static UI_MainScene instance;

    #region ToolTip
    [Header("ToolTip")]
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
    [Header("Menus")]
    //记录各UI，以便使用按键切换
    public GameObject inGameUI;
    public GameObject characterUI;
    [SerializeField] private GameObject skillsUI;
    [SerializeField] private GameObject optionsUI;
    public GameObject cdPlayerUI;
    #endregion

    #region FadeScreen
    [Header("FadeScreen")]
    public GameObject fadeScreen;
    public GameObject deathText;
    public GameObject reSpawnButton;
    #endregion

    #region Settings
    [Header("Settings")]
    //储存音量大小设置的列表
    [SerializeField] private UI_VolumeSlider[] volumeSettings;
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
        //游戏主场景的bgm
        AudioManager.instance.isPlayBGM = true;
        AudioManager.instance.bgmIndex = 0;

        //初始状态打开的是游戏内界面UI
        SwitchToUI(inGameUI);

        //防止在不需要的时候打开
        statToolTip.gameObject.SetActive(false);
        itemToolTip.gameObject.SetActive(false);
        deathText.gameObject.SetActive(false);
        reSpawnButton.gameObject.SetActive(false);

        //播放开始时候的渐入动画，以及保证黑屏组件的激活状态
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<UI_FadeScreen>().FadeIn();
    }

    private void Update()
    {
        //按键控制UI的检测
        UIWithKeyController();
        //按键提示UI的检测
        CheckWhtherShowInteractToolTip();
    }

    #region UIControl
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
            //保证按键提示UI可以正常显示；要防止fadeScreen被直接关闭而不能激活屏幕fade的相关动画
            if (transform.GetChild(i).gameObject != interactToolTipUI && transform.GetChild(i).gameObject != fadeScreen)
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
            AudioManager.instance.PlaySFX(8, null);
        }

        #region GamePause
        //打开UI时暂停游戏
        if(GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
        #endregion
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
            //检测不允许做除了移动动作外的动作的UI
            if (transform.GetChild(i).gameObject == characterUI || transform.GetChild(i).gameObject == skillsUI || transform.GetChild(i).gameObject == optionsUI || transform.GetChild(i).gameObject == cdPlayerUI)
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

    #region Death
    public void PlayDeathText()
    {
        StartCoroutine(DeathScreenAnimation());
    }
    IEnumerator DeathScreenAnimation()
    {
        //死亡提示
        yield return new WaitForSeconds(1.5f);
        deathText.SetActive(true);
        //重生按钮
        yield return new WaitForSeconds(2.5f);
        reSpawnButton.SetActive(true);
    }
    //重新加载场景的函数，即重生
    public void ReStartGame() => GameManager.instance.RestartScene();
    #endregion

    #region SwitchScene
    //切换到游戏开始界面
    public void SwitchToMainMenu() => GameManager.instance.SwitchToMainMenu();
    #endregion

    #region ISaveManager
    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> _pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider _slider in volumeSettings)
            {
                if (_slider.parameter == _pair.Key)
                    _slider.LoadSlider(_pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider _slider in volumeSettings)
        {
            _data.volumeSettings.Add(_slider.parameter, _slider.slider.value);
        }    
    }
    #endregion
}