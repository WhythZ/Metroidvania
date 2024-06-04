using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerGroundedState : EnemyState
{
    //这是为了与Enemy基类做一个区分；protected为了给idle和move状态继承；一般private即可
    protected Bringer bringer;

    public BringerGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Bringer _bringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        //注意这里多传入了一个特定怪物类别Bringer的参数，这是为了与Enemy基类做一个区分；所以在创建BringerIdle类时要多传入自身来填补这个参数
        this.bringer = _bringer;
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
