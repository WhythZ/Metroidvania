using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerGroundedState : EnemyState
{
    //����Ϊ����Enemy������һ�����֣�protectedΪ�˸�idle��move״̬�̳У�һ��private����
    protected BossBringer bossBringer;

    public BossBringerGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        //ע������ഫ����һ���ض��������BossBringer�Ĳ���������Ϊ����Enemy������һ�����֣������ڴ���BossBringerIdle��ʱҪ�ഫ����������������
        this.bossBringer = _bossBringer;
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
    }
}
