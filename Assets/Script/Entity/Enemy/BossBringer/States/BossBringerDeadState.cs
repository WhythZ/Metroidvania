using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerDeadState : EnemyState
//��������ҪExit����ֻ��Enter��Update����
{
    private BossBringer bossBringer;

    public BossBringerDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.bossBringer = _bossBringer;
    }

    public override void Enter()
    {
        base.Enter();
        
        #region AbandonedDesign
        /*//��Ϊ����Ϸ��Ƶĵ�������û�����⶯�������ǲ�ȡ��������������ĵ���������������һ��Ȼ�������ͼ��Ч��
        //��������״̬ʱ��ǿ�ƽ���һ��״̬��AnimBoolName����Ϊ�棬���������ϸ�״̬�Ķ���
        bossBringer.anim.SetBool(bossBringer.lastAnimBoolName, true);
        
        //���ö����Ĳ����ٶ�Ϊ0����ֹͣ���ţ�������һ֡
        bossBringer.anim.speed = 0;

        //�ر�ʵ�����ײ
        bossBringer.cd.enabled = false;

        //����������һ�����ϵ��ٶȵĳ���ʱ�䣬�¼�������ͻ���Ϊ������׹��ȥ
        stateTimer = 0.1f;*/
        #endregion
    }

    public override void Update()
    {
        base.Update();

        //��
        bossBringer.SetVelocity(0, 0);
    }
}
