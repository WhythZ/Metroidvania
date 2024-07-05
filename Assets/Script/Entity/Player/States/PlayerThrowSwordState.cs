using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowSwordState : PlayerState
{
    public PlayerThrowSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //瞄准时静止
        player.SetVelocity(0, 0);

        //动作结束后离开此状态，此状态末尾是有伤害判定的
        if(stateActionFinished)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
