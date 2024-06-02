using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//管理场景的using
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    //用于储存Continue Game的按钮，在没有存档的时候隐藏这个按钮
    [SerializeField] private GameObject continueButton;

    //用于连接到黑屏这个组件，便于调用相关渐入渐出动画
    [SerializeField] GameObject fadeScreen;

    private void Start()
    {
        //开始菜单的场景bgm
        AudioManager.instance.isPlayBGM = true;
        AudioManager.instance.bgmIndex = 0;

        //播放开始时候的渐入动画，以及保证黑屏组件的激活状态
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<UI_FadeScreen>().FadeIn();

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
        SceneManager.LoadScene("MainScene");
    }

    public void NewGame()
    //开始新的游戏
    {
        //删除存档
        SavesManager.instance.DeleteSavedGameDate();

        //加载游戏场景
        SceneManager.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        Debug.Log("Game Exited");
        //Application.Quit();
    }
}
