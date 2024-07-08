using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerStunnedState : EnemyState
{
    private BossBringer bossBringer;

    public BossBringerStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.bossBringer = _bossBringer;
    }

    public override void Enter()
    {
        base.Enter();

        //�����ʱ��ѣ��ʱ��
        stateTimer = bossBringer.stunnedDuration;

        //����һ���������������RedBlink����һֱ��Invoke���ã��ڶ������������ӳ٣�delay����ú��һ��ִ������������������Ǻ����ͷż��
        bossBringer.fx.InvokeRepeating("RedBlink", 0, 0.1f);
    }

    public override void Exit()
    {
        base.Exit();

        //�뿪ʱȡ����ɫ���⣻�ڶ������������ӳٵ��ô˺�������˼
        bossBringer.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();

        //��������ʱ��ʵ���ϱ���ҹ������ˣ��ᴥ��knockback�����ʱ��λ��ʮ�ֹ��죬�������õĻ��ͱ�������������߹켣�ˣ�����������
        bossBringer.SetVelocity(0, 0);

        //ѣ�ν��������idle
        if(stateTimer < 0)
            bossBringer.stateMachine.ChangeState(bossBringer.idleState);
    }
}
