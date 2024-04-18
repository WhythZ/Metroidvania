using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //登墙跳反向水平加速持续时长
        stateTimer = player.wallJumpReverseSpeedDuration;

        //翻转一下，防止螺旋升天；我也不知道为什么要这样，但是不这样就会出bug
        player.Flip();
        //Debug.Log("WallJump中的Flip被调用");
    }

    public override void Exit()
    {
        base.Exit();

        //仅在此状态后变为真，下一状态airState后立刻赋值为假
        player.FromWallJumpToAirStateSetting(true);
    }

    public override void Update()
    {
        base.Update();

        //给予反向墙壁的水平速度和竖直跳跃速度，由于在Enter处已经Flip翻转过了，所以moveSpeed乘上的不是负的facingDir
        player.SetVelocity(player.moveSpeed * player.facingDir * 0.5f, player.jumpForce * 0.8f);

        //时间到了后进入坠落模式
        if(stateTimer < 0)
        {
            player.stateMachine.ChangeState(player.airState);
        }
    }
}
