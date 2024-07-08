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
        
        //վ��ʱ��
        stateTimer = bossBringer.pauseTime;

        //�����״̬��ֹ
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
        
        //����վ����һ����Զ���ʼ�ƶ�
        if (bossBringer.isGround && stateTimer < 0)
        {
            bossBringer.stateMachine.ChangeState(bossBringer.moveState);
        }

        //������Һ󣬻�����Ȼ��������״̬������BattleState
        if (bossBringer.isPlayer || bossBringer.shouldEnterBattle)
        {
            if (stateTimer < 0)
            {
                //�������ڹ���ı�������Ҫת��
                if (bossBringer.battleMoveDir != bossBringer.facingDir)
                {
                    bossBringer.Flip();
                }
                //��������˾������һ���ľ��뷶Χ�ڣ����ҹ�����ȴ�������㷢������
                if (bossBringer.enterAttackRegion && bossBringer.canAttack)
                {
                    bossBringer.stateMachine.ChangeState(bossBringer.attackState);
                }
                else
                {
                    //��������ģʽ��ֻ�г������뷶Χ��ʱ��Ż��false
                    bossBringer.shouldEnterBattle = true;
                    //����battle
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
