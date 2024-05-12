using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
//继承自更大的状态：人物在地面的状态
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    //人物的移动状态
    {
    }

    public override void Enter()
    {
        base.Enter();

        //播放走路音效
        //AudioManager.instance.PlaySFX(1, null);
    }

    public override void Exit()
    {
        base.Exit();

        //结束走路音效
        //AudioManager.instance.StopSFX(1);
    }

    public override void Update()
    {
        base.Update();

        //用xInput * moveSpeed赋值给水平速度，用当前速度rb.velocity.y赋值竖直移动速度即保持原来竖直速度；当没有输入时，水平速度乘上值为0的xInput
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        //如果走着走着掉下悬架，即脚不着地，则进入airState
        if(!player.isGround)
        {
            player.stateMachine.ChangeState(player.airState);
        }

        //如果面向墙壁则转移到站立，在站立立应加一个判断：若面对着墙且向着墙走则不应转换到Move状态。不加的话则会导致Move和Idle来回不断切换
        if(player.isWall)
        {
            player.stateMachine.ChangeState(player.idleState);
        }

        //如果在地上且无水平输入，则进入站立状态
        if (player.isGround && xInput == 0)
        {
            //通过自己从PlayerState继承来的成员player（这个player由于被Plaer.cs初始化的时候链接到Player.cs）恢复到站立状态
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
