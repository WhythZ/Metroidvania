using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CDPlayer : MonoBehaviour
{
    #region Components
    private BoxCollider2D cd;
    private Rigidbody rb;
    #endregion

    private void Awake()
    {
        cd = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    //当玩家与唱片机的碰撞箱接触时执行的语句
    {
        //必须是玩家，而非别的什么怪物都能触发
        if (collision.GetComponent<Player>()  != null)
        {
            //表示人物处于可触发交互界面的区域内，显示按键提示
            UI_MainScene.instance.SetWhetherShowInteractToolTip(true);
            //表示现在接触的可交互物是唱片机
            UI_MainScene.instance.isAtCDPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    //当玩家离开唱片机的碰撞箱范围时执行的语句
    {
        if (collision.GetComponent<Player>() != null)
        {
            //关闭按键提示
            UI_MainScene.instance.SetWhetherShowInteractToolTip(false);
            UI_MainScene.instance.isAtCDPlayer = false;

            //若离开时唱片机UI是开启的，则关闭
            //此处有点莫名其妙的bug...?
            if (UI_MainScene.instance.cdPlayerUI != null)
            {
                if (UI_MainScene.instance.cdPlayerUI.activeSelf)
                    UI_MainScene.instance.SwitchToUI(UI_MainScene.instance.inGameUI);
            }
        }
    }
}