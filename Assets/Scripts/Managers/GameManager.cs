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
}
