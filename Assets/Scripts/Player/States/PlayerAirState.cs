using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerUntouchedState
//在空中自由落体的状态
{
    //我需要在按了一次A/D键位后便不能再保持墙跳的速度了，而不是按了一次后再松开还能继续保持
    private bool keepWallJumpVelocity;

    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //默认是需要保持的
        keepWallJumpVelocity = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        #region DoubleJump
        //当人物从GroundedState直接进入AirState时，给予跳跃次数为1
        if(player.stateMachine.formerState == player.idleState || player.stateMachine.formerState == player.moveState)
        {
            //跳跃次数为2的话（其实是废话，但是为了以防万一）
            if (player.jumpNum == 2)
            {
                player.jumpNum = 1;
            }
        }
        //当人物在坠落时，若剩余可跳跃次数为1（即进行过一次跳跃），可进行二段跳
        if (Input.GetKeyDown(KeyCode.Space) && player.jumpNum == 1)
        {
            player.stateMachine.ChangeState(player.jumpState);
        }
        #endregion

        //若在地面上，则转化为站立状态
        if (player.isGround)
        {
            player.stateMachine.ChangeState(player.idleState);
        }

        //坠落的时候如果蹭着墙，则进入滑墙状态
        if (player.isWall && !player.isGround)
        {
            //你总不能面向左边的墙壁按了A还能进入滑墙状态吧；注意这里是GetKey而不是GetKeyDown
            if( (Input.GetKey(KeyCode.A) && player.facingDir == -1) || (Input.GetKey(KeyCode.D) && player.facingDir == 1) )
            {
                player.stateMachine.ChangeState(player.wallSlideState);
                //Debug.Log("Air to WallSlide");
            }
        }

        //如果是从墙跳状态转移过来本状态的，且许可保持原有速度，则需要保持原有水平速度，若按了A/D则结束这种保持
        if (player.stateMachine.formerState == player.wallJumpState && keepWallJumpVelocity)
        {
            //此间如果手动按了A/D，则提前进入手动控制模式，不继承原有水平速度
            if(xInput != 0)
            {
                //若有水平速度输入，则提前结束速度的保持
                keepWallJumpVelocity = false;
            }
            else
            {
                //保持原有水平速度
                player.SetVelocity(rb.velocity.x, rb.velocity.y);
            }
        }
        else
        {
            //下坠过程中通过A/D键左右移动
            player.SetVelocity(xInput * player.airMoveSpeedRate * player.moveSpeed, rb.velocity.y);
        }
    }
}
