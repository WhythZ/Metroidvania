using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerIdleState : BossBringerGroundedState
{
    public BossBringerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName, _bossBringer)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        //站立时间
        stateTimer = bossBringer.pauseTime;

        //进入该状态后静止
        bossBringer.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    stateMachine.ChangeState(bossBringer.teleportState);
        //}
        
        //怪物站立了一会后自动开始移动
        if (bossBringer.isGround && stateTimer < 0)
        {
            bossBringer.stateMachine.ChangeState(bossBringer.moveState);
        }

        //看到玩家后，或者仍然处于索敌状态，进入BattleState
        if (bossBringer.isPlayer || bossBringer.shouldEnterBattle)
        {
            if (stateTimer < 0)
            {
                //如果玩家在怪物的背后，则需要转身
                if (bossBringer.battleMoveDir != bossBringer.facingDir)
                {
                    bossBringer.Flip();
                }
                //如果到达了距离玩家一定的距离范围内，并且攻击冷却结束，便发动攻击
                if (bossBringer.enterAttackRegion && bossBringer.canAttack)
                {
                    bossBringer.stateMachine.ChangeState(bossBringer.attackState);
                }
                else
                {
                    //进入索敌模式，只有超出距离范围或时间才会变false
                    bossBringer.shouldEnterBattle = true;
                    //进入battle
                    bossBringer.stateMachine.ChangeState(bossBringer.battleState);
                }
            }
        }
        else /*if (bossBringer.teleportTimer < 0)*/
        {
            Debug.Log("from idlestate");
            bossBringer.stateMachine.ChangeState(bossBringer.teleportState);
        }
        
    }
}
