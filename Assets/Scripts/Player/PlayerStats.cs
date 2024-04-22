using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{
    //减少代码量
    Player player;

    //投掷剑技能的伤害
    public Stat swordDamage;

    protected override void Start()
    {
        base.Start();

        player = PlayerManager.instance.player;
    }

    public override void GetDamaged(int _damage)
    {
        //冲刺的时候不触发受击
        if (player.stateMachine.currentState != player.dashState)
        {
            //被攻击时，调用对方的攻击数值，在自己的当前生命值上减掉
            currentHealth -= _damage;

            //受攻击的材质变化，使得有闪烁的动画效果
            player.fx.StartCoroutine("FlashHitFX");
        }
    }
}
