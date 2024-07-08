using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerSpellCastState : EnemyState
{
    private BossBringer bossBringer;

    public BossBringerSpellCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
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
