using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerUntouchedState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //进入跳跃状态时，给一个瞬间的向上的速度，这个速度只需要给一次，故而不需要放在Update内一直更新
        player.SetVelocity(rb.velocity.x, player.jumpForce);

        //进入一次跳跃状态，可跳跃次数减一
        player.jumpNum--;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //当人物在跳跃上升时，若不想等到下坠才能二段跳，再按一下可以快速上升
        if (Input.GetKeyDown(KeyCode.Space) && player.jumpNum > 0)
        {
            player.stateMachine.ChangeState(player.jumpState);
        }

        //若向上的速度为负，则转换为下落状态
        if (rb.velocity.y < 0)
        {
            player.stateMachine.ChangeState(player.airState);
        }

        //跳跃过程中也可以左右移动，但是会慢一点
        player.SetVelocity(xInput * player.airMoveSpeedRate * player.moveSpeed, rb.velocity.y);
    }
}
