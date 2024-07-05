using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private CapsuleCollider2D cd;
    private Animator anim;
    //绑定另一个传送门作为传送目的地
    public Portal teleportTarget;
    //是否处于可触发传送的状态，防止传送到目的地的传送门后立刻与该传送门碰撞后触发传送，导致无限传送
    public bool canTeleport;

    private void OnValidate()
    //此函数在Unity的Hierarchy内进行各种对象的操作时，就会进行调用，而不用等到开始测试游戏时才进行更新（即在Start函数内更新）
    {
        //两个传送门是相互绑定的
        if (teleportTarget != null)
            teleportTarget.teleportTarget = this;
    }

    private void Start()
    {
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();

        //初始化的时候设置可以触发传送
        canTeleport = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //不可传送则不发生下面的检测
        if (!canTeleport)
            return;

        if (collision.GetComponent<Player>() != null)
        {
            //暂时取消目标的传送许可，防止一过去就被传送走
            teleportTarget.canTeleport = false;

            //触发传送
            GameObject.Find("Player").transform.position = teleportTarget.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            //恢复自身的传送许可
            canTeleport = true;
        }
    }
}