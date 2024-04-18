using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerUntouchedState
//在空中自由落体的状态
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        //此变量仅在从墙跳状态进入本状态时变为真，完成air后立刻需要变为假
        player.FromWallJumpToAirStateSetting(false);
    }

    public override void Update()
    {
        base.Update();

        //当人物在坠落时，若剩余可跳跃次数为1，才可进行二段跳
        if(Input.GetKeyDown(KeyCode.Space) && player.jumpNum == 1)
        {
            player.stateMachine.ChangeState(player.jumpState);
        }

        //若在地面上，则转化为站立状态
        if(player.isGround)
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

        //如果是从墙跳状态转移过来本状态的，则需要保持原有水平速度，若按了A/D则结束这种保持
        if (player.isFromWallJumpToAirState)
        {
            //此间如果手动按了A/D，则提前进入手动控制模式，不继承原有水平速度
            if(xInput != 0)
            {
                //本来是要等air结束后才变假，此时提前变假
                player.FromWallJumpToAirStateSetting(false);
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
