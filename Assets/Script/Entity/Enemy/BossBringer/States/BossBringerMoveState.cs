using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerMoveState : BossBringerGroundedState
{
    public BossBringerMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName, _bossBringer)
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

        //�����ƶ��ٶ�
        bossBringer.SetVelocity(bossBringer.moveSpeed * bossBringer.facingDir, rb.velocity.y);

        //�������ǽ�ڻ������£�����ĵ������߷��ڹ����ǰ��һ�㣩����ת��
        if(bossBringer.isWall || !bossBringer.isGround)
        {
            bossBringer.Flip();

            //�л���վ��״̬����վ��ʱ��idleStayTime��ȥ�������ʼ�ƶ������л��Ļ��ᷴ��Flip������֣�
            //bossBringer.stateMachine.ChangeState(bossBringer.idleState);
            bossBringer.stateMachine.ChangeState(bossBringer.teleportState);
    }

        //������Һ󣬻�����Ȼ��������״̬������BattleState
        if (bossBringer.isPlayer || bossBringer.shouldEnterBattle)
        {
            //��������ģʽ��ֻ�г������뷶Χ��ʱ��Ż��false
            bossBringer.shouldEnterBattle = true;
            //����battle
            bossBringer.stateMachine.ChangeState(bossBringer.battleState);
        }
    }
}
