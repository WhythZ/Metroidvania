using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{
    //其实直接用etity就好，基类里的entity已经获取了Player脚本了，但是这里还是习惯用player
    Player player;

    #region Skill
    [Header("Skill Stats")]
    //投掷剑技能的附加的额外伤害，故而飞剑的伤害是primaryAttackDamage的加成后（暴击判断等）加上这个额外数值
    //这样的好处是不用为剑的伤害额外计算暴击率等信息了，后面可能要改，先暂时这样
    public Stat extraSwordDamage;
    #endregion

    protected override void Start()
    {
        base.Start();

        //和player = entity等效
        player = PlayerManager.instance.player;
    }

    public override void GetDamagedBy(int _damage)
    {
        //冲刺的时候不触发受击
        if (player.stateMachine.currentState == player.dashState)
        {
            return;
        }
        else
        {
            base.GetDamagedBy(_damage);
        }
        
    }

    public override void StatsDie()
    {
        base.StatsDie();
    }
}
