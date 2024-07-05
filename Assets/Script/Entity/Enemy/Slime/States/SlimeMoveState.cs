using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMoveState : SlimeGroundedState
{
    public SlimeMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName, _slime)
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

        //赋予移动速度
        slime.SetVelocity(slime.moveSpeed * slime.facingDir, rb.velocity.y);

        //如果遇到墙壁或者悬崖（怪物的地面检测线放在怪物的前面一点），则转身
        if(slime.isWall || !slime.isGround)
        {
            slime.Flip();

            //切换至站立状态，等站立时间idleStayTime过去后继续开始移动（不切换的话会反复Flip，很奇怪）
            slime.stateMachine.ChangeState(slime.idleState);
        }

        //看到玩家后，或者仍然处于索敌状态，进入BattleState
        if (slime.isPlayer || slime.shouldEnterBattle)
        {
            //进入索敌模式，只有超出距离范围或时间才会变false
            slime.shouldEnterBattle = true;
            //进入battle
            slime.stateMachine.ChangeState(slime.battleState);
        }
    }
}
