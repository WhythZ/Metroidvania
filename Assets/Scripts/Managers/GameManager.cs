using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//注意
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISavesManager
{
    public static GameManager instance;
    //储存存档点的列表
    [SerializeField] private CheckPoint[] checkpointsList;

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

    #region ISaveManager
    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string, bool> _pair in _data.checkpointsDict)
        {
            foreach(CheckPoint _checkpoint in checkpointsList)
            {
                if(_checkpoint.ID == _pair.Key && _pair.Value == true)
                    _checkpoint.ActivateCheckPoint();
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        //以防万一，先清除再存储
        _data.checkpointsDict.Clear();

        foreach(CheckPoint _checkpoint in checkpointsList)
        {
            _data.checkpointsDict.Add(_checkpoint.ID, _checkpoint.isActive);
        }
    }
    #endregion
}
