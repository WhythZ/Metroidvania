using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class playerWallSlideState : PlayerState
{
    public playerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //每次碰触地面或者墙壁后，则可以继续在冷却结束后进行冲刺
        player.CanDashSetting(true);
    }

    public override void Exit()
    {
        base.Exit();

        //每次碰到墙后可跳跃次数刷新为2，但是由于wallJumpState会进行一次跳跃，故刷新为1
        player.jumpNum = 1;
    }

    public override void Update()
    {
        base.Update();

        //如果滑墙过程中按了跳跃键，则进入墙跳状态
        //当按下空格键时，Update内所有命令会在同时间执行，导致反向跳的速度会触发进入airState并Flip的条件
        //这样会导致螺旋升天，故而把此命令放在Update函数的最开始，触发之后即结束
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.stateMachine.ChangeState(player.wallJumpState);
            //用return跳出命令，防止下列命令被执行
            return;
        }

        //滑动速度慢于普通坠落速度
        if (yInput < 0)
        {
            //可以通过按S键提升滑动速度
            player.SetVelocity(0, rb.velocity.y * player.biggerSlideSpeed);
        }
        else
        {
            //滑动速度慢，乘以滑动速度倍率
            player.SetVelocity(0, rb.velocity.y * player.slideSpeed);
        }

        #region OtherCasesToQuitWallSlideState
        //如果着陆了，则进入Idle状态
        if (player.isGround)
        {
            player.stateMachine.ChangeState(player.idleState);
        }

        //若是滑出了墙壁范围，则进入airState
        if (!player.isWall && !player.isGround)
        {
            player.stateMachine.ChangeState(player.airState);
        }

        /*暂不提供除了跳跃和脱出墙壁外中途离开滑墙的方法，除非提供其他按键
        //如果滑墙过程中xInput为反向面朝方向，且没有按跳跃键，则在转向后进入airState
        if(xInput != 0 && (player.facingDir != xInput) && !Input.GetKeyDown(KeyCode.Space))
        {
            //这个转向至关重要！因为Flip函数里并没有考虑到滑墙的情况，只考虑了水平有速度时候的转向
            player.Flip();
            //Debug.Log("WallSlide中的Flip被调用");

            player.stateMachine.ChangeState(player.airState);
        }
        */
        #endregion
    }
}
