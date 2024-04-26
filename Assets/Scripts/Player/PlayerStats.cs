using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{
    //其实直接用etity就好，基类里的entity已经获取了Player脚本了，但是这里还是习惯用player
    Player player;

    #region Skill
    [Header("Skill Stats")]
    //投掷剑技能的附加的额外伤害，故而飞剑的伤害是的玩家原本的伤害（含暴击判断等）加上这个额外数值
    //这样的好处是不用为剑的伤害额外计算暴击率等信息了，后面可能要改，先暂时这样
    public Stat swordExtraDamage;
    #endregion

    protected override void Start()
    {
        base.Start();

        #region SetDefault
        //初始飞剑伤害额外为10
        swordExtraDamage.SetDefaultValue(10);
        #endregion

        //和player=entity等效
        player = PlayerManager.instance.player;
    }

    #region DamagedOverride
    public override void GetMagicalDamagedBy(int _damage)
    {
        //冲刺的时候不触发受击
        if (player.stateMachine.currentState == player.dashState)
            return;
        base.GetMagicalDamagedBy(_damage);
    }
    public override void GetPhysicalDamagedBy(int _damage)
    {
        //冲刺的时候不触发受击
        if (player.stateMachine.currentState == player.dashState)
            return;
        base.GetPhysicalDamagedBy(_damage);
    }
    #endregion

    #region Die
    public override void StatsDie()
    {
        base.StatsDie();
    }
    #endregion
}
