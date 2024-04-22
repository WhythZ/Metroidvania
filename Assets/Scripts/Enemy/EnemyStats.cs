using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnemyStats : EntityStats
{
    protected override void Start()
    {
        base.Start();
    }

    public override void GetDamaged(int _damage)
    {
        base.GetDamaged(_damage);
    }
}
