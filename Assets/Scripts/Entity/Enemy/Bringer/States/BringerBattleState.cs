using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BringerBattleState : EnemyState
{
    private Bringer bringer;

    public BringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Bringer _bringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.bringer = _bringer;
    }

    public override void Enter()
    {
        base.Enter();

        //进入状态设置速度为零，防止被击退后落地改变朝向
        bringer.SetVelocity(0, 0);

        //定义多久未攻击后脱战，进入idle状态
        stateTimer = bringer.quitBattleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        #region ApproachPlayer
        //如果玩家在怪物的背后，则需要转身
        if (bringer.battleMoveDir != bringer.facingDir)
        {
            bringer.Flip();
        }
        //不一定非要走到完全和玩家重合的位置，预留一点空间，大概是attackCheckRadius的0.65倍左右
        if ((rb.position.x < (PlayerManager.instance.player.transform.position.x + bringer.attackCheckRadius * 0.65f)) && (rb.position.x > (PlayerManager.instance.player.transform.position.x - bringer.attackCheckRadius * 0.65f)))
        {
            //在范围内得先停下再转换到站立状态，不然会直接走到重合
            bringer.SetVelocity(0, 0);
            //人物在这个范围内的时候，保持站立
            bringer.stateMachine.ChangeState(bringer.idleState);
        }
        else
        {
            {
                //防止敌人与玩家走的太近
                if(!bringer.enterAttackRegion)
                {
                    //赋予朝向玩家的移动速度
                    bringer.SetVelocity(bringer.moveSpeed * bringer.fasterSpeedInBattle * bringer.battleMoveDir, rb.velocity.y);
                }
                else
                {
                    //防止在battleState里原地踏步
                    bringer.stateMachine.ChangeState(bringer.idleState);
                }
            }
        }
        #endregion

        #region Attack
        //如果到达了距离玩家一定的距离范围内，并且攻击冷却结束，便发动攻击
        if (bringer.enterAttackRegion && bringer.canAttack)
        {
            bringer.stateMachine.ChangeState(bringer.attackState);
        }
        //如果人物在可攻击半径内，则给予其碰撞伤害
        //if(PlayerManager.instance.transform.position)
        #endregion

        #region CaseThatQuitBattle
        //如果遇到悬崖，则脱战进入idle
        if (!bringer.isGround)
        {
            bringer.stateMachine.ChangeState(bringer.idleState);
        }
        //愤怒时间到了后，或者玩家距离超出范围，则脱战
        if (stateTimer < 0 || Vector2.Distance(bringer.transform.position, PlayerManager.instance.player.transform.position) > bringer.GetQuitBattleDisance())
        {
            //关闭索敌状态
            bringer.shouldEnterBattle = false;
            //回到站立状态
            bringer.stateMachine.ChangeState(bringer.idleState);
        }
        #endregion
    }
}
