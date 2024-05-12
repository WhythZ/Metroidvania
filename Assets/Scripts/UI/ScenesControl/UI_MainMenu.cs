using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//管理场景的using
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "MainScene";

    //用于储存Continue Game的按钮，在没有存档的时候隐藏这个按钮
    [SerializeField] private GameObject continueButton;

    //用于连接到黑屏这个组件，便于调用相关渐入渐出动画
    [SerializeField] UI_FadeScreen fadeScreen;

    private void Start()
    {
        if (SavesManager.instance.WhetherHasSavedGameData() == false)
        {
            //没有存档的时候隐藏继续游戏的按钮
            continueButton.SetActive(false);
        }
        else
        {
            continueButton.SetActive(true);
        }
    }

    public void ContinueGame()
    //继续游戏的函数，沿用保存好的存档；函数用于绑定给Button
    {
        //加载游戏场景
        SceneManager.LoadScene(nextSceneName);
    }

    public void NewGame()
    //开始新的游戏
    {
        //删除存档
        SavesManager.instance.DeleteSavedGameDate();

        //加载游戏场景
        SceneManager.LoadScene(nextSceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Game Exited");
        //Application.Quit();
    }
}
