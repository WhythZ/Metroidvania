using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBattleState : EnemyState
{
    private Slime slime;

    public SlimeBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.slime = _slime;
    }

    public override void Enter()
    {
        base.Enter();

        //进入状态设置速度为零，防止被击退后落地改变朝向
        slime.SetVelocity(0, 0);

        //定义多久未攻击后脱战，进入idle状态
        stateTimer = slime.quitBattleTime;
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
        if (slime.battleMoveDir != slime.facingDir)
        {
            slime.Flip();
        }
        //不一定非要走到完全和玩家重合的位置，预留一点空间，大概是attackCheckRadius的0.65倍左右
        if ((rb.position.x < (PlayerManager.instance.player.transform.position.x + slime.attackCheckRadius * 0.65f)) && (rb.position.x > (PlayerManager.instance.player.transform.position.x - slime.attackCheckRadius * 0.65f)))
        {
            //在范围内得先停下再转换到站立状态，不然会直接走到重合
            slime.SetVelocity(0, 0);
            //人物在这个范围内的时候，保持站立
            slime.stateMachine.ChangeState(slime.idleState);
        }
        else
        {
            {
                //防止敌人与玩家走的太近
                if (!slime.enterAttackRegion)
                {
                    //赋予朝向玩家的移动速度
                    slime.SetVelocity(slime.moveSpeed * slime.battleSpeedMultiplier * slime.battleMoveDir, rb.velocity.y);
                }
                else
                {
                    //防止在battleState里原地踏步
                    slime.stateMachine.ChangeState(slime.idleState);
                }
            }
        }
        #endregion

        #region Attack
        //如果到达了距离玩家一定的距离范围内，并且攻击冷却结束，便发动攻击
        if (slime.enterAttackRegion && slime.canAttack)
        {
            slime.stateMachine.ChangeState(slime.attackState);
        }
        //如果人物在可攻击半径内，则给予其碰撞伤害
        //if(PlayerManager.instance.transform.position)
        #endregion

        #region CaseThatQuitBattle
        //如果遇到悬崖，则脱战进入idle
        if (!slime.isGround)
        {
            slime.stateMachine.ChangeState(slime.idleState);
        }
        //愤怒时间到了后，或者玩家距离超出范围，则脱战
        if (stateTimer < 0 || Vector2.Distance(slime.transform.position, PlayerManager.instance.player.transform.position) > slime.GetQuitBattleDisance())
        {
            //关闭索敌状态
            slime.shouldEnterBattle = false;
            //回到站立状态
            slime.stateMachine.ChangeState(slime.idleState);
        }
        #endregion
    }
}
