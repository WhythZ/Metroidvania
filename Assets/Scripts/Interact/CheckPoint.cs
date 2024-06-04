using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator anim;

    //代表特定存档点的标识
    public string id;

    //记录这个存档点是否被激活过
    public bool isActive;

    //这个函数每次调用都会生成一个新的ID，所以只需要借助ContextMenu调用一次
    //在Unity内该脚本对象上右键该脚本组件，点击"Generate CheckPoint ID"即可调用此函数
    [ContextMenu("Generate CheckPoint ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //必须是玩家，而非别的什么怪物都能触发
        if (collision.GetComponent<Player>() != null)
        {
            //表示人物处于可触发交互界面的区域内，显示按键提示
            UI_MainScene.instance.SetWhetherShowInteractToolTip(true);
            //表示现在接触的可交互物是重生点
            UI_MainScene.instance.isAtCheckPoint = true;
            //给予自己的本身以便其操作
            UI_MainScene.instance.touchedCheckPoint = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            //关闭按键提示
            UI_MainScene.instance.SetWhetherShowInteractToolTip(false);
            UI_MainScene.instance.isAtCheckPoint = false;
            //销毁关于对方链接自己的权限
            UI_MainScene.instance.touchedCheckPoint = null;
        }
    }

    public void ActivateCheckPoint()
    {
        //激活的音效
        //AudioManager.instance.PlaySFX()
        
        //记录激活的状态
        isActive = true;

        //激活的动画
        anim.SetBool("active", true);        
    }
}
