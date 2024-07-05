using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SlimeIdleState : SlimeGroundedState
{
    public SlimeIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName, _slime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //站立时间
        stateTimer = slime.pauseTime;

        //进入该状态后静止
        slime.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //怪物站立了一会后自动开始移动
        if (slime.isGround && stateTimer < 0)
        {
            slime.stateMachine.ChangeState(slime.moveState);
        }

        //看到玩家后，或者仍然处于索敌状态，进入BattleState
        if (slime.isPlayer || slime.shouldEnterBattle)
        {
            if (stateTimer < 0)
            {
                //如果玩家在怪物的背后，则需要转身
                if (slime.battleMoveDir != slime.facingDir)
                {
                    slime.Flip();
                }
                //如果到达了距离玩家一定的距离范围内，并且攻击冷却结束，便发动攻击
                if (slime.enterAttackRegion && slime.canAttack)
                {
                    slime.stateMachine.ChangeState(slime.attackState);
                }
                else
                {
                    //进入索敌模式，只有超出距离范围或时间才会变false
                    slime.shouldEnterBattle = true;
                    //进入battle
                    slime.stateMachine.ChangeState(slime.battleState);
                }
            }
        }
    }
}
