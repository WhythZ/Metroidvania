using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//注意
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        //确保管理器仅有一个
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public void RestartScene()
    {
        //开始的时候加载游戏存档，由于SavesManager组件是一直存在的Prefab，所以重新加载场景的时候其Start函数不会被调用，故需要在这里调用一次
        SavesManager.instance.LoadGame();

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
}
