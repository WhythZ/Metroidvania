using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BossBringerBattleState : EnemyState
{
    private BossBringer bossBringer;

    public BossBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, BossBringer _bossBringer) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.bossBringer = _bossBringer;
    }

    public override void Enter()
    {
        base.Enter();

        //进入状态设置速度为零，防止被击退后落地改变朝向
        bossBringer.SetVelocity(0, 0);

        //定义多久未攻击后脱战，进入idle状态
        stateTimer = bossBringer.quitBattleTime;
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
        if (bossBringer.battleMoveDir != bossBringer.facingDir)
        {
            bossBringer.Flip();
        }
        //不一定非要走到完全和玩家重合的位置，预留一点空间，大概是attackCheckRadius的0.65倍左右
        if ((rb.position.x < (PlayerManager.instance.player.transform.position.x + bossBringer.attackCheckRadius * 0.65f)) && (rb.position.x > (PlayerManager.instance.player.transform.position.x - bossBringer.attackCheckRadius * 0.65f)))
        {
            //在范围内得先停下再转换到站立状态，不然会直接走到重合
            bossBringer.SetVelocity(0, 0);
            //人物在这个范围内的时候，保持站立

            bossBringer.stateMachine.ChangeState(bossBringer.idleState);
        }
        else
        {
            {
                //防止敌人与玩家走的太近
                if(!bossBringer.enterAttackRegion)
                {
                    //赋予朝向玩家的移动速度
                    bossBringer.SetVelocity(bossBringer.moveSpeed * bossBringer.battleSpeedMultiplier * bossBringer.battleMoveDir, rb.velocity.y);
                }
                else
                {
                    //防止在battleState里原地踏步
                    bossBringer.stateMachine.ChangeState(bossBringer.idleState);
                }
            }
        }
        #endregion

        #region Attack
        //如果到达了距离玩家一定的距离范围内，并且攻击冷却结束，便发动攻击
        if (bossBringer.enterAttackRegion && bossBringer.canAttack)
        {
            bossBringer.stateMachine.ChangeState(bossBringer.attackState);
        }
        //如果人物在可攻击半径内，则给予其碰撞伤害
        //if(PlayerManager.instance.transform.position)
        #endregion

        #region CaseThatQuitBattle
        //如果遇到悬崖，则脱战进入idle
        if (!bossBringer.isGround)
        {
            //bossBringer.stateMachine.ChangeState(bossBringer.idleState);
            //传送至玩家附近
            Debug.Log("from battleState1");
            bossBringer.stateMachine.ChangeState(bossBringer.teleportState);
        }
        //愤怒时间到了后，或者玩家距离超出范围，则脱战
        if (stateTimer < 0 || Vector2.Distance(bossBringer.transform.position, PlayerManager.instance.player.transform.position) > bossBringer.GetQuitBattleDisance()/* && bossBringer.teleportTimer < 0*/)
        {
            //关闭索敌状态
            //bossBringer.shouldEnterBattle = false;
            //回到站立状态
            //bossBringer.stateMachine.ChangeState(bossBringer.idleState);
            //传送至玩家附近
            Debug.Log("from battleState2");
            bossBringer.stateMachine.ChangeState(bossBringer.idleState);
        }
        #endregion
    }
}
