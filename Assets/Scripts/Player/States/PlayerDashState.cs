using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //冲刺时间，只调用一次，故放在Enter
        stateTimer = player.dashDuration;

        //进入冲刺，则让canDash为假，在GroundedState中设置条件：只有当接触了地面或墙壁后此值才能恢复为真
        player.CanDashSetting(false);
    }

    public override void Exit()
    {
        base.Exit();

        //若保留此速度，可以减缓冲刺运动的水平速度，保留惯性感，乘上0.2f
        //player.SetVelocity(0.2f * rb.velocity.x, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        //当stateTimer降到负数后停止冲刺
        if (stateTimer > 0 && xInput != 0)
        {
            //冲刺时候要求竖直速度为零
            player.SetVelocity(xInput * player.dashSpeed, 0);
        }
        //在水平速度为零的时候朝面朝方向冲刺
        if (stateTimer > 0 && xInput == 0)
        {
            player.SetVelocity(player.facingDir * player.dashSpeed, 0);
        }

        //冲刺时间结束时，若是在地面上，则转换到IdleState；而若是在空中冲刺，则切换到AirState下坠状态
        if (player.isGround)
        {
            if (stateTimer <= 0)
            {
                player.stateMachine.ChangeState(player.idleState);
            }
        }
        else if(!player.isGround)
        {
            if (stateTimer <= 0)
            {
                player.stateMachine.ChangeState(player.airState);
            }
        }

        //与墙壁相关的判断
        if(!player.isGround && player.isWall)
        {
            //可以冲刺离开墙壁
            if(player.facingDir != xInput)
            {
                player.Flip();
            }
            //冲刺到墙上立即进入WallState而不是进入AirState再进入WallState
            else if(player.facingDir == xInput)
            {
                player.stateMachine.ChangeState(player.wallSlideState);
            }
        }

    }
}
