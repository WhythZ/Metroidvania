using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerStats : EnemyStats
{
    Bringer bringer;

    protected override void Start()
    {
        base.Start();

        bringer = GetComponent<Bringer>();
    }

    public override void GetDamaged(int _damage)
    {
        base.GetDamaged(_damage);

        //受攻击的材质变化，使得有闪烁的动画效果
        bringer.fx.StartCoroutine("FlashHitFX");

        //受伤的击退效果
        bringer.StartCoroutine("HitKnockback");
    }

    public override void StatsDie()
    {
        base.StatsDie();

        bringer.EntityDie();
    }
}
