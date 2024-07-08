using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBringerDeadState : EnemyState
//死亡后不需要Exit，故只有Enter和Update函数
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
        /*//因为本游戏设计的敌人死亡没有特殊动画，而是采取像是马里奥那样的滴人死亡后向上跳一下然后落出地图的效果
        //进入死亡状态时，强制将上一个状态的AnimBoolName设置为真，即保留了上个状态的动画
        bossBringer.anim.SetBool(bossBringer.lastAnimBoolName, true);
        
        //设置动画的播放速度为0，即停止播放，保留第一帧
        bossBringer.anim.speed = 0;

        //关闭实体的碰撞
        bossBringer.cd.enabled = false;

        //给敌人设置一个向上的速度的持续时间，事件结束后就会因为重力下坠出去
        stateTimer = 0.1f;*/
        #endregion
    }

    public override void Update()
    {
        base.Update();

        //别动
        bossBringer.SetVelocity(0, 0);
    }
}
