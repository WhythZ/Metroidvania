using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerAttackState : EnemyState
{
    private BossBringer bossBringer;

    public BossBringerAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.bossBringer = _bossBringer;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter AttackState");
        //�����״̬��ֹ
        bossBringer.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit AttackState");
        //ÿ�ι������ˢ�¹�����ȴ
        bossBringer.AttackCooldownRefresher();
    }

    public override void Update()
    {
        base.Update();

        //����ʱ���Ҷ���Ҳ���ɱ�����
        bossBringer.SetVelocity(0, 0);

        //����һ�κ󷵻�battleState
        if (stateActionFinished)
        {
            Debug.Log("battleState");
            bossBringer.stateMachine.ChangeState(bossBringer.battleState);
        }

        //�ڹ���״̬�����������������canBeStunned��true״̬���л�״̬��stunnedState
        //�ⲿ��ת��״̬�Ĵ�����BossBringer�ű�����д��WhetherCanBeStunned����ִ�У��������߼����ԣ�������������˵��һ�£���ֹ�Ҳ���
        //if (bossBringer.WhetherCanBeStunned){...}
    }
}
