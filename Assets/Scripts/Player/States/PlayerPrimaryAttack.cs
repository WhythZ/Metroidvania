using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttack : PlayerState
{
    #region ComboSettings
    //设置判断连招用的计数器
    private int comboCounter = 1;
    //最后一次攻击的时间
    private float lastTimeAttack;
    //多久不攻击后退出连招数累积，从第一次连招开始
    private float comboRefreshDuration = 1;
    #endregion

    public PlayerPrimaryAttack(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        #region ComboCounter
        //如果超出了连招最大个数，则归1（即下一次攻击回归第一招攻击）
        //如果攻击之间间隔太久（超出comboRefreshDuration），则会重新从第一招开始攻击
        if (comboCounter > 3 || (lastTimeAttack + comboRefreshDuration < Time.time))
        {
            comboCounter = 1;
        }
        //Debug.Log(comboCounter);

        //这段代码必须放在下面，保证如果上面归1了这边也能接收到
        //把AttackStack内的comboCounter和Animator内的对应Parameter链接起来
        player.anim.SetInteger("ComboCounter", comboCounter);
        #endregion

        #region AttackDetails
        //跑动进入攻击时，让任务能够维持原来速度一段时间，体现惯性感
        stateTimer = player.runIntoAttackInertiaDuration;
        
        /*（弃用）攻击方向的设置
        float attackDir = player.facingDir;
        //如果连招中间想改变攻击方向，会有一点延迟，这里这样手动让其转向更舒服点
        if(xInput != 0)
        {
            attackDir = xInput;
        }
        */

        //不同段攻击产生不同段的位移，这里用了矢量的纵速度，让角色攻击时有一个向上的抖动，更生动
        player.SetVelocity(player.attackMovement[comboCounter - 1].x * player.facingDir, player.attackMovement[comboCounter - 1].y);
        #endregion
    }

    public override void Exit()
    {
        base.Exit();

        //递增这个状态类对象（Player.cs脚本中的primaryAttack）的成员comboCounter（连招数）
        comboCounter++;

        //记录最后一次攻击的时间点
        lastTimeAttack = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //进入状态时，维持原来的速度一段时间，时间结束后则静止
        if (stateTimer < 0)
        {
            player.SetVelocity(0, 0);
        }

        //如果第一段攻击结束，进入idle状态；因为攻击只能在地上被触发，所以不用担心若是空中攻击结束后进入idle而无法触发air
        if (stateActionFinished)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
