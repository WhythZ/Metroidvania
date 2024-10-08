using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//注意
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISavesManager
{
    public static GameManager instance;

    #region CheckPoint
    [Header("CheckPoint")]
    //储存存档点的列表
    [SerializeField] private CheckPoint[] checkpointsList;
    //储存上一次休息的存档点
    public string lastRestCheckPointID = "";
    #endregion

    #region AbilityActivator
    [Header("Ability Activator")]
    //储存解锁人物能力的关键道具
    [SerializeField] private GameObject wallslideActivator;
    [SerializeField] private GameObject dashActivator;
    [SerializeField] private GameObject throwswordActivator;
    [SerializeField] private GameObject fireballActivator;
    [SerializeField] private GameObject iceballActivator;
    [SerializeField] private GameObject blackholeActivator;
    #endregion

    private void Awake()
    {
        //确保管理器仅有一个
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        //建议手动拖动赋值，因为存档点的录入必须在存档加载之前完成，不然无法读取
        //获取所有（当前场景中的？）存档点
        //checkpointsList = FindObjectsOfType<CheckPoint>();
    }

    private void Start()
    {
        #region CheckAbility
        //根据角色的能力激活权限来判断这些激活能力的关键道具是否应该出现
        if (!PlayerManager.instance.ability_CanWallSlide) { wallslideActivator.gameObject.SetActive(true); } else { wallslideActivator.gameObject.SetActive(false);}
        if (!PlayerManager.instance.ability_CanDash) { dashActivator.gameObject.SetActive(true); } else { dashActivator.gameObject.SetActive(false);}
        if (!PlayerManager.instance.ability_CanThrowSword) { throwswordActivator.gameObject.SetActive(true); } else { throwswordActivator.gameObject.SetActive(false);}
        if (!PlayerManager.instance.ability_CanFireBall) { fireballActivator.gameObject.SetActive(true); } else { fireballActivator.gameObject.SetActive(false);}
        if (!PlayerManager.instance.ability_CanIceBall) { iceballActivator.gameObject.SetActive(true); } else { iceballActivator.gameObject.SetActive(false);}
        if (!PlayerManager.instance.ability_CanBlackhole) { blackholeActivator.gameObject.SetActive(true); } else { blackholeActivator.gameObject.SetActive(false);}
        #endregion
    }

    #region Scenes
    public void RestartScene()
    {
        //自动保存
        SavesManager.instance.SaveGame();

        //结束死亡音效
        AudioManager.instance.StopSFX(10);

        //获取当前激活的场景
        Scene _scene = SceneManager.GetActiveScene();

        //重新加载当前场景
        SceneManager.LoadScene(_scene.name);
    }

    public void SwitchToMainMenu()
    {
        //自动保存一下，否则数据会不一致
        SavesManager.instance.SaveGame();

        //加载游戏开始界面
        SceneManager.LoadScene("MainMenu");
    }

    public void SwitchToMainScene()
    {
        //自动保存一下，否则数据会不一致
        SavesManager.instance.SaveGame();

        //加载游戏开始界面
        SceneManager.LoadScene("MainScene");
    }
    #endregion

    #region Pause
    public void PauseGame(bool _pause)
    //暂停游戏
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    #endregion

    #region ClosestCP
    public CheckPoint FindClosestCheckPoint()
    //返回和玩家距离最近的已激活存档点
    {
        float _closestDistance = Mathf.Infinity;
        CheckPoint _closestCP = null;

        foreach (var _cp in checkpointsList)
        {
            float _distanceToCP = Vector2.Distance(PlayerManager.instance.player.transform.position, _cp.transform.position);

            if (_distanceToCP < _closestDistance && _cp.isActive)
            {
                _closestDistance = _distanceToCP;
                _closestCP = _cp;
            }
        }
        return _closestCP;
    }
    #endregion

    #region ISaveManager
    public void LoadData(GameData _data)
    {
        #region CheckPoint
        //读取存档点的列表，包括激活与否的信息
        foreach (KeyValuePair<string, bool> _pair in _data.checkpointsDict)
        {
            foreach(CheckPoint _checkpoint in checkpointsList)
            {
                if(_checkpoint.id == _pair.Key && _pair.Value == true)
                    _checkpoint.ActivateCheckPoint();
            }
        }

        //读取玩家上次休息的存档点
        lastRestCheckPointID = _data.lastRestCPID;

        //通过存储的id锁定玩家上次休息的存档点
        foreach (CheckPoint _cp in checkpointsList)
        {
            //把玩家定位在此处
            if (_data.lastRestCPID == _cp.id)
            {
                //生成位置在其上方一点，防止卡在地底下
                GameObject.Find("Player").transform.position = _cp.transform.position + new Vector3(0, 2, 0);
            }
        }

        /*//通过存储的id锁定距离玩家最近的已激活存档点
        foreach(CheckPoint _cp in checkpointsList)
        {
            //把玩家定位在此处
            if(_data.closestCheckPointID == _cp.id)
            {
                //生成位置在其上方一点，防止卡在地底下
                GameObject.Find("Player").transform.position = _cp.transform.position + new Vector3(0, 2, 0);
            }
        }*/
        #endregion
    }
    public void SaveData(ref GameData _data)
    {
        //以防万一，先清除再存储
        _data.checkpointsDict.Clear();

        #region CheckPoint
        //储存存档点的列表，包括激活与否的信息
        foreach (CheckPoint _checkpoint in checkpointsList)
        {
            _data.checkpointsDict.Add(_checkpoint.id, _checkpoint.isActive);
        }
        //储存上一次休息的存档点
        if (lastRestCheckPointID != null)
        {
            _data.lastRestCPID = lastRestCheckPointID;
            //Debug.Log("Save lastRestCPID As " + lastRestCheckPointID);
        }
        if (lastRestCheckPointID == null)
            _data.lastRestCPID = "";

        /*//储存保存游戏的时候距离玩家最近的已激活存档点，而不是直接存储一个CheckPoint类型的数据，只有字符串等基本数据类型才能被存储在存档文本文件里
        if (FindClosestCheckPoint() != null)
            _data.closestCheckPointID = FindClosestCheckPoint().id;
        if (FindClosestCheckPoint() == null)
            _data.closestCheckPointID = "";*/
        #endregion
    }
    #endregion
}
