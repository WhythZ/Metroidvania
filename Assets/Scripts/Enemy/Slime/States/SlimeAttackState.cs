using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : EnemyState
{
    private Slime slime;

    public SlimeAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.slime = _slime;
    }

    public override void Enter()
    {
        base.Enter();

        //进入该状态后静止
        slime.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();

        //每次攻击完后，刷新攻击冷却
        slime.AttackCooldownRefresher();
    }

    public override void Update()
    {
        base.Update();

        //攻击时别乱动，也不可被击飞
        slime.SetVelocity(0, 0);


        //攻击一次后返回battleState
        if (stateActionFinished)
        {
            slime.stateMachine.ChangeState(slime.battleState);
        }

        //在攻击状态中若被弹反后进入了canBeStunned的true状态，切换状态到stunnedState
        //这部分转换状态的代码由Slime脚本内重写的WhetherCanBeStunned函数执行，但由于逻辑惯性，我在这里特意说明一下，防止找不到
        //if (slime.WhetherCanBeStunned){...}
    }
}
