using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
//继承自更大的状态：人物在地面的状态
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    //人物的站立状态；此构造函数主要目的是调用基类的构造函数，不需额外的初始化操作；
    //其中base关键字调用了基类PlayerState的构造函数，传入三个参数，旨在在创建PlayerIdle对象时，首先执行基类PlayerState的构造函数，确保基类初始化的完成
    {
    }

    //下列是对PlayerState中一系列重要操作的重写
    public override void Enter()
    {
        base.Enter();

        //进入站立状态就别动
        player.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    //这个函数不断地在被Player中的Update函数更新，所以同样一直在更新
    {
        base.Update();

        //对面对着墙壁的情况做单独的判断：向着墙壁无法转移到Move，不动则保持静止，反走则可以进入Move
        if (player.isWall)
        {
            //若是向着墙壁则无法走到；特判xInput为零的时候也不动，否则站着不动出问题
            if(player.facingDir == xInput || xInput == 0)
            {
                //直接停止，无需判断是否执行最外层if往下的内容
                return;
            }
            //反着墙壁走
            else if(player.facingDir * xInput < 0)
            {
                //不知为何，总是要帮助人物进行一次手动翻转
                player.Flip();
                player.stateMachine.ChangeState(player.moveState);
            }
        }

        //当在地上且x水平方向有输入的时候才进入移动状态
        if(xInput != 0 && player.isGround)
        {
            //通过自己从PlayerState继承来的成员player（这个player由于被Plaer.cs初始化的时候链接到Player.cs）转换状态到移动状态
            player.stateMachine.ChangeState(player.moveState);
        }
    }
}
