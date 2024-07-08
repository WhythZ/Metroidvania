using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerGroundedState : EnemyState
{
    //这是为了与Enemy基类做一个区分；protected为了给idle和move状态继承；一般private即可
    protected BossBringer bossBringer;

    public BossBringerGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        //注意这里多传入了一个特定怪物类别BossBringer的参数，这是为了与Enemy基类做一个区分；所以在创建BossBringerIdle类时要多传入自身来填补这个参数
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
