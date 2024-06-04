using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerIdleState : BringerGroundedState
{
    public BringerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Bringer _bringer) : base(_enemyBase, _stateMachine, _animBoolName, _bringer)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //站立时间
        stateTimer = bringer.pauseTime;

        //进入该状态后静止
        bringer.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //怪物站立了一会后自动开始移动
        if(bringer.isGround && stateTimer < 0)
        {
            bringer.stateMachine.ChangeState(bringer.moveState);
        }

        //看到玩家后，或者仍然处于索敌状态，进入BattleState
        if (bringer.isPlayer || bringer.shouldEnterBattle)
        {
            if (stateTimer < 0)
            {
                //如果玩家在怪物的背后，则需要转身
                if (bringer.battleMoveDir != bringer.facingDir)
                {
                    bringer.Flip();
                }
                //如果到达了距离玩家一定的距离范围内，并且攻击冷却结束，便发动攻击
                if (bringer.enterAttackRegion && bringer.canAttack)
                {
                    bringer.stateMachine.ChangeState(bringer.attackState);
                }
                else
                {
                    //进入索敌模式，只有超出距离范围或时间才会变false
                    bringer.shouldEnterBattle = true;
                    //进入battle
                    bringer.stateMachine.ChangeState(bringer.battleState);
                }
            }
        }
    }
}
