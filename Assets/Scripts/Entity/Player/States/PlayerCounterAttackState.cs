using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //弹反状态有效持续时间
        stateTimer = player.counterAttackDuration;
        //我们在这里控制这两个弹反相关的parameters，故无需在Player脚本内新建这两个状态的对象变量了
        //值得一提，我们只需要一个PlayerCounterAttackState脚本即可，用于控制弹反准备状态和成功状态两个动画
        player.anim.SetBool("SuccessCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //防御反击的时候别乱动啊
        player.SetVelocity(0, 0);

        //建立一个临时数组，储存此时在人物攻击检测圈内的所有实体
        Collider2D[] collidersInAttackZone = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        //循环遍历上述数组内的敌人实体，进行弹反判定
        foreach (var beHitEntity in collidersInAttackZone)
        {
            //对敌人类实体造成弹反眩晕
            if (beHitEntity.GetComponent<Enemy>() != null)
            {
                //弹反的前提是敌人处于破绽状态
                if(beHitEntity.GetComponent<Enemy>().WhetherCanBeStunned())
                {
                    //赋予一个大数，防止触发下面那个if的(stateTimer < 0)条件
                    stateTimer = 100;

                    //返回弹反成功的讯号
                    player.anim.SetBool("SuccessCounterAttack", true);

                    //弹反成功的音效
                    AudioManager.instance.PlaySFX(15, null);
                }
            }
        }

        //若弹反时间结束，或者弹反的动作完成了（AnimationTrigger的函数被调用），则回到idle
        if(stateTimer < 0 || stateActionFinished)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
